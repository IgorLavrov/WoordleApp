using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WoordleApp
{
    public partial class MainPage : CarouselPage
    {
        private ObservableCollection<string> usedLetters;
        private ObservableCollection<string> words;
        private string secretWord;

        private int score;
        private TimeSpan timeRemaining;
        private const int MaxTimeInSeconds = 60; // Set the maximum time for the game

        
        



        public ICommand ClearDataCommand { get; private set; }
        public ICommand AddWordCommand { get; private set; }
        public ICommand StartGameCommand { get; private set; }
        public ICommand UpdateUserInput { get; private set; }
        public ICommand SaveToFileCommand { get; private set; }

        private const string FileName = "wordList.json";

        public int Score
        {
            get { return score; }
            set
            {
                score = value;
                OnPropertyChanged(nameof(Score));
            }
        }

        // Add TimeRemaining property
        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
            set
            {
                timeRemaining = value;
                OnPropertyChanged(nameof(TimeRemaining));
            }
        }



        public ObservableCollection<string> Words
        {
            get { return words; }
            set
            {
                words = value;
                OnPropertyChanged(nameof(Words));
            }
        }



        public MainPage()
        {

            InitializeComponent();
            usedLetters = new ObservableCollection<string>();
            words = new ObservableCollection<string>();// Initialize the Words property
            score = 0;
            Score = score;
            timeRemaining = TimeSpan.FromSeconds(MaxTimeInSeconds);
            TimeRemaining = timeRemaining;



            UpdateUserInput = new Command<string>(OnUpdateUserInput);
            AddWordCommand = new Command(OnAddWord);
            StartGameCommand = new Command(OnStartGame);
            SaveToFileCommand = new Command(OnSaveToFile);
            ClearDataCommand = new Command(OnClearData);
            LoadWordsFromFile(); // Load words from file when the app starts

            BindingContext = this;
            StartNewRound();
        }

        private void OnClearData()
        {
            try
            {
                if (File.Exists(GetFilePath()))
                {
                    File.Delete(GetFilePath());
                    Words.Clear();  // Clear the ObservableCollection
                    OnPropertyChanged(nameof(Words));
                    ResultLabel.Text = "Data cleared successfully.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing data: {ex.Message}");
                ResultLabel.Text = "Error clearing data.";
            }
        }
        private void OnUpdateUserInput(string letter)
        {
            if (!usedLetters.Contains(letter))
            {
                usedLetters.Add(letter);
                UpdateUsedLettersLabel();
                UpdateSecretWordLabel();

                // Other logic remains unchanged
            }
            else
            {
                ResultLabel.Text = "You already guessed this letter. Try another one.";
            }
            if (usedLetters.Contains(letter) && secretWord.Contains(letter))
            {
                Score += 10;
            }
        }

        private void OnAddWord()
        {
            string newWord = NewWordEntry.Text.ToUpper().Trim();
            if (!string.IsNullOrEmpty(newWord))
            {
                words.Add(newWord);
                NewWordEntry.Text = "";
                OnPropertyChanged(nameof(Words)); // Notify the binding system that the collection has changed
            }
            else
            {
                ResultLabel.Text = "Please enter a valid word.";
            }
        }

        private void OnStartGame()
        {
            if (WordPicker.SelectedItem != null && Words != null)
            {
                StartNewRound();
            }
            else
            {
                ResultLabel.Text = "Game started";
            }
        }
        private async Task UpdateTime()
        {
            while (TimeRemaining.TotalSeconds > 0)
            {
                await Task.Delay(1000);
                TimeRemaining = TimeRemaining.Subtract(TimeSpan.FromSeconds(1));
            }
        }
        private void StartUpdatingTime()
        {
            Task.Run(async () => await UpdateTime());
        }
        private void StartNewRound()
        {
            if (WordPicker != null && WordPicker.SelectedItem != null)
            {
                secretWord = WordPicker.SelectedItem.ToString();
                usedLetters.Clear();
               
                UpdateUsedLettersLabel();
                UpdateSecretWordLabel();
                timeRemaining = TimeSpan.FromSeconds(MaxTimeInSeconds);

                TimeRemaining = timeRemaining;
                StartUpdatingTime();

                //UpdateScoreLabel();
            }
          
        }
        private void SaveWordsToFile()
        {
            try
            {
                var filePath = GetFilePath();
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Words);

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving words to file: {ex.Message}");
            }
        }

        private void LoadWordsFromFile()
        {
            try
            {
                var filePath = GetFilePath();

                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var loadedWords = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<string>>(json);

                    if (loadedWords != null)
                    {
                        Words = loadedWords;
                        OnPropertyChanged(nameof(Words));

                        // Set the ItemsSource of the Picker to the loaded words
                        WordPicker.ItemsSource = Words;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading words from file: {ex.Message}");
            }
        }


        private string GetFilePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, FileName);
        }
        private void OnSaveToFile()
        {
            SaveWordsToFile(); // Save words to file when the "Save to File" button is clicked
        }

        private void UpdateUsedLettersLabel()
        {
            UsedLettersLabel.Text = "Used Letters: " + string.Join(", ", usedLetters);
        }

      

        private void UpdateSecretWordLabel()
        {
            string displayWord = "";
            foreach (char letter in secretWord)
            {
                displayWord += usedLetters.Contains(letter.ToString()) ? letter + " " : "_ ";
            }
            SecretWordLabel.Text = displayWord.Trim();
        }



    }
}

