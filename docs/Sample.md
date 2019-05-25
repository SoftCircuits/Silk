# Sample Source Code

Below is a sample Silk source file. It makes the following assumptions about the host application:

- It has registered an intrinsic function called `Color`, which sets the foreground color and, optionally (if two arguments are passed), the background color.
- It has registered intrinsic variables with all the standard colors (Black, Blue, White, etc.).
- It has registered an intrinsic function called `ClearScreen`, which clears the console window.
- It has registered an intrinsic function called `PrintLn`, which prints all the arguments to the console window, followed by a new line.
- It has registered an intrinsic function called `Print`, which prints all the arguments to the console window (no new line).
- It has not disabled the internal functions.

```
///////////////////////////////////////////////////////////////
// Silk - Sample script
//

// Variables declared here are global to all functions
var array = { 20, 30, "abc", 40, "def", 50, [5], { 1, 2, 3 } }

main()
{
    // Set colors and clear screen
    color white, blue
    clearscreen

    // Print header
    println string("*", 78)
    println "* SILK Example Program"
    println string("*", 78)
    println

    // Print global array
    println "Array: " & array
    println

    // Print local array
    months = {
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    }
    println "Months: ", months
    println

    // Print each element in array
    println "Array Elements:"
    for i = 1 to len(array)
        println "  \"" & array[i] & "\""
    println

    // Expressions
    println "Expressions:"
    println "  2 + 2 = ", 2 + 2
    println "  50 / (2 + 3) = ", 50 / (2 + 3)
    println "  (2 + 7) * (2 * (8 + double(5) * (100 + triple(7)))) = ", (2 + 7) * (2 * (8 + double(5) * (100 + triple(7))))
    println

    // Characters
    println "Characters:"
    for i = 48 to 126
        print chr(i)
    println
}

double(x)
{
    return x * 2
}

triple(x)
{
    return x * 3
}
```
