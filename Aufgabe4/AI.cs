//-----------------------------------------------------------------------
// <copyright file="AI.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents an artificial intelligence, which guesses words by the frequency of all letters.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents an artificial intelligence, which guesses words by the frequency of all letters.
    /// </summary>
    public class AI
    {
        /// <summary>
        /// Instance of the game, which contains the word list.
        /// </summary>
        private Game game;

        /// <summary>
        /// Frequency of each letter.
        /// </summary>
        private int[] letterRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AI"/> class.
        /// </summary>
        /// <param name="game">Instance of the game, which contains the word list.</param>
        public AI(Game game)
        {
            this.game = game;
            this.letterRate = new int[Hangman.GetAvailableLetters().Length];

            this.IsPlaying = false;
        }

        /// <summary>
        /// Gets a value indicating whether the AI is playing or not.
        /// </summary>
        /// <value>A boolean, which indicates whether the AI is playing or not.</value>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Tells the AI that a new game has been started.
        /// This leads to the calculation of the frequency of all letters.
        /// </summary>
        public void StartNewGame()
        {
            this.IsPlaying = true;

            for (int i = 0; i < this.letterRate.Length; i++)
            {
                this.letterRate[i] = 0;
            }

            this.CalculateLetterRate();
        }

        /// <summary>
        /// Returns a letter, which should be contained in the word.
        /// </summary>
        /// <returns>A letter, which should be contained in the word.</returns>
        public char GuessLetter()
        {
            char[] alpha = Hangman.GetAvailableLetters();
            int preferredLetter = 0; // Index of the alphabet

            for (int i = 0; i < alpha.Length; i++)
            {
                if (this.letterRate[i] > this.letterRate[preferredLetter])
                {
                    preferredLetter = i;
                }
            }

            // This prevents the AI from using this letter again.
            this.letterRate[preferredLetter] = -1;

            return alpha[preferredLetter];
        }

        /// <summary>
        /// Calculates the frequency of each available letter.
        /// </summary>
        private void CalculateLetterRate()
        {
            char[] alpha = Hangman.GetAvailableLetters();

            for (int i = 0; i < this.game.WordList.Words.Count; i++)
            {
                for (int j = 0; j < this.game.WordList.Words[i].Length; j++)
                {
                    int index = Array.IndexOf(alpha, this.game.WordList.Words[i].ToUpper()[j]);

                    if (index >= 0)
                    {
                        this.letterRate[index]++;
                    }
                }
            }
        }
    }
}
