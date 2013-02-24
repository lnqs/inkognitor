using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Inkognitor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public delegate void TextEnteredHandler(object sender, TextEnteredEventArgs e);
        public event TextEnteredHandler TextEntered;

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                TextBox textbox = (TextBox)sender;
                string text = textbox.Text;
                textbox.Clear();

                if (TextEntered != null)
                {
                    TextEntered.Invoke(this, new TextEnteredEventArgs(text));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inputBox.Focus();
        }

        public class TextEnteredEventArgs : EventArgs
        {
            public string Text { get; set; }

            public TextEnteredEventArgs(string text)
            {
                Text = text;
            }
        }
    }
}
