﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using log4net;

namespace GoreUpdater
{
    static class Program
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var runUpdater = false;
            foreach (var arg in args)
            {
                if (arg == "-runUpdater")
                    runUpdater = true;
            }

            if (runUpdater)
            {
                log.Info("Starting GoreUpdater client...");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                try
                {
                    var procInfo = new ProcessStartInfo();
                    procInfo.UseShellExecute = true;
                    procInfo.FileName = Application.ExecutablePath; //The file in that DIR.
                    procInfo.WorkingDirectory = ""; //The working DIR.
                    procInfo.Verb = "runas";
                    procInfo.Arguments = "-runUpdater";
                    Process.Start(procInfo); //Start that process.
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}