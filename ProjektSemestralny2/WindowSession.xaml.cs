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

        private bool VerifyForeignKey(int userId, int sessionId, int candidateId)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True"))
            {
                connection.Open();

                // Проверка существования пользователя
                SqlCommand commandUser = new SqlCommand("SELECT COUNT(*) FROM Users WHERE id_user = @UserId", connection);
                commandUser.Parameters.AddWithValue("@UserId", userId);
                int countUser = (int)commandUser.ExecuteScalar();

                // Проверка существования сессии
                SqlCommand commandSession = new SqlCommand("SELECT COUNT(*) FROM SessionTrue WHERE id_session = @SessionId", connection);
                commandSession.Parameters.AddWithValue("@SessionId", sessionId);
                int countSession = (int)commandSession.ExecuteScalar();

                // Проверка существования кандидата
                SqlCommand commandCandidate = new SqlCommand("SELECT COUNT(*) FROM candidates2table WHERE idCandidate = @CandidateId", connection);
                commandCandidate.Parameters.AddWithValue("@CandidateId", candidateId);
                int countCandidate = (int)commandCandidate.ExecuteScalar();

                return (countUser > 0 && countSession > 0 && countCandidate > 0);
            }
        }

        private void VoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CandidatesListBox.SelectedItem is ListBoxItem selectedItem)
            {
                int candidateId = (int)selectedItem.Tag;
                if (!VerifyForeignKey(_userId, _sessionId, candidateId))
                {
                    MessageBox.Show("Invalid data for voting. Please check user, session, and candidate IDs.");
                    return;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True"))
                    {
                        connection.Open();
                        SqlCommand voteCommand = new SqlCommand("INSERT INTO Votes (id_user, id_session, id_candidate) VALUES (@UserId, @SessionId, @CandidateId)", connection);
                        voteCommand.Parameters.AddWithValue("@UserId", _userId);
                        voteCommand.Parameters.AddWithValue("@SessionId", _sessionId);
                        voteCommand.Parameters.AddWithValue("@CandidateId", candidateId);
                        voteCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Vote cast successfully!");
                    this.Close();
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
