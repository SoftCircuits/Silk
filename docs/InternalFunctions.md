# Internal Functions and Variables

The Silk library makes no assumptions about the host application's domain or platform. It is up to the host application to define built-in (intrinsic) functions that can be referenced from the Silk source code.

That said, the library does provide provide some internal functions and variables. These are added by default when a Silk program is compiled. (You can disable this by setting the `Compiler.EnableInternalFunctions` property to false.) This page documents those internal functions and variables.

Note that when your application adds functions or variables (using `Compiler.RegisterFunction` or `Compiler.RegisterVariable`), you can override the internal ones by adding functions or variables with the same name as the internal ones.

## Functions

### Abs(*value*)

Returns the absolute value of *value*.

### Acos(*value*)

Returns the angle whose cosine is equal to *value*.

### Asc(*s*)

Returns the ASCII/Unicode value of the first character in the string *s*.

### Atn(*value*)

Returns an angle whose tangent is equal to *value*.

### Avg(*value*, ...)

Returns the average of the given arguments. Any number of arguments can be provided. In addition, any argument can be a list. In this case, each item in the list will be averaged.

### Bin(*value*)

Converts the given value to a binary (base 2) string.

### Chr(*value*)

Creates a string with one character with the specified ASCII/Unicode value.

### Cos(*value*)

Returns the cosine of *value*.

### Date()

Returns a string with the current date.

### Environ(*name*)

Returns the value of the specified environment variable.

### Exp(*value*)

Returns `e` raised to the specified power.

### Float(*value*)

Converts the given value to a floating point number.

### Hex(*value*)

Returns a hexedecimal string equal to the given value.

### InStr(*s1*, *s2*[, *start*])

Returns the 1-based position where the string *s2* appears within the the string *s1*. The optional *start* argument specifies the 1-based position to begin the search.

### Int(*value*)

Converts the *value* to an integer, truncating any fractional portion.

### IsList(*a*)

Returns true if the variable *a* contains a list.

### Left(*s*, *count*)

Returns a string with the left-most *count* characters from *s*.

### Len(*value*)

If *value* is a list, this function returns the number of items in the list. Otherwise, it returns the number of characters in *value*, converted to a string if needed.

### Max(*value*, ...)

Returns the maximum value of the given arguments. Any number of arguments can be provided. In addition, any argument can be a list. In this case, the maximum value of each item in the list is returned.

### Mid(*s*, *start*[, *count*])

Returns a section of the string given as the first argument. The *start* specifies the 1-based position where the string should be extracted. If *count* is provided, it specifies the maximum number of characters to return.

### Min(*value*, ...)

Returns the minimum value of the given arguments. Any number of arguments can be provided. In addition, any argument can be a list. In this case, the minimum value of each item in the list is returned.

### Oct(*value*)

Converts the given *value* to an octal string.

### Right(*s*, *count*)

Returns a string with the right-most *count* characters from *s*.

### Round(*value*)

Rounds the given value to the nearest integer.

### Sin(*value*)

Returns the sign of the angle specified by *value*.

### Sqr(*value*)

Returns the square root of the given argument.

### String(*value*, *count*)

Returns a string with *value* repeated the number of times specified by *count*.

If *value* is a string value, then that string is repeated. Otherwise, the string repeated will contain one character with the specified ASCII/Unicode value specified by *value*

### Tan(*value*)

Returns the tanget of the angle specified by *value*.

### Time()

Returns a string with the current time.

## Variables

### False

Represents Boolean false (0).

### True

Represents Boolean true (-1).
