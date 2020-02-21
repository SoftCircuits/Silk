// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;

namespace SoftCircuits.Silk
{
    internal class Operator
    {
        public char Char { get; set; }
        public TokenType Type { get; set; }
        public List<Operator> SecondaryChars { get; private set; }

        /// <summary>
        /// Constructs a new OperatorInfo instance.
        /// </summary>
        /// <param name="c">Operator character.</param>
        /// <param name="type">Operator token for specified character.</param>
        /// <param name="secondaries">Specifies the operator token if the
        /// second operator character matches any of these (for operators that
        /// are two characters long).
        /// </param>
        public Operator(char c, TokenType type, params Operator[] secondaries)
        {
            Char = c;
            Type = type;
            SecondaryChars = new List<Operator>(secondaries);
        }
    }
}
