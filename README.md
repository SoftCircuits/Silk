# Silk Overview

The Simple Interpreted Language Kit (SILK) was designed to make it easy to add scripting and automation to your .NET applications.

The package include three main components. A compiler, a compiled program, and a runtime. The compiler compiles the Silk source code to bytecode. This allows faster execution and also catches all source code syntax errors before running the Silk program.

The compiler produces a compiled program. A compiled program can be saved to a file, and later read from a file. This allows you to load and run a Silk program without recompiling it each time.

Finally, the runtime component executes a compiled program.

The main power of this library is that it allows you to register your own functions and variables with the compiler and those functions and variables can be called from the Silk program. When one of your registered functions is called, the `Function` event is raised, allowing the host application to provide key functionality specific to the host application's domain.

The Silk language itself is designed to be relatively easy to learn. It has no semicolons or other, excess punctuation, and the language is not case sensitive.

## Also See:
- [Using the Silk Library](docs/UsingLibrary.md)
- [The Silk Language](docs/SilkLanguage.md)
- [Internal Functions and Variables](docs/InternalFunctions.md)
