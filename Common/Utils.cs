using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NetBianImg.Common
{
    public class Utils
    {
        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static bool SetWallpaper(string path)
        {
            try
            {
                SystemParametersInfo(20, 0, path, 0x01 | 0x02);
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string DownLoadImage(string url)
        {
            WebClient webClient = new WebClient();
            var filename = $"{DateTime.Now.ToString("yyyy-MM-dd")}.jpg";
            webClient.DownloadFile(url, filename);
            return Directory.GetCurrentDirectory() + "\\" + filename;
        }

        public static string GetHttpResponse(string url, string cookie = "", string encoding = "utf-8")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static NetBianImageListInfo GetNetBianImage(string url)
        {
            NetBianImageListInfo result = new NetBianImageListInfo();
            var httpResponseStr = GetHttpResponse(url, encoding:"gbk");
            MatchCollection matches = Regex.Matches(httpResponseStr, "<a href=\"(?<detail>[^\"]+)\" target=\"[\\S]*\"><img src=\"(?<source>[^\"]+)\" alt=\"(?<name>[\\s\\S]*?)\" /><b>[\\s\\S]*?</b></a></li>");
            if (matches.Count > 0)
            {
                foreach (Match item in matches)
                {
                    string source = item.Groups["source"].Value;
                    string name = item.Groups["name"].Value;
                    string detail = item.Groups["detail"].Value;
                    name = Regex.Replace(name, "[\\s]*[\\d][k|K][\\s\\S]*$", "");
                    name = Regex.Replace(name, "[\\s]*[\\d]+[x|X][\\d]+[\\s\\S]*$", "");
                    name = name.Replace("壁纸", "");
                    result.ImageInfo.Add(new NetBianImageInfo() { Name = name, Source = source, Detail = detail });
                }
            }
            Match match = Regex.Match(httpResponseStr, "<div class=\"page\">.*?</div>");
            if (match.Success)
            {
                matches = Regex.Matches(match.Value, ">(?<page>[0-9]+)<");
                if (matches.Count > 0)
                {
                    int total = 0;
                    foreach (Match item in matches)
                    {
                        int page = int.Parse(item.Groups["page"].Value);
                        if(page > total)
                        {
                            total = page;
                        }
                    }
                    result.TotalPage = total;
                }
            }
            return result;
        }

        public static string GetDetailImage(string detail)
        {
            if (string.IsNullOrWhiteSpace(detail)) return "";
            var httpResponseStr = GetHttpResponse(detail, encoding: "gbk");
            Match match = Regex.Match(httpResponseStr, "id=\"img\"[\\s]*><img src=\"(?<url>[^\"]+)");
            if (match.Success)
            {
                return match.Groups["url"].Value;
            }
            return "";
        }

        public static System.Windows.Media.Imaging.BitmapSource GetBitmapSource(System.Drawing.Bitmap bmp)
        {
            System.Windows.Media.Imaging.BitmapFrame bf = null;

            using (MemoryStream ms = new System.IO.MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bf = System.Windows.Media.Imaging.BitmapFrame.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);

            }
            return bf;
        }
    }

    public class NetBianImageListInfo
    {
        public List<NetBianImageInfo> ImageInfo { get; set; } = new List<NetBianImageInfo>();
        public int TotalPage { get; set; } = 0;
    }

    public class NetBianImageInfo
    {
        public string Source { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
    }
}
