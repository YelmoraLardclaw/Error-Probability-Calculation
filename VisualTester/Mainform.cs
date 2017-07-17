using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VisualTester
{
    public partial class Mainform : Form
    {
        public SpeedTestInfo data = new SpeedTestInfo();
        private string tempFolder;

        public Mainform()
        {
            InitializeComponent();
            data = new SpeedTestInfo();
            tempFolder = @"H:\Fortune's_Test\";
            data.ExeSimplePath = tempFolder + @"voronver4.exe";
            data.ExeSimplePathPreparation = tempFolder + @"MODULMF4.exe";
            data.ExeFortunePath = @"E:\Github\Fortune\yLibrary.LinearModulation\ConsoleFortune\bin\Debug\ConsoleFortune.exe";
            data.OutputPath = tempFolder;
            data.OutputLogPath = tempFolder + "Log" + DateTime.Today.ToString() + ".txt";
            data.InputFiles = new string[]
            {
                tempFolder + "d100n10.txt",
                tempFolder + "d100n20.txt",
                tempFolder + "d100n30.txt",
                tempFolder + "d100n40.txt",
                tempFolder + "d100n50.txt"
            };
        }

        private void launchTestButton_Click(object sender, EventArgs e)
        {
            //Here we'll be gathering info from upcoming form pieces,
            //For now, they are set in stone (look class constructor).

            //INSERT CODE HERE

            if(!data.Validate())
            {
                MessageBox.Show("Given data is not valid.");
                return;
            }

            //Calling this method, we consider all data validated.
            ExecuteTests(ref data);
        }

        private void ExecuteTests(ref SpeedTestInfo localData)
        {
            //First timespan is time of simple algo, second is Fortune
            List<Tuple<TimeSpan, TimeSpan>> timeList = new List<Tuple<TimeSpan, TimeSpan>>();
            foreach(string input in localData.InputFiles)
            {
                TimeSpan simpleTime, fortuneTime;

                #region Prepare simple call
                string outputSimple = "OutputSimple_" + Path.GetFileName(input),
                       buffer = "bufferFile.txt";
                ProcessStartInfo simplePrePSI = new ProcessStartInfo(localData.ExeSimplePathPreparation);
                simplePrePSI.Arguments = "i.ct i.qfb " + Path.GetFileName(input) + " 1 " + outputSimple + " > " + buffer;
                simplePrePSI.UseShellExecute = true;
                simplePrePSI.WindowStyle = ProcessWindowStyle.Hidden;
                ProcessStartInfo simplePSI = new ProcessStartInfo(localData.ExeSimplePath);
                simplePSI.Arguments = outputSimple + "   >> " + buffer;
                simplePSI.UseShellExecute = false;
                simplePSI.WindowStyle = ProcessWindowStyle.Hidden;
                #endregion
                #region Prepare fortune call
                ProcessStartInfo fortunePSI = new ProcessStartInfo(localData.ExeFortunePath);
                fortunePSI.Arguments = input + " " + tempFolder + "OutputFortune_" + Path.GetFileName(input);
                fortunePSI.UseShellExecute = false;
                fortunePSI.WindowStyle = ProcessWindowStyle.Hidden;
                #endregion

                DateTime start, end;
                #region Simple call
                start = DateTime.Now;
                Process simplePrepProcess = Process.Start(simplePrePSI);
                simplePrepProcess.WaitForExit();
                Process simpleProcess = Process.Start(simplePSI);
                simpleProcess.WaitForExit();
                end = DateTime.Now;
                simpleTime = end.Subtract(start);
                #endregion
                #region Fortune call
                start = DateTime.Now;
                Process fortune = Process.Start(fortunePSI);
                fortune.WaitForExit();
                end = DateTime.Now;
                fortuneTime = end.Subtract(start);
                #endregion

                timeList.Add(new Tuple<TimeSpan, TimeSpan>(simpleTime, fortuneTime));
            }

            //Here will be writing down the time record output data.
            Simulate(timeList);
        }

        private void Simulate(List<Tuple<TimeSpan, TimeSpan>> info)
        {
            int period = 10, baseCount = 10;
            double[] coefficients = new double[info.Count];
            for(int i = 0; i < info.Count; i++)
            {
                int n = baseCount + period * i;
                n = (int)Math.Pow(n, 2);

                coefficients[i] = info[i].Item2.TotalMilliseconds / (n * Math.Log(n, 2));
            }
        }

        public struct SpeedTestInfo
        {
            public string ExeSimplePath { get; set; }
            public string ExeSimplePathPreparation { get; set; }
            public string ExeFortunePath { get; set; }
            public string OutputPath { get; set; }
            public string OutputLogPath { get; set; }
            public string[] InputFiles { get; set; }            

            public bool Validate()
            {
                bool validation = true;
                validation &= ValidateInput(ExeSimplePath);
                validation &= ValidateInput(ExeSimplePathPreparation);
                validation &= ValidateInput(ExeFortunePath);
                validation &= ValidateOutput(OutputLogPath, ".txt");
                foreach (string s in InputFiles)
                    validation &= ValidateOutput(s, ".txt");
                validation &= Directory.Exists(OutputPath);

                return validation;
            }

            private bool ValidateInput(string path) => path != "" && File.Exists(path);
            private bool ValidateOutput(string path, string fileFormat) => path != "" && Directory.Exists(Path.GetDirectoryName(path)) &&
                                                        Regex.IsMatch(Path.GetFileName(path), @"\w*" + fileFormat);
        }
    }
}
