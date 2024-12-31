// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace SilkPlatforms
{
    internal partial class RunForm : Form
    {
        private readonly Platform Platform;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Runtime? Runtime { get; set; }

        public RunForm(Platform platform)
        {
            InitializeComponent();

            Platform = platform;
            Text = Platform.Description;
        }

        public void Center() => CenterToScreen();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool BufferedPainting
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        private void RunForm_Load(object sender, EventArgs e)
        {
            // Show form
            Center();
            Show();

            // Run program
            Runtime?.Execute();
        }

        private void RunForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Platform.CleanUp();
        }
    }
}
