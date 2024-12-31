// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace TestSilk
{
    internal class SilkPlatformListItem(string text, SilkPlatform platform)
    {
        public string Text { get; set; } = text;
        public SilkPlatform Platform { get; set; } = platform;

        public override string ToString() => Text;
    }
}
