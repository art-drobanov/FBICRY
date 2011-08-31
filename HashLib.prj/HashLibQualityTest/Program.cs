using System;
using System.Windows.Forms;

namespace HashLibQualityTest
{
    // TODO: predkosc bajty na cykl

    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HashLibQualityTestForm());
        }
    }
}
