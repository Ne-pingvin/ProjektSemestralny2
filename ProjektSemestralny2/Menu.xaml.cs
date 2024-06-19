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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        DataBase dataBase = new DataBase();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateSession session = new CreateSession();
            session.Show();
            this.Hide();
        }

        private void ListOfCandidatesButton_Click(object sender, RoutedEventArgs e)
        {
            ListOfCandidates list = new ListOfCandidates();
            list.Show();
            this.Hide();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadSessions();
        }

        private void LoadSessions()
        {
            try
            {
                ListAvSessions.Items.Clear(); // Очистка ListBox перед загрузкой новых данных
                dataBase.OpenConnection();

                SqlCommand command = new SqlCommand("SELECT session_name, session_description FROM SessionTrue", dataBase.GetConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string sessionName = reader["session_name"].ToString();
                    string sessionDescription = reader["session_description"].ToString();
                    ListAvSessions.Items.Add($"{sessionName}: {sessionDescription}");
                }

                reader.Close();
                dataBase.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void DeleteSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ListAvSessions.SelectedItem == null)
            {
                MessageBox.Show("Please select a session to delete.");
                return;
            }

            if (ListAvSessions.SelectedItem is ListBoxItem selectedItem)
            {
                int sessionId = (int)selectedItem.Tag;

                try
                {
                    dataBase.OpenConnection();

                    // Удаление сессии и всех связей с кандидатами
                    SqlCommand deleteSessionCandidatesCommand = new SqlCommand("DELETE FROM SessionCandidatesTruw WHERE id_session = @SessionId", dataBase.GetConnection());
                    deleteSessionCandidatesCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    deleteSessionCandidatesCommand.ExecuteNonQuery();

                    SqlCommand deleteSessionCommand = new SqlCommand("DELETE FROM SessionTrue WHERE id_session = @SessionId", dataBase.GetConnection());
                    deleteSessionCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    deleteSessionCommand.ExecuteNonQuery();

                    dataBase.CloseConnection();

                    MessageBox.Show("Session deleted successfully!");
                    LoadSessions(); // Обновление списка сессий
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Invalid selection.");
            }
        }
    }
}
