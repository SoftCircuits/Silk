// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Helper class for reading bytecode data.
    /// </summary>
    internal class ByteCodeReader
    {
        private readonly int[] ByteCodes;
        private readonly Stack<int> IPStack;

        /// <summary>
        /// Instruction pointer (IP). Returns the current read position.
        /// </summary>
        public int IP { get; private set; }

        public ByteCodeReader(int[] bytecodes)
        {
            ByteCodes = bytecodes ?? throw new ArgumentNullException(nameof(bytecodes));
            IPStack = new Stack<int>();
            IP = 0;
        }

        public bool EndOfFile => (IP >= ByteCodes.Length);

        public ByteCode GetNext()
        {
            if (EndOfFile)
                throw new Exception("Attempted to read beyond the last bytecode");
#if DEBUG
            if (!Enum.IsDefined(typeof(ByteCode), ByteCodes[IP]))
                throw new Exception($"Attempted to read non-bytecode value {ByteCodes[IP]} as bytecode");
#endif
            return (ByteCode)ByteCodes[IP++];
        }

        public int GetNextValue()
        {
            if (EndOfFile)
                throw new Exception("Attempted to read beyond the last bytecode");
            return ByteCodes[IP++];
        }

        /// <summary>
        /// Jumps to the specified read position.
        /// </summary>
        /// <param name="ip"></param>
        public void GoTo(int ip)
        {
            Debug.Assert(ip >= 0 && ip < ByteCodes.Length);
            IP = ip;
        }

        /// <summary>
        /// Saves the current read position.
        /// </summary>
        public void Push()
        {
            IPStack.Push(IP);
        }

        /// <summary>
        /// Restores the read position previously saved with <see>Push</see>.
        /// </summary>
        public void Pop()
        {
            Debug.Assert(IPStack.Count > 0);
            GoTo(IPStack.Pop());
        }
    }
}
