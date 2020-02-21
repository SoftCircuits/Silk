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
    /// <summary>
    /// Represents a bytecode entry. Additional properties used primarily for
    /// creating bytecode log file, and for extracting line numbers for
    /// runtime.
    /// </summary>
    internal class ByteCodeEntry
    {
        public int Value { get; set; }
        public bool IsBytecode { get; set; }
        public int Line { get; set; }

        public ByteCodeEntry(ByteCode bytecode, int line)
        {
            Value = (int)bytecode;
            IsBytecode = true;
            Line = line;
        }

        public ByteCodeEntry(int value, int line)
        {
            Value = value;
            IsBytecode = false;
            Line = line;
        }

        public override string ToString()
        {
            if (IsBytecode)
            {
                return string.Format("ByteCode.{0} ({1})", (ByteCode)Value, Value);
            }
            else
            {
                string s = string.Format("{0} (0x{0:x08})", Value);
                ByteCodeVariableFlag flag = (ByteCodeVariableFlag)Value & ByteCodeVariableFlag.All;
                if (flag != ByteCodeVariableFlag.None)
                    s += $" : {flag}[{Value & ~(int)ByteCodeVariableFlag.All}]";
                return s;
            }
        }
    }

    /// <summary>
    /// Helper class for writing bytecode data.
    /// </summary>
    internal class ByteCodeWriter
    {
        private List<ByteCodeEntry> ByteCodeEntries;
        private List<int> Counters;
        private LexicalAnalyzer Lexer;

        /// <summary>
        /// Instruction pointer. Returns the current write position.
        /// </summary>
        public int IP => ByteCodeEntries.Count;

        public ByteCodeWriter(LexicalAnalyzer lexer)
        {
            // Bytecode size must not change (won't be able to read saved compiled data)
            Debug.Assert(sizeof(ByteCode) == sizeof(Int32));
            ByteCodeEntries = new List<ByteCodeEntry>();
            Counters = new List<int>();
            Counters.Add(0);
            Lexer = lexer ?? throw new ArgumentNullException(nameof(lexer));
        }

        /// <summary>
        /// Writes a bytecode value and increments the current counter.
        /// Returns the position at which this bytecode is written.
        /// </summary>
        /// <param name="bytecode">Bytecode to write.</param>
        public int Write(ByteCode bytecode)
        {
            int ip = IP;
            ByteCodeEntries.Add(new ByteCodeEntry(bytecode, Lexer.LastTokenLine));
            IncrementCount();
            return ip;
        }

        /// <summary>
        /// Writes an integer value and increments the current counter.
        /// Returns the position at which this value is written.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public int Write(int value)
        {
            int ip = IP;
            ByteCodeEntries.Add(new ByteCodeEntry(value, Lexer.LastTokenLine));
            IncrementCount();
            return ip;
        }

        /// <summary>
        /// Writes a bytecode and integer value pair and increments the current
        /// counter. The counter is incremented only once. Returns the
        /// position at which the bytecode is written.
        /// </summary>
        /// <param name="bytecode">Bytecode to write.</param>
        /// <param name="value">Value to write.</param>
        public int Write(ByteCode bytecode, int value)
        {
            int ip = IP;
            ByteCodeEntries.Add(new ByteCodeEntry(bytecode, Lexer.LastTokenLine));
            ByteCodeEntries.Add(new ByteCodeEntry(value, Lexer.LastTokenLine));
            IncrementCount();
            return ip;
        }

        /// <summary>
        /// Writes a bytecode over a previously written bytecode.
        /// Does not affect counter.
        /// </summary>
        /// <param name="ip">Position to write bytecode.</param>
        /// <param name="bytecode">Bytecode to write.</param>
        public void WriteAt(int ip, ByteCode bytecode)
        {
            Debug.Assert(ip >= 0 && ip < IP);
            Debug.Assert(ByteCodeEntries[ip].IsBytecode);
            ByteCodeEntries[ip].Value = (int)bytecode;
        }

        /// <summary>
        /// Writes a value over a previously written value.
        /// Does not affect counter.
        /// </summary>
        /// <param name="ip">Position to write value.</param>
        /// <param name="byteCode">Value to write.</param>
        public void WriteAt(int ip, int value)
        {
            Debug.Assert(ip >= 0 && ip < IP);
            Debug.Assert(!ByteCodeEntries[ip].IsBytecode);
            ByteCodeEntries[ip].Value = value;
        }

        /// <summary>
        /// Deletes the last value written. If the last write included
        /// a bytecode and a value combination, only the value will be
        /// deleted. Does not affect counter.
        /// </summary>
        public void UndoLastWrite()
        {
            Debug.Assert(ByteCodeEntries.Count > 0);
            if (ByteCodeEntries.Count > 0)
                ByteCodeEntries.RemoveAt(ByteCodeEntries.Count - 1);
        }

        #region Counters

        /// <summary>
        /// Resets the current counter to zero.
        /// </summary>
        public void ResetCounter()
        {
            Debug.Assert(Counters.Count > 0);
            Counters[Counters.Count - 1] = 0;
        }

        /// <summary>
        /// Increments the current counter. Normally, this is only called
        /// internally.
        /// </summary>
        private void IncrementCount()
        {
            Debug.Assert(Counters.Count > 0);
            Counters[Counters.Count - 1]++;
        }

        /// <summary>
        /// Returns the value of the current counter.
        /// </summary>
        public int Counter
        {
            get
            {
                Debug.Assert(Counters.Count > 0);
                return Counters[Counters.Count - 1];
            }
        }

        /// <summary>
        /// Saves the current counter and starts a new one. Restore the
        /// previous counter by calling PopCounter().
        /// </summary>
        public void PushCounter()
        {
            Counters.Add(0);
        }

        /// <summary>
        /// Restores the counter previously saved using PushCounter().
        /// </summary>
        public void PopCounter()
        {
            Debug.Assert(Counters.Count >= 2);
            // Don't delete the last counter
            if (Counters.Count >= 2)
                Counters.RemoveAt(Counters.Count - 1);
        }

        #endregion

        /// <summary>
        /// Dumps the bytecode log to a file.
        /// </summary>
        /// <param name="path">Name of file that contains the source code.</param>
        public void WriteLogFile(string sourcePath, string logPath)
        {
            // Read source file
            string[] lines = File.ReadAllLines(sourcePath);
            // Must only be called after creating bytecodes
            if (ByteCodeEntries == null || ByteCodeEntries.Count == 0)
            {
                Debug.Assert(false);
                return;
            }
            // Write log file
            int lastPrintedLine = 0;
            using (StreamWriter file = new StreamWriter(logPath, false))
            {
                file.WriteLine("SILK Bytecode Listing - Created {0:d} {0:t}", DateTime.Now);
                file.WriteLine("Source File: {0}", sourcePath);
                file.WriteLine();
                for (int i = 0; i < ByteCodeEntries.Count; i++)
                {
                    while (lastPrintedLine < ByteCodeEntries[i].Line)
                    {
                        file.WriteLine("Line {0} : {1}", lastPrintedLine + 1, lines[lastPrintedLine].Trim());
                        lastPrintedLine++;
                    }
                    file.WriteLine("\t{0:D5}: {1}", i, ByteCodeEntries[i].ToString());
                }
                // Write any trailing lines of source code
                while (lastPrintedLine < lines.Length)
                {
                    file.WriteLine("Line {0} : {1}", lastPrintedLine + 1, lines[lastPrintedLine].Trim());
                    lastPrintedLine++;
                }
            }
        }

        /// <summary>
        /// Returns the bytecodes created by this writer.
        /// </summary>
        public int[] GetBytecodes()
        {
            Debug.Assert(ByteCodeEntries != null && ByteCodeEntries.Count > 0);
            return ByteCodeEntries.Select(e => e.Value).ToArray();
        }

        /// <summary>
        /// Returns an array of line numbers. Each line number in the array
        /// corresponds to the bytecode at the same position in the bytecodes
        /// array.
        /// </summary>
        public int[] GetLineNumbers()
        {
            Debug.Assert(ByteCodeEntries != null && ByteCodeEntries.Count > 0);
            return ByteCodeEntries.Select(e => e.Line).ToArray();
        }
    }
}
