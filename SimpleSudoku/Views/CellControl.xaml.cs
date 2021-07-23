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

namespace SimpleSudoku.Views
{
    /// <summary>
    /// Interaktionslogik für CellControl.xaml
    /// </summary>
    public partial class CellControl : UserControl
    {
        static Key[] NumKeys = new Key[]
        {
                        Key.D0,
            Key.D1,
            Key.D2,
            Key.D3,
            Key.D4,
            Key.D5,
            Key.D6,
            Key.D7,
            Key.D8,
            Key.D9,
            Key.NumPad0,
            Key.NumPad1,
            Key.NumPad2,
            Key.NumPad3,
            Key.NumPad4,
            Key.NumPad5,
            Key.NumPad6,
            Key.NumPad7,
            Key.NumPad8,
            Key.NumPad9,
        };

        static Key[] AllowKeys = (new Key[]
        {
            Key.Delete,
            Key.Back,
            Key.Left,
            Key.Right,
            Key.Up,
            Key.Down,
            Key.Tab
        }).Concat(NumKeys).ToArray();

        public CellControl()
        {
            InitializeComponent();
        }

        private void ValueTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (AllowKeys.Contains(e.Key))
                {
                    if (NumKeys.Contains(e.Key) && textBox.IsReadOnly == false)
                    {
                        textBox.Clear();
                    }
                    else
                    {
                        switch (e.Key)
                        {
                            case Key.Delete:
                                textBox.Clear();
                                break;
                            case Key.Left:
                                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                                break;
                            case Key.Right:
                                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                                break;
                            case Key.Up:
                                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                                break;
                            case Key.Down:
                                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                                break;
                        }
                    }
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
    }
}
