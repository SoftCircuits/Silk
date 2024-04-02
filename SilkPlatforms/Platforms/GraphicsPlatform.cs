// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
//#define LIVE_UPDATE     // Uncomment to have window refreshed as it's painted

using SoftCircuits.Silk;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SilkPlatforms
{
    internal class GraphicsPlatform : Platform
    {
        private Bitmap Bitmap;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public GraphicsPlatform()
            : base(SilkPlatform.Graphics)
        {
            // Add support functions
            AddFunction(new FunctionInfo("SetPixel", SetPixel, 3, 3));
            AddFunction(new FunctionInfo("Clear", Clear, 0, 0));
            AddFunction(new FunctionInfo("Width", Width, 0, 0));
            AddFunction(new FunctionInfo("Height", Height, 0, 0));

            // Add support variables
            foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
                AddVariable(new VariableInfo(color.ToString(), (int)color));
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public override void Begin()
        {
            // Drawing takes forever if image is too large
            Form!.SetBounds(0, 0, 500, 300, BoundsSpecified.Width | BoundsSpecified.Height);
            Form!.Center();
#if LIVE_UPDATE
            Form!.BufferedPainting = true;
#endif

            Bitmap = new Bitmap(Form!.ClientSize.Width, Form!.ClientSize.Height);

            Form!.Paint += Form_Paint;
        }

        public override void End()
        {
        }

        public override void CleanUp()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
        }

        #region Event handlers

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Bitmap, 0, 0, Form!.ClientSize.Width, Form!.ClientSize.Height);
        }

        #endregion

        #region Functions

        private void SetPixel(Variable[] parameters, Variable returnValue)
        {
            Color color = Color.FromKnownColor((KnownColor)parameters[2].ToInteger());
            Bitmap.SetPixel(parameters[0], parameters[1], color);

            Form!.Invalidate();
#if LIVE_UPDATE
            Form!.Update();
#endif
        }

        private void Clear(Variable[] parameters, Variable returnValue)
        {
            Color color = (parameters.Length > 0) ?
                Color.FromKnownColor((KnownColor)parameters[0].ToInteger()) :
                Form!.BackColor;

            using Graphics graphics = Graphics.FromImage(Bitmap!);
            graphics.Clear(color);

            Form!.Invalidate();
#if LIVE_UPDATE
            Form!.Update();
#endif
        }

        private void Width(Variable[] parameters, Variable returnValue)
        {
            returnValue.SetValue((double)Form!.ClientSize.Width);
        }

        private void Height(Variable[] parameters, Variable returnValue)
        {
            returnValue.SetValue((double)Form!.ClientSize.Height);
        }

        #endregion

    }
}
