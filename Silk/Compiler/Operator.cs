// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Constructs a new <see cref="Operator"/> instance.
    /// </summary>
    /// <param name="c">Operator character.</param>
    /// <param name="type">Operator token for specified character.</param>
    /// <param name="secondaries">Specifies the operator token if the
    /// second operator character matches any of these (for operators that
    /// are two characters long).
    /// </param>
    internal class Operator(char c, TokenType type, params Operator[] secondaries)
    {
        public char Char { get; set; } = c;
        public TokenType Type { get; set; } = type;
        public List<Operator> SecondaryChars { get; private set; } = new(secondaries);
    }
}
