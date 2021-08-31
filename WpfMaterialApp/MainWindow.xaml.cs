using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using Path = System.IO.Path;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Husty.OpenCvSharp;
using System.Threading;

namespace WpfMaterialApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {

        private string _saveDir = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            using var cofd = new CommonOpenFileDialog()
            {
                Title = "ファイルを選択してください",
                InitialDirectory = _saveDir,
                IsFolderPicker = false,
            };
            cofd.Filters.Add(new CommonFileDialogFilter("画像 or 動画", "*.png;*.jpg;*.avi;*.mp4"));
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var f = cofd.FileName;
                _saveDir = Path.GetDirectoryName(f);
                if (Path.GetExtension(f) == ".png" || Path.GetExtension(f) == ".jpg")
                {
                    Image.Source = Cv2.ImRead(f).ToBitmapSource();
                }
                else
                {
                    var frame = new Mat();
                    var cap = new VideoCapture(f);
                    var interval = 1000 / cap.Fps;
                    Task.Run(async () =>
                    {
                        while (cap.Read(frame))
                        {
                            Dispatcher.Invoke(() => Image.Source = frame.ToBitmapSource());
                            await Task.Delay((int)interval);
                        }
                        cap.Dispose();
                        frame.Dispose();
                    });
                }
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
