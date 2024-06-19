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
    /// Interaction logic for ListOfCandidates.xaml
    /// </summary>
    public partial class ListOfCandidates : Window
    {
        public ListOfCandidates()
        {
            InitializeComponent();
        }

        DataBase dataBase = new DataBase();

        private void AddCandidate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CandidateNameTextBox.Text))
            {
                string candidateName = CandidateNameTextBox.Text;
                dataBase.OpenConnection();

                SqlCommand command = new SqlCommand("INSERT INTO candidates2table (nameOfCandidate) VALUES (@Name)", dataBase.GetConnection());

                command.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = candidateName;

                command.ExecuteNonQuery();

                MessageBox.Show("Sukces!");
                dataBase.CloseConnection();
            }
            else
            {
                MessageBox.Show("You are not provide candidate name!");
            }
        }

        private void Back_Click_1(object sender, RoutedEventArgs e)
        {

            Menu menu = new Menu();
            menu.Show();
            this.Hide();
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
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

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CandidatesListBox.SelectedItem != null)
            {
                string selectedCandidate = CandidatesListBox.SelectedItem.ToString();

                try
                {
                    dataBase.OpenConnection();

                    SqlCommand command = new SqlCommand("DELETE FROM candidates2table WHERE nameOfCandidate = @Name", dataBase.GetConnection());
                    command.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = selectedCandidate;

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Candidate deleted successfully!");
                        LoadCandidates();
                    }
                    else
                    {
                        MessageBox.Show("Candidate not found in the database.");
                    }

                    dataBase.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a candidate to delete.");
            }
        }
    }
}
