using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayMonitor
{/*
  * Developer references:
  * https://stackoverflow.com/questions/36379547/writing-text-to-the-system-tray-instead-of-an-icon
  * https://stackoverflow.com/questions/1364809/the-windows-gui-main-loop-in-c-where-is-it
  * https://stackoverflow.com/questions/6944779/determine-operating-system-and-processor-type-in-c-sharp
  * https://stackoverflow.com/questions/1730731/how-to-start-winform-app-minimized-to-tray
  * https://www.codeproject.com/Articles/21483/Create-Icons-at-Runtime-and-Show-Them-in-the-Syste
  * https://www.codeproject.com/Articles/26310/Using-WMI-to-retrieve-processor-information-in-C
  * http://www.tutorialspanel.com/create-system-tray-icon-windows-forms-application-using-c-vb-net/index.htm
  * https://docs.microsoft.com/pt-br/dotnet/csharp/programming-guide/inside-a-program/coding-conventions
  * https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor
  * https://docs.microsoft.com/en-us/windows/win32/wmisdk/retrieving-an-instance
  * https://itnext.io/memory-wise-apps-with-c-net-50b6379ed86
  * 
  */
    public partial class Form1 : Form
    {
        private String formattedCPUSpeed;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void SetVisibleCore(bool value)
        {
            value = false;
            if (!this.IsHandleCreated) 
                CreateHandle();
            
            base.SetVisibleCore(value);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }
        public void CreateTextIcon(string str)
        {
            //Font fontToUse = new Font("Microsoft Sans Serif", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            //Font fontToUse = new Font("Tahoma", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontToUse = new Font("Trebuchet MS", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.Black);
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Yellow);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -2, 0);
            hIcon = (bitmapText.GetHicon());
            notifyIcon1.Icon = System.Drawing.Icon.FromHandle(hIcon);
            
            g.Dispose();
            bitmapText.Dispose();
            brushToUse.Dispose();
            fontToUse.Dispose();
            hIcon = IntPtr.Zero;
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(formattedCPUSpeed);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "Você pode conferir a velocidade do seu processador no ícone ao lado do relógio";
            notifyIcon1.BalloonTipTitle = "Velocidade do Processador";
            notifyIcon1.ShowBalloonTip(2000);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            formattedCPUSpeed = getCPUSpeed1().ToString("N1");
            notifyIcon1.Text = formattedCPUSpeed + " Ghz";
            CreateTextIcon(formattedCPUSpeed);
        }

        private Double getCPUSpeed1()
        {
            /*
            * Implemented WMI access
            * https://docs.microsoft.com/en-us/windows/win32/wmisdk/retrieving-an-instance
            * https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-processor
            * https://stackoverflow.com/questions/1730731/how-to-start-winform-app-minimized-to-tray
            */

            ManagementObjectSearcher objSearchPerf = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM  Win32_PerfFormattedData_Counters_ProcessorInformation");
            ManagementObjectCollection colPerf = objSearchPerf.Get();
            Double cpuPerf = 0;

            foreach (ManagementObject objPerf in colPerf)
            {
                Console.WriteLine("CPU Performance: ", objPerf["PercentProcessorPerformance"]);
                cpuPerf += Double.Parse(objPerf["PercentProcessorPerformance"].ToString());
            }
            return maxCPU * (cpuPerf/(100000 * colPerf.Count));
        }
    }
}
