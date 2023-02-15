using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageSlyderTest.UserControlls
{
    /// <summary>
    /// Логика взаимодействия для ImageSlyder.xaml
    /// </summary>
    public partial class ImageSlyder : UserControl
    {
        private static readonly DependencyProperty _listPathsImage = DependencyProperty.Register("ListPathImage", typeof(List<string>), typeof(ImageSlyder));

        public List<string> ListPathImage
        {
            get { return (List<string>)GetValue(_listPathsImage); }
            set { SetValue(_listPathsImage, value); }
        }

        public int ImageWidth { get; set; } = 512;
        public int ImageHeight { get; set; } = 214;

        private Dictionary<string, BitmapImage> _loadedPathImage = new Dictionary<string, BitmapImage>();

        private int _currentPosition = 0;

        public ImageSlyder()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                bool isNotNull = false;
                while (!isNotNull)
                {
                    this.Dispatcher.Invoke(() => {
                        isNotNull = checkList(); 
                         });
                }

                this.Dispatcher.Invoke(() => { LoadImage(ListPathImage[0]); });
            });
        }

        private bool checkList()
        {
            if(ListPathImage == null)
                return false;
            return true;
        }

        private void LoadImage(string path)
        {
            if (_loadedPathImage.ContainsKey(path))
            {
                currentimage.Source = _loadedPathImage.Where(x => x.Key.ToString() == path).Select(s => s.Value).FirstOrDefault();
            }
            else
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var ImagePath = System.IO.Path.Combine(baseDirectory + "..\\..\\", path);
                using (var stream = new MemoryStream(File.ReadAllBytes(ImagePath)))
                {
                    var newBitmapImage = new BitmapImage();
                    newBitmapImage.BeginInit();
                    newBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    newBitmapImage.DecodePixelHeight = ImageHeight;
                    newBitmapImage.DecodePixelWidth = ImageWidth;
                    newBitmapImage.StreamSource = stream;
                    newBitmapImage.EndInit();

                    _loadedPathImage.Add(path, newBitmapImage);

                    currentimage.Source = newBitmapImage;
                }
            }
        }

        private void _LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPosition < 0)
            {
                _currentPosition = ListPathImage.Count - 1;
            }

            LoadImage(ListPathImage[_currentPosition]);
            _currentPosition--;
        }

        private void _RightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPosition > ListPathImage.Count - 1)
            {
                _currentPosition = 0;
            }

            LoadImage(ListPathImage[_currentPosition]);
            _currentPosition++;
        }
    }
}
