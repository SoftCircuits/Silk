# Sample Source Code

Below are two sample Silk scripts.

### Example 1: Varied Tests

This script demonstrates various constructs of the Silk language. It makes the following assumptions about the host application:

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

### Example 2: Mandelbrot

This script renders the Mandelbrot set. Note that, depending on the size of the window, it can take a while for this script to run. An interpreted script is probably not the best choice for this task. But it does demonstrate some of the capabilities of the language.

**IMPORTANT: Unless you size the window small, it will take a long time to render when you run this program. This is not a typical use of this library. It is simply intended to show the flexibility of SILK.**

This script makes the following assumptions about the host application:

- It has registered an intrinsic function called `Clear`, which clears the current graphics canvas.
- It has registered an intrinsic function called `Width`, which returns the width of the current graphics canvas.
- It has registered an intrinsic function called `Height`, which returns the height of the current graphics canvas.
- It has registered an intrinsic function called `SetPixel`, which draws a pixel at the given coordinates.
- It has registered intrinsic variables with all the values of the `KnownColor` enum.
- It has not disabled the internal functions.

```
///////////////////////////////////////////////////////////////
// Silk - Manelbrot example script
// Adapted from:
// http://csharphelper.com/blog/2014/07/draw-a-mandelbrot-set-fractal-in-c/
//

var MAX_MAG_SQUARED = 4.0

var MaxIterations = 64
var Zr = 0.0
var Zim = 0.0
var Z2r = 0.0
var Z2im = 0.0

var m_Xmin = -2.2
var m_Xmax = 1.0
var m_Ymin = -1.2
var m_Ymax = 1.2

var Colors

Main()
{
    Colors = { Black, Red, Orange, Yellow, Green, Cyan, Blue, Magenta }
    DrawMandelbrot
}

DrawMandelbrot()
{
    // Clear the drawing surface
    Clear

    // Adjust the coordinate bounds to fit drawing surface
    AdjustAspect

    // dReaC is the change in the real part (X value) for C
    // dImaC is the change in the imaginary part (Y value)
    width = float(Width())
    height = float(Height())
    dReaC = (m_Xmax - m_Xmin) / (width - 1)
    dImaC = (m_Ymax - m_Ymin) / (height - 1)

    // Calculate the values
    num_colors = len(Colors)
    ReaC = m_Xmin
    for X = 0 to width - 1
    {
        ImaC = m_Ymin
        for Y = 0 to height - 1
        {
            ReaZ = Zr
            ImaZ = Zim
            ReaZ2 = Z2r
            ImaZ2 = Z2im
            clr = 1
            while ((clr < MaxIterations) and (ReaZ2 + ImaZ2 < MAX_MAG_SQUARED))
            {
                // Calculate Z (clr)
                ReaZ2 = ReaZ * ReaZ
                ImaZ2 = ImaZ * ImaZ
                ImaZ = 2 * ImaZ * ReaZ + ImaC
                ReaZ = ReaZ2 - ImaZ2 + ReaC
                clr = clr + 1
            }
            // Set the pixel's value
            SetPixel X, Y, Colors[(clr % num_colors) + 1]

            ImaC = ImaC + dImaC
        }
        ReaC = ReaC + dReaC
    }
}

// Adjust the aspect ratio of the selected coordinates so they fit
// the window properly
AdjustAspect()
{
    want_aspect = (m_Ymax - m_Ymin) / (m_Xmax - m_Xmin)
    picCanvas_aspect = float(Height()) / float(Width())
    if (want_aspect > picCanvas_aspect)
    {
        // The selected area is too tall and thin.
        // Make it wider.
        width = (m_Ymax - m_Ymin) / picCanvas_aspect
        mid = (m_Xmin + m_Xmax) / 2
        m_Xmin = mid - width / 2
        m_Xmax = mid + width / 2
    }
    else
    {
        // The selected area is too short and wide.
        // Make it taller.
        height = (m_Xmax - m_Xmin) * picCanvas_aspect
        mid = (m_Ymin + m_Ymax) / 2
        m_Ymin = mid - height / 2
        m_Ymax = mid + height / 2
    }
}
```
