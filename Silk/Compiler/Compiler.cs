// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SoftCircuits.Silk
{
    public partial class Compiler
    {
        /// <summary>
        /// Gets or sets the maximum number of compile errors allowed before compiling aborts.
        /// </summary>
        public int MaxErrors { get; set; }

        /// <summary>
        /// Gets or sets whether the compiler generates a log file of all bytecodes generated. Log
        /// file will have the same name as the input file except with the LOG extension. False by
        /// default.
        /// </summary>
        public bool CreateLogFile { get; set; }

        /// <summary>
        /// Gets or sets whether line numbers are included in the generated
        /// <see>CompiledProgram</see>. Makes line numbers available during run-time exceptions.
        /// True by default.
        /// </summary>
        public bool EnableLineNumbers { get; set; }

        /// <summary>
        /// Gets or sets whether internal, intrinsic functions and variables defined in the
        /// library will be added and available to the source program. If false, the only functions
        /// available to the source program are those defined in the source code and those added
        /// using the <see>RegisterFunction</see>. True by default.
        /// </summary>
        public bool EnableInternalFunctions { get; set; }

        /// <summary>
        /// Contains compile errors after <see>Compile</see> returns false.
        /// </summary>
        public List<Error> Errors { get; private set; }

        private OrderedDictionary<string, IntrinsicFunction> IntrinsicFunctions;
        private OrderedDictionary<string, Variable> IntrinsicVariables;
        private OrderedDictionary<string, Function> Functions;
        private OrderedDictionary<string, Variable> Variables;
        private List<Variable> Literals;
        private LexicalAnalyzer Lexer;
        private ByteCodeWriter Writer;
        private CompileTimeUserFunction CurrentFunction;
        private bool InFunction => (CurrentFunction != null);
        private bool InHeader;

        public Compiler()
        {
            IntrinsicFunctions = new OrderedDictionary<string, IntrinsicFunction>(StringComparer.OrdinalIgnoreCase);
            IntrinsicVariables = new OrderedDictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            MaxErrors = 30;
            CreateLogFile = false;
            EnableLineNumbers = true;
            EnableInternalFunctions = true;
        }

        public bool Compile(string path, out CompiledProgram program)
        {
            Functions = new OrderedDictionary<string, Function>(StringComparer.OrdinalIgnoreCase);
            Variables = new OrderedDictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            Literals = new List<Variable>();
            Lexer = new LexicalAnalyzer(this);
            Lexer.Error += Lexer_Error;
            Writer = new ByteCodeWriter(Lexer);
            InHeader = true;
            CurrentFunction = null;
            Errors = new List<Error>();
            program = null;

            // Add intrinsic functions to function collection
            foreach (var function in IntrinsicFunctions.Values)
                Functions.Add(function.Name, function);
            // Add intrinsic variables to variable collection
            foreach (var pair in IntrinsicVariables.GetKeyValuePairs())
                Variables.Add(pair.Key, pair.Value);
            // Add internal functions and variables
            if (EnableInternalFunctions)
                InternalFunctions.AddInternalFunctionsAndVariables(Functions, Variables);

            try
            {
                // Load file and initialize lexer
                Lexer.Reset(File.ReadAllText(path));
                // Write bytecodes to call function main.
                // Also causes error if main function is not defined
                Writer.Write(ByteCode.ExecFunction, GetFunctionId(Function.Main));
                // Parse statements
                while (ParseStatement())
                    ;
                // Verify end of file
                Token token = Lexer.GetNext();
                if (token.Type != TokenType.EndOfFile)
                    Error(ErrorCode.UnexpectedToken, token);
                // Check for undefined functions
                foreach (var funcInfo in Functions.GetKeyValuePairs())
                {
                    if (funcInfo.Value == null)
                    {
                        if (funcInfo.Key.Equals(Function.Main, StringComparison.CurrentCultureIgnoreCase))
                            Error(ErrorCode.MainNotDefined, Function.Main.MakeQuoted());
                        else
                            Error(ErrorCode.FunctionNotDefined, funcInfo.Key.MakeQuoted());
                    }
                }
            }
            catch (TooManyErrorsException)
            {
                // Already handled
            }
            catch (Exception ex)
            {
                Error(ErrorCode.InternalError, ex.Message, ErrorLevel.FatalError);
            }
            // Done if compile failed
            if (Errors.Count > 0)
                return false;
            // Implement logging
            if (CreateLogFile)
                Writer.WriteLogFile(path, Path.ChangeExtension(path, "log"));
            // Return compiled data
            program = new CompiledProgram
            {
                ByteCodes = Writer.GetBytecodes(),
                Functions = Functions.Values.Select(f =>
                    (f is CompileTimeUserFunction userFunction) ? new UserFunction(userFunction) : f).ToArray(),
                Variables = Variables.Values.ToArray(),
                Literals = Literals.ToArray(),
                LineNumbers = EnableLineNumbers ? Writer.GetLineNumbers() : null,
            };
            return true;
        }

        /// <summary>
        /// Processes a statement, which can recursively process additional statements if
        /// a code-block structure is encountered (function, if, while, etc.). Returns false
        /// if a right curly brace or the end of file is reached. (Does not consume right
        /// curly brace.) Otheriwse, true is returned.
        /// </summary>
        private bool ParseStatement()
        {
            Token token = Lexer.GetNextSkipNewLines();
            switch (token.Type)
            {
                case TokenType.Keyword:
                    ParseKeyword(token);
                    break;
                case TokenType.Symbol:
                    ParseSymbol(token);
                    break;
                case TokenType.EndOfFile:
                    return false;
                case TokenType.RightBrace:
                    Lexer.UngetToken(token);
                    return false;
                default:
                    Error(ErrorCode.UnexpectedToken, token);
                    NextLine();
                    break;
            }
            return true;
        }

        /// <summary>
        /// Parses a symbol. Expected to be either a label, an assignment, a function
        /// definition, or function call.
        /// </summary>
        private void ParseSymbol(Token token)
        {
            Token nextToken = Lexer.GetNext();
            switch (nextToken.Type)
            {
                case TokenType.Colon:
                    // Label definition
                    if (!InFunction)
                    {
                        Error(ErrorCode.CodeOutsideFunction, token);
                        NextLine();
                        return;
                    }
                    AddLabel(token.Value, Writer.IP);
                    // Allow another statement on same line
                    break;
                case TokenType.LeftBracket:
                    // Assignment to list item
                    if (!InFunction)
                    {
                        Error(ErrorCode.CodeOutsideFunction, token);
                        NextLine();
                        return;
                    }
                    Writer.Write(ByteCode.AssignListVariable, GetVariableId(token.Value));
                    // Parse list index
                    if (!ParseExpression())
                        return;
                    token = Lexer.GetNext();
                    if (token.Type != TokenType.RightBracket)
                    {
                        Error(ErrorCode.ExpectedRightBracket, token);
                        NextLine();
                        return;
                    }
                    // Parse equal sign
                    token = Lexer.GetNext();
                    if (token.Type != TokenType.Equal)
                    {
                        Error(ErrorCode.ExpectedEquals, token);
                        NextLine();
                        return;
                    }
                    // Parse expression
                    if (!ParseExpression())
                        return;
                    VerifyEndOfLine();
                    break;
                case TokenType.Equal:
                    // Assignment
                    if (!InFunction)
                    {
                        Error(ErrorCode.CodeOutsideFunction, token);
                        NextLine();
                        return;
                    }
                    Writer.Write(ByteCode.Assign, GetVariableId(token.Value));
                    if (!ParseExpression())
                        return;
                    VerifyEndOfLine();
                    break;
                default:
                    if (InFunction)
                    {
                        // Function call
                        int functionId = GetFunctionId(token.Value);
                        Function function = Functions[functionId];
                        Writer.Write(ByteCode.ExecFunction, functionId);
                        // Next token might be part of argument expression
                        Lexer.UngetToken(nextToken);
                        if (!ParseFunctionArguments(function, false))
                            return;
                        VerifyEndOfLine();
                    }
                    else
                    {
                        // Function definition requires parentheses
                        if (nextToken.Type != TokenType.LeftParen)
                        {
                            Error(ErrorCode.ExpectedLeftParen, nextToken);
                            break;
                        }
                        // Create function
                        var function = new CompileTimeUserFunction(token.Value, Writer.IP);
                        // Parse and add parameters
                        if (!ParseFunctionParameters(function))
                            return;
                        // Add user function
                        AddUserFunction(token.Value, function);

                        InHeader = false;
                        CurrentFunction = function;

                        token = Lexer.GetNextSkipNewLines();
                        if (token.Type != TokenType.LeftBrace)
                        {
                            Error(ErrorCode.ExpectedLeftBrace, token);
                            return;
                        }

                        // Parse statements in function
                        while (ParseStatement())
                            ;

                        token = Lexer.GetNext();
                        if (token.Type != TokenType.RightBrace)
                        {
                            Error(ErrorCode.ExpectedRightBrace);
                            return;
                        }

                        // Write return (no return expression)
                        Writer.Write(ByteCode.Return, 0);

                        // Check for undefined labels
                        foreach (var labelInfo in function.Labels.GetKeyValuePairs())
                        {
                            if (labelInfo.Value.IP == null)
                            {
                                Error(ErrorCode.LabelNotDefined, labelInfo.Key.MakeQuoted());
                                return;
                            }
                        }
                        CurrentFunction = null;
                    }
                    break;
            }
        }

        /// <summary>
        /// Parses the function definition.
        /// It is assumed that the function name and left parenthesis have already been parsed.
        /// Consumes up to and including the right parenthesis.
        /// Returns false on error.
        /// </summary>
        private bool ParseFunctionParameters(CompileTimeUserFunction function)
        {
            Token token;

            if (Lexer.PeekNext().Type == TokenType.Symbol)
            {
                do
                {
                    token = Lexer.GetNext();
                    if (token.Type != TokenType.Symbol)
                    {
                        Error(ErrorCode.ExpectedSymbol, token);
                        return false;
                    }
                    function.Parameters.Add(token.Value, new Variable());
                    token = Lexer.GetNext();
                } while (token.Type == TokenType.Comma);
            }
            else token = Lexer.GetNext();

            if (token.Type != TokenType.RightParen)
            {
                Error(ErrorCode.ExpectedRightParen, token);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parses arguments for a function call. If <paramref name="usingParentheses"/>
        /// is true, assumes opening parenthesis has been read and reads up to and
        /// including the closing parenthesis. Returns false on error.
        /// </summary>
        /// <param name="function">Function being called. May be null for user
        /// functions. Used to verify argument count for intrinsic functions.</param>
        private bool ParseFunctionArguments(Function function, bool usingParentheses)
        {
            Token token;
            int count = 0;

            // Save position and write placeholder for argument count
            int argumentCountIP = Writer.Write(0);

            if (ParseExpression(false))
            {
                count++;
                token = Lexer.GetNext();
                while (token.Type == TokenType.Comma)
                {
                    if (!ParseExpression())
                        return false;
                    count++;
                    token = Lexer.GetNext();
                }
            }
            else token = Lexer.GetNext();

            if (usingParentheses)
            {
                if (token.Type != TokenType.RightParen)
                {
                    Error(ErrorCode.ExpectedRightParen, token);
                    return false;
                }
            }
            else Lexer.UngetToken(token);

            // Enforce argument count for intrinsic functions
            // (May be null for user functions not yet defined)
            if (function is IntrinsicFunction intrinsicFunction)
            {
                if (!intrinsicFunction.IsParameterCountValid(count, out string error))
                {
                    Error(ErrorCode.WrongNumberOfArguments, error);
                    return false;
                }
            }
            // Fixup argument count
            Writer.WriteAt(argumentCountIP, count);
            return true;
        }

        #region Function/Variable/Literal/Labels handling

        /// <summary>
        /// Registers an intrinsic function with the compiler. If the program calls this function at runtime,
        /// the runtime will raise the <see>Function</see> event.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="minParameters">Minimum allowed number of parameters.</param>
        /// <param name="maxParameters">Maximum allowed number of parameters.</param>
        public void RegisterFunction(string name, int minParameters = Function.NoParameterLimit, int maxParameters = Function.NoParameterLimit)
        {
            // Validate function name
            if (!Keywords.IsValidSymbolName(name))
                throw new Exception($"Cannot add function. \"{name}\" is not a valid function name.");
            // Cannot use reserved keywords
            if (Keywords.IsReservedSymbol(name))
                throw new Exception($"Cannot add function. \"{name}\" is a reserved identifier or keyword.");
            // Look if we already have an intrinsic function with this name
            if (IntrinsicFunctions.IndexOf(name) >= 0)
                throw new Exception($"An intrinisic function with the name \"{name}\" has already been added.");
            // Add intrinsic function
            IntrinsicFunctions.Add(name, new IntrinsicFunction(name, minParameters, maxParameters));
        }

        /// <summary>
        /// Registers an intrinsic variable with the compiler.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="variable">Variable value.</param>
        public void RegisterVariable(string name, Variable variable)
        {
            // Validate variable name
            if (!Keywords.IsValidSymbolName(name))
                throw new Exception($"Cannot add variable. \"{name}\" is not a valid variable name.");
            // Cannot use reserved keywords
            if (Keywords.IsReservedSymbol(name))
                throw new Exception($"Cannot add variable. \"{name}\" is a reserved identifier or keyword.");
            // Look if we already have a variable with this name
            if (IntrinsicVariables.IndexOf(name) >= 0)
                throw new Exception($"An variable with the name \"{name}\" has already been added.");
            // Add predefined variable
            IntrinsicVariables.Add(name, variable);
        }

        /// <summary>
        /// Returns the ID for the named function, creating a placeholder for it if it's not already defined.
        /// </summary>
        /// <param name="name">Function name.</param>
        internal int GetFunctionId(string name)
        {
            int index = Functions.IndexOf(name);
            if (index >= 0)
                return index;
            // Create null placeholder if function not defined
            return Functions.Add(name, null);
        }

        /// <summary>
        /// Returns the ID for the named variable. If <paramref name="createIfNeeded"/> is true, the variable
        /// will be created if it doesn't exist. If <paramref name="createIfNeeded"/> is false, this method
        /// returns -1 if the variable does not exist.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="createIfNeeded">If true, the variable will be created if it doesn't exist.</param>
        internal int GetVariableId(string name, bool createIfNeeded = true)
        {
            //UserFunctionEx function = CurrentFunction;
            int index;

            // Search local variables
            if (InFunction)
            {
                Debug.Assert(CurrentFunction != null);
                index = CurrentFunction.Parameters.IndexOf(name);
                if (index >= 0)
                    return index | (int)ByteCodeVariableFlag.Parameter;
                index = CurrentFunction.Variables.IndexOf(name);
                if (index >= 0)
                    return index | (int)ByteCodeVariableFlag.Local;
            }
            // Search global variables
            index = Variables.IndexOf(name);
            if (index >= 0)
                return index | (int)ByteCodeVariableFlag.Global;
            // Variable not found
            if (createIfNeeded)
            {
                if (InHeader)
                {
                    index = Variables.Add(name, new Variable());
                    return index | (int)ByteCodeVariableFlag.Global;
                }
                else if (InFunction)
                {
                    return CurrentFunction.GetVariableId(name);
                }
                Debug.Assert(false);
                throw new Exception("Variable referenced outside of header or function!");
            }
            else return -1;
        }

        /// <summary>
        /// Returns the ID of the given value, adding it if needed.
        /// </summary>
        internal int GetLiteralId(Variable var)
        {
            // Use existing literal if one matches
            int index = Literals.IndexOf(var);
            if (index >= 0)
                return index;
            // Otherwise, add it
            index = Literals.Count;
            Literals.Add(var);
            return index;
        }

        /// <summary>
        /// Returns the address (IP) for the named label. Or, if the label has not been
        /// defined, a placeholder value is returned and the current write location
        /// is registered to be "fixed up" later.
        /// </summary>
        /// <param name="name">Label name</param>
        internal int ReferenceLabel(string name)
        {
            // Get or create label
            Debug.Assert(InFunction);
            if (!CurrentFunction.Labels.TryGetValue(name, out Label label))
            {
                label = new Label(name);
                CurrentFunction.Labels.Add(name, label);
            }
            // Return jump IP if we have it
            if (label.IP.HasValue)
                return label.IP.Value;
            // Otherwise, log this location for fix up later
            label.FixUpIPs.Add(Writer.IP);
            // Return 0 as a placeholder
            return ByteCodes.InvalidIP;
        }

        /// <summary>
        /// Adds a label.
        /// </summary>
        /// <param name="name">Label name.</param>
        /// <param name="ip">Label address</param>
        internal void AddLabel(string name, int ip)
        {
            // Get or create label
            Debug.Assert(InFunction);
            if (CurrentFunction.Labels.TryGetValue(name, out Label label))
            {
                if (label.IP.HasValue)
                {
                    Error(ErrorCode.DuplicateLabel, name.MakeQuoted());
                    return;
                }
                label.IP = ip;
            }
            else
            {
                label = new Label(name, ip);
                CurrentFunction.Labels.Add(name, label);
            }
            // Test if label has already been defined
            // Fix up any forward references
            foreach (int location in label.FixUpIPs)
                Writer.WriteAt(location, label.IP.Value);
            label.FixUpIPs.Clear();
        }

        private bool AddUserFunction(string name, CompileTimeUserFunction userFunction)
        {
            int index = Functions.IndexOf(name);
            if (index >= 0)
            {
                // Let user set undefined functions, or override intrinsic ones
                if (Functions[index] == null || Functions[index].IsIntrinsic)
                {
                    Functions[index] = userFunction;
                    return true;
                }
                else
                {
                    Error(ErrorCode.DuplicateFunctionName, name);
                    return false;
                }
            }
            Functions.Add(name, userFunction);
            return true;
        }

        #endregion

        #region Error handling

        internal void Error(ErrorCode code, ErrorLevel level = ErrorLevel.Error)
        {
            Errors.Add(new Error(level, code, Lexer.LastTokenLine));
            CheckTooManyErrors();
        }

        internal void Error(ErrorCode code, Token token, ErrorLevel level = ErrorLevel.Error)
        {
            Errors.Add(new Error(level, code, token.Value.MakeQuoted(), Lexer.LastTokenLine));
            CheckTooManyErrors();
        }

        internal void Error(ErrorCode code, string description, ErrorLevel level = ErrorLevel.Error)
        {
            Errors.Add(new Error(level, code, description, Lexer.LastTokenLine));
            CheckTooManyErrors();
        }

        internal void Error(ErrorCode code, Token token, string description, ErrorLevel level = ErrorLevel.Error)
        {
            Errors.Add(new Error(level, code, description, Lexer.LastTokenLine));
            CheckTooManyErrors();
        }

        private void CheckTooManyErrors()
        {
            if (Errors.Count >= MaxErrors)
            {
                Errors.Add(new Error(ErrorLevel.FatalError, ErrorCode.TooManyErrors, Lexer.CurrentLine));
                throw new TooManyErrorsException();
            }
        }

        #endregion

        #region Support methods

        /// <summary>
        /// Checks if there are any other tokens on the current line. If another token is
        /// found, this method logs an error and returns false. Otherwise, it returns true.
        /// In either case, this method consumes all tokens up to and including the next
        /// newline.
        /// </summary>
        private bool VerifyEndOfLine()
        {
            Token token = Lexer.GetNext();
            if (token.Type == TokenType.EndOfLine)
                return true;
            Error(ErrorCode.UnexpectedToken, token);
            NextLine();
            return false;
        }

        /// <summary>
        /// Consumes all tokens up to and including the next newline.
        /// </summary>
        private void NextLine()
        {
            Token token;
            do
            {
                token = Lexer.GetNext();
            } while (token.Type != TokenType.EndOfLine && token.Type != TokenType.EndOfFile);
        }

        private void Lexer_Error(object sender, ErrorEventArgs e)
        {
            if (e.Token != null)
                Error(e.ErrorCode, e.Token);
            else
                Error(e.ErrorCode);
        }

        #endregion

    }
}
