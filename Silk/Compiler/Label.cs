// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;

namespace SoftCircuits.Silk
{
    internal class Label(string name, int? ip = null)
    {
        public string Name { get; set; } = name;
        public int? IP { get; set; } = ip;
        public List<int> FixUpIPs { get; private set; } = [];
    }
}
