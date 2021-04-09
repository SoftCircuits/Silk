// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    internal class LoopContext : IDisposable
    {
        private readonly CompileTimeUserFunction Function;
        private readonly ByteCodeWriter Writer;
        private int ContinueIP;

        /// <summary>
        /// IP location of the top of this loop.
        /// </summary>
        public int StartIP { get; set; }

        /// <summary>
        /// List of references to the break target IP that need to be set
        /// when that location is known.
        /// </summary>
        public List<int> BreakFixups { get; private set; }

        /// <summary>
        /// List of references to the continue target IP that need to be set
        /// when that location is known.
        /// </summary>
        public List<int> ContinueFixups { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="LoopContext"/> instance.
        /// </summary>
        /// <param name="function">The function that contains this loop. Must not be null.</param>
        /// <param name="writer">The <see cref="ByteCodeWriter"/>.</param>
        public LoopContext(CompileTimeUserFunction? function, ByteCodeWriter writer)
        {
            Debug.Assert(function != null);
            Function = function;
            Writer = writer;
            ContinueIP = StartIP = Writer.IP;
            BreakFixups = new();
            ContinueFixups = new();

            // Push this on the function's loop stack
            Function.LoopContexts.Push(this);
        }

        /// <summary>
        /// Sets the target IP for continue statements.
        /// </summary>
        public void SetContinueIP()
        {
            ContinueIP = Writer.IP;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                // Fixup break targets references
                int loopEndIp = Writer.IP;
                foreach (var ip in BreakFixups)
                    Writer.WriteAt(ip, loopEndIp);

                // Fixup continue targets
                foreach (var ip in ContinueFixups)
                    Writer.WriteAt(ip, ContinueIP);

                // Remove this context from loop stack
                Debug.Assert(Function.LoopContexts.Count > 0);
                Debug.Assert(Function.LoopContexts.Peek() == this);
                Function.LoopContexts.Pop();

                IsDisposed = true;
            }
        }
        private bool IsDisposed = false;
    }
}
