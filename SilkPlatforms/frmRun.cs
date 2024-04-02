// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Windows.Forms;

namespace SilkPlatforms
{
    internal partial class frmRun : Form
    {
        private readonly Platform Platform;
        public Runtime? Runtime { get; set; }

        public frmRun(Platform platform)
        {
            InitializeComponent();

            Platform = platform;
            Text = Platform.Description;
        }

        public void Center() => CenterToScreen();

        public bool BufferedPainting
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        private void frmRun_Load(object sender, EventArgs e)
        {
            // Show form
            Center();
            Show();

            // Run program
            if (Runtime != null)
                Runtime.Execute();
        }

        private void frmRun_FormClosing(object sender, FormClosingEventArgs e)
        {
            Platform.CleanUp();
        }
    }
}
