## Introduction

The Simple Interpreted Language Kit (SILK) is a .NET class library that makes it easy to add scripting and automation to any .NET application.

The language is designed to be minimal and relatively easy to use. It's very loose with data types: you can mix integers, floating-point and string values in expressions. If possible, the language will simply do the best it can with expressions that don't make sense rather than raise an error or exception.

The language is also streamlined in that it has a small set of internal functions. For example, there are no functions to display information to, or get input from the user. Rather, the library makes no assumptions about the environment or whether the app will be a console or windowed application. It is up to the host application to provide functions for needed tasks, which the library makes very easy to do.

## Functions

All statements must appear inside a function. A function is declared by the function name, followed by
open and closing parentheses. Optionally, identifier names can appear within the parentheses. Multiple
identifiers must be separated by commas. These identifiers specify the function's parameters, which will
take on the value of arguments passed to the function.

The parentheses must be followed by open and closing curly braces. The statements for this function
must appear within these curly braces. Functions cannot be nested (defined inside of another function).

All programs must define a function called main. When the program is executed, this function will be called
by the runtime. And so the main function is where execution begins. All other functions in the code will only
be called if the code explicitly calls them.

The following example creates an empty function called main.

```cs
main()
{
}
```

Execution will return to the previous function when the closing curly brace is reached. In addition, the
return keyword can be used to return at any point within the function. The return keyword can optionally
be followed by any valid expression. In this case, the expression will be evaluated and the result will
be returned by the function.

The following example declares a function named main, and another function named double. The main function
calls double, which takes a single argument and multiplies it by 2, and assigns the result to a variable
named i. The result is that the variable i would have the value 4.

```cs
main()
{
    i = double(2)
}

double(value)
{
    return value * 2
}
```

## Variables

A variable is an identifer that represents a value. Variables can be declared a couple of ways.

Within a function, you can declare a variable simply by assigning a value to an identifier. The
following example declares a variable named i and gives it a value of 5.

```cs
i = 5
```

Optionally, you can preceed the identifier with the var keyword. When you use the var keyword,
assigning a value to the variable is optional. When you don't assign a value, the variable will
have the default value of 0. So the following two examples would produce the same result.

Example 1:

```cs
var i
```

Example 2:

```cs
i = 0
```

The variables described above are local to the function where they are declared. This means that
if you declare a variable with the same name in two different functions, they will be independent
and contain completely separate values.

If you want to share a variable between functions, you can declare a global variable. All global
variables must be declared before the first function in your source code. This is the only case
where code can appear outside of a function. When declaring a global variable, the var keyword
is required. It is optional to initialize the variable with a value.

The following example declares two global variables. The first time doSomething is called, it
assigns the value 5 to i. The second time it is called, it assigns the value 10 to i.

```cs
var i       // Initialized with default value of 0
var j = 5   // Initialized to 5

main()
{
    doSomething
    j = 10
    doSomething
}

doSomething()
{
    i = j
}
```

## Lists (Arrays)

Silk also supports lists. There are two ways to create a list. The first way is using square
brackets to specify the size of the list.

```cs
a = [10]
```

The example above creates a list with 10 variables. It does not initialize the values of those
variables, and so they all have default values (the default value is zero).

The second approach uses curly braces to initialize the value of each variable in the list.

```cs
a = { "abc", 27, "def", 50, 1.5 }
```

The example above creates a list with five variables and initializes the value of each variable.
As you can see, each variable can be initialized to any type of value, including other lists.

You can use square brackets to access variables in the list. The number within the square
brackets specifies the 1-based list index. So the following example would assign a value to the
first variable in the list.

a[1] = 25

And the next example reads the variable at the fifth position within the list.

i = a[5]

Global lists can be created using the VAR keyword before any functions are defined. The following
example creates a global list with ten variables, two of the variables are also lists.

```cs
var global = { 1, 2, 3, 4, 5, 6, 7, "abc", [10], { 10, 20, 30 } }
```

As when intializing global variables before any functions are defined, the variable values must
be literals (no variables or function return values). If a list is initialized within a function,
default values can include expressions, variables and function return values.

## Code

No more than one statement per line.

## Comments

The SILK language supports C-style comments. There are two forms of comments.

Line Comment
A line comment is signifies by two forward slashes (`//`). The forward slashes and everything that follows on
the same line will be considered as a comment and ignored by the compiler.

Multiline Comment
A multiline comment starts with a forward slash and asterisk (`/*`) and ends with an asterisk and forward
slash (`*/`). These delimiters and anything that appears between them will be considered a comment and ignored
by the compiler.




Intrinsic functions
-> Number of parameters passed restricted per settings at compile time
