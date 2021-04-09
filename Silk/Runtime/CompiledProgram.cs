﻿// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SoftCircuits.Silk
{
    public class CompiledProgram
    {
        private const int FileSignature = 0x4B4C4953;   // "SILK"
        private const string InvalidFileFormat = "Invalid file format";

        [Flags]
        private enum FileFlag
        {
            None = 0x0000,
            HasLineNumbers = 0x0001,
        }

        private enum FunctionType
        {
            User,
            Intrinsic,
            Internal,
        }

        internal int[] ByteCodes { get; set; }
        internal Function[] Functions { get; set; }
        internal Variable[] Variables { get; set; }
        internal Variable[] Literals { get; set; }
        internal int[]? LineNumbers { get; set; }

        /// <summary>
        /// Constructs a new <see cref="CompiledProgram"/> instance.
        /// </summary>
        /// <param name="byteCodes">Program byte codes.</param>
        /// <param name="functions">Program functions.</param>
        /// <param name="variables">Program variables.</param>
        /// <param name="literals">Program literals.</param>
        /// <param name="lineNumbers">Line numbers.</param>
        internal CompiledProgram(int[] byteCodes, Function[] functions, Variable[] variables, Variable[] literals, int[]? lineNumbers)
        {
            ByteCodes = byteCodes;
            Functions = functions;
            Variables = variables;
            Literals = literals;
            LineNumbers = lineNumbers;
        }

        internal CompiledProgram()
        {
            Reset();
        }

#if NET5_0
        [MemberNotNull(nameof(ByteCodes))]
        [MemberNotNull(nameof(Functions))]
        [MemberNotNull(nameof(Variables))]
        [MemberNotNull(nameof(Literals))]
#endif
        public void Reset()
        {
            ByteCodes = Array.Empty<int>();
            Functions = Array.Empty<Function>();
            Variables = Array.Empty<Variable>();
            Literals = Array.Empty<Variable>();
            LineNumbers = null;
        }

        public bool IsEmpty => ByteCodes.Length == 0;

        internal int[] GetByteCodes() => ByteCodes;
        internal Function[] GetFunctions() => Functions;
        internal Variable[] GetVariables()
        {
            // Return copies of global variables so the original
            // values do not get modified
            Variable[] variables = new Variable[Variables.Length];
            for (int i = 0; i < Variables.Length; i++)
                variables[i] = new Variable(Variables[i]);
            return variables;
        }
        internal Variable[] GetLiterals() => Literals;

        /// <summary>
        /// Saves the current compiled program to the specified file. Throws an exception
        /// if there is currently no compiled program loaded.
        /// </summary>
        /// <remarks>
        /// Unlike functions, we just write any intrinsic variables to the file even if
        /// they are not used by the code. In addition, variables do not need to be
        /// resolved after we load a compiled program like we must do with functions. When
        /// loading a compiled program, we do not care about any intrinsice variables that
        /// have already been added.
        /// 
        /// NOTE: We should employ some sort of checksum to verify bytecode integrity.
        /// </remarks>
        /// <param name="path">Target file name.</param>
        public void Save(string path)
        {
            if (IsEmpty)
                throw new Exception("Cannot save empty program.");

            using BinaryWriter writer = new(File.Open(path, FileMode.Create));

            // Write file signature
            writer.Write(FileSignature);
            // Reserve room for future changes
            writer.Write(0);
            // Write file flags
            writer.Write((LineNumbers != null) ? (int)FileFlag.HasLineNumbers : (int)FileFlag.None);
            // Write byte codes
            writer.Write(ByteCodes.Length);
            foreach (int value in ByteCodes)
                writer.Write(value);
            // Write functions
            writer.Write(Functions.Length);
            foreach (var function in Functions)
            {
                writer.Write(function.Name);
                if (function is InternalFunction)
                    writer.Write((int)FunctionType.Internal);
                else if (function is IntrinsicFunction)
                    writer.Write((int)FunctionType.Intrinsic);
                else if (function is UserFunction userFunction)
                {
                    writer.Write((int)FunctionType.User);
                    writer.Write(userFunction.IP);
                    writer.Write(userFunction.NumVariables);
                    writer.Write(userFunction.NumParameters);
                }
                else Debug.Assert(false);
            }
            // Write global variables
            writer.Write(Variables.Length);
            foreach (var value in Variables)
                WriteVariable(value, writer);
            // Write literals
            writer.Write(Literals.Length);
            for (int i = 0; i < Literals.Length; i++)
                WriteVariable(Literals[i], writer);
            // Write line numbers
            if (LineNumbers != null)
            {
                Debug.Assert(LineNumbers.Length == ByteCodes.Length);
                writer.Write(LineNumbers.Length);
                for (int i = 0; i < LineNumbers.Length; i++)
                    writer.Write(LineNumbers[i]);
            }
        }

        /// <summary>
        /// Loads a precompiled program from disk.
        /// </summary>
        /// <remarks>
        /// NOTE: We should employ some sort of checksum to verify bytecode integrity.
        /// </remarks>
        /// <param name="path"></param>
        public void Load(string path)
        {
            int i, count;
            Reset();

            try
            {
                using BinaryReader reader = new(File.Open(path, FileMode.Open));

                // Check signature
                i = reader.ReadInt32();
                if (i != FileSignature)
                    throw new Exception(InvalidFileFormat);
                // Read reserved int
                i = reader.ReadInt32();
                if (i != 0)
                    throw new Exception(InvalidFileFormat);
                // Read flags
                FileFlag flags = (FileFlag)reader.ReadInt32();
                // Read bytecodes
                count = reader.ReadInt32();
                ByteCodes = new int[count];
                for (i = 0; i < count; i++)
                    ByteCodes[i] = reader.ReadInt32();
                // Read functions
                count = reader.ReadInt32();
                List<Function> functions = new(count);
                for (i = 0; i < count; i++)
                {
                    string name = reader.ReadString();
                    FunctionType type = (FunctionType)reader.ReadInt32();
                    if (type == FunctionType.Internal)
                    {
                        // Temporarily set non-nullable action to null
                        // We will correct further down
                        var function = new InternalFunction(name, null!);
                        functions.Add(function);
                    }
                    else if (type == FunctionType.Intrinsic)
                    {
                        var function = new IntrinsicFunction(name);
                        functions.Add(function);
                    }
                    else // FunctionType.User
                    {
                        var function = new UserFunction(name, 0)
                        {
                            IP = reader.ReadInt32(),
                            NumVariables = reader.ReadInt32(),
                            NumParameters = reader.ReadInt32(),
                        };
                        functions.Add(function);
                    }
                }
                // Read global variables
                count = reader.ReadInt32();
                Variables = new Variable[count];
                for (i = 0; i < count; i++)
                    Variables[i] = ReadVariable(reader);
                // Read literals
                count = reader.ReadInt32();
                Literals = new Variable[count];
                for (i = 0; i < count; i++)
                    Literals[i] = ReadVariable(reader);
                // Read line numbers
                if (flags.HasFlag(FileFlag.HasLineNumbers))
                {
                    count = reader.ReadInt32();
                    LineNumbers = new int[count];
                    for (i = 0; i < count; i++)
                        LineNumbers[i] = reader.ReadInt32();
                    Debug.Assert(LineNumbers.Length == ByteCodes.Length);
                }
                // Fixup internal functions
                foreach (var function in functions.OfType<InternalFunction>())
                {
                    if (InternalFunctions.InternalFunctionLookup.TryGetValue(function.Name, out InternalFunctionInfo? info))
                    {
                        function.Action = info.Action;
                        function.MinParameters = info.MinParameters;
                        function.MaxParameters = info.MaxParameters;
                    }
                    else throw new Exception($"Internal intrinsic function \"{function.Name}\" not found");
                }
                Functions = functions.ToArray();
            }
            catch (Exception)
            {
                // Clear to known state if error
                Reset();
                throw;
            }
        }

        private void WriteVariable(Variable value, BinaryWriter writer)
        {
            writer.Write((int)value.Type);
            switch (value.Type)
            {
                case VarType.String:
                    writer.Write(value.ToString());
                    break;
                case VarType.Integer:
                    writer.Write(value.ToInteger());
                    break;
                case VarType.Float:
                    writer.Write(value.ToFloat());
                    break;
                case VarType.List:
                    writer.Write(value.ListCount);
                    foreach (var item in value.GetList())
                        WriteVariable(item, writer);
                    break;
                default:
                    throw new Exception("Unable to write unknown variable type.");
            }
        }

        private Variable ReadVariable(BinaryReader reader)
        {
            VarType type = (VarType)reader.ReadInt32();
            switch (type)
            {
                case VarType.String:
                    return new Variable(reader.ReadString());
                case VarType.Integer:
                    return new Variable(reader.ReadInt32());
                case VarType.Float:
                    return new Variable(reader.ReadDouble());
                case VarType.List:
                    int count = reader.ReadInt32();
                    List<Variable> variables = new();
                    for (int i = 0; i < count; i++)
                        variables.Add(ReadVariable(reader));
                    return new Variable(variables);
                default:
                    throw new Exception("Unable to read unknown variable type.");
            }
        }

        /// <summary>
        /// Creates a <see cref="CompiledProgram"/> from one that was saved to a file.
        /// The target file must have been created using <see cref="CompiledProgram.Save(string)"/>.
        /// </summary>
        /// <param name="path">The filename and path.</param>
        /// <returns>The <see cref="CompiledProgram"/>.</returns>
        public static CompiledProgram FromFile(string path)
        {
            CompiledProgram program = new();
            program.Load(path);
            return program;
        }
    }
}
