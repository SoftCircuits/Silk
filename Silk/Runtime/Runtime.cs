// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SoftCircuits.Silk
{
    public class Runtime
    {
        private CompiledProgram Program;

        private ByteCodeReader Reader;
        private Stack<RuntimeFunction> FunctionStack;
        private Stack<Variable> VarStack;

        private object? UserData;

        /// <summary>
        /// Initializes a new <see cref="Runtime"/> instance and prepares to execute the
        /// given program.
        /// </summary>
        /// <param name="program">The program to prepare to execute.</param>
        public Runtime(CompiledProgram program)
        {
            Reset(program);
        }

        /// <summary>
        /// Resets this runtime instance to be ready to execute the given <see cref="CompiledProgram"/>.
        /// </summary>
        /// <param name="program">The program to prepare to execute.</param>
#if NET5_0
        [MemberNotNull(nameof(Program), nameof(Reader), nameof(FunctionStack), nameof(VarStack))]
#endif
        public void Reset(CompiledProgram program)
        {
            Program = program ?? throw new ArgumentNullException(nameof(program));
            Reader = new ByteCodeReader(Program.GetByteCodes());
            FunctionStack = new Stack<RuntimeFunction>();
            VarStack = new Stack<Variable>();
            UserData = null;
        }

        /// <summary>
        /// Executes the given program.
        /// </summary>
        /// <returns>Returns a variable that contains the result of the program.</returns>
        public Variable Execute()
        {
            Reader.GoTo(0);
            FunctionStack.Clear();
            VarStack.Clear();
            UserData = null;

            OnBegin();

            try
            {
                // Initial bytecodes are call to main() function
                ByteCode bytecode = Reader.GetNext();
                Debug.Assert(bytecode == ByteCode.ExecFunction);
                int mainId = Reader.GetNextValue();

                // Validate
                if (bytecode != ByteCode.ExecFunction || Program.Functions[mainId] is not UserFunction userFunction)
                    throw new Exception("Invalid bytecode program : Missing call to function main.");

                RuntimeFunction function = new(userFunction);

                // Execute this function
                ExecuteFunction(function);

                // Return result
                return function.ReturnValue;
            }
            catch (Exception ex)
            {
                // Include line-number information if possible
                if (Program.LineNumbers != null)
                {
                    Debug.Assert(Program.LineNumbers.Length == Program.ByteCodes.Length);
                    int ip = (Reader.IP - 1);
                    if (ip >= 0 && ip < Program.LineNumbers.Length)
                    {
                        string s = $"\"{ex.Message}\" exception on line {Program.LineNumbers[ip]}. See inner exception for details.";
                        throw new Exception(s, ex);
                    }
                }
                throw;
            }
            finally
            {
                OnEnd();
            }
        }

        private void ExecuteFunction(RuntimeFunction function)
        {
            Debug.Assert(function != null);
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            FunctionStack.Push(function);
            Reader.Push();
            Debug.Assert(function.IP != ByteCodes.InvalidIP);
            Reader.GoTo(function.IP);

            // Dispatch each bytecode to its handler
            ByteCode bytecode;
            do
            {
                bytecode = Reader.GetNext();
                Debug.Assert(ByteCodeHandlerLookup.ContainsKey(bytecode));
                ByteCodeHandlerLookup[bytecode](this);
            } while (bytecode != ByteCode.Return);

            FunctionStack.Pop();
            Reader.Pop();
        }

        #region ByteCode dispatcher and handlers

        /// <summary>
        /// ByteCode dispatch table
        /// </summary>
        private static readonly Dictionary<ByteCode, Action<Runtime>> ByteCodeHandlerLookup = new()
        {
            [ByteCode.Nop] = r => r.Nop(),
            [ByteCode.ExecFunction] = r => r.ExecFunction(),
            [ByteCode.Return] = r => r.Return(),
            [ByteCode.Jump] = r => r.Jump(),
            [ByteCode.JumpIfFalse] = r => r.JumpIfFalse(),
            [ByteCode.Assign] = r => r.Assign(),
            [ByteCode.AssignListVariable] = r => r.AssignListVariable(),
            [ByteCode.AssignListVariableMulti] = r => r.AssignListVariableMulti(),
        };

        private void Nop()
        {
            // Should never be used
            Debug.Assert(false);
        }

        private void ExecFunction()
        {
            // Evaluate function and discard result
            EvalFunction();
            VarStack.Pop();
        }

        private void Return()
        {
            // Set return value if an expression is found
            var function = GetFunctionContext();
            if (EvalExpression())
                function.ReturnValue.SetValue(VarStack.Pop());
        }

        private void Jump()
        {
            int ip = Reader.GetNextValue();
            Debug.Assert(ip != ByteCodes.InvalidIP);
            Reader.GoTo(ip);
        }

        private void JumpIfFalse()
        {
            int ip = Reader.GetNextValue();
            bool result = EvalExpression();
            Debug.Assert(result);
            Variable value = VarStack.Pop();
            if (value.IsFalse())
                Reader.GoTo(ip);
        }

        private void Assign()
        {
            Variable var = GetVariable(Reader.GetNextValue());
            bool result = EvalExpression();
            Debug.Assert(result);
            var.SetValue(VarStack.Pop());
        }

        // Retained for backwards compatibility
        // but now AssignListVariableMulti is used,
        // which supports multiple indexes.
        private void AssignListVariable()
        {
            Variable array = GetVariable(Reader.GetNextValue());
            // Evaluate index
            bool result = EvalExpression();
            Debug.Assert(result);
            Variable index = VarStack.Pop();
            Variable var = array.GetAt(index.ToInteger() - 1);
            // Evaluate expression
            result = EvalExpression();
            Debug.Assert(result);
            var.SetValue(VarStack.Pop());
        }

        private void AssignListVariableMulti()
        {
            bool result;

            Variable variable = GetVariable(Reader.GetNextValue());
            // Evaluate index(es)
            int count = Reader.GetNextValue();
            Debug.Assert(count > 0);
            while (count > 0)
            {
                // Evaluate index
                result = EvalExpression();
                Debug.Assert(result);
                Variable index = VarStack.Pop();
                variable = variable.GetAt(index.ToInteger() - 1);
                count--;
            }
            // Evaluate expression
            result = EvalExpression();
            Debug.Assert(result);
            variable.SetValue(VarStack.Pop());
        }

        #endregion

        #region Expression evaluator

        /// <summary>
        /// ByteCode evaluators dispatch table.
        /// </summary>
        private static readonly Dictionary<ByteCode, Action<Runtime>> EvalHandlerLookup = new()
        {
            [ByteCode.EvalLiteral] = r => r.EvalLiteral(),
            [ByteCode.EvalVariable] = r => r.EvalVariable(),
            [ByteCode.EvalCreateList] = r => r.EvalCreateList(),
            [ByteCode.EvalInitializeList] = r => r.EvalInitializeList(),
            [ByteCode.EvalListVariable] = r => r.EvalListVariable(),
            [ByteCode.EvalFunction] = r => r.EvalFunction(),
            [ByteCode.EvalAdd] = r => r.EvalAdd(),
            [ByteCode.EvalSubtract] = r => r.EvalSubtract(),
            [ByteCode.EvalMultiply] = r => r.EvalMultiply(),
            [ByteCode.EvalDivide] = r => r.EvalDivide(),
            [ByteCode.EvalPower] = r => r.EvalPower(),
            [ByteCode.EvalModulus] = r => r.EvalModulus(),
            [ByteCode.EvalConcat] = r => r.EvalConcat(),
            [ByteCode.EvalNegate] = r => r.EvalNegate(),
            [ByteCode.EvalAnd] = r => r.EvalAnd(),
            [ByteCode.EvalOr] = r => r.EvalOr(),
            [ByteCode.EvalXor] = r => r.EvalXor(),
            [ByteCode.EvalNot] = r => r.EvalNot(),
            [ByteCode.EvalIsEqual] = r => r.EvalIsEqual(),
            [ByteCode.EvalIsNotEqual] = r => r.EvalIsNotEqual(),
            [ByteCode.EvalIsGreaterThan] = r => r.EvalIsGreaterThan(),
            [ByteCode.EvalIsGreaterThanOrEqual] = r => r.EvalIsGreaterThanOrEqual(),
            [ByteCode.EvalIsLessThan] = r => r.EvalIsLessThan(),
            [ByteCode.EvalIsLessThanOrEqual] = r => r.EvalIsLessThanOrEqual(),
            [ByteCode.EvalListVariableMulti] = r => r.EvalListVariableMulti(),
        };

        /// <summary>
        /// Evaluates the next expression, pushes the result on the stack and
        /// returns true. If no expression is found, this method does not
        /// push anything on the stack and returns false.
        /// </summary>
        private bool EvalExpression()
        {
            // Get number of expression elements
            int count = Reader.GetNextValue();
            if (count == 0)
                return false;
            // Call handlers for each expression bytecode
            for (int i = 0; i < count; i++)
            {
                ByteCode bytecode = Reader.GetNext();
                Debug.Assert(EvalHandlerLookup.ContainsKey(bytecode));
                EvalHandlerLookup[bytecode](this);
            }
            return true;
        }

        private void EvalLiteral()
        {
            // Must be copy to avoid modifying original
            VarStack.Push(new Variable(Program.Literals[Reader.GetNextValue()]));
        }

        private void EvalVariable()
        {
            // Must be copy to avoid modifying original
            VarStack.Push(new Variable(GetVariable(Reader.GetNextValue())));
        }

        private void EvalCreateList()
        {
            // Create new array variable
            bool result = EvalExpression();
            Debug.Assert(result);
            Variable size = VarStack.Pop();
            VarStack.Push(Variable.CreateList(size.ToInteger()));
        }

        private void EvalInitializeList()
        {
            int count = Reader.GetNextValue();
            List<Variable> variables = new(count);
            for (int i = 0; i < count; i++)
            {
                bool result = EvalExpression();
                Debug.Assert(result);
                variables.Add(VarStack.Pop());
            }
            VarStack.Push(new Variable(variables));
        }

        // Retained for backwards compatibility
        // but now EvalListVariableMulti is used,
        // which supports multiple indexes.
        private void EvalListVariable()
        {
            Variable array = GetVariable(Reader.GetNextValue());
            // Evaluate index
            bool result = EvalExpression();
            Debug.Assert(result);
            Variable index = VarStack.Pop();
            VarStack.Push(array.GetAt(index.ToInteger() - 1));
        }

        private void EvalListVariableMulti()
        {
            Variable variable = GetVariable(Reader.GetNextValue());
            // Evaluate index(es)
            int count = Reader.GetNextValue();
            Debug.Assert(count > 0);
            while (count > 0)
            {
                bool result = EvalExpression();
                Debug.Assert(result);
                Variable index = VarStack.Pop();
                variable = variable.GetAt(index.ToInteger() - 1);
                count--;
            }
            VarStack.Push(variable);
        }

        private void EvalFunction()
        {
            // Get function
            int functionId = Reader.GetNextValue();
            Function function = Program.Functions[functionId];

            // Build arguments
            int argCount = Reader.GetNextValue();
            Variable[] arguments = new Variable[argCount];
            for (int i = 0; i < argCount; i++)
            {
                bool result = EvalExpression();
                Debug.Assert(result);
                arguments[i] = VarStack.Pop();
            }

            if (function.IsIntrinsic)
            {
                // Intrinsic functions are those defined from C# code
                Variable returnValue = new();
                // Run function
                if (function is InternalFunction internalFunction)
                    internalFunction.Action(arguments, returnValue);
                else
                    OnFunction(function.Name, arguments, returnValue);
                // Push result onto stack
                VarStack.Push(returnValue);
            }
            else if (function is UserFunction userFunction)
            {
                // User function are those defined in the script we are running.
                // We don't require the number of arguments passed to match the number of
                // function parameters in user functions
                var runtimeFunction = new RuntimeFunction(userFunction);
                // Set parameter values
                Debug.Assert(runtimeFunction.Parameters != null);
                Array.Copy(arguments, runtimeFunction.Parameters, Math.Min(argCount, runtimeFunction.Parameters.Length));
                // Run function
                ExecuteFunction(runtimeFunction);
                // Push result onto stack
                VarStack.Push(new Variable(runtimeFunction.ReturnValue));
            }
        }

        private void EvalAdd()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Add(var2));
        }

        private void EvalSubtract()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Subtract(var2));
        }

        private void EvalMultiply()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Multiply(var2));
        }

        private void EvalDivide()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Divide(var2));
        }

        private void EvalPower()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Power(var2));
        }

        private void EvalModulus()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Modulus(var2));
        }

        private void EvalConcat()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(var1.Concat(var2));
        }

        private void EvalNegate()
        {
            Variable var = VarStack.Pop();
            VarStack.Push(var.Negate());
        }

        private void EvalAnd()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.ToInteger() & var2.ToInteger()));
        }

        private void EvalOr()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.ToInteger() | var2.ToInteger()));
        }

        private void EvalXor()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.ToInteger() ^ var2.ToInteger()));
        }

        private void EvalNot()
        {
            Variable var = VarStack.Pop();
            VarStack.Push(new Variable(~var.ToInteger()));
        }

        private void EvalIsEqual()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.IsEqual(var2) ? Boolean.True : Boolean.False));
        }

        private void EvalIsNotEqual()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.IsNotEqual(var2) ? Boolean.True : Boolean.False));
        }

        private void EvalIsGreaterThan()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.IsGreaterThan(var2) ? Boolean.True : Boolean.False));
        }

        private void EvalIsGreaterThanOrEqual()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.IsGreaterThanOrEqual(var2) ? Boolean.True : Boolean.False));
        }

        private void EvalIsLessThan()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.IsLessThan(var2) ? Boolean.True : Boolean.False));
        }

        private void EvalIsLessThanOrEqual()
        {
            Variable var2 = VarStack.Pop();
            Variable var1 = VarStack.Pop();
            VarStack.Push(new Variable(var1.IsLessThanOrEqual(var2) ? Boolean.True : Boolean.False));
        }

        #endregion

        #region Events

        public event EventHandler<BeginEventArgs>? Begin;
        public event EventHandler<EndEventArgs>? End;
        public event EventHandler<FunctionEventArgs>? Function;

        internal void OnBegin()
        {
            BeginEventArgs e = new()
            {
                UserData = UserData,
            };
            Begin?.Invoke(this, e);
            UserData = e.UserData;
        }

        internal void OnEnd()
        {
            EndEventArgs e = new()
            {
                UserData = UserData,
            };
            End?.Invoke(this, e);
            UserData = e.UserData;
        }

        internal void OnFunction(string name, Variable[] parameters, Variable returnValue)
        {
            FunctionEventArgs e = new(name, parameters, returnValue, UserData);
            Function?.Invoke(this, e);
            UserData = e.UserData;
        }

        #endregion

        #region Helpers

        private Variable GetVariable(int varId)
        {
            if (ByteCodes.IsGlobalVariable(varId))
                return Program.Variables[ByteCodes.GetVariableIndex(varId)];
            var context = GetFunctionContext();
            if (ByteCodes.IsLocalVariable(varId))
                return context.Variables[ByteCodes.GetVariableIndex(varId)];
            // Function parameter
            return context.Parameters[ByteCodes.GetVariableIndex(varId)];
        }

        private RuntimeFunction GetFunctionContext()
        {
            Debug.Assert(FunctionStack.Count > 0);
            return FunctionStack.Peek();
        }

        #endregion

    }
}
