using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        int MediaIndex = 0;
        int PlayStopIndex = 0;
        string[] files;
        bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            Player.MediaOpened += Player_MediaOpened;
            Player.MediaEnded += Player_MediaEnded;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void FindMusic(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                MusicList.Items.Clear();

                files = Directory.GetFiles(dialog.FileName, "*.*")
                                    .Where(s => s.EndsWith(".mp3") || s.EndsWith(".mp4"))
                                    .ToArray();
                foreach (string file in files)
                {
                    MusicList.Items.Add(Path.GetFileName(file));
                }

                PlayStop.IsEnabled = true;
                SkipNext.IsEnabled = true;
                SkipPrevious.IsEnabled = true;

                PlayMusic();
            }
        }

        private void PlayMusic()
        {
            var file = files[MediaIndex];
            Player.Source = new Uri(file);
            Player.Play();
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaSlider.Value = 0;
            MediaSlider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null && !isDragging)
            {
                MediaSlider.Value = Player.Position.TotalSeconds;
            }
        }

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Position = TimeSpan.FromSeconds(MediaSlider.Value);
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (MediaIndex == files.Length - 1)
            {
                MediaIndex = 0;
            }
            else
            {
                MediaIndex++;
            }
            PlayMusic();
        }

        private void PlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (PlayStopIndex == 0)
            {
                Player.Pause();
                PlayStopIndex = 1;
                PlayStop.Content = "Play";
            }
            else if (PlayStopIndex == 1)
            {
                Player.Play();
                PlayStopIndex = 0;
                PlayStop.Content = "Pause";
            }
        }

        private void SkipPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (MediaIndex != 0)
            {
                MediaIndex -= 1;
            }
            PlayMusic();
        }

        private void SkipNext_Click(object sender, RoutedEventArgs e)
        {
            if (MediaIndex != files.Length - 1)
            {
                MediaIndex += 1;
            }
            PlayMusic();
        }

        private void MusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MediaSlider_DragStarted(object sender, RoutedEventArgs e)
        {
            isDragging = true;
        }

        private void MediaSlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            isDragging = false;
        }
    }
}
