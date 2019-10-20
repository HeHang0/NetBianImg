using NetBianImg.Common;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace NetBianImg.Model
{
    public class NetBian: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler<string> MessageIssued;
        private void OnMessageIssued(string msg)
        {
            MessageIssued?.Invoke(this, msg);
        }
        public event EventHandler LostLogin;

        public NetBian()
        {
            DisplayImage();
        }

        private string netbianHost = "http://pic.netbian.com";
        private string currentType = "";
        public void DisplayImage(string type = "FengJing", int page = 1)
        {
            if (type == currentType && page == CurrentPage) return;
            string pageParam = page > 1 ? $"_{page}" : "";
            var result = Utils.GetNetBianImage($"{netbianHost}/4k{type.ToLower()}/index{pageParam}.html");
            TotalPage = result.TotalPage;
            CurrentPage = page;
            currentType = type;
            ImageList.Clear();
            foreach (var item in result.ImageInfo)
            {
                ImageList.Add(new NetBianImage(item.Name, netbianHost + item.Source, netbianHost + item.Detail));
            }
        }

        public void LastPage()
        {
            if(CanLastPage)
            {
                DisplayImage(page: CurrentPage - 1);
            }
        }

        public void NextPage()
        {
            if (CanNextPage)
            {
                DisplayImage(page: CurrentPage + 1);
            }
        }

        public void ToPage()
        {
            if (ToPageNum <= TotalPage && ToPageNum > 0)
            {
                DisplayImage(page: ToPageNum);
            }
        }

        public void ImageOpen()
        {
            try
            {
                System.Diagnostics.Process.Start(netbianHost + Utils.GetDetailImage(ImageSelected.Detail));
            }
            catch (Exception)
            {
            }
        }

        private NetBianImage imageSelected;
        public NetBianImage ImageSelected
        {
            get
            {
                return imageSelected;
            }
            set
            {
                if(imageSelected != value)
                {
                    imageSelected = value;
                    OnPropertyChanged("ImageSelected");
                }
            }
        }

        public ObservableCollection<NetBianImage> ImageList { get; } = new ObservableCollection<NetBianImage>() { };

        public bool CanLastPage => CurrentPage > 1;
        public bool CanNextPage => CurrentPage < TotalPage;

        private int currentPage = 1;
        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                if(currentPage != value)
                {
                    currentPage = value;
                    OnPropertyChanged("CurrentPage");
                    OnPropertyChanged("CanLastPage");
                    OnPropertyChanged("CanNextPage");
                }
            }
        }

        private int totalPage = 0;
        public int TotalPage
        {
            get
            {
                return totalPage;
            }
            set
            {
                if (totalPage != value)
                {
                    totalPage = value;
                    OnPropertyChanged("TotalPage");
                }
            }
        }

        private int toPageNum = 1;
        public int ToPageNum
        {
            get
            {
                return toPageNum;
            }
            set
            {
                if (toPageNum != value)
                {
                    toPageNum = value;
                    OnPropertyChanged("ToPageNum");
                }
            }
        }

        private string cookie = "";
        public void SetCookie(string str)
        {
            cookie = str;
        }

        public bool CheckCookie()
        {
            return !string.IsNullOrWhiteSpace(cookie);
        }
    }

    public class NetBianImage
    {
        public NetBianImage(string name, string source, string detail)
        {
            Name = name;
            Source = source;
            Detail = detail;
        }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Detail { get; set; }
        public static BitmapSource IndexImage { get; } = Utils.GetBitmapSource(Properties.Resources.Index);
    }
}
