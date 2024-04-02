// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Identifies a <see cref="Variable"/> type.
    /// </summary>
    public enum VarType
    {
        /// <summary>
        /// Indicates the variable holds a text value.
        /// </summary>
        String,
        /// <summary>
        /// Indicates the variable holds an integer value.
        /// </summary>
        Integer,
        /// <summary>
        /// Indicates the variable holds a floating-point value.
        /// </summary>
        Float,
        /// <summary>
        /// Indicates the variable holds a collection value.
        /// </summary>
        List,
    }
}
