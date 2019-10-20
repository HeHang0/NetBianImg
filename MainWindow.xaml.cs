using NetBianImg.Model;
using NetBianImg.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetBianImg
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetBian model = new NetBian();
        public MainWindow()
        {
            InitializeComponent();
            model.MessageIssued += Model_MessageIssued;
            model.LostLogin += Model_LostLogin;
            DataContext = model;
        }

        private void Model_LostLogin(object sender, EventArgs e)
        {
            var loginWin = new Login();
            loginWin.CookieReceived += LoginWin_CookieReceived;
            loginWin.ShowDialog();
        }

        private void Model_MessageIssued(object sender, string msg)
        {
            MessageBox.Show(msg);
        }

        private void LoginWin_CookieReceived(object sender, string cookie)
        {
            model.SetCookie(cookie);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            string type = (sender as Button)?.Name ?? "";
            switch (type)
            {
                case "LastPage":
                    model.LastPage();
                    break;
                case "NextPage":
                    model.NextPage();
                    break;
                case "ToPageConfirm":
                    model.ToPage();
                    break;
                default:
                    model.DisplayImage(type);
                    break;
            }
        }
        private void Image_Click(object sender, EventArgs e)
        {
            if (sender is Image)
            {
                model.ImageOpen();
            }
            else
            {
                switch ((sender as MenuItem)?.Name ?? "")
                {
                    case "SaveAs":
                        model.ToPage();
                        break;
                    case "SetWallPaper":
                        model.ToPage();
                        break;

                }
            }
        }
    }
}
