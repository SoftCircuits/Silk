// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TestSilk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Document handling

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.New();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.Open();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.Save();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentManager1.SaveAs();
        }

        private void DocumentManager1_FileChanged(object sender, EmailBlaster.DocumentEventArgs e)
        {
            Text = $"{documentManager1.FileTitle} - Test Silk";
        }

        private void DocumentManager1_NewFile(object sender, EmailBlaster.DocumentEventArgs e)
        {
            txtScript.Text = string.Empty;
        }

        private void DocumentManager1_ReadFile(object sender, EmailBlaster.DocumentEventArgs e)
        {
            txtScript.Text = File.ReadAllText(e.FileName);
        }

        private void DocumentManager1_WriteFile(object sender, EmailBlaster.DocumentEventArgs e)
        {
            File.WriteAllText(e.FileName, txtScript.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!documentManager1.PromptSaveIfModified())
                e.Cancel = true;
        }

        private void TxtScript_TextChanged(object sender, EventArgs e)
        {
            documentManager1.IsModified = true;
        }

        #endregion

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CompileAndRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string script = txtScript.Text;
            if (string.IsNullOrWhiteSpace(script))
            {
                MessageBox.Show("There is no code to run.");
                return;
            }

            if (documentManager1.Save())
            {
                try
                {
                    lvwErrors.Items.Clear();

                    // Compile program
                    RunProgram runProgram = new RunProgram();
                    if (runProgram.Compile(documentManager1.FileName, out CompiledProgram program, false))
                    {
                        // Success: run program in Run form
                        frmRun frm = new frmRun(runProgram, program);
                        frm.ShowDialog();
                    }
                    else
                    {
                        // Build failed: Display errors
                        foreach (Error error in runProgram.Errors)
                        {
                            ListViewItem item = lvwErrors.Items.Add(error.Level.ToString());
                            item.SubItems.Add(string.Format("1{0,03:D3}", (int)error.Code));
                            item.SubItems.Add(error.Line.ToString());
                            item.SubItems.Add(error.Description);
                            item.Tag = error;
                            item.ForeColor = Color.Red;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LvwErrors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var items = lvwErrors.SelectedItems;
            if (items.Count > 0)
            {
                var item = items[0];
                var error = item.Tag as Error;
                if (error != null)
                {
                    txtScript.SelectionStart = txtScript.GetFirstCharIndexFromLine(error.Line - 1);
                    txtScript.SelectionLength = 0;
                    txtScript.ScrollToCaret();
                    txtScript.Focus();
                }
            }
        }
    }
}
