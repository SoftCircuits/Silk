﻿// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SilkExamples.Examples;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SilkPlatforms;

namespace SilkExamples
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IExample).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (Type type in types)
            {
                if (Activator.CreateInstance(type) is IExample example)
                    lstExamples.Items.Add(example);
            }
            if (lstExamples.Items.Count > 0)
                lstExamples.SelectedIndex = 0;
        }

        private void LstExamples_DoubleClick(object sender, EventArgs e)
        {
            CmdRun_Click(sender, e);
        }

        private void CmdRun_Click(object sender, EventArgs e)
        {
            if (lstExamples.SelectedItem is IExample example)
            {
                RunProgram run = new(example.Platform);
                if (!run.Run(example.SourceCode))
                {
                    // Display errors
                    MessageBox.Show($"Error compile example:\r\n\r\n{string.Join("\r\n\r\n", run.Errors)}");
                }
                else
                {
                    // Select next example
                    if (lstExamples.SelectedIndex < (lstExamples.Items.Count - 1))
                        lstExamples.SelectedIndex++;
                }
            }
        }

        private void CmdExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
