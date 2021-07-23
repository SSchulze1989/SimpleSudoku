using SimpleSudoku.Game;
using SimpleSudoku.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleSudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private static string SaveGamesPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleSudoku");
        private static string SaveGamesPath = "";

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
            if (string.IsNullOrEmpty(SaveGamesPath) == false)
            {
                System.IO.Directory.CreateDirectory(SaveGamesPath);
            }
        }

        public void StartGame()
        {
            //var creator = new SudokuCreator();
            //for (int i = 0; i < 10; i++)
            //{
            //    var field = creator.Create();
            //}
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var fieldVM = DataContext as SudokuFieldViewModel;
            var saveGame = System.IO.Path.Combine(SaveGamesPath, "game.txt");

            if (fieldVM != null)
            {
                using (var streamWriter = new StreamWriter(saveGame, false))
                {
                    var data = fieldVM.SaveAction();
                    streamWriter.Write(data);
                }
            }
        }

        private void LoadButton_Click(object sende, RoutedEventArgs e)
        {
            var fieldVM = DataContext as SudokuFieldViewModel;
            var saveGame = System.IO.Path.Combine(SaveGamesPath, "game.txt");

            if (fieldVM != null)
            {
                using(var streamReader = new StreamReader(saveGame))
                {
                    var data = streamReader.ReadToEnd();
                    fieldVM.LoadAction(data);
                }
            }
        }
    }
}
