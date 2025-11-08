using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using WpfMediaPlayer.ViewModels;

namespace WpfMediaPlayer.ViewModels
{
    /// <summary>
    /// ViewModel razred - vsebuje aplikacijsko logiko in podatkovni model
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        // ========================================
        // IMPLEMENTACIJA INotifyPropertyChanged
        // ========================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ========================================
        // PODATKOVNI MODEL
        // ========================================

        // Seznam multimedijskih vsebin
        private ObservableCollection<MediaItem> _playlistItems;
        public ObservableCollection<MediaItem> PlaylistItems
        {
            get { return _playlistItems; }
            set
            {
                if (_playlistItems != value)
                {
                    _playlistItems = value;
                    OnPropertyChanged(nameof(PlaylistItems));
                }
            }
        }

        // Trenutno izbran element
        private MediaItem _selectedItem;
        public MediaItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    // Označimo prejšnji element kot neaktiven
                    if (_selectedItem != null)
                    {
                        _selectedItem.IsPlaying = false;
                    }

                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));

                    // Posodobimo stanje gumbov
                    RemoveCommand.RaiseCanExecuteChanged();
                    EditCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // Števec za dodajanje novih elementov
        private int _newItemCounter = 1;

        // ========================================
        // COMMAND OBJEKTI
        // ========================================

        public Command AddCommand { get; private set; }
        public Command RemoveCommand { get; private set; }
        public Command EditCommand { get; private set; }

        // ========================================
        // KONSTRUKTOR
        // ========================================

        public ViewModel()
        {
            // Inicializiramo seznam
            PlaylistItems = new ObservableCollection<MediaItem>();

            // Dodamo privzete elemente
            InitializeDefaultItems();

            // Inicializiramo Command objekte
            AddCommand = new Command(AddItem);
            RemoveCommand = new Command(RemoveItem, CanRemoveOrEdit);
            EditCommand = new Command(EditItem, CanRemoveOrEdit);
        }

        // ========================================
        // INICIALIZACIJA PRIVZETIH ELEMENTOV
        // ========================================

        private void InitializeDefaultItems()
        {
            PlaylistItems.Add(new MediaItem(
                title: "Sunrise Symphony",
                filePath: "Media/video1.mp4",
                thumbnailPath: "Thumbnails/video1.jpg",
                duration: "03:45",
                genre: "Ambientalna glasba",
                artist: "Nature Sounds Collective"
            ));

            PlaylistItems.Add(new MediaItem(
                title: "Ocean Waves",
                filePath: "Media/audio1.mp3",
                thumbnailPath: "Thumbnails/audio1.jpg",
                duration: "05:20",
                genre: "Relaksacijska",
                artist: "Relaxation Masters"
            ));

            PlaylistItems.Add(new MediaItem(
                title: "Mountain Journey",
                filePath: "Media/video2.mp4",
                thumbnailPath: "Thumbnails/video2.jpg",
                duration: "04:15",
                genre: "Dokumentarec",
                artist: "Travel Films"
            ));

            PlaylistItems.Add(new MediaItem(
                title: "Piano Meditation",
                filePath: "Media/audio2.mp3",
                thumbnailPath: "Thumbnails/audio2.jpg",
                duration: "06:30",
                genre: "Klasična",
                artist: "Classical Piano Ensemble"
            ));

            PlaylistItems.Add(new MediaItem(
                title: "City Lights",
                filePath: "Media/video3.mp4",
                thumbnailPath: "Thumbnails/video3.jpg",
                duration: "03:55",
                genre: "Urban",
                artist: "Urban Filmmakers"
            ));
        }

        // ========================================
        // METODE ZA UPRAVLJANJE S PODATKI
        // ========================================

        /// <summary>
        /// Doda novo multimedijsko vsebino v seznam
        /// </summary>
        private void AddItem(object parameter)
        {
            try
            {
                // Ustvarimo nov element s statično določenimi podatki
                MediaItem newItem = new MediaItem(
                    title: $"Nova vsebina {_newItemCounter}",
                    filePath: "Media/video1.mp4", // Uporabimo obstoječo datoteko
                    thumbnailPath: "Thumbnails/video1.jpg",
                    duration: "00:30",
                    genre: "Različno",
                    artist: "Dodano statično"
                );

                // Dodamo v seznam
                PlaylistItems.Add(newItem);

                // Povečamo števec
                _newItemCounter++;

                // Obvestimo uporabnika
                MessageBox.Show(
                    $"Uspešno dodana vsebina:\n\n{newItem.Title}",
                    "Dodajanje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Napaka pri dodajanju vsebine:\n{ex.Message}",
                    "Napaka",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Odstrani trenutno izbran element iz seznama
        /// </summary>
        private void RemoveItem(object parameter)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(
                    "Prosim, najprej izberite element za odstranitev.",
                    "Opozorilo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                // Vprašamo uporabnika za potrditev
                MessageBoxResult result = MessageBox.Show(
                    $"Ali ste prepričani, da želite odstraniti:\n\n{SelectedItem.Title}?",
                    "Potrditev odstranitve",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    string removedTitle = SelectedItem.Title;

                    // Odstranimo element
                    PlaylistItems.Remove(SelectedItem);

                    // Resetiramo izbran element
                    SelectedItem = null;

                    // Obvestimo uporabnika
                    MessageBox.Show(
                        $"Uspešno odstranjena vsebina:\n\n{removedTitle}",
                        "Odstranitev",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Napaka pri odstranjevanju vsebine:\n{ex.Message}",
                    "Napaka",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Uredi trenutno izbran element
        /// </summary>
        private void EditItem(object parameter)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(
                    "Prosim, najprej izberite element za urejanje.",
                    "Opozorilo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                // Statično spremenimo naslov
                string oldTitle = SelectedItem.Title;
                SelectedItem.Title = $"{oldTitle} (urejeno)";

                // Spremenimo tudi druge lastnosti
                SelectedItem.Artist = "Statično urejeno";
                SelectedItem.Genre = "Urjeno";

                // Obvestimo uporabnika
                MessageBox.Show(
                    $"Uspešno urejena vsebina:\n\nStaro: {oldTitle}\nNovo: {SelectedItem.Title}",
                    "Urejanje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // POMEMBNO: Ne ustvarjamo novega objekta, samo spreminjamo lastnosti!
                // Vezava podatkov (DataBinding) bo avtomatsko posodobila UI
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Napaka pri urejanju vsebine:\n{ex.Message}",
                    "Napaka",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Preveri ali je element izbran (za Remove in Edit)
        /// </summary>
        private bool CanRemoveOrEdit(object parameter)
        {
            return SelectedItem != null;
        }
    }
}