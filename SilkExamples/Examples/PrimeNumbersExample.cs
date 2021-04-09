// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace SilkExamples.Examples
{
    public class PrimeNumbersExample : IExample
    {
        public string Description => "Prime Numbers Example";

        public string SourceCode => @"///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    // Set colors and clear screen
    setcolor white, darkblue
    clearscreen

    // Get start number
    print ""Enter starting number: ""
    start = readline()
    if start = """"
        return

    // Get end number
    print ""Enter ending number: ""
    end = readline()
    if end = """"
        return

    for i = start to end
    {
        flag_var = 0
        for j = 2 to i / 2
        {
            if(i % j = 0)
            {
                flag_var = 1
                break
            }
        }
        if flag_var = 0
            printline i
    }
}";

        public SilkPlatform Platform { get; init; } = SilkPlatform.Console;
    }
}
