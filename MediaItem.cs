using System;
using System.ComponentModel;

namespace WpfMediaPlayer
{
    public class MediaItem : INotifyPropertyChanged
    {
        // Event za obveščanje o spremembah lastnosti
        public event PropertyChangedEventHandler PropertyChanged;

        // Privatne spremenljivke
        private string _title;
        private string _filePath;
        private string _thumbnailPath;
        private string _duration;
        private string _genre;
        private string _artist;
        private bool _isPlaying;

        // Konstruktor
        public MediaItem(string title = "", string filePath = "", string thumbnailPath = "",
                         string duration = "00:00", string genre = "", string artist = "")
        {
            Title = title;
            FilePath = filePath;
            ThumbnailPath = thumbnailPath;
            Duration = duration;
            Genre = genre;
            Artist = artist;
            IsPlaying = false;
        }

        // Javne lastnosti z implementacijo obveščanja

        // 1. NASLOV (obvezno)
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        // 2. POT DO DATOTEKE (obvezno)
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        // 3. POT DO SLIKE (obvezno)
        public string ThumbnailPath
        {
            get { return _thumbnailPath; }
            set
            {
                if (_thumbnailPath != value)
                {
                    _thumbnailPath = value;
                    OnPropertyChanged(nameof(ThumbnailPath));
                }
            }
        }

        // 4. TRAJANJE
        public string Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        // 5. ŽANR
        public string Genre
        {
            get { return _genre; }
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged(nameof(Genre));
                }
            }
        }

        // 6. IZVAJALEC
        public string Artist
        {
            get { return _artist; }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }

        // 7. ALI SE PREDVAJA (za DataTrigger)
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        // Metoda za proženje dogodka PropertyChanged
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Override ToString za lažje debugiranje
        public override string ToString()
        {
            return $"{Title} - {Artist}";
        }
    }
}