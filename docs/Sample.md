# Sample Source Code

This page provide some sample Silk scripts (source code).

### Payment Calculator Example

This example calculates the principle and interest payment amount on a loan. It assumes the host application defines the functions `Print()`, `PrintLine()`, `ReadLine()`, `ReadKey()`, `ClearScreen()` and `SetColor()`. It also assumed the host application has defined color variables for user with `SetColor()`.

```
///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    setcolor white, darkblue
    clearscreen

    print "Enter loan amount: "
    loanamount = readline()

    print "Enter number of years: "
    payments = readline()

    print "Enter the interest rate percent: "
    interestrate = readline()

    print "Payment (principle and interest) is: "
    print "$", round(calculatepayment(loanamount, payments * 12, interestrate / 100 / 12), 2)
    printline
}

calculatepayment(loanamount, payments, interestrate)
{
    if payments = 0
        return loanamount

    if interestrate = 0
        return loanamount / payments

    temp = pow(interestrate + 1.0, payments)

    return -(-(loanamount * temp) / ((temp - 1) / interestRate))
}
```

### Prime Numbers Example

This example displays all the prime numbers within the specified range. It assumes the host application defines the functions `Print()`, `PrintLine()`, `ReadLine()`, `ReadKey()`, `ClearScreen()` and `SetColor()`. It also assumed the host application has defined color variables for user with `SetColor()`.

```
///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    // Set colors and clear screen
    setcolor white, darkblue
    clearscreen

    // Get start number
    print "Enter starting number: "
    start = readline()
    if start = ""
        return

    // Get end number
    print "Enter ending number: "
    end = readline()
    if end = ""
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
}
```

### Leap Year Example

This example indicates whether or not the entered years are leap years. It assumes the host application defines the functions `Print()`, `PrintLine()`, `ReadLine()`, `ReadKey()`, `ClearScreen()` and `SetColor()`. It also assumed the host application has defined color variables for user with `SetColor()`.

```
///////////////////////////////////////////////////////////////
// Silk - Sample script
//

main()
{
    // Set colors and clear screen
    setcolor cyan, darkblue
    clearscreen

    // Get year
    print "Enter year: "
    year = readline()
    while len(year) = 4
    {
        print year & " is"
        if not isleapyear(year)
            print " not"
        printline " a leap year"
        printline

        print "Enter year: "
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
}
```

### Mandelbrot Example
This example paints a Mandelbrot image. This is probably not an idea application of the Silk library as the image can take a while to render, especially if the window is large. But it does show the flexibility and power of Silk. This examplle assumes the host application defines the functions `SetPixel()`, `Clear()`, `Width()` and `Height()`. It also assumes the host application has defined color variables for use with `SetPixel()`.

```
///////////////////////////////////////////////////////////////
// Silk - Mandelbrot example script
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
