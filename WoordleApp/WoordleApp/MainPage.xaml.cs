using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace WoordleApp
{
    public partial class MainPage : CarouselPage
    {
        private ObservableCollection<string> usedLetters;
        private ObservableCollection<string> words;
        private string secretWord;
        private int score;

        public ICommand AddWordCommand { get; private set; }
        public ICommand StartGameCommand { get; private set; }
        public ICommand UpdateUserInput { get; private set; }

        // Define the Words property
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
            score = 0;
            words = new ObservableCollection<string>();// Initialize the Words property

            // Add some initial words if needed
            Words.Add("XAMARIN");
            Words.Add("WORDLE");

            UpdateUserInput = new Command<string>(OnUpdateUserInput);
            AddWordCommand = new Command(OnAddWord);
            StartGameCommand = new Command(OnStartGame);

            BindingContext = this;
            StartNewRound();
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
                ResultLabel.Text = "Gam started";
            }
        }

        private void StartNewRound()
        {
            if (WordPicker != null && WordPicker.SelectedItem != null)
            {
                secretWord = WordPicker.SelectedItem.ToString();
                usedLetters.Clear();
                score = 0;
                UpdateUsedLettersLabel();
                UpdateSecretWordLabel();


                //UpdateScoreLabel();
            }
            else
            {
                ResultLabel.Text = "Please select a word to start the game.";
            }
        }




        private void UpdateUsedLettersLabel()
        {
            UsedLettersLabel.Text = "Used Letters: " + string.Join(", ", usedLetters);
        }

        //private void UpdateScoreLabel()
        //{
        //    ScoreLabel.Text = $"Score: {score}";
        //}

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

