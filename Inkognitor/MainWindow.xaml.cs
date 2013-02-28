using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Inkognitor
{
    public partial class MainWindow : Window
    {
        private const double ScrollingSpeed = 20.0f;

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

        private void inputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Up))
            {
                double offset = scrollViewer.VerticalOffset - ScrollingSpeed;
                scrollViewer.ScrollToVerticalOffset(offset);
            }
            else if (e.Key.Equals(Key.Down))
            {
                double offset = scrollViewer.VerticalOffset + ScrollingSpeed;
                scrollViewer.ScrollToVerticalOffset(offset);   
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inputBox.Focus();
            inputBox.LostFocus += (s, fe) => Dispatcher.BeginInvoke((Action)(() => inputBox.Focus()));
        }

        public class TextEnteredEventArgs : EventArgs
        {
            public TextEnteredEventArgs(string text)
            {
                Text = text;
            }

            public string Text { get; set; }
        }
    }
}
