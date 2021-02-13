using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ProjectsNow.DataInput
{
    class Input
    {
        public static List<Key> Systemkeys = new List<Key>() 
        { 
            Key.Enter, Key.Left, Key.Right, Key.Up, Key.Down, Key.Escape, Key.Back, Key.Delete, Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.Tab, Key.CapsLock 
        };

        public static List<Key> Numberskeys = new List<Key>() 
        { 
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D7, Key.D9,
            Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
        };

        public static void IntOnly(KeyEventArgs e, int digit)
        {
            TextBox textBox = e.OriginalSource as TextBox;

            if (!Numberskeys.Contains(e.Key) && !Systemkeys.Contains(e.Key))
                e.Handled = true;

            if (textBox.Text.Length >= digit && textBox.SelectedText.Length == 0 && !Systemkeys.Contains(e.Key))
                e.Handled = true;
        }


        public static void DoubleOnly(KeyEventArgs e)
        {
            TextBox textBox = e.OriginalSource as TextBox;

            if (textBox.Text.Contains(".") && e.Key == Key.Decimal)
                e.Handled = true;
            if (!textBox.Text.Contains(".") && e.Key == Key.Decimal)
                e.Handled = false;
            else
            {
                if (!Numberskeys.Contains(e.Key) && !Systemkeys.Contains(e.Key))
                    e.Handled = true;
            }
        }

        public static void ArrowsOnly(KeyEventArgs e)
        {
            if (!Systemkeys.Contains(e.Key))
                e.Handled = true;
            else if (e.Key == Key.Delete || e.Key == Key.Back)
                e.Handled = true;
        }


        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

    }
}
