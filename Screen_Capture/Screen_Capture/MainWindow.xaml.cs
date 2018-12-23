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
using System.Drawing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using System.Threading;
using System.Timers;

namespace Screen_Capture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread _screen_capture_thread;
        private static BitmapSource _printed_Screen;
        private static System.Timers.Timer aTimer;

        private static int _file_counter = 0;
        private string _file_suffix = @".bmp";
        private string _file_name = @"Screen_Capture_";
        private string _file_path = @"C:\Users\Itamar Katz\Desktop\screen_captures\";

        public MainWindow() { InitializeComponent(); }

        public BitmapSource Printed_Screen
        {
            get => _printed_Screen;
            set
            {

                _printed_Screen = value;
            }
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            _screen_capture_thread = new Thread(() =>
            {
                // Create a timer with a two second interval.
                aTimer = new System.Timers.Timer(2000);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                while ( true ) ;
            });

            _screen_capture_thread.Start();

            while ( true )
            {
                Image_Print_Screen.Source = Printed_Screen;
                Thread.Sleep(500);
            }
        }

        private void OnTimedEvent( Object source, ElapsedEventArgs e )
        {
            Printed_Screen = CopyScreen();
            SaveClipboardImageToFile(_file_path + _file_name + _file_counter++ + _file_suffix, ref _printed_Screen);
            //Application.Current.Dispatcher.Invoke((Action) (() =>
            //{
            //}));
        }

        private void Start_Print_Screen()
        {
            //for ( int i = 0; i < 5; i++ )
            //{
            //    Printed_Screen = CopyScreen();
            //    SaveClipboardImageToFile(_file_path + _file_name + _file_counter++ + _file_suffix, ref _printed_Screen);
            //    Application.Current.Dispatcher.Invoke((Action) (() =>
            //    {
            //        Image_Print_Screen.Source = Printed_Screen;
            //    }));
            //    Thread.Sleep(5000);
            //}
        }

        private static BitmapSource CopyScreen()
        {
            using ( var screenBmp = new Bitmap(
                (int) SystemParameters.PrimaryScreenWidth,
                (int) SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb) )
            {
                using ( var bmpGraphics = Graphics.FromImage(screenBmp) )
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        private static void SaveClipboardImageToFile(string filePath, ref BitmapSource bitmapSource)
        {
            var image = bitmapSource;
            using ( var fileStream = new FileStream(filePath, FileMode.Create) )
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        private void Window_Closing( object sender, CancelEventArgs e )
        {
            aTimer?.Stop();
            aTimer?.Dispose();
            //_screen_capture_thread?.Abort();
        }



        //private void btnOpenFile_Click( object sender, RoutedEventArgs e )
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Multiselect = false;
        //    openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        //    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    if ( openFileDialog.ShowDialog() == true )
        //    {
        //            filePath = System.IO.Path.GetFileName(openFileDialog.FileName);
        //    } else
        //    {
        //        throw new Exception();
        //    }
        //}
    }
}
