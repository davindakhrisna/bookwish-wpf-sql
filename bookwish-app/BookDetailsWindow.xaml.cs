using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for BookDetailsWindow.xaml
    /// </summary>
    public partial class BookDetailsWindow : Window
    {
        private string _username;
        private Book _book;

        private int totalBooks = 1;
        private decimal totalPrice;
        private decimal borrowPrice;

        public BookDetailsWindow(Book book, string username)
        {
            InitializeComponent();
            _username = username;
            _book = book;

            totalPrice = book.Price;
            borrowPrice = book.Price / 2;


            UsernameTextBlock.Text = username;
            LoadBookDetails(book);
        }

        // FUNCTIONALITY LOGIC
        private string TruncateTitle(string title, int maxLength)
        {
            if (title.Length <= maxLength)
            {
                return title;
            }
            return title.Substring(0, maxLength) + "...";
        }
        private void LoadBookDetails(Book book)
        {
            TitleTextBlock.Text = TruncateTitle(book.Title, 40);
            AuthorTextBlock.Text = book.Author;
            DescriptionTextBlock.Text = book.Description;
            PagesTextBlock.Text = book.Pages.ToString();
            RatingsTextBlock.Text = book.Ratings.ToString();
            PurchasedTextBlock.Text = book.Sold.ToString();
            SubtitleTextBlock.Text = "     " + book.Synopsis.ToString();
            LanguageTextBlock.Text = book.Language.ToString();
            PriceTextBlock.Text = book.Price.ToString();

            if (book.Image != null)
            {
                CoverImage.Source = LoadImage(book.Image);
            }

            if (IsFavorite(_username, book.Title))
            {
                string newImageSource = "pack://application:,,,/Resources/Detail/active/favorite.png";
                FavImage.Tag = newImageSource;
            }
        }
        private BitmapImage LoadImage(byte[] imageData)
        {
            BitmapImage image = new BitmapImage();
            using (MemoryStream mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        // FAVORITE LOGIC
        private bool IsFavorite(string username, string title)
        {
            string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
            string query = "SELECT COUNT(*) FROM favorite WHERE username = @username AND title = @title";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@title", title);

                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection Error! : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void AddFavorite(string username, string title)
        {
            string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
            string query = "INSERT INTO favorite (username, title) VALUES (@username, @title)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@title", title);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding favorite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveFavorite(string username, string title)
        {
            string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
            string query = "DELETE FROM favorite WHERE username = @username AND title = @title";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@title", title);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing favorite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // CART LOGIC
        private bool IsCart(string username, string title, decimal price)
        {
            string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
            string query = "SELECT COUNT(*) FROM cart WHERE username = @username AND title = @title AND price = @price";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@price", price);

                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection Error! : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void AddCart(string username, string title, decimal price)
        {
            string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
            string query = "INSERT INTO cart (username, title, price) VALUES (@username, @title, @price)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@price", price);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show($"Added to Cart!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding favorite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveCart(string username, string title, decimal price)
        {
            string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
            string query = "DELETE FROM cart WHERE username = @username AND title = @title AND price = @price";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@price", price);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show($"Removed from Cart.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing favorite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // BUTTON
        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            if (IsFavorite(_username, _book.Title))
            {
                RemoveFavorite(_username, _book.Title);
                FavImage.Tag = "pack://application:,,,/Resources/Detail/favorite.png";
            }
            else
            {
                AddFavorite(_username, _book.Title);
                FavImage.Tag = "pack://application:,,,/Resources/Detail/active/favorite.png";
            }
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            CheckoutWindow checkoutWindow = new CheckoutWindow(totalBooks, totalPrice, _username);
            checkoutWindow.Show();
        }

        private void Borrow_Click(object sender, RoutedEventArgs e)
        {
            CheckoutWindow checkoutWindow = new CheckoutWindow(totalBooks, borrowPrice, _username);
            checkoutWindow.Show();
        }

        private void Add_Cart(object sender, RoutedEventArgs e)
        {
            if (IsCart(_username, _book.Title, _book.Price))
            {
                RemoveCart(_username, _book.Title, _book.Price);
            }
            else
            {
                AddCart(_username, _book.Title, _book.Price);
            }
        }
    }
}
