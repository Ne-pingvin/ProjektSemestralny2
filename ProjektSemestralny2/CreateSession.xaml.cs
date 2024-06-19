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
            // Логика для обработки изменения текста (если необходимо)
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
            // Получение названия и описания сессии
            string sessionName = TitleTextBox.Text;
            string sessionDescription = DescriptionTextBox.Text;

            // Проверка, что поля не пустые
            if (string.IsNullOrWhiteSpace(sessionName) || string.IsNullOrWhiteSpace(sessionDescription))
            {
                MessageBox.Show("Please provide both a session title and description.");
                return;
            }

            // Получение выбранных кандидатов
            var selectedCandidates = CandidatesListBox.SelectedItems.Cast<ListBoxItem>().Select(item => item.Content.ToString()).ToList();

            if (!selectedCandidates.Any())
            {
                MessageBox.Show("Please select at least one candidate.");
                return;
            }

            try
            {
                dataBase.OpenConnection();

                // Создание новой сессии
                SqlCommand insertSessionCommand = new SqlCommand("INSERT INTO SessionTrue (session_name, session_description) OUTPUT INSERTED.id_session VALUES (@Name, @Description)", dataBase.GetConnection());
                insertSessionCommand.Parameters.AddWithValue("@Name", sessionName);
                insertSessionCommand.Parameters.AddWithValue("@Description", sessionDescription);
                int sessionId = (int)insertSessionCommand.ExecuteScalar();

                // Добавление выбранных кандидатов к сессии
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

                // Очистка полей после успешного создания сессии
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

    public class Session
    {
        public int IdSession { get; set; }
        public string SessionName { get; set; }
        public string SessionDescription { get; set; }
        public ICollection<SessionCandidate> SessionCandidates { get; set; }
    }

    public class Candidate
    {
        public int IdCandidate { get; set; }
        public string NameOfCandidate { get; set; }
        public ICollection<SessionCandidate> SessionCandidates { get; set; }
    }

    public class SessionCandidate
    {
        public int IdSession { get; set; }
        public Session Session { get; set; }

        public int IdCandidate { get; set; }
        public Candidate Candidate { get; set; }
    }
}
