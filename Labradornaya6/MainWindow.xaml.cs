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

namespace mp3Player
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
        

        string tmp;
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
                    slider.Value = player.Position.TotalSeconds;
                    if (player.Position.TotalSeconds == player.NaturalDuration.TimeSpan.TotalSeconds)
                    {
                        if (cb.IsChecked == true)
                        {
                            int count = playList.Items.Count;
                            Random rnd = new Random();

                            count = rnd.Next(0, count - 1);
                            playList.SelectedIndex = count;
                        }
                        else
                        {
                            if (playList.SelectedIndex==playList.Items.Count-1)
                            {
                                playList.SelectedIndex = 0;
                            }
                            else
                                playList.SelectedIndex++;
                        }
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
                slider.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
                slider.Value = 0;
            }
            catch
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Choose right format!!!", "Error");
            }
        }

        private void AddBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.ShowDialog();
                tmp = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                compos.Add(tmp, dlg.FileName);
                playList.Items.Add(tmp);
            }
            catch (ArgumentException)
            {               
            }
        }

        private void StartBt_Click(object sender, RoutedEventArgs e)
        {

            if (playList.Items.Count > 0)
            {
                if (playList.SelectedIndex > -1)
                {
                    if (cb.IsChecked == false)
                    {
                        player.Play();
                        Timer.Start();
                        length.Content = player.NaturalDuration;
                        time.Content = "00:00:00";
                    }
                    else
                    {
                        int count = playList.Items.Count;
                        Random rnd = new Random();

                        count = rnd.Next(0, count - 1);

                        tmp = playList.Items[count].ToString();
                        string melody = compos[tmp];
                        player.Open(new Uri(melody, UriKind.Relative));
                        Thread.Sleep(800);
                        player.Play();
                        Timer.Start();
                        playList.SelectedItem = tmp;
                        length.Content = player.NaturalDuration;
                        time.Content = "00:00:00";
                    }
                }
                else
                {
                    if (cb.IsChecked==true)
                    {
                        int count = playList.Items.Count;
                        Random rnd = new Random();

                        count = rnd.Next(0, count - 1);

                        tmp = playList.Items[count].ToString();
                        string melody = compos[tmp];
                        player.Open(new Uri(melody, UriKind.Relative));
                        Thread.Sleep(800);
                        player.Play();
                        Timer.Start();
                        playList.SelectedItem = tmp;
                        length.Content = player.NaturalDuration;
                        time.Content = "00:00:00";
                    }
                    else
                    {
                        SystemSounds.Hand.Play();
                        MessageBox.Show("Choose the song", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Add music in playlist", "Error");
            }
        }

        private void StopBt_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Timer.Stop();
            length.Content = "";
            time.Content = "";
        }

        private void PauseBt_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void PlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tmp = playList.Items[playList.SelectedIndex].ToString();
                string melody = compos[tmp];
                player.Open(new Uri(melody, UriKind.Relative));
            }
            catch
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Choose the composition", "Error");
            }
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sliderValue = (double)volume.Value;
            player.Volume = sliderValue;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)slider.Value;
            time.Content = (sliderValue / 3600).ToString() + ":" + (sliderValue / 60).ToString() + ":" + (sliderValue % 60).ToString();
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            play = true;
        }

        private void Slider_DragEnded(object sender, DragCompletedEventArgs e)
        {
            int sliderValue = (int)slider.Value;

            ts = new TimeSpan(0, 0, sliderValue);
            player.Position = ts;
            play = false;
        }
    }
}
