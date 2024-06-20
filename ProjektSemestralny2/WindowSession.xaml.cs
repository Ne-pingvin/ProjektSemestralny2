using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ProjektSemestralny2
{
    /// <summary>
    /// Interaction logic for WindowSession.xaml
    /// </summary>
    public partial class WindowSession : Window
    {
        private int _sessionId;
        private int _userId;

        public WindowSession(int sessionId, int userId)
        {
            InitializeComponent();
            _sessionId = sessionId;
            _userId = userId;
            LoadCandidates();
        }

        private void LoadCandidates()
        {
            try
            {
                CandidatesListBox.Items.Clear();
                using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True"))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT c.idCandidate, c.nameOfCandidate FROM candidates2table c INNER JOIN SessionCandidatesTruw sc ON c.idCandidate = sc.id_candidate WHERE sc.id_session = @SessionId", connection);
                    command.Parameters.AddWithValue("@SessionId", _sessionId);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int candidateId = reader.GetInt32(0);
                        string candidateName = reader.GetString(1);

                        ListBoxItem item = new ListBoxItem
                        {
                            Content = candidateName,
                            Tag = candidateId
                        };
                        CandidatesListBox.Items.Add(item);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void VoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CandidatesListBox.SelectedItem is ListBoxItem selectedItem)
            {
                int candidateId = (int)selectedItem.Tag;

                // Create the connection string and command string separately
                string connectionString = "Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True";
                string commandText = "UPDATE VoteCount SET vote_count = vote_count + 1 WHERE id_session = @SessionId AND id_candidate = @CandidateId";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Use the command text
                        SqlCommand updateVoteCountCommand = new SqlCommand(commandText, connection);
                        updateVoteCountCommand.Parameters.AddWithValue("@SessionId", _sessionId);
                        updateVoteCountCommand.Parameters.AddWithValue("@CandidateId", candidateId);
                        updateVoteCountCommand.ExecuteNonQuery();

                        // New string creation for confirmation message
                        string confirmationMessage = $"Vote for candidate ID {candidateId} cast successfully!";
                        MessageBox.Show(confirmationMessage);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a candidate to vote for.");
            }
        }
    }
}
