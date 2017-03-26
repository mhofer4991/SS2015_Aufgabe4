//-----------------------------------------------------------------------
// <copyright file="Hangman.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents the hangman and provides methods to guess and check letters and words.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the hangman and provides methods to guess and check letters and words.
    /// </summary>
    public class Hangman
    {
        /// <summary>
        /// Contains all parts, which are needed to draw the hang to begin.
        /// </summary>
        private string[] hangParts = new string[]
        { 
            "  ========= ", 
            "  ||        ", 
            "  ||        ", 
            "  ||        ", 
            "  ||        ", 
            "  ||        ", 
            "======      ", 
            "||  ||      " 
        };

        /// <summary>
        /// Contains all parts, which are needed to draw the man.
        /// </summary>
        private string[] manParts = new string[] 
        { 
            "========\n", 
            " /", 
            "     |\n", 
            "/", 
            "      O\n", 
            "      \\", 
            "|", 
            "/\n", 
            "       |\n", 
            "      /", 
            " \\"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Hangman"/> class.
        /// </summary>
        public Hangman()
        {
            this.WrongLetters = new List<char>();

            this.Reset();
        }

        /// <summary>
        /// Delegate for event OnHangmanWon.
        /// </summary>
        public delegate void HangmanWon();

        /// <summary>
        /// Delegate for event OnHangmanLost.
        /// </summary>
        public delegate void HangmanLost();

        /// <summary>
        /// Is called when the hangman is completed.
        /// </summary>
        public event HangmanWon OnHangmanWon;

        /// <summary>
        /// Is called when the word is completed.
        /// </summary>
        public event HangmanLost OnHangmanLost;

        /// <summary>
        /// The enumeration of all available difficulty levels.
        /// </summary>
        public enum HangmanDifficultyLevel
        {
            /// <summary> This is the Level 1. </summary>
            L1 = 1,

            /// <summary> This is the Level 2. </summary>
            L2 = 2,

            /// <summary> This is the Level 3. </summary>
            L3 = 3,

            /// <summary> This is the Level 4. </summary>
            L4 = 4,

            /// <summary> This is the Level 5. </summary>
            L5 = 5,

            /// <summary> This is the Level 6. </summary>
            L6 = 6
        }

        /// <summary>
        /// Gets the word, which has to be guessed.
        /// </summary>
        /// <value>The word, which has to be guessed.</value>
        public string GuessingWord { get; private set; }

        /// <summary>
        /// Gets all letters, which represent the guessing word.
        /// </summary>
        /// <value>All letters, which represent the guessing word.</value>
        public char[] GuessedLetters { get; private set; }

        /// <summary>
        /// Gets a list of all letters, which has already been guessed.
        /// </summary>
        /// <value>A list of all letters, which has already been guessed.</value>
        public List<char> WrongLetters { get; private set; }

        /// <summary>
        /// Gets the current difficulty level of the hangman.
        /// </summary>
        /// <value>The current difficulty level of the hangman.</value>
        public HangmanDifficultyLevel DifficultyLevel { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the hangman is playing or not.
        /// </summary>
        /// <value>A boolean indicating whether the hangman is playing or not.</value>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Returns the maximum amount of wrong letters, which can be guessed on a specified difficulty level.
        /// </summary>
        /// <param name="difficultyLevel">The given difficulty level.</param>
        /// <returns>The maximum amount of wrong letters, which can be guessed on a specified difficulty level.</returns>
        public static int GetMaximumAmountOfWrongLetters(HangmanDifficultyLevel difficultyLevel)
        {
            switch (difficultyLevel)
            {
                case HangmanDifficultyLevel.L1:
                    return 10;
                case HangmanDifficultyLevel.L2:
                    return 8;
                case HangmanDifficultyLevel.L3:
                    return 6;
                case HangmanDifficultyLevel.L4:
                    return 4;
                case HangmanDifficultyLevel.L5:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns all available letters, which can be used to guess the word.
        /// </summary>
        /// <returns>All available letters, which can be used to guess the word.</returns>
        public static char[] GetAvailableLetters()
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜß".ToCharArray();
        }

        /// <summary>
        /// Resets the hangman by clearing all words and lists.
        /// </summary>
        public void Reset()
        {
            this.GuessingWord = string.Empty;
            this.GuessedLetters = new char[] { };
            this.WrongLetters.Clear();
            this.DifficultyLevel = HangmanDifficultyLevel.L1;

            this.IsPlaying = false;
        }

        /// <summary>
        /// Tells the hangman that a new game with a given word and difficulty level has been started.
        /// </summary>
        /// <param name="guessingWord">The given word.</param>
        /// <param name="difficultyLevel">The given difficulty level.</param>
        public void StartNewGame(string guessingWord, HangmanDifficultyLevel difficultyLevel)
        {
            this.GuessingWord = guessingWord.ToUpper();
            this.DifficultyLevel = difficultyLevel;
            this.WrongLetters.Clear();

            // At the begin of a new game, all letters have not been guessed yet -> char code 0.
            this.GuessedLetters = new char[this.GuessingWord.Length];

            this.IsPlaying = true;
        }

        /// <summary>
        /// Tells the hangman that the given letter has been guessed.
        /// </summary>
        /// <param name="letter">The given letter.</param>
        /// <returns>A boolean indicating whether the guessed letter was right or wrong.</returns>
        public bool GuessLetter(char letter)
        {
            if (!Hangman.GetAvailableLetters().Contains(letter.ToString().ToUpper()[0]))
            {
                return false;
            }

            // Convert the letter to a capitalized letter.
            letter = letter.ToString().ToUpper()[0];

            if (this.GuessingWord.Contains(letter))
            {
                // Only true, if no item of the guessed letters has the char code 0.
                bool won = true;

                for (int i = 0; i < this.GuessingWord.Length; i++)
                {
                    if (this.GuessingWord[i] == letter)
                    {
                        this.GuessedLetters[i] = letter;
                    }
                    else if (this.GuessedLetters[i] == 0)
                    {
                        won = false;
                    }
                }

                // If the user guessed all words, an event will be fired.
                if (won)
                {
                    if (this.OnHangmanWon != null)
                    {
                        this.OnHangmanWon();
                    }
                }

                return true;
            }
            else
            {
                if (!this.WrongLetters.Contains(letter))
                {
                    this.WrongLetters.Add(letter);
                }

                // Check if the user guessed too many wrong letters.
                if (this.WrongLetters.Count > Hangman.GetMaximumAmountOfWrongLetters(this.DifficultyLevel))
                {
                    if (this.OnHangmanLost != null)
                    {
                        this.OnHangmanLost();
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Draws the hangman on the console.
        /// </summary>
        /// <param name="x">X coordinate of the hangman.</param>
        /// <param name="y">Y coordinate of the hangman.</param>
        public void Draw(int x, int y)
        {
            int wrongLettersCount = this.WrongLetters.Count;

            // How many parts have to be drawn per wrong letter? This depends on the difficulty level.
            double partsPerWrongLetter = this.manParts.Length / ((double)Hangman.GetMaximumAmountOfWrongLetters(this.DifficultyLevel) + 1);

            wrongLettersCount = (int)(wrongLettersCount * partsPerWrongLetter);

            // Draw the hang on the screen.
            for (int i = 0; i < this.hangParts.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.WriteLine(this.hangParts[i]);
            }

            Console.SetCursorPosition(x + 4, y);

            int tempX = Console.CursorLeft;

            // Draw the man on the screen.
            for (int i = 0; i < wrongLettersCount && i < this.manParts.Length; i++)
            {
                if (i > 0 && this.manParts[i - 1].Contains("\n"))
                {
                    Console.CursorLeft = tempX;
                }

                Console.Write(this.manParts[i]);
            }
        }
    }
}
