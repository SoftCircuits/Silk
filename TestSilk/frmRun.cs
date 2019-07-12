// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Windows.Forms;

namespace TestSilk
{
    public partial class frmRun : Form
    {
        private RunProgram RunProgram;
        private CompiledProgram Program;

        public frmRun(RunProgram runProgram, CompiledProgram program)
        {
            if (runProgram == null)
                throw new NullReferenceException(nameof(runProgram));
            if (program == null)
                throw new NullReferenceException(nameof(program));

            RunProgram = runProgram;
            Program = program;
            InitializeComponent();
        }

        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            picCanvas.Image = null;
            DoRun();
        }

        private void DoRun()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                RunProgram.Execute(Program, picCanvas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running script : {ex.Message}");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

    }
}
