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
using MySql.Data.MySqlClient;

namespace bookwish_app
{
    /// <summary>
    /// Interaction logic for CheckoutWindow.xaml
    /// </summary>
    public partial class CheckoutWindow : Window
    {
        private string username;
        private string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
        private decimal price;

        private decimal payment;
        private string method;

        public CheckoutWindow(int totalBooks, decimal totalPrice, string username)
        {
            InitializeComponent();
            TotalBooksTextBlock.Text = $"Total Books: {totalBooks}";
            TotalPriceTextBlock.Text = $"Total Price: ${totalPrice}";
            this.username = username;
            this.price = totalPrice;

            UsernameTextBlock.Text = username;
        }

        private void ConfirmPurchase_Click(object sender, RoutedEventArgs e)
        {
            payment = int.Parse(PaymentTextBox.Text);
            method = CardTextBox.Text;

            if (payment < price)
            {
                MessageBox.Show("Please Buy the Full Price!", "Price Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UpdateSoldCount();
                DeleteCartItems();

                MessageBox.Show("Thank You for Buying!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCartItems()
        {
            string deleteQuery = "DELETE FROM cart WHERE username = @username";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void UpdateSoldCount()
        {
            string updateQuery = "UPDATE books SET sold = sold + 1 WHERE title IN (SELECT title FROM cart WHERE username = @username)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            ConfirmPurchase_Click(sender, e);
        }
    }
}
