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
        MediaPlayer player = new MediaPlayer();

        bool play;

        public MainWindow()
        {
            InitializeComponent();

            screen.MediaOpened += Player_MediaOpened;
            Timer = new System.Windows.Threading.DispatcherTimer();
            Timer.Tick += new EventHandler(dispatcherTimer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (play == false)
                {
                    time.Content = screen.Position.Hours + ":" + screen.Position.Minutes + ":" + screen.Position.Seconds;
                    timeline.Value = screen.Position.TotalSeconds;
                    length.Content = screen.NaturalDuration;
                    if (screen.Position.TotalSeconds == screen.NaturalDuration.TimeSpan.TotalSeconds)
                    {
                        Thread.Sleep(1000);
                        Timer.Start();
                        time.Content = "0:0:0";
                    }
                }
            }
            catch
            {
                MessageBox.Show("Add video", "Error");
            }
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            try
            {
                timeline.Maximum = screen.NaturalDuration.TimeSpan.TotalSeconds;
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
            screen.Play();
            Timer.Start();
            length.Content = screen.NaturalDuration;
            time.Content = "00:00:00";
        }

        private void baddb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //выбор файла
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Video (.mp4)|*.mp4";
                dlg.Multiselect = true;
                dlg.ShowDialog();
                
                //установка источника
                screen.Source = new Uri(dlg.FileName, UriKind.Relative);
            }
            catch (ArgumentException)
            {
            }
        }

        private void bpause_Click(object sender, RoutedEventArgs e)
        {
            screen.Pause();
        }

        private void bstop_Click(object sender, RoutedEventArgs e)
        {
            screen.Stop();
            Timer.Stop();
            length.Content = "";
            time.Content = "";
        }

        private void timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)timeline.Value;            
            //ts = new TimeSpan(0, 0, sliderValue);
            //screen.Position = ts;
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
            screen.Position = ts;
            play = false;
        }

        //private void bstart_Click_1(object sender, RoutedEventArgs e)
        //{
        //    player.Play();
        //}
    }
}
