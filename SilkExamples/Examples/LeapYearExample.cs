// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace SilkExamples.Examples
{
    public class LeapYearExample : IExample
    {
        public string Description => "Leap Year Example";

        public string SourceCode => @"///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    // Set colors and clear screen
    setcolor cyan, darkblue
    clearscreen

    // Get year
    print ""Enter year: ""
    year = readline()
    while len(year) = 4
    {
        print year & "" is""
        if not isleapyear(year)
            print "" not""
        printline "" a leap year""
        printline

        print ""Enter year: ""
        year = readline()
    }
}

isleapyear(y)
{
    // leap year if perfectly divisible by 400
    if (y % 400 = 0)
        return true
    // not a leap year if divisible by 100
    // but not divisible by 400
    if (y % 100 = 0)
        return false
    // leap year if not divisible by 100
    // but divisible by 4
    if (y % 4 = 0)
        return true
    // all other years are not leap years
    return false
}";

        public SilkPlatform Platform { get; init; } = SilkPlatform.Console;
    }
}
