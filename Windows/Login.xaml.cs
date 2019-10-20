using NetBianImg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetBianImg.Windows
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public event EventHandler<string> CookieReceived;

        public Login()
        {
            InitializeComponent();
            InitUrl();
        }

        private void InitUrl()
        {
            LoginBrowser.LoadCompleted += LoginBrowser_LoadCompleted;
            LoginBrowser.Navigate("http://pic.netbian.com/e/memberconnect/?apptype=qq");
            //string qqUrlContent = Utils.GetHttpResponse("http://pic.netbian.com/e/memberconnect/?apptype=qq");
            //string qqAuthUrl = RegexQQAuthUrl(qqUrlContent);
            //if (string.IsNullOrWhiteSpace(qqAuthUrl))
            //{
            //    Application.Current.Shutdown();
            //}
            //string baseContent = Utils.GetHttpResponse(qqAuthUrl);
            //int index = baseContent.IndexOf("<head>")+6;
            //LoginBrowser.NavigateToString(baseContent);//.Insert(index, scriptInjection));
        }

        private void LoginBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Console.WriteLine(e.Uri.AbsoluteUri);
            if (e.Uri.AbsoluteUri == "http://pic.netbian.com/")
            {
                dynamic document = LoginBrowser.Document;
                CookieReceived?.Invoke(this, document.cookie);
                Close();
            }
            else if(e.Uri.AbsoluteUri.StartsWith("http://pic.netbian.com/e"))
            {
                Hide();
            }
        }

        private string RegexQQAuthUrl(string qqUrlContent)
        {
            Match match = Regex.Match(qqUrlContent, "top\\.location\\.href='(?<url>[^']+)'");
            if (match.Success)
            {
                return match.Groups["url"].Value;
            };
            return "";
        }

        private string scriptInjection => @"
            <script>
                window.onload = function(){
                    var iframeLogin =  document.getElementById('ptlogin_iframe');
                    if(iframeLogin && iframeLogin.contentWindow && iframeLogin.contentWindow.document && iframeLogin.contentWindow.document.getElementById){
                        var bql = iframeLogin.contentWindow.document.getElementById('bottom_qlogin');
                        if(bql){
                            bql.style.display = 'none';
                        }
                        var qlg = iframeLogin.contentWindow.document.getElementById('qlogin');
                        if(qlg){
                            qlg.style.display = 'none';
                        }
                        var wql = iframeLogin.contentWindow.document.getElementById('web_qr_login');
                        if(wql){
                            wql.style.display = 'block';
                        }
                        var un = iframeLogin.contentWindow.document.getElementById('u');
                        if(un){
                            un.value = '724590957';
                        }
                        var pw = iframeLogin.contentWindow.document.getElementById('p');
                        if(pw){
                            pw.value = 'hehang2333';
                        }
                        var lgb = iframeLogin.contentWindow.document.getElementById('login_button');
                        if(lgb){
                            lgb.click();
                        }
                    }
                }
            </script>";
    }
}
