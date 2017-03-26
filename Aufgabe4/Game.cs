//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents the game, which can be played by the user or the computer.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the game, which can be played by the user or the computer.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Is needed for choosing a random word from the list.
        /// </summary>
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            this.WordList = new WordList();
            this.HangMan = new Hangman();
            this.random = new Random();

            this.HangMan.OnHangmanLost += this.HangMan_OnHangmanLost;
            this.HangMan.OnHangmanWon += this.HangMan_OnHangmanWon;

            this.Reset();
        }

        /// <summary>
        /// Delegate for event OnGameWon.
        /// </summary>
        public delegate void GameWon();

        /// <summary>
        /// Delegate for event OnGameLost.
        /// </summary>
        public delegate void GameLost();

        /// <summary>
        /// Is called when the player lost the game.
        /// </summary>
        public event GameWon OnGameWon;

        /// <summary>
        /// Is called when the player won the game.
        /// </summary>
        public event GameLost OnGameLost;

        /// <summary>
        /// Gets the current difficulty level of the game.
        /// </summary>
        /// <value>The current difficulty level of the game.</value>
        public Hangman.HangmanDifficultyLevel DifficultyLevel { get; private set; }

        /// <summary>
        /// Gets the amount of guessed words in a row.
        /// </summary>
        /// <value>The amount of guessed words in a row.</value>
        public int GuessedWords { get; private set; }

        /// <summary>
        /// Gets the list of all available words.
        /// </summary>
        /// <value>The list of all available words.</value>
        public WordList WordList { get; private set; }

        /// <summary>
        /// Gets an instance of the hangman class.
        /// </summary>
        /// <value>An instance of the hangman class.</value>
        public Hangman HangMan { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game is running or not.
        /// </summary>
        /// <value>A value indicating whether the game is running or not.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Adds the given word list to the game.
        /// </summary>
        /// <param name="list">The given word list.</param>
        public void AddWordList(WordList list)
        {
            this.WordList.AddWordList(list);
        }

        /// <summary>
        /// Adds the given word to the game.
        /// </summary>
        /// <param name="word">The given word.</param>
        public void AddWord(string word)
        {
            this.WordList.AddWord(word);
        }

        /// <summary>
        /// Returns the help text for the game.
        /// </summary>
        /// <returns>The help text for the game.</returns>
        public string GetHelpText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("The goal of this game is to find out the word by guessing letters.");
            sb.Append("\n\n");
            sb.Append("The maximum amount of wrong letters depends on the difficulty level [1 - 6]:\n\n");
            sb.Append("  - L1: Game over after 11 wrong letters\n");
            sb.Append("  - L2:                  9 wrong letters\n");
            sb.Append("  - L3:                  7 wrong letters\n");
            sb.Append("  - L4:                  5 wrong letters\n");
            sb.Append("  - L5:                  3 wrong letters\n");
            sb.Append("  - L6:                  1 wrong letter\n");
            sb.Append("\nTo play a new game, press [F2]. The difficulty level always starts with L1\nand increases after every third victory, when you continue to play.");
            sb.Append("\n\nTo let the computer play a game, press [F3] and set\nthe difficulty level for the AI.");
            sb.Append("\n\nThere are 2 approaches to add words to the game: ");
            sb.Append("\n\n - By Command line arguments: -f file1.txt file2.txt -w word1 word2");
            sb.Append("\n - By pressing [F3]: You can add multiple words.");

            return sb.ToString();
        }

        /// <summary>
        /// Stops the game and resets the hangman.
        /// </summary>
        public void Stop()
        {
            this.HangMan.Reset();

            this.IsRunning = false;
        }

        /// <summary>
        /// Stops the game and resets the difficulty level and the amount of guessed words in a row.
        /// </summary>
        public void Reset()
        {
            this.Stop();

            this.DifficultyLevel = (Hangman.HangmanDifficultyLevel)1;
            this.GuessedWords = 0;
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void StartNewGame()
        {
            this.StartNewGame(this.DifficultyLevel);
        }

        /// <summary>
        /// Starts a new game with the given difficulty level.
        /// </summary>
        /// <param name="difficultyLevel">The given difficulty level.</param>
        public void StartNewGame(Hangman.HangmanDifficultyLevel difficultyLevel)
        {
            string randomWord = this.WordList.Words[this.random.Next(0, this.WordList.Words.Count)];

            this.DifficultyLevel = difficultyLevel;
            this.HangMan.StartNewGame(randomWord, this.DifficultyLevel);
            this.IsRunning = true;
        }

        /// <summary>
        /// Guesses the given letter.
        /// </summary>
        /// <param name="letter">The given letter.</param>
        public void GuessLetter(char letter)
        {
            this.HangMan.GuessLetter(letter);
        }

        /// <summary>
        /// Draws the game on the console.
        /// </summary>
        /// <param name="x">X coordinate of the game.</param>
        /// <param name="y">Y coordinate of the game.</param>
        public void Draw(int x, int y)
        {
            char[] alpha = Hangman.GetAvailableLetters();

            this.HangMan.Draw(x, y);

            Console.SetCursorPosition(x + 16, y);

            Console.Write("Difficulty level: {0}", ((int)this.DifficultyLevel).ToString());

            Console.SetCursorPosition(x + 16, y + 2);

            Console.Write("Word: ");

            for (int i = 0; i < this.HangMan.GuessedLetters.Length; i++)
            {
                if (this.HangMan.GuessedLetters[i] == 0)
                {
                    Console.Write("_ ");
                }
                else
                {
                    Console.Write("{0} ", this.HangMan.GuessedLetters[i]);
                }
            }

            ConsoleColor temp = Console.ForegroundColor;

            Console.SetCursorPosition(x + 16, y + 5);

            for (int i = 0; i < alpha.Length; i++)
            {
                if (this.HangMan.WrongLetters.Contains(alpha[i]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (this.HangMan.GuessedLetters.Contains(alpha[i]))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = temp;
                }

                Console.Write("{0} ", alpha[i]);

                if (i == 14)
                {
                    Console.SetCursorPosition(x + 16, y + 7);
                }
            }

            Console.ForegroundColor = temp;
        }

        /// <summary>
        /// Is called when the word is guessed.
        /// </summary>
        private void HangMan_OnHangmanWon()
        {
            this.GuessedWords++;

            if (this.GuessedWords % 3 == 0)
            {
                if ((int)this.DifficultyLevel < (int)Hangman.HangmanDifficultyLevel.L6)
                {
                    this.DifficultyLevel = (Hangman.HangmanDifficultyLevel)((int)this.DifficultyLevel + 1);
                }
            }

            if (this.OnGameWon != null)
            {
                this.OnGameWon();
            }
        }

        /// <summary>
        /// Is called when the hangman is completed.
        /// </summary>
        private void HangMan_OnHangmanLost()
        {
            if (this.OnGameLost != null)
            {
                this.OnGameLost();
            }
        }
    }
}
