using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfMediaPlayer;

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        // ObservableCollection za seznam predvajanja
        private ObservableCollection<MediaItem> playlistItems;

        // Trenutno predvajani element
        private MediaItem currentPlayingItem;

        // Stanje predvajanja
        private bool isPlaying = false;

        // Timer za posodabljanje sliderja
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializePlaylist();
            InitializeTimer();

            currentTimeLabel.Content = "00:00:00";
            totalTimeLabel.Content = "00:00:00";
        }

        private void InitializePlaylist()
        {
            // Ustvarimo ObservableCollection
            playlistItems = new ObservableCollection<MediaItem>();

            // Dodamo privzete elemente (vsaj 3)
            playlistItems.Add(new MediaItem(
                title: "Sunrise Symphony",
                filePath: "Media/video1.mp4",
                thumbnailPath: "Thumbnails/video1.jpg",
                duration: "03:45",
                genre: "Ambientalna glasba",
                artist: "Nature Sounds Collective"
            ));

            playlistItems.Add(new MediaItem(
                title: "Ocean Waves",
                filePath: "Media/audio1.mp3",
                thumbnailPath: "Thumbnails/audio1.jpg",
                duration: "05:20",
                genre: "Relaksacijska",
                artist: "Relaxation Masters"
            ));

            playlistItems.Add(new MediaItem(
                title: "Mountain Journey",
                filePath: "Media/video2.mp4",
                thumbnailPath: "Thumbnails/video2.jpg",
                duration: "04:15",
                genre: "Dokumentarec",
                artist: "Travel Films"
            ));

            playlistItems.Add(new MediaItem(
                title: "Piano Meditation",
                filePath: "Media/audio2.mp3",
                thumbnailPath: "Thumbnails/audio2.jpg",
                duration: "06:30",
                genre: "Klasična",
                artist: "Classical Piano Ensemble"
            ));

            playlistItems.Add(new MediaItem(
                title: "City Lights",
                filePath: "Media/video3.mp4",
                thumbnailPath: "Thumbnails/video3.jpg",
                duration: "03:55",
                genre: "Urban",
                artist: "Urban Filmmakers"
            ));

            // Povežemo z ListView
            playlistView.ItemsSource = playlistItems;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                // Posodobimo slider
                positionSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                positionSlider.Value = mediaPlayer.Position.TotalSeconds;

                // Posodobimo časovne oznake
                currentTimeLabel.Content = FormatTime(mediaPlayer.Position);
                totalTimeLabel.Content = FormatTime(mediaPlayer.NaturalDuration.TimeSpan);
            }
        }

        private string FormatTime(TimeSpan time)
        {
            return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }

        // ========================================
        // DOGODKI - MENI
        // ========================================
        private void MenuItem_Izhod_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Close();
            }
            Application.Current.Shutdown();
        }

        // ========================================
        // DOGODKI - GUMBI
        // ========================================

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source == null)
            {
                MessageBox.Show("Prosim, izberite datoteko za predvajanje.",
                    "Ni izbrane datoteke", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (isPlaying)
            {
                mediaPlayer.Pause();
                ChangePlayPauseIcon("Icons/play.png");
                isPlaying = false;
                timer.Stop();
            }
            else
            {
                mediaPlayer.Play();
                ChangePlayPauseIcon("Icons/pause.png");
                isPlaying = true;
                timer.Start();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            ChangePlayPauseIcon("Icons/play.png");
            isPlaying = false;
            timer.Stop();
            currentTimeLabel.Content = "00:00:00";
            positionSlider.Value = 0;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlistView.SelectedIndex > 0)
            {
                playlistView.SelectedIndex--;
                PlaySelectedItem();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlistView.SelectedIndex < playlistItems.Count - 1)
            {
                playlistView.SelectedIndex++;
                PlaySelectedItem();
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle Tag property (String "True"/"False")
            bool isActive = repeatButton.Tag?.ToString() == "True";
            repeatButton.Tag = (!isActive).ToString();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle Tag property
            bool isActive = shuffleButton.Tag?.ToString() == "True";
            shuffleButton.Tag = (!isActive).ToString();
        }

        // ========================================
        // DOGODKI - LISTVIEW (DVOKLIK)
        // ========================================

        private void PlaylistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaySelectedItem();
        }

        private void PlaylistView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (playlistView.SelectedItem != null)
            {
                MediaItem selectedItem = (MediaItem)playlistView.SelectedItem;

                MessageBox.Show(
                    $"Izbrana vsebina:\n\n" +
                    $"Naslov: {selectedItem.Title}\n" +
                    $"Izvajalec: {selectedItem.Artist}\n" +
                    $"Žanr: {selectedItem.Genre}\n" +
                    $"Trajanje: {selectedItem.Duration}\n" +
                    $"Pot: {selectedItem.FilePath}",
                    "Informacija",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show(
                    "Prosim, izberite element s seznama.",
                    "Opozorilo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        // ========================================
        // POMOŽNE METODE
        // ========================================

        private void PlaySelectedItem()
        {
            if (playlistView.SelectedItem != null)
            {
                MediaItem selectedItem = (MediaItem)playlistView.SelectedItem;

                // Označimo prejšnji element kot neaktiven
                if (currentPlayingItem != null)
                {
                    currentPlayingItem.IsPlaying = false;
                }

                // Označimo trenutni element kot aktiven
                selectedItem.IsPlaying = true;
                currentPlayingItem = selectedItem;

                try
                {
                    // Naložimo medij
                    mediaPlayer.Source = new Uri(selectedItem.FilePath, UriKind.Relative);
                    mediaPlayer.Play();

                    // Posodobimo UI
                    ChangePlayPauseIcon("Icons/pause.png");
                    isPlaying = true;
                    timer.Start();

                    // Posodobimo ime trenutne vsebine
                    currentMediaTextBox.Text = $"{selectedItem.Title} - {selectedItem.Artist}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Napaka pri nalaganju datoteke:\n{ex.Message}",
                        "Napaka", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangePlayPauseIcon(string iconPath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(iconPath, UriKind.Relative));
                playPauseImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Napaka pri nalaganju ikone:\n{ex.Message}",
                    "Napaka", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Preprečimo rekurzivne klice
            if (Math.Abs(positionSlider.Value - mediaPlayer.Position.TotalSeconds) > 1)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(positionSlider.Value);
            }
        }
    }
}