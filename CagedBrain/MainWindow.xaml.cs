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

namespace CagedBrain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool lastGuess;
        private List<bool> guesses = new List<bool>();
        private int myScore = 0;
        private int yourScore = 0;

        private int statNGram;
        private int statBalance;
        private bool debug = false;

        public MainWindow()
        {
            InitializeComponent();

            this.readyButton.Click += ReadyButton_Click;
            this.correctButton.Click += CorrectButton_Click;
            this.incorrectButton.Click += IncorrectButton_Click;
            this.titleLabel.MouseDoubleClick += TitleLabel_MouseDoubleClick;
            this.resetButton.Click += ResetButton_Click;

            UpdateGuessLabel();
            ShowReply(false);
            this.lastGuess = MakeGuess();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this.myScore = 0;
            this.yourScore = 0;
            this.statNGram = 0;
            this.statBalance = 0;
            this.guesses.Clear();

            UpdateGuessLabel();
            ShowReply(false);
            this.lastGuess = MakeGuess();
        }

        private void TitleLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.debug = !this.debug;
            UpdateGuessLabel();
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            this.myGuessIsLabel.Content = $"Mit gæt er  {(this.lastGuess ? "1" : "0")}";
            ShowReply(true);
        }

        private void CorrectButton_Click(object sender, RoutedEventArgs e)
        {
            this.myScore++;
            guesses.Add(this.lastGuess);
            this.lastGuess = MakeGuess();
            UpdateGuessLabel();
            ShowReply(false);
        }

        private void IncorrectButton_Click(object sender, RoutedEventArgs e)
        {
            this.yourScore++;
            guesses.Add(!this.lastGuess);
            this.lastGuess = MakeGuess();
            UpdateGuessLabel();
            ShowReply(false);
        }

        private bool MakeGuess()
        {
            int balance = 0;
            this.statNGram = 1;
            for (int ngram=5; ngram>=2; ngram--)
            {
                balance = GetBalance(ngram);
                if (balance != 0)
                {
                    this.statNGram = ngram;
                    break;
                }
            }

            this.statBalance = balance;

            if (balance < 0)
            {
                return false;
            }
            else if (balance > 0)
            {
                return true;
            }

            return new Random().Next(0, 2) == 0;
        }

        private int GetBalance(int localNgram)
        {
            if (this.guesses.Count <= localNgram)
            {
                return 0;
            }

            var balance = 0;

            for (int n = 0; n < this.guesses.Count - localNgram; ++n)
            {
                var match = true;
                for (int o = 0; o < localNgram; ++o)
                {
                    if (this.guesses[this.guesses.Count - localNgram + o] != this.guesses[n + o])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    if (this.guesses[n + localNgram])
                    {
                        balance++;
                    }
                    else
                    {
                        balance--;
                    }
                }
            }

            return balance;
        }

        private void ShowReply(bool show)
        {
            this.myGuessIsLabel.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            this.replyButtonsStackPanel.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            this.readyButton.Visibility = !show ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateGuessLabel()
        {
            this.statsLabel.Content = this.debug ? $"{this.statNGram}:{this.statBalance}" : String.Empty;
            this.guessesSoFarLabel.Content = string.Join(" ", this.guesses.Select(g => g ? "1" : "0"));
            this.myPoints.Content = $"{this.myScore} points";
            this.yourPoints.Content = $"{this.yourScore} points";
        }
    }
}
