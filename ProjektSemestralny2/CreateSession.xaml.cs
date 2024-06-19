using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ProjektSemestralny2
{
    /// <summary>
    /// Interaction logic for CreateSession.xaml
    /// </summary>
    public partial class CreateSession : Window
    {
        public CreateSession()
        {
            InitializeComponent();
        }
        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Hide();
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
