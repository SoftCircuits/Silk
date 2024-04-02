// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace TestSilk
{
    class SilkPlatformListItem
    {
        public string Text { get; set; }
        public SilkPlatform Platform { get; set; }
        public SilkPlatformListItem(string text, SilkPlatform platform)
        {
            Text = text;
            Platform = platform;
        }
        public override string ToString() => Text;
    }
}
