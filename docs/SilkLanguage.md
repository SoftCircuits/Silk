# The Silk Language

The Silk language is designed to be minimal and relatively easy to use. It is not case-sensitive. There are no semicolons or other excessive punctuation. It's very loose with data types: you can mix integers, floating-point and string values in expressions. If possible, the language will simply do the best it can with expressions that don't make sense rather than raise an error or exception.

The language is also streamlined in that it has a small set of internal functions. For example, there are no functions to display information to, or get input from the user. Rather, the library makes no assumptions about the environment or whether the app will be a console or windowed application. It is up to the host application to provide functions for needed tasks, which the library makes very easy to do. The host application will need to define these functions to make a Silk program truly useful.

This page describes the Silk language itself.

## Functions

All statements must appear inside of a function. (The one exception to this is when defining global variables, which is described in the Variables section.) A function is declared by the function name followed by open and closing parentheses. Optionally, identifier can appear within the parentheses. Multiple identifiers must be separated by commas. These identifiers specify the function's parameters, which will take on the value of arguments passed to the function.

The parentheses must be followed by open and closing curly braces. The area between the curly braces is the function's body. This is where you will write statements. Functions cannot be nested (defined inside of another function).

All programs must define a function called `main()`. When the program is executed, this function will be called by the runtime. And so the `main()` function is where execution begins. All other functions in the code will only be called if the code explicitly calls them.

The following example creates an empty function called `main()`.

```cs
main()
{
}
```

Execution will return to the previous function when the closing curly brace is reached. When the main function returns, the program terminates. In addition, the `return` keyword can be used to return at any point within the function. The return keyword can optionally be followed by any valid expression. In this case, the expression will be evaluated and the result will be returned by the function. Note that `Runtime.Execute()` will return the value returned by `main()`.

When calling a function with no arguments, you can simply enter the function name. This is demonstrated in the following example, which assumes the host application has registered a function called `print()`.

```cs
main()
{
    print
}
```

To pass arguments, the function name should be followed by the argument values. Multiple arguments should be separated by commas.

```cs
main()
{
    print "Hello world!", 50
}
```

When using the result from a function, (for example, when using a function as part of an expression), the function name must be followed by open and closing parentheses. Any arguments to that function should appear within those parentheses.

The following example declares a function named `main()`, and another function named `double()`. The `main()` function calls `double()`, which takes a single argument and multiplies the argument by 2. When the result is returned to `main()`, it is assigned to the variable `i`. The result in this case is that the variable `i` would have the value 4.

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

Within a function, you can declare a variable simply by assigning a value to an identifier. The following example declares a variable named `i` and gives it a value of 5.

```cs
main()
{
    i = 5
}
```

Optionally, you can preceed the identifier with the var keyword. When you use the var keyword, assigning a value to the variable is optional. When you don't assign a value, the variable will have the default value of 0. So the following two examples would produce the same result.

Example 1:

```cs
main()
{
    var i
}
```

Example 2:

```cs
main()
{
    i = 0
}
```

The variables described above are local to the function where they are declared. This means that if you declare a variable with the same name in two different functions, they will be independent and contain completely separate values.

If you want to share a variable between functions, you can declare a global variable. All global variables must be declared before the first function in your source code. This is the only case where code can appear outside of a function. When declaring a global variable, the `var` keyword is required. It is optional to initialize the variable with a value. Note: If you do initialize a global variable, the values must be literals (no variables or function return values).

The following example declares two global variables. The first time the `doSomething()` function is called, it assigns the value 5 to `i`. The second time it is called, it would assign the value 10 to `i`.

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

Note that global variables are not initialized at runtime. Instead, the compiler stores the value of global variables with the `CompiledProgram` and the global variables are already initialized when the runtime loads the program. Contrast this to local variables, which must be initialized at runtime.

## Lists (Arrays)

Silk also supports lists. There are two ways to create a list. The first way is using square brackets to specify the size of the list.

```cs
main()
{
    a = [10]
}
```

The example above creates a list with 10 variables. It does not initialize the values of those variables, and so they all have the default value of 0.

The second approach uses curly braces to initialize the value of each variable in the list.

```cs
main()
{
    a = { "abc", 27, "def", 50, 1.5, [5], { 1, 2, 3 } }
}
```

The example above creates a list with seven variables and initializes the value of each variable. As you can see, each variable can be initialized to any type of value, including other lists.

You can use square brackets to access variables in a list. The number within the square brackets specifies the *1-based* list index. So the following example would assign a value to the first variable in the list.

```cs
main()
{
    a[1] = 25
}
```

And the next example reads the variable at the fifth position within the list, and assigns it to a variable called `i`.

```cs
main()
{
    i = a[5]
}
```

As with other variables, global lists can be created using the `var` keyword before any functions are defined.

As when intializing other global variables, the values must be literals (no variables or function return values). If a list is initialized within a function, default values can include expressions, variables and function return values.

## Code

Although the Silk language is C-like, there are a number of differences, primarily in an attempt to simplify the language. Silk has no semicolons at the end of each line. And in general only one statement is allowed per line.

Parentheses are not required around logical expressions in `if`, `while` and `for` clauses. Also, parentheses are not required when calling a function unless the return value of the function is being used.

## Comments

The SILK language supports C-style comments. There are two forms of comments.

Line Comment
A line comment is signifies by two forward slashes (`//`). The forward slashes and everything that follows on the same line will be considered as a comment and ignored by the compiler.

Multiline Comment
A multiline comment starts with a forward slash and asterisk (`/*`) and ends with an asterisk and forward slash (`*/`). These delimiters and anything that appears between them will be considered a comment and ignored by the compiler.

## Structures

The Silk language supports a number of standard language structures.

#### If

The `if` keyword allows you to conditionally execute a block of code. The following line is executed only if the `if` condition is true. If the `if` statement is followed by a pair of curly braces, all statements within the curly braces are executed only if the condition is true. (The curly braces are optional if they contain only one statement.)

An `if` block can be followed by an `else` block. The code in the `else` block is only executed if the `if` condition was false. In addition, the `else` keyword can be followed by another `if` expression. In this case, the code in the `else` block will only be executed if the original `if` condition was false and the new `if` condition is true. You can have any number of `else if` sections. There can be no more than one `else` without `if` section, and it must always come last.

```cs
main()
{
    if i >= 5 and i <= 10
        print "i is between 5 and 10"
    else
    {
        print "i is not between 5 and 10"
        return
    }
}
```
#### GoTo

The `GoTo` keyword jumps to another location within the same function. Locations are defined using labels. You can create a label using the label name followed by a colon. The following example jumps over a statement.

```cs
main()
{
    goto label1
    print "This statement will be skipped over"
Label1:
    print "This is the next statement executed after GoTo"
}
```

#### While

A while loop executes a block of code as long as a condition is true. The curly braces are optional if the code block contains only one statement.

The following example loops as long as the variable `i` is less then 50. Please note the line that adds 1 to `i`. If this line was removed, the loop would repeat forever. Be careful when using the `while` statement to ensure that it will eventually terminate.

```cs
main()
{
    while i < 50
    {
        print i
        i = i + 1
    }
}
```

#### For

The `for` loop executes a block of code similar to a `for` loop. The curly braces are optional if the code block contains only one statement.

The `for` loop requires a variable. By default, the variable will be incremented by 1 each time after executing the loop. You can override this using the optional `step` keyword. If the `step` keyword is used, it can be followed by any positive or negative number (integer or fractional). However, the `step` value must be a literal (it cannot include expressions, variables or function return values).

```cs
main()
{
    for i = 100 to 1 step -1
    {
        print i
    }
}
```

## Operators

Silk supports the following operators.

| Operator  | Meaning |
| ------------- | ------------- |
| + | Addition, unary positive  |
| - | Subtraction, unary negative |
| * | Multiplication |
| / | Division |
| ^ | Raise to power |
| % | Modulus |
| & | Concatenation |
| = | Equal, assignment |
| <> | Not equal |
| < | Less than |
| <= | Less than or equal |
| > | Greater than |
| >= | Greater than or equal |
| And | Logical and bitwise AND |
| Or | Logical and bitwise OR |
| Xor | Logical and bitwise XOR |
| Not | Logical and bitwise NOT |
| // | Line comment |
| /* | Multiline comment start |
| */ | Multiline commend end |

Note that, like some earlier versions of the BASIC language, the logical operators (`And`, `Or`, `Xor`, and `Not`) do bitwise operations. But they also work for logical operations. A Silk expression that evaluates logically to true returns -1 (all bits set), although any non-zero value is considered to be true. And so Silk does not provide separate logical and bitwise operators.
