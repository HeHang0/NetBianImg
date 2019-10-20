using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace NetBianImg
{
    class NetBianImage
    {
        NotifyIcon notifyIcon;

        public NetBianImage Run()
        {
            InitNotifyIcon();
            return this;
        }

        void InitNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "彼岸图壁纸自动更换";//最小化到托盘时，鼠标点击时显示的文本
            notifyIcon.Icon = Properties.Resources.wallpaper;//程序图标
            //退出菜单项
            MenuItem exit = new MenuItem("退出");
            exit.Click += new EventHandler((sender, e) => { Application.Exit(); });
            //上一个
            MenuItem last = new MenuItem("上一个");
            last.Click += Last_Click;
            //下一个
            MenuItem next = new MenuItem("下一个");
            next.Click += Next_Click;

            //开机启动菜单项
            MenuItem start = new MenuItem("开机启动");
            start.RadioCheck = false;
            start.Click += new EventHandler(Start);

            //关联托盘控件
            MenuItem[] childen = new MenuItem[] { last, next, start, exit };
            notifyIcon.ContextMenu = new ContextMenu(childen);

            if (SetBootStartUp())
            {
                start.Checked = true;
            }
            BalloonTips("OK");
            notifyIcon.Visible = true;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Last_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        bool SetBootStartUp()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\AutoWallpaperByNetBianImg.lnk"))
            {
                try
                {
                    CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "AutoWallpaperByNetBianImg", Application.ExecutablePath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
                shortcut.TargetPath = targetPath;//指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
                shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
                shortcut.Description = description;//设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;//设置图标路径
                shortcut.Save();//保存快捷方式

                return true;
            }
            catch
            { }
            return false;
        }

        void Start(object sender, EventArgs e)
        {
            var start = sender as MenuItem;
            try
            {
                if (!start.Checked) //设置开机自启动  
                {
                    //System.Windows.MessageBox.Show("设置开机自启动，需要修改注册表", "提示");
                    string path = System.Windows.Forms.Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("AutoBianImgWallpaperStart", path);
                    rk2.Close();
                    rk.Close();
                    start.Checked = true;
                    BalloonTips("设置开机启动成功");
                }
                else //取消开机自启动  
                {
                    //System.Windows.Forms.MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                    string path = System.Windows.Forms.Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("AutoBianImgWallpaperStart", false);
                    rk2.Close();
                    rk.Close();
                    start.Checked = false;
                    BalloonTips("取消开机启动成功");
                }
            }
            catch (Exception)
            {
                if (!start.Checked) //设置开机自启动  
                {
                    if (SetBootStartUp())
                    {
                        BalloonTips("设置开机启动成功");
                        start.Checked = true;
                    }
                    else
                    {
                        BalloonTips("设置开机启动失败");
                    }
                }
                else //取消开机自启动  
                {
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\AutoWallpaperByBing.lnk"))
                    {
                        try
                        {
                            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\AutoWallpaperByBing.lnk");
                            BalloonTips("取消开机启动成功");
                            start.Checked = false;
                        }
                        catch (Exception)
                        {
                        }
                    }

                }
            }
        }
        void BalloonTips(string msg)
        {
            notifyIcon.BalloonTipText = msg; //设置托盘提示显示的文本
            notifyIcon.ShowBalloonTip(500);
        }
    }
}
