using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        DataBase dataBase = new DataBase();
        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Hide();
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCandidates();
        }
        private void LoadCandidates()
        {
            try
            {
                CandidatesListBox.Items.Clear(); // Очистка ListBox перед загрузкой новых данных
                dataBase.OpenConnection();

                SqlCommand command = new SqlCommand("SELECT nameOfCandidate FROM candidates2table", dataBase.GetConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string candidateName = reader["nameOfCandidate"].ToString();
                    CandidatesListBox.Items.Add(candidateName);
                }

                reader.Close();
                dataBase.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

    }
}
