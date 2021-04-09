﻿// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkPlatforms;

namespace SilkExamples.Examples
{
    public class MandelbrotExample : IExample
    {
        public string Description => "Mandelbrot Example";

        public string SourceCode => @"///////////////////////////////////////////////////////////////
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
}";

        public SilkPlatform Platform { get; init; } = SilkPlatform.Graphics;
    }
}
