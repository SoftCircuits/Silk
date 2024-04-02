// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using SilkPlatforms;

namespace SilkExamples.Examples
{
    public class ExpressionsExample : IExample
    {
        public string Description => "Expressions Example";

        public string SourceCode => """
            ///////////////////////////////////////////////////////////////
            // Silk - Sample script
            //

            main()
            {
                // Set colors and clear screen
                setcolor white, darkblue
                clearscreen

                // Print header
                printline string("*", 78)
                printline "* SILK Example Program"
                printline string("*", 78)
                printline

                printline "Expressions:"
                printline "  2 + 2 = ", 2 + 2
                printline "  2 * 2 = ", 2 * 2
                printline "  2 + 7 * 3 = ", 2 + 7 * 3
                printline "  (2 + 7) * 3 = ", (2 + 7) * 3
                printline "  50 / (2 + 3) = ", 50 / (2 + 3)
                printline "  (2 + 7) * (2 * (8 + double(5) * (100 + triple(7)))) = ", (2 + 7) * (2 * (8 + double(5) * (100 + triple(7))))
                printline "  \"abc\" * 5 = ", "abc" * 5
                printline "  \"123\" * 5 = ", "123" * 5
                printline "  \"abc\" + 5 = ", "abc" + 5
                printline "  \"123\" + 5 = ", "123" + 5
                printline "  \"2.3\" + 5 = ", "2.3" + 5
                printline "  \"2.3\" & 5 = ", "2.3" & 5

                printline
            }

            double (x)
            {
                return x * 2
            }

            triple(x)
            {
                return x * 3
            }
            """;

        public SilkPlatform Platform { get; init; } = SilkPlatform.Console;
    }
}
