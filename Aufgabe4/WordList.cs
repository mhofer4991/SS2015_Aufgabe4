//-----------------------------------------------------------------------
// <copyright file="WordList.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a list of words and provides methods to add and check words.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe4
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a list of words and provides methods to add and check words.
    /// </summary>
    public class WordList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordList"/> class.
        /// </summary>
        public WordList()
        {
            this.Words = new List<string>();
        }

        /// <summary>
        /// Gets a list of all words.
        /// </summary>
        /// <value>A list of all words.</value>
        public List<string> Words { get; private set; }

        /// <summary>
        /// Checks if the given word is a valid word.
        /// </summary>
        /// <param name="word">Given word.</param>
        /// <returns>A boolean indicating whether the given word is valid or not.</returns>
        public static bool IsValidWord(string word)
        {
            if (word.Length > 0)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (!char.IsLetter(word[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new word to the list and returns the changed list.
        /// </summary>
        /// <param name="word">The new word.</param>
        /// <returns>The changed list of all words.</returns>
        public List<string> AddWord(string word)
        {
            if (!WordList.IsValidWord(word))
            {
                throw new ArgumentException("The word is not valid!");
            }

            if (!this.Words.Contains(word.ToUpper()))
            {
                this.Words.Add(word.ToUpper());
            }

            return this.Words;
        }

        /// <summary>
        /// Adds a word list to the current list and returns the changed list.
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <returns>The changed list of all words.</returns>
        public List<string> AddWordList(WordList wordList)
        {
            this.Words.AddRange(wordList.Words);

            return this.Words;
        }

        /// <summary>
        /// Adds multiple words by reading a text file from a given path.
        /// </summary>
        /// <param name="filename">The given file path.</param>
        /// <returns>The changed list of all words.</returns>
        public List<string> AddWordsByFile(string filename)
        {
            string[] lines = File.ReadAllLines(filename);

            for (int i = 0; i < lines.Length; i++)
            {
                this.AddWord(lines[i]);
            }

            return this.Words;
        }
    }
}
