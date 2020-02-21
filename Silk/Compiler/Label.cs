// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;

namespace SoftCircuits.Silk
{
    internal class Label
    {
        public string Name { get; set; }
        public int? IP { get; set; }
        public List<int> FixUpIPs { get; private set; }

        public Label(string name, int? ip = null)
        {
            Name = name;
            IP = ip;
            FixUpIPs = new List<int>();
        }
    }
}
