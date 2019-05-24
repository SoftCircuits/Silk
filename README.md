# Silk
The Simple Interpreted Language Kit (SILK) was designed to make it easy to add scripting and automation to your .NET applications.

## Using the Library

The Silk library is designed to be very simple to use. The first step is to create an instance of the `Compiler` class.

The next step is the critical part. Use the `RegisterFunction` and `RegisterVariable` methods to add functions and variables, which will be available to the source code. This will give the language the ability to perform the tasks you choose specific to the domain of your application.

Next, call the `Compile` method. If it returns `false`, the compile failed and you can use the `Errors` property to access the compile errors. Otherwise, the `Compile` method returns an instance of the `CompiledProgram` class.

This is demonstrated in the following example.

```cs
Compiler compiler = new Compiler();
compiler.EnableLineNumbers = true;

// Register intrinsic functions
compiler.RegisterFunction("Print", 0, Function.NoParameterLimit);
compiler.RegisterFunction("Color", 1, 2);
compiler.RegisterFunction("ClearScreen", 0, 0);
compiler.RegisterFunction("ReadKey", 0, 0);

// Register intrinsic variables
foreach (var color in Enum.GetValues(typeof(ConsoleColor)))
  compiler.RegisterVariable(color.ToString(), new Variable((int)color));

if (compiler.Compile(path, out CompiledProgram program))
{
  Console.WriteLine("Compile succeeded.");
}
else
{
  Console.WriteLine("Compile failed.");
  Console.WriteLine();
  foreach (Error error in compiler.Errors)
    Console.WriteLine(error.ToString());
}
```

The `CompiledProgram` object contains the compiled code. You can use this class' `Save` and `Load` methods to save a compiled program to a file, and load a compiled program from a file. This allows you to load a previously compiled program and run it without needing to compile it each time.

To run a `CompiledProgram`, create an instance of the `Runtime` class and pass the `CompiledProgram` to the `Execute` method. The `Runtime` class has three methods: `Begin`, `Function` and `End`.

The `Begin` event is called when the program starts to run. The `BeginEventArgs` argument contains a property called `UserData`. You can use this property to store any contextual data in your application and this same object will be passed to the other `Runtime` events.

The `Function` event is called when the program executes a call that you registered with `Compiler.RegisterFunction`. The `FunctionEventArgs` includes a Parameters` property, which is an array of arguments that were passed to the function. And it includes a `ReturnValue` property, which specifies the function's return value.

This is demonstrated by the following example.

```cs
private void RunProgram(CompiledProgram program)
{
  Runtime runtime = new Runtime();
  runtime.Begin += Runtime_Begin;
  runtime.Function += Runtime_Function;
  runtime.End += Runtime_End;

  Variable result = runtime.Execute(program);

  Console.WriteLine();
  Console.WriteLine($"Program ran successfully with exit code {result}.");
}

private static void Runtime_Begin(object sender, BeginEventArgs e)
{
  e.UserData = this;
}

private static void Runtime_Function(object sender, FunctionEventArgs e)
{
  switch (e.Name)
  {
    case "Print":
      Console.WriteLine(string.Join('\t', e.Parameters.Select(p => p.ToString())));
      break;
    case "Color":
      Debug.Assert(e.Parameters.Length >= 1);
      Debug.Assert(e.Parameters.Length <= 2);
      if (e.Parameters.Length >= 1)
        Console.ForegroundColor = (ConsoleColor)e.Parameters[0].ToInteger();
      if (e.Parameters.Length >= 2)
        Console.BackgroundColor = (ConsoleColor)e.Parameters[1].ToInteger();
      break;
    case "ClearScreen":
      Console.Clear();
      break;
    case "ReadKey":
      e.ReturnValue.SetValue(Console.ReadKey().KeyChar);
      break;
    default:
      Debug.Assert(false);
      break;
  }
}

private static void Runtime_End(object sender, EndEventArgs e)
{
}
```
