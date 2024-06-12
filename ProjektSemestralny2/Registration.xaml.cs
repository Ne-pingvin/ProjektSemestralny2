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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        DataBase database = new DataBase();
        public Registration()
        {
            InitializeComponent();
        }

        private void backButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Hide();
        }

        //button registers the user and creates it in the database
        private void SingUpBtn2_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameSingUp.Text == "" || PasswordSingUp.Password == "")
            {
                MessageBox.Show("Fill the data!");
                return;
            }
            else
            {     
                SingUpUser(UsernameSingUp.Text, PasswordSingUp.Password);
            }
        }
        private void SingUpUser(string login, string password)
        {
            if (IsUsernameTaken(login))
            {
                MessageBox.Show("Username already used!");
                return;
            }
        
            database.OpenConnection();
     
            SqlCommand command = new SqlCommand("INSERT INTO Login (Username, Password) VALUES (@Login, @Password)", database.GetConnection());
    
            command.Parameters.Add("@Login", System.Data.SqlDbType.VarChar).Value = login;
            command.Parameters.Add("@Password", System.Data.SqlDbType.VarChar).Value = password;

            command.ExecuteNonQuery();

            database.CloseConnection();

            MessageBox.Show("User has been registered!");
 
            UsernameSingUp.Text = "";
            PasswordSingUp.Password = "";
        }
        private bool IsUsernameTaken(string login)
        {     
            database.OpenConnection();

            SqlCommand command = new SqlCommand("SELECT * FROM Login WHERE Username = @Login", database.GetConnection());

            command.Parameters.Add("@Login", System.Data.SqlDbType.VarChar).Value = login;

            //creating an object that will store the result of the request
            SqlDataReader reader = command.ExecuteReader();

            //checking if such a user exists
            bool userExists = reader.HasRows;
            reader.Close();
            database.CloseConnection();

            return userExists;
        }
    }
}
