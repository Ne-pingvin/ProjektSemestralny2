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
    /// Interaction logic for ListOfCandidates.xaml
    /// </summary>
    public partial class ListOfCandidates : Window
    {
        public ListOfCandidates()
        {
            InitializeComponent();
        }
      

        private void AddCandidate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Back_Click_1(object sender, RoutedEventArgs e)
        {

            Menu menu = new Menu();
            menu.Show();
            this.Hide();
        }
    }
}
