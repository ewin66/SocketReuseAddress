using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SocketReuseAddress
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSocketReuseAddress());
        }
    }
}