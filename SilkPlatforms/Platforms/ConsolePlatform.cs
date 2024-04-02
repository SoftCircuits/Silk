// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;

namespace SilkPlatforms
{
    internal class ConsolePlatform : Platform
    {
        public ConsolePlatform()
            : base(SilkPlatform.Console)
        {
            // Add support functions
            AddFunction(new FunctionInfo("Print", Print, 0, 99));
            AddFunction(new FunctionInfo("PrintLine", PrintLine, 0, 99));
            AddFunction(new FunctionInfo("ReadLine", ReadLine, 0, 0));
            AddFunction(new FunctionInfo("ReadKey", ReadKey, 0, 0));
            AddFunction(new FunctionInfo("ClearScreen", ClearScreen, 0, 0));
            AddFunction(new FunctionInfo("SetColor", SetColor, 1, 2));

            // Add support variables
            foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
                AddVariable(new VariableInfo(color.ToString(), (int)color));
        }

        public override bool RequiresForm => false;

        public override void Begin()
        {
            ConsoleWindow.Title = Description;
            ConsoleWindow.DisableCloseButton();
            ConsoleWindow.CenterWindow();
            ConsoleWindow.Visible = true;

            Console.ResetColor();
            Console.Clear();
        }

        public override void End()
        {
            Console.ResetColor();
            Console.Write("Press any key to continue ");
            Console.ReadKey();
        }

        public override void CleanUp()
        {
            ConsoleWindow.Visible = false;
        }

        #region Functions

        private void Print(Variable[] parameters, Variable returnValue)
        {
            foreach (Variable variable in parameters)
                Console.Write(variable.ToString());
        }

        private void PrintLine(Variable[] parameters, Variable returnValue)
        {
            foreach (Variable variable in parameters)
                Console.Write(variable.ToString());
            Console.WriteLine();
        }

        private void ReadLine(Variable[] parameters, Variable returnValue)
        {
            returnValue.SetValue(Console.ReadLine() ?? string.Empty);
        }

        private void ReadKey(Variable[] parameters, Variable returnValue)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            returnValue.SetValue(keyInfo.KeyChar.ToString());
        }

        private void ClearScreen(Variable[] parameters, Variable returnValue)
        {
            Console.Clear();
        }

        private void SetColor(Variable[] parameters, Variable returnValue)
        {
            Console.ForegroundColor = (ConsoleColor)parameters[0].ToInteger();
            if (parameters.Length > 1)
                Console.BackgroundColor = (ConsoleColor)parameters[1].ToInteger();
        }

        #endregion

    }
}
