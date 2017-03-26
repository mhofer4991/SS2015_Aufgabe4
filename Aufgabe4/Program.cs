//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>The user can use this program to play hangman or let the computer guess the letters.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe4
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The user can use this program to play hangman or let the computer guess the letters.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Contains all argument tags, which will be interpreted by the program.
        /// </summary>
        private static string[] argumentTags = new string[] { "-f", "-w" };

        /// <summary>
        /// Used for handling user input.
        /// </summary>
        private static InputHandler inputHandler;

        /// <summary>
        /// An instance of a game, which can be played be the user or the computer.
        /// </summary>
        private static Game game;

        /// <summary>
        /// An instance of the AI, which can play a game.
        /// </summary>
        private static AI computer;

        /// <summary>
        /// This method represents the entry point of the program.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void Main(string[] args)
        {
            game = new Game();
            inputHandler = new InputHandler();
            computer = new AI(game);

            game.OnGameWon += Game_OnGameWon;
            game.OnGameLost += Game_OnGameLost;

            game.AddWordList(GetWordsByArguments(args));

            inputHandler.SubscribeForKey(ConsoleKey.F1);
            inputHandler.SubscribeForKey(ConsoleKey.F2);
            inputHandler.SubscribeForKey(ConsoleKey.F3);
            inputHandler.SubscribeForKey(ConsoleKey.F4);
            inputHandler.SubscribeForKey(ConsoleKey.F5);

            inputHandler.OnSubscribedKeyCalled += HandleSpecialKeyInput;
            
            while (true)
            {
                Draw();
                inputHandler.WaitForSubscribedKey();
            }
        }

        /// <summary>
        /// Gets called when the user or the computer lost.
        /// </summary>
        private static void Game_OnGameLost()
        {
            Draw();

            game.Stop();

            Console.Write("\n\n -> GAME OVER! ");

            Console.ReadLine();
        }

        /// <summary>
        /// Gets called when the user or the computer won.
        /// </summary>
        private static void Game_OnGameWon()
        {
            Draw();

            game.Stop();

            if (!computer.IsPlaying)
            {
                Console.Write("\n\n -> Congratulation, you guessed the word!\n    To continue playing, press [Enter]: ");

                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    game.StartNewGame();
                    PlayGame();
                }
            }
            else
            {
                Console.Write("\n\n -> The computer guessed the word! ");

                Console.ReadLine();
            }
        }

        /// <summary>
        /// Handles the given console key from the user input.
        /// </summary>
        /// <param name="key">The given console key.</param>
        private static void HandleSpecialKeyInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.F1:
                    DrawHelp();
                    break;
                case ConsoleKey.F2:
                    if (game.WordList.Words.Count > 0)
                    {
                        StartNewGame();
                    }

                    break;
                case ConsoleKey.F3:
                    game.AddWordList(AddNewWords());
                    break;
                case ConsoleKey.F4:
                    if (game.WordList.Words.Count > 0)
                    {
                        StartNewAIGame();
                    }

                    break;
                case ConsoleKey.F5:
                    ExitGame();
                    break;
            }
        }

        /// <summary>
        /// Draws the program on the console.
        /// </summary>
        private static void Draw()
        {
            Console.Clear();

            DrawMenu();

            if (game.IsRunning)
            {
                game.Draw(1, 3);
            }
            else
            {
                Console.WriteLine(" There are currently {0} different words available.\n To add words, press [F3]", game.WordList.Words.Count);
            }
        }

        /// <summary>
        /// Draws the menu by displaying all currently available options.
        /// </summary>
        private static void DrawMenu()
        {
            if (game.WordList.Words.Count > 0)
            {
                if (game.IsRunning)
                {
                    Console.WriteLine("\n [F1] Help  [F2] New game  [F3] New word  [F4] New AI - game  [F5] Exit game\n");
                }
                else
                {
                    Console.WriteLine("\n [F1] Help  [F2] New game  [F3] New word  [F4] New AI - game  [F5] Exit program\n");
                }
            }
            else
            {
                Console.WriteLine("\n [F1] Help  [F3] New word  [F5] Exit program\n");
            }
        }

        /// <summary>
        /// Draws the help screen.
        /// </summary>
        private static void DrawHelp()
        {
            string[] textLines = game.GetHelpText().Split('\n');

            Console.Clear();

            Console.WriteLine("\n [Enter] Close help\n");

            for (int i = 0; i < textLines.Length; i++)
            {
                Console.WriteLine("  {0}", textLines[i]);
            }
            
            Console.ReadLine();
        }

        /// <summary>
        /// Starts a new game, which will be played by the user.
        /// </summary>
        private static void StartNewGame()
        {
            Console.Clear();

            // If a game is currently running, the user will be prompted if he wants to exit it.
            if (game.IsRunning)
            {
                Console.WriteLine("\n New game\n");
                Console.WriteLine("  The current game will be canceled to start a new one.");
                Console.Write("  Do you want to continue? [Y/N]:");

                if (Console.ReadLine().ToUpper().Equals("Y"))
                {
                    game.Stop();
                    StartNewGame();
                }
            }
            else
            {
                game.Reset();
                game.StartNewGame();

                PlayGame();
            }
        }

        /// <summary>
        /// Starts a new game, which will be played by the computer.
        /// </summary>
        private static void StartNewAIGame()
        {
            Console.Clear();

            // If a game is currently running, the user will be prompted if he wants to exit it.
            if (game.IsRunning)
            {
                Console.WriteLine("\n New AI - game\n");
                Console.WriteLine("  The current game will be canceled to start a new one.");
                Console.Write("  Do you want to continue? [Y/N]:");

                if (Console.ReadLine().ToUpper().Equals("Y"))
                {
                    game.Stop();
                    StartNewAIGame();
                }
            }
            else
            {
                // Difficulty level, which will be used for the computer.
                int level = -1;

                // If  true, the end result of the game will be shown.
                bool skipping = false;

                ConsoleKey key;

                Console.WriteLine("\n New AI - game\n");

                // The user can select the difficulty level for the AI.
                do
                {
                    Console.Write("  Difficulty level for the AI [1 - 6]: ");
                }
                while (!(int.TryParse(Console.ReadLine(), out level) && level >= (int)Hangman.HangmanDifficultyLevel.L1 && level <= (int)Hangman.HangmanDifficultyLevel.L6));

                // Start a new game.
                game.Reset();
                game.StartNewGame((Hangman.HangmanDifficultyLevel)level);
                computer.StartNewGame();

                while (game.IsRunning)
                {
                    Draw();

                    Console.Write("\n\n -> Press [Enter] to let the AI guess the next letter.\n");
                    Console.Write(" -> Press [Space] to see the final state of the game.");

                    if (!skipping)
                    {
                        key = Console.ReadKey().Key;

                        if (key == ConsoleKey.Enter)
                        {
                            game.GuessLetter(computer.GuessLetter());
                        }
                        else if (key == ConsoleKey.Spacebar)
                        {
                            skipping = true;
                        }
                        else
                        {
                            // If the user presses any other key, the program checks if it controls the menu.
                            HandleSpecialKeyInput(key);
                        }
                    }
                    else
                    {
                        game.GuessLetter(computer.GuessLetter());
                    }
                }
            }
        }

        /// <summary>
        /// The user plays a game by guessing letters.
        /// </summary>
        private static void PlayGame()
        {
            string word = string.Empty;

            while (game.IsRunning)
            {
                Draw();

                Console.Write("\n\n -> Your letter: ");

                if (inputHandler.ReadChars(ref word, 1))
                {
                    if (WordList.IsValidWord(word))
                    {
                        game.GuessLetter(word[0]);
                    }

                    word = string.Empty;
                }
            }
        }

        /// <summary>
        /// Exits the game or the program, depending on the current state.
        /// </summary>
        private static void ExitGame()
        {
            Console.Clear();
            Console.WriteLine("\n\n");

            if (!game.IsRunning)
            {
                Console.Write("  Are you sure you want to exit the program? [Y/N]");

                if (Console.ReadLine().ToUpper().Equals("Y"))
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("  The current game will be canceled.");
                Console.Write("  Do you want to continue? [Y/N]:");

                if (Console.ReadLine().ToUpper().Equals("Y"))
                {
                    game.Stop();
                }
            }
        }

        /// <summary>
        /// Adds new words from the user input.
        /// </summary>
        /// <returns>A list of all words, which have been entered by the user.</returns>
        private static WordList AddNewWords()
        {
            Console.Clear();

            Console.WriteLine("\n Add new words (Enter only whitespaces to close this window)\n");

            WordList wl = new WordList();

            string input = string.Empty;

            do
            {
                Console.Write("  New word: ");

                input = Console.ReadLine();

                // To finish the input, the user must enter nothing or whitespaces.
                if (!string.IsNullOrWhiteSpace(input))
                {
                    try
                    {
                        wl.AddWord(input);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("  The word must contain only letters!\n");
                    }
                }
            }
            while (!string.IsNullOrWhiteSpace(input));

            return wl;
        }

        /// <summary>
        /// Gets a new word list by interpreting all given command line arguments.
        /// </summary>
        /// <param name="args">The given command line arguments.</param>
        /// <returns>A new word list.</returns>
        private static WordList GetWordsByArguments(string[] args)
        {
            WordList newWordList = new WordList();
            List<string> foundTags = new List<string>();
            string foundTag = string.Empty;

            for (int i = 0; i < args.Length; i++)   
            {
                try
                {
                    if (argumentTags.Contains(args[i]))
                    {
                        foundTag = args[i];

                        // Each tag will be interpreted only once.
                        if (foundTags.Contains(foundTag))
                        {
                            foundTag = string.Empty;
                        }
                        else
                        {
                            foundTags.Add(foundTag);
                        }
                    }
                    else if (foundTag.Equals("-f"))
                    {
                        newWordList.AddWordsByFile(args[i]);
                    }
                    else if (foundTag.Equals("-w"))
                    {
                        newWordList.AddWord(args[i]);
                    }
                }
                catch (FileNotFoundException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return newWordList;
        }
    }
}
