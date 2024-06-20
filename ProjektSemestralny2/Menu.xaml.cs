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
                ListAvSessions.Items.Clear(); // Clearing ListBox before loading new data
                dataBase.OpenConnection();

                SqlCommand command = new SqlCommand("SELECT id_session, session_name, session_description FROM SessionTrue", dataBase.GetConnection());
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int sessionId = reader.GetInt32(0); // Предполагаем, что первый столбец содержит id
                    string sessionName = reader["session_name"].ToString();
                    string sessionDescription = reader["session_description"].ToString();

                    ListBoxItem item = new ListBoxItem();
                    item.Content = $"{sessionName}: {sessionDescription}";
                    item.Tag = sessionId;
                    ListAvSessions.Items.Add(item);
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

                    // Deleting a session and all connections with candidates
                    SqlCommand deleteSessionCandidatesCommand = new SqlCommand("DELETE FROM SessionCandidatesTruw WHERE id_session = @SessionId", dataBase.GetConnection());
                    deleteSessionCandidatesCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    deleteSessionCandidatesCommand.ExecuteNonQuery();

                    SqlCommand deleteSessionCommand = new SqlCommand("DELETE FROM SessionTrue WHERE id_session = @SessionId", dataBase.GetConnection());
                    deleteSessionCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    deleteSessionCommand.ExecuteNonQuery();

                    dataBase.CloseConnection();

                    MessageBox.Show("Session deleted successfully!");
                    LoadSessions(); // Updating the list of sessions
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

        private int _userId;
        private string _username;

        private void GetIdFromBD()
        {
            try
            {
                dataBase.OpenConnection();

                SqlCommand command = new SqlCommand("SELECT id_user FROM Users WHERE username = @Username", dataBase.GetConnection());
                command.Parameters.AddWithValue("@Username", _username);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    _userId = reader.GetInt32(0);
                }

                reader.Close();
                dataBase.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void GoToSession_Click(object sender, RoutedEventArgs e)
        {
            if (ListAvSessions.SelectedItem != null)
            {
                if (ListAvSessions.SelectedItem is ListBoxItem selectedItem)
                {
                    int sessionId = (int)selectedItem.Tag;
                    WindowSession sessionWindow = new WindowSession(sessionId, _userId);
                    sessionWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Invalid selection.");
                }
            }
            else
            {
                MessageBox.Show("Please select a session to go to.");
            }
        }
    }
}
