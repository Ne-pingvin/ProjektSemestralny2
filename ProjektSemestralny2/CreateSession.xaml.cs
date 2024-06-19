using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjektSemestralny2
{
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
                CandidatesListBox.Items.Clear(); // Clearing ListBox before loading new data
                dataBase.OpenConnection();

                SqlCommand command = new SqlCommand("SELECT nameOfCandidate FROM candidates2table", dataBase.GetConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string candidateName = reader["nameOfCandidate"].ToString();
                    CandidatesListBox.Items.Add(new ListBoxItem { Content = candidateName });
                }

                reader.Close();
                dataBase.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void СreateSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            // Getting the name and description of the session
            string sessionName = TitleTextBox.Text;
            string sessionDescription = DescriptionTextBox.Text;

            // Checking that fields are not empty
            if (string.IsNullOrWhiteSpace(sessionName) || string.IsNullOrWhiteSpace(sessionDescription))
            {
                MessageBox.Show("Please provide both a session title and description.");
                return;
            }

            // Receiving selected candidates
            var selectedCandidates = CandidatesListBox.SelectedItems.Cast<ListBoxItem>().Select(item => item.Content.ToString()).ToList();

            if (!selectedCandidates.Any())
            {
                MessageBox.Show("Please select at least one candidate.");
                return;
            }

            try
            {
                dataBase.OpenConnection();

                // Creating a new session
                SqlCommand insertSessionCommand = new SqlCommand("INSERT INTO SessionTrue (session_name, session_description) OUTPUT INSERTED.id_session VALUES (@Name, @Description)", dataBase.GetConnection());
                insertSessionCommand.Parameters.AddWithValue("@Name", sessionName);
                insertSessionCommand.Parameters.AddWithValue("@Description", sessionDescription);
                int sessionId = (int)insertSessionCommand.ExecuteScalar();

                // Adding selected candidates to the session
                foreach (var candidateName in selectedCandidates)
                {
                    SqlCommand getCandidateIdCommand = new SqlCommand("SELECT idCandidate FROM candidates2table WHERE nameOfCandidate = @Name", dataBase.GetConnection());
                    getCandidateIdCommand.Parameters.AddWithValue("@Name", candidateName);
                    int candidateId = (int)getCandidateIdCommand.ExecuteScalar();

                    SqlCommand insertSessionCandidateCommand = new SqlCommand("INSERT INTO SessionCandidatesTruw (id_session, id_candidate) VALUES (@SessionId, @CandidateId)", dataBase.GetConnection());
                    insertSessionCandidateCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    insertSessionCandidateCommand.Parameters.AddWithValue("@CandidateId", candidateId);
                    insertSessionCandidateCommand.ExecuteNonQuery();
                }

                dataBase.CloseConnection();

                MessageBox.Show("Session created successfully!");

                // Clearing fields after successful session creation
                TitleTextBox.Clear();
                DescriptionTextBox.Clear();
                CandidatesListBox.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    } 
}
