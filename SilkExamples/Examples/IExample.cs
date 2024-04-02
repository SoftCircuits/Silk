// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace SilkExamples.Examples
{
    public interface IExample
    {
        public string Description { get; }
        public string SourceCode { get; }
        public SilkPlatform Platform { get; }
    }
}
