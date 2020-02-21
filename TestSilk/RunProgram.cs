// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TestSilk
{
    class FunctionInfo
    {
        public Action<Variable[], Variable> Handler { get; set; }
        public int MinParameters { get; set; }
        public int MaxParameters { get; set; }

        public FunctionInfo(Action<Variable[], Variable> handler, int minParameters, int maxParameters)
        {
            Handler = handler;
            MinParameters = minParameters;
            MaxParameters = maxParameters;
        }
    }

    public class RunProgram
    {
        private PictureBox PicCanvas;
        private Bitmap Bitmap;
        private Graphics Graphics;
        private Dictionary<string, FunctionInfo> FunctionLookup;

        public List<Error> Errors { get; private set; }

        public RunProgram()
        {
            FunctionLookup = new Dictionary<string, FunctionInfo>
            {
                ["Clear"] = new FunctionInfo(Clear, 0, 0),
                ["Width"] = new FunctionInfo(Width, 0, 0),
                ["Height"] = new FunctionInfo(Height, 0, 0),
                ["SetPixel"] = new FunctionInfo(SetPixel, 3, 3),

                //["Message"] = new FunctionInfo(Message, 0, Function.NoParameterLimit),
                //["Inspect"] = new FunctionInfo(Inspect, 1, Function.NoParameterLimit),
            };
        }

        public bool Compile(string path, out CompiledProgram program, bool createLogFile = false)
        {
            // Compile program
            Compiler compiler = new Compiler();
            compiler.CreateLogFile = createLogFile;

            // Register intrinsic functions
            foreach (string name in FunctionLookup.Keys)
            {
                FunctionInfo info = FunctionLookup[name];
                compiler.RegisterFunction(name, info.MinParameters, info.MaxParameters);
            }

            // Register intrinsic variables
            foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
                compiler.RegisterVariable(color.ToString(), new Variable((int)color));

            // Compile program
            if (compiler.Compile(path, out program))
            {
                return true;
            }
            else
            {
                Errors = compiler.Errors;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Variable Execute(CompiledProgram program, PictureBox picCanvas)
        {
            if (picCanvas == null)
                throw new NullReferenceException(nameof(picCanvas));

            Runtime runtime = new Runtime();
            runtime.Begin += Runtime_Begin;
            runtime.Function += Runtime_Function;
            runtime.End += Runtime_End;

            // TODO: Do we need to dispose of any of these objects?

            PicCanvas = picCanvas;
            Bitmap = new Bitmap(PicCanvas.ClientSize.Width, PicCanvas.ClientSize.Height);
            using (Graphics = Graphics.FromImage(Bitmap))
            {

                //using (Bitmap = new Bitmap(PicCanvas.ClientSize.Width, PicCanvas.ClientSize.Height))
                //using (Graphics = Graphics.FromImage(Bitmap))
                PicCanvas.Image = Bitmap;

                return runtime.Execute(program);
            }
        }

        private void Runtime_Begin(object sender, BeginEventArgs e)
        {
        }

        private void Runtime_Function(object sender, FunctionEventArgs e)
        {
            if (FunctionLookup.TryGetValue(e.Name, out FunctionInfo functionInfo))
                functionInfo.Handler(e.Parameters, e.ReturnValue);
            else
                Debug.Assert(false);    // Unknown function
        }

        private void Runtime_End(object sender, EndEventArgs e)
        {
        }

        #region Intrinsic functions

        private void Clear(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length <= 1);
            Color color = (parameters.Length > 0) ?
                Color.FromKnownColor((KnownColor)parameters[0].ToInteger()) :
                PicCanvas.BackColor;
            Graphics.Clear(color);
        }

        private void Width(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 0);
            returnValue.SetValue((double)PicCanvas.ClientSize.Width);
        }

        private void Height(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 0);
            returnValue.SetValue((double)PicCanvas.ClientSize.Height);
        }

        private void SetPixel(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 3);
            Color color = Color.FromKnownColor((KnownColor)parameters[2].ToInteger());
            Bitmap.SetPixel(parameters[0].ToInteger(), parameters[1].ToInteger(), color);
        }

        //private void Message(Variable[] parameters, Variable returnValue)
        //{
        //    MessageBox.Show(string.Join(" ", parameters.AsEnumerable()));
        //}

        //private void Inspect(Variable[] parameters, Variable returnValue)
        //{
        //    int i = parameters.Length;
        //}

        #endregion

    }
}
