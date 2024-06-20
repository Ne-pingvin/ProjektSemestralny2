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

        public WindowSession(int sessionId, int userId)
        {
            InitializeComponent();
            _sessionId = sessionId;
            LoadCandidates();
        }
        MainWindow mainWin = new MainWindow();

        public int GetUserId(string username)
        {
            int userId = -1; // -1 означает, что пользователь не найден или ошибка

            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True"))
                {
                    connection.Open();

                    // SQL-запрос для получения ID пользователя на основе имени пользователя
                    SqlCommand command = new SqlCommand("SELECT UserID FROM Login WHERE Username = @Username", connection);
                    command.Parameters.AddWithValue("@Username", username);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        userId = reader.GetInt32(0); // Получение UserID из результата запроса
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            return userId;
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
                int userId = GetUserId(mainWin.actualUsername);

                if (userId == -1)
                {
                    MessageBox.Show("Invalid user.");
                    return;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-472C2EDF;Initial Catalog=VotesOnline;Integrated Security=True"))
                    {
                        connection.Open();

                        // Проверка на существующий голос в данной сессии
                        SqlCommand checkVoteCommand = new SqlCommand("SELECT COUNT(*) FROM VotesTrueNr2 WHERE id_user = @UserId AND id_session = @SessionId", connection);
                        checkVoteCommand.Parameters.AddWithValue("@UserId", userId);
                        checkVoteCommand.Parameters.AddWithValue("@SessionId", _sessionId);
                        int voteExists = (int)checkVoteCommand.ExecuteScalar();

                        if (voteExists > 0)
                        {
                            MessageBox.Show("You have already voted in this session.");
                            return;
                        }

                        // Добавление нового голоса
                        SqlCommand voteCommand = new SqlCommand("INSERT INTO VotesTrueNr2 (id_user, id_session, id_candidate) VALUES (@UserId, @SessionId, @CandidateId)", connection);
                        voteCommand.Parameters.AddWithValue("@UserId", userId);
                        voteCommand.Parameters.AddWithValue("@SessionId", _sessionId);
                        voteCommand.Parameters.AddWithValue("@CandidateId", candidateId);
                        voteCommand.ExecuteNonQuery();

                        // Обновление счетчика голосов
                        SqlCommand updateVoteCountCommand = new SqlCommand("UPDATE VoteCount SET vote_count = vote_count + 1 WHERE id_session = @SessionId AND id_candidate = @CandidateId", connection);
                        updateVoteCountCommand.Parameters.AddWithValue("@SessionId", _sessionId);
                        updateVoteCountCommand.Parameters.AddWithValue("@CandidateId", candidateId);
                        updateVoteCountCommand.ExecuteNonQuery();

                        MessageBox.Show("Vote cast successfully!");
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
