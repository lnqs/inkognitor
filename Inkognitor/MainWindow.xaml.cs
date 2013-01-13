using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inkognitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void TextEnteredHandler(object sender, TextEnteredEventArgs e);
        public event TextEnteredHandler TextEntered;

        public MainWindow()
        {
            InitializeComponent();
        }

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
