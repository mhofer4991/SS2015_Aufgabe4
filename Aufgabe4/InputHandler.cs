//-----------------------------------------------------------------------
// <copyright file="InputHandler.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class reads user input and fires events at special keys.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class reads user input and fires events at special keys.
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// List of subscribed keys, which fire an event.
        /// </summary>
        private List<ConsoleKey> subscribedKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputHandler"/> class.
        /// </summary>
        public InputHandler()
        {
            this.subscribedKeys = new List<ConsoleKey>();
        }

        /// <summary>
        /// Delegate for event SubscribedKeyCalled.
        /// </summary>
        /// <param name="key">Subscribed key.</param>
        public delegate void SubscribedKeyCalled(ConsoleKey key);

        /// <summary>
        /// Is called when the user presses a subscribed key.
        /// </summary>
        public event SubscribedKeyCalled OnSubscribedKeyCalled;

        /// <summary>
        /// Subscribes a key by adding it to the list.
        /// </summary>
        /// <param name="key">Subscribed key.</param>
        public void SubscribeForKey(ConsoleKey key)
        {
            this.subscribedKeys.Add(key);
        }

        /// <summary>
        /// Waits for the user to press a subscribed key.
        /// </summary>
        /// <returns>The subscribed key.</returns>
        public ConsoleKey WaitForSubscribedKey()
        {
            ConsoleKey key;

            while (!this.subscribedKeys.Contains(key = Console.ReadKey(true).Key))
            {
            }

            if (this.OnSubscribedKeyCalled != null)
            {
                this.OnSubscribedKeyCalled(key);
            }

            return key;
        }

        /// <summary>
        /// Represents a version of the Console.ReadLine() method, which also can read special keys.
        /// </summary>
        /// <param name="begin">String, which will be displayed and modified.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <returns>A boolean, which indicates whether the input was terminated by ENTER or not.</returns>
        public bool ReadChars(ref string begin, int maxLength)
        {
            ConsoleKeyInfo cki;

            Console.Write(begin);

            while (true)
            {
                cki = Console.ReadKey(true);

                if (this.subscribedKeys.Contains(cki.Key))
                {
                    if (this.OnSubscribedKeyCalled != null)
                    {
                        this.OnSubscribedKeyCalled(cki.Key);

                        return false;
                    }
                }
                else if (cki.Key == ConsoleKey.Backspace)
                {
                    if (begin.Length > 0)
                    {
                        begin = begin.Remove(begin.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    return true;
                }
                else if ((cki.KeyChar >= 32 && cki.KeyChar <= 126) || char.IsLetterOrDigit(cki.KeyChar))
                {
                    if (maxLength < 0 || begin.Length < maxLength)
                    {
                        begin += cki.KeyChar;
                        Console.Write(cki.KeyChar);
                    }
                }
            }
        }
    }
}
