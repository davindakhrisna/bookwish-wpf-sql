using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace bookwish_app
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        private string connectionString;
        private MySqlConnection connection;

        public SignUp()
        {
            InitializeComponent();
            InitializeConnectionString();
            InitializeConnection();
        }

        private void InitializeConnectionString()
        {
            connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
        }

        private void InitializeConnection()
        {
            connection = new MySqlConnection(connectionString);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MainWindow LoginForm = new MainWindow();
            LoginForm.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            // Check if username already exists
            if (IsUsernameExists(username))
            {
                MessageBox.Show("Username already exists. Please choose another username.");
                return;
            }

            // Insert new user into the database
            if (InsertUser(username, email, password))
            {
                MessageBox.Show("Sign-up successful. You can now login.");
                MainWindow LoginSign = new MainWindow();
                LoginSign.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Sign-up failed. Please try again later.");
            }
        }

        private bool IsUsernameExists(string username)
        {
            // Check if username exists in the database
            string query = "SELECT COUNT(*) FROM users WHERE username = @username";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return count > 0;
            }
        }

        private bool InsertUser(string username, string email, string password)
        {
            // Insert new user into the database
            string query = "INSERT INTO users (username, email, password) VALUES (@username, @email, @password)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;
            }
        }

    }
}
