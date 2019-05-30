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
using System.Media;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace mp4Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer Timer;
        TimeSpan ts;

        Dictionary<string, string> compos = new Dictionary<string, string>();
        MediaElement player = new MediaElement();

        bool play;

        public MainWindow()
        {
            InitializeComponent();

            player.MediaOpened += Player_MediaOpened;
            Timer = new System.Windows.Threading.DispatcherTimer();
            Timer.Tick += new EventHandler(dispatcherTimer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
                if (play == false)
                {
                    time.Content = player.Position.Hours + ":" + player.Position.Minutes + ":" + player.Position.Seconds;
                    timeline.Value = player.Position.TotalSeconds;
                    if (player.Position.TotalSeconds == player.NaturalDuration.TimeSpan.TotalSeconds)
                    {
                        Thread.Sleep(1000);
                        player.Play();
                        Timer.Start();
                        length.Content = player.NaturalDuration;
                        time.Content = "0:0:0";
                    }
                }
            
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            try
            {
                timeline.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
                timeline.Value = 0;
            }
            catch
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Choose right format!!!", "Error");
            }
        }

        private void bstart_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
            Timer.Start();
            length.Content = player.NaturalDuration;
            time.Content = "00:00:00";
        }

        private void baddb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.ShowDialog();
                player.Source = new Uri(dlg.FileName, UriKind.Relative);
                VideoDrawing aVideoDrawing = new VideoDrawing();
                aVideoDrawing.Rect = new Rect(10, 0, 560, 315);
                aVideoDrawing.Player = player;
                DrawingImage exampleDrawingImage = new DrawingImage(aVideoDrawing);
                img.Source = exampleDrawingImage;
                img.Stretch = Stretch.Uniform;
                img.HorizontalAlignment = HorizontalAlignment.Left;
            }
            catch (ArgumentException)
            {
            }
        }

        private void bpause_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void bstop_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Timer.Stop();
            length.Content = "";
            time.Content = "";
        }

        private void timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)timeline.Value;
            time.Content = (sliderValue / 3600).ToString() + ":" + (sliderValue / 60).ToString() + ":" + (sliderValue % 60).ToString();
        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sliderValue = (double)volume.Value;
            screen.Volume = sliderValue;
        }

        private void timeline_DragStarted(object sender, DragStartedEventArgs e)
        {
            play = true;
        }

        private void timeline_DragEnded(object sender, DragCompletedEventArgs e)
        {
            int sliderValue = (int)timeline.Value;

            ts = new TimeSpan(0, 0, sliderValue);
            player.Position = ts;
            play = false;
        }       
    }
}
