using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskHide
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 隐藏盘符
        /// </summary>
        /// <param name="Values"></param>
        private void DiskHide(byte[] Values)
        {
            try
            {
                RegistryKey rgK = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
                rgK.SetValue("NoDrives", Values, RegistryValueKind.Binary);//给默认值赋值
                MessageBox.Show("OK");
            }
            catch { }
        }
        /// <summary>
        /// 禁用盘符
        /// </summary>
        /// <param name="Values"></param>
        private void DiskDis(byte[] Values)
        {
            try
            {
                RegistryKey rgK = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
                rgK.SetValue("NoViewOnDrive", Values, RegistryValueKind.Binary);//给默认值赋值
                MessageBox.Show("OK");
            }
            catch { }
        }

        /// <summary>
        /// 以杀死进程的方式杀死explorer
        /// </summary>
        /// <param name="name">要杀死的进程友好名（无 .exe）</param>
        static public void killProcess(string name)
        {
            try
            {
                Process[] myProcesses = Process.GetProcesses();
                foreach (Process myProcess in myProcesses)
                {
                    if (name == myProcess.ProcessName)
                        myProcess.Kill();
                }
            }
            catch (Exception ee)
            {
                //抛出异常
                MessageBox.Show(ee.Message);
            }
        }
        #region 第一种运行CMD的方式
        /// <summary>
        /// 调用CMD命令来杀死或重启进程
        /// </summary>
        /// <param name="a">杀死或重启进程</param>
        /// <returns>cmd命令返回</returns>
        public static void cmdkill(bool a)
        {
            string str;
            //string str = Console.ReadLine();
            if (a)
            {
                str = @"taskkill /f /im explorer.exe";
            }
            else
            {
                str = @"C:\Windows\explorer.exe";
                //str = @"explorer.exe";
            }
            Process p = new Process();
            p.StartInfo.FileName = "cmd";
            p.StartInfo.UseShellExecute = false;                 //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;            //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;           //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;            //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;                   //不显示程序窗口
            p.Start();                                           //启动程序
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(str + "&exit");
            //p.StandardInput.WriteLine(str);
            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            //获取cmd窗口的输出信息
            //string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            //return output;
        }
        #endregion


        private void Button1_Click(object sender, EventArgs e)
        {
            /*
             * 01表示A盘、02表示B盘、04表示C盘、08表示D盘、10表示E盘、20表示F盘、40表示G盘、80表示H盘。如果要隐藏F盘时，将数值数据设置为“20000000
另一个方法隐藏硬盘分区
运行输入gpedit.msc回车打开组策略，在左侧选用户配置/管理模板/Windows组件/Windows资源管理器/在右侧选“隐藏“我的电脑”中的这些指定的驱动器”，双击它在打开的对话框中选择“已启用”，然后将你要隐藏的盘符添进去，然后按应用确定。通过设置随然隐藏了盘符，但是还可以从我的电脑地址栏访问隐藏的盘符。可通过设置禁止这样做，在右侧还有一项“防止从我的电脑访问驱动器”双击它在打开的对话框中选择“已启用”然后选择你要禁止访问的盘符添进去后，按应用确定，重启电脑即可。
*/
            byte data = 00;
            if (checkBox1.Checked)
                data += 0x01;
            if (checkBox2.Checked)
                data += 0x02;
            if (checkBox3.Checked)
                data += 0x04;
            if (checkBox4.Checked)
                data += 0x08;
            if (checkBox5.Checked)
                data += 0x10;
            if (checkBox6.Checked)
                data += 0x20;
            if (checkBox7.Checked)
                data += 0x40;
            if (checkBox8.Checked)
                data += 0x80;

            byte[] bvalues = new byte[] { data, 00, 00, 00 }; //隐藏F盘
            DiskHide(bvalues);
            //重启 explorer
            cmdkill(true);
            cmdkill(false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            byte[] bvalues = new byte[] { 00, 00, 00, 00 };
            DiskHide(bvalues);
            //重启 explorer
            cmdkill(true);
            cmdkill(false);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            byte[] bvalues = new byte[] { 00, 00, 00, 00 };
            DiskDis(bvalues);
            //重启 explorer
            cmdkill(true);
            cmdkill(false);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            /*
             * 01表示A盘、02表示B盘、04表示C盘、08表示D盘、10表示E盘、20表示F盘、40表示G盘、80表示H盘。如果要隐藏F盘时，将数值数据设置为“20000000
另一个方法隐藏硬盘分区
运行输入gpedit.msc回车打开组策略，在左侧选用户配置/管理模板/Windows组件/Windows资源管理器/在右侧选“隐藏“我的电脑”中的这些指定的驱动器”，双击它在打开的对话框中选择“已启用”，然后将你要隐藏的盘符添进去，然后按应用确定。通过设置随然隐藏了盘符，但是还可以从我的电脑地址栏访问隐藏的盘符。可通过设置禁止这样做，在右侧还有一项“防止从我的电脑访问驱动器”双击它在打开的对话框中选择“已启用”然后选择你要禁止访问的盘符添进去后，按应用确定，重启电脑即可。
*/
            byte data = 00;
            if (checkBox1.Checked)
                data += 0x01;
            if (checkBox2.Checked)
                data += 0x02;
            if (checkBox3.Checked)
                data += 0x04;
            if (checkBox4.Checked)
                data += 0x08;
            if (checkBox5.Checked)
                data += 0x10;
            if (checkBox6.Checked)
                data += 0x20;
            if (checkBox7.Checked)
                data += 0x40;
            if (checkBox8.Checked)
                data += 0x80;

            byte[] bvalues = new byte[] { data, 00, 00, 00 }; //隐藏F盘
            DiskDis(bvalues);
            //重启 explorer
            cmdkill(true);
            cmdkill(false);
        }
    }
}
