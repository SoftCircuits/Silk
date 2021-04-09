// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using SilkPlatforms;

namespace SilkExamples.Examples
{
    public class ListsExample : IExample
    {
        public string Description => "Lists Example";

        public string SourceCode => @"///////////////////////////////////////////////////////////////
// Silk - Sample script
//

// Variables declared here are global to all functions
var array = { 20, 30, ""abc"", 40, ""def"", 50, [5], { 1, 2, 3 } }

main()
{
    // Set colors and clear screen
    setcolor white, darkblue
    clearscreen

    // Print global array
    printline ""Array: "" & array
    printline

    printline ""Array elements:""
    for i = 1 to len(array)
    {
        if (islist(array[i]))
        {
            printline ""  Array:""
            for j = 1 to len(array[i])
                printline ""    "", array[i][j]
        }
        else printline ""  "", array[i]
    }
    printline

    // Print local array
    months = {
        ""January"",
        ""February"",
        ""March"",
        ""April"",
        ""May"",
        ""June"",
        ""July"",
        ""August"",
        ""September"",
        ""October"",
        ""November"",
        ""December""
    }
    printline ""Months: "", months
    printline

    // Print each element in array
    printline ""Month elements:""
    for i = 1 to len(months)
        printline ""  "", months[i]
    printline
}";

        public SilkPlatform Platform { get; init; } = SilkPlatform.Console;
    }
}
