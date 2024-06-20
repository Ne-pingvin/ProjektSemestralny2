using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace ProjektSemestralny2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase database = new DataBase();
        public MainWindow()
        {
            InitializeComponent();
        }

        private string _actualUsername;
        public string actualUsername { get; set; }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckUserData(UsernameLogIn.Text, PasswordLogIn.Password);
           
        }

        private void SingUpBtn_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Hide();
        }
        private void LogInUser(string login, string password)
        {    
            database.OpenConnection();
           
            SqlCommand command = new SqlCommand("SELECT * FROM Login WHERE Username = @Login AND password = @Password", database.GetConnection());

            command.Parameters.Add("@Login", System.Data.SqlDbType.VarChar).Value = login;
            command.Parameters.Add("@Password", System.Data.SqlDbType.VarChar).Value = password;
            //create an object that will store the result of the request
            SqlDataReader reader = command.ExecuteReader();

            //if the request returned a result, then a user with this login and password exists
            if (reader.HasRows)
            {
                actualUsername = login;
                MessageBox.Show("User has been logged in!");
                Menu menu = new Menu();
                menu.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username or password not found!");

            }

            database.CloseConnection();
        }
        private void CheckUserData(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Fill the data!");
                return;
            }
            else
            {
                //check that a user with the same login and password exists in the database
                LogInUser(login, password);
            }
        }
    }
}