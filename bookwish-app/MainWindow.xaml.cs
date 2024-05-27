using MySql.Data.MySqlClient;
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

namespace bookwish_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString;
        private MySqlConnection connection;
        public MainWindow()
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
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Validate email and password (e.g., check for empty fields)

            // Authenticate user against database
            if (AuthenticateUser(username, password))
            {
                if ((username == "admin") && (password == "admin"))
                {

                    MessageBox.Show("Welcome back boss!");

                    Admin adminMenu = new Admin(username);
                    adminMenu.Show();
                    this.Close();
                }
                else
                {
                    MainMenu mainMenu = new MainMenu(username);
                    mainMenu.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
            this.Close();
        }

        private bool AuthenticateUser(string username, string password)
        {
            // Implement authentication logic here
            // Query the database to check if the email and password match a user record
            // Return true if authentication succeeds, false otherwise

            // Example query (use parameterized queries to prevent SQL injection)
            string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return count > 0;
            }
        }
    }
}