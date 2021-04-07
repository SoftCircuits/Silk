# Silk Library

[![NuGet version (SoftCircuits.Silk)](https://img.shields.io/nuget/v/SoftCircuits.Silk.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.Silk/)

```
Install-Package SoftCircuits.Silk
```

## Overview

![Sample Program Screenshot](https://user-images.githubusercontent.com/3191331/113802537-1f980d80-9718-11eb-8dc7-f2947038ed06.PNG)

The Simple Interpreted Language Kit (SILK) is a .NET class library that makes it easy to add scripting and automation to your .NET applications.

The library includes three main components. A compiler, a compiled program, and a runtime. The compiler compiles the Silk source code to bytecode. This allows faster execution and also catches all source code syntax errors before running the Silk program.

The compiler produces a compiled program. A compiled program can be saved to a file, and later read from a file. This allows you to load and run a Silk program without recompiling it each time.

Finally, the runtime component executes a compiled program.

The main power of this library is that it allows you to register your own functions and variables with the compiler and those functions and variables can be called from the Silk program. When one of your registered functions is called, the `Function` event is raised, allowing the host application to provide key functionality specific to the host application's domain.

The Silk language itself is designed to be relatively easy to learn. It has no semicolons or other excessive punctuation, and the language is not case sensitive.

This project includes both the class library (Silk), and a test project/solution (TestSilk). If you download everything and run it, it will run the TestSilk application. You just need the class library to include Silk in your own projects.

## See Also:
- [Using the Silk Library](docs/UsingLibrary.md)
- [The Silk Language](docs/SilkLanguage.md)
- [Internal Functions and Variables](docs/InternalFunctions.md)
- [Sample Source Code](docs/Sample.md)
