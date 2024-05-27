using MySql.Data.MySqlClient;
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

namespace bookwish_app
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        private Dictionary<Button, string> buttonImagePaths;
        private Dictionary<Button, string> activeButtonImagePaths;
        private string currentSection;
        private Button activeButton;
        private string username;
        private string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";

        public MainMenu(string username)
        {
            this.username = username;

            InitializeComponent();

            UsernameTextBlock.Text = username;

            buttonImagePaths = new Dictionary<Button, string>
            {
                { DiscoverButton, "pack://application:,,,/Resources/Main/discover.png" },
                { CategoryButton, "pack://application:,,,/Resources/Main/category.png" },
                { BestSellerButton, "pack://application:,,,/Resources/Main/bestseller.png" },
                { FavoriteButton, "pack://application:,,,/Resources/Main/favorite.png" },
                { MyCartButton, "pack://application:,,,/Resources/Main/cart.png" }
            };

            activeButtonImagePaths = new Dictionary<Button, string>
            {
                { DiscoverButton, "pack://application:,,,/Resources/Main/active/discover.png" },
                { CategoryButton, "pack://application:,,,/Resources/Main/active/category.png" },
                { BestSellerButton, "pack://application:,,,/Resources/Main/active/bestseller.png" },
                { FavoriteButton, "pack://application:,,,/Resources/Main/active/favorite.png" },
                { MyCartButton, "pack://application:,,,/Resources/Main/active/cart.png" }
            };

            SetActiveButton(DiscoverButton);

            LoadBooks_Discover();
        }

        // BOOK FUNCTION LOAD
        private void LoadBooks_Discover()
        {
            if (currentSection == "Discover")
            {
                return;
            }

            CheckoutButton.Visibility = Visibility.Hidden;
            currentSection = "Discover";
            TextSection.Text = "Recommended";

            List<Book> books = FetchBooksFromDatabase().OrderBy(b => b.Author).ToList();

            BooksWrapPanel.Children.Clear();
            foreach (var book in books)
            {
                var bookItem = CreateBookItem(book);
                bookItem.MouseDown += (s, e) => OnBookClick(book);
                BooksWrapPanel.Children.Add(bookItem);
            }
        }

        private void LoadBooks_Category()
        {
            if (currentSection == "Category")
            {
                return;
            }

            CheckoutButton.Visibility = Visibility.Hidden;
            currentSection = "Category";
            TextSection.Text = "Books by Category";

            List<Book> books = FetchBooksFromDatabase().OrderBy(b => b.Category).ToList();

            BooksWrapPanel.Children.Clear();
            foreach (var book in books)
            {
                var bookItem = CreateBookItem(book);
                bookItem.MouseDown += (s, e) => OnBookClick(book);
                BooksWrapPanel.Children.Add(bookItem);
            }
        }

        private void LoadBooks_BestSeller()
        {
            if (currentSection == "BestSeller")
            {
                return;
            }

            CheckoutButton.Visibility = Visibility.Hidden;
            currentSection = "BestSeller";
            TextSection.Text = "Best Sellers";

            List<Book> books = FetchBooksFromDatabase().OrderByDescending(b => b.Sold).ToList();

            BooksWrapPanel.Children.Clear();
            foreach (var book in books)
            {
                var bookItem = CreateBookItem(book);
                bookItem.MouseDown += (s, e) => OnBookClick(book);
                BooksWrapPanel.Children.Add(bookItem);
            }
        }

        private void LoadBooks_Favorite()
        {
            if (currentSection == "Favorite")
            {
                return;
            }

            CheckoutButton.Visibility = Visibility.Hidden;
            currentSection = "Favorite";
            TextSection.Text = "Your Favorite";

            List<Book> books = FetchFavoriteBooksFromDatabase();

            BooksWrapPanel.Children.Clear();
            foreach (var book in books)
            {
                var bookItem = CreateBookItem(book);
                bookItem.MouseDown += (s, e) => OnBookClick(book);
                BooksWrapPanel.Children.Add(bookItem);
            }
        }

        private void LoadBooks_Cart()
        {
            if (currentSection == "Cart")
            {
                return;
            }

            CheckoutButton.Visibility = Visibility.Visible;
            currentSection = "Cart";
            TextSection.Text = "Your Cart";

            List<Book> books = FetchCartBooksFromDatabase();

            BooksWrapPanel.Children.Clear();
            foreach (var book in books)
            {
                var bookItem = CreateBookItem(book);
                bookItem.MouseDown += (s, e) => OnBookClick(book);
                BooksWrapPanel.Children.Add(bookItem);
            }
        }

        // FETCH LOGIC
        private List<Book> FetchBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            string query = "SELECT id, title, author, category, sold, image, pages, ratings, description, price, synopsis, language FROM books";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Author = reader.GetString("author"),
                        Category = reader.GetString("category"),
                        Sold = reader.GetInt32("sold"),
                        Image = (byte[])reader["image"],
                        Pages = reader.GetInt32("pages"),
                        Price = reader.GetInt32("price"),
                        Language = reader.GetString("language"),
                        Ratings = reader.GetDouble("ratings"),
                        Description = reader.GetString("description"), // Fetch description
                        Synopsis = reader.GetString("synopsis")
                    });
                }
            }

            return books;
        }

        private List<Book> FetchFavoriteBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            string query = "SELECT books.title, books.author, books.category, books.sold, books.image, books.pages, books.ratings, books.description, books.price, books.synopsis, books.language " +
                           "FROM books INNER JOIN favorite ON books.title = favorite.title " +
                           "WHERE favorite.username = @username";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Title = reader.GetString("title"),
                        Author = reader.GetString("author"),
                        Category = reader.GetString("category"),
                        Sold = reader.GetInt32("sold"),
                        Image = (byte[])reader["image"],
                        Pages = reader.GetInt32("pages"),
                        Price = reader.GetDecimal("price"),
                        Language = reader.GetString("language"),
                        Ratings = reader.GetDouble("ratings"),
                        Description = reader.GetString("description"),
                        Synopsis = reader.GetString("synopsis")
                    });
                }
            }

            return books;
        }

        private List<Book> FetchCartBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            string query = "SELECT books.title, books.author, books.category, books.sold, books.image, books.pages, books.ratings, books.description, books.price, books.synopsis, books.language " +
                           "FROM books INNER JOIN cart ON books.title = cart.title AND books.price = cart.price " +
                           "WHERE cart.username = @username";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Title = reader.GetString("title"),
                        Author = reader.GetString("author"),
                        Category = reader.GetString("category"),
                        Sold = reader.GetInt32("sold"),
                        Image = (byte[])reader["image"],
                        Pages = reader.GetInt32("pages"),
                        Price = reader.GetDecimal("price"),
                        Language = reader.GetString("language"),
                        Ratings = reader.GetDouble("ratings"),
                        Description = reader.GetString("description"),
                        Synopsis = reader.GetString("synopsis")
                    });
                }
            }

            return books;
        }

        private List<Book> SearchBooksFromDatabase(string searchTerm)
        {
            List<Book> books = new List<Book>();

            string query = "SELECT id, title, author, category, sold, image, pages, ratings, description, price, synopsis, language " +
                           "FROM books " +
                           "WHERE title LIKE @searchTerm OR author LIKE @searchTerm OR category LIKE @searchTerm";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Author = reader.GetString("author"),
                        Category = reader.GetString("category"),
                        Sold = reader.GetInt32("sold"),
                        Image = (byte[])reader["image"],
                        Pages = reader.GetInt32("pages"),
                        Price = reader.GetDecimal("price"),
                        Language = reader.GetString("language"),
                        Ratings = reader.GetDouble("ratings"),
                        Description = reader.GetString("description"),
                        Synopsis = reader.GetString("synopsis")
                    });
                }
            }

            return books;
        }

        // BOOK CREATION LOGIC
        private Border CreateBookItem(Book book)
        {
            var bookItem = new StackPanel
            {
                Width = 150,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Top
            };

            var image = new Image
            {
                Source = LoadImage(book.Image),
                Height = 200,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var title = new TextBlock
            {
                Text = book.Title,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var subtext = new TextBlock
            {
                Text = currentSection switch
                {
                    "Category" => book.Category,
                    "Discover" => book.Author,
                    "Favorite" => book.Author,
                    "Cart" => $"Price : {book.Price}",
                    _ => $"Purchased : {book.Sold}",
                },
                Foreground = new SolidColorBrush(Colors.Gray),
                TextAlignment = TextAlignment.Center
            };

            bookItem.Children.Add(image);
            bookItem.Children.Add(title);
            bookItem.Children.Add(subtext);

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                Child = bookItem,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 320,
                    ShadowDepth = 5,
                    Opacity = 0.5
                }
            };

            return border;
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

        // BOOK CLICK LOGIC
        private void OnBookClick(Book book)
        {
            BookDetailsWindow bookDetailsWindow = new BookDetailsWindow(book, username);
            bookDetailsWindow.Show();
        }

        // BUTTON HANDLERS
        private void Discover_Click(object sender, RoutedEventArgs e)
        {
            SidebarButton_Click(sender, e);
            BooksWrapPanel.Children.Clear(); // Clear previous items
            LoadBooks_Discover();
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            SidebarButton_Click(sender, e);
            BooksWrapPanel.Children.Clear(); // Clear previous items
            LoadBooks_Category();
        }

        private void BestSeller_Click(object sender, RoutedEventArgs e)
        {
            SidebarButton_Click(sender, e);
            BooksWrapPanel.Children.Clear(); // Clear previous items
            LoadBooks_BestSeller();
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            SidebarButton_Click(sender, e);
            BooksWrapPanel.Children.Clear(); // Clear previous items
            LoadBooks_Favorite();
        }

        private void MyCart_Click(object sender, RoutedEventArgs e)
        {
            SidebarButton_Click(sender, e);
            BooksWrapPanel.Children.Clear(); // Clear previous items
            LoadBooks_Cart();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow LogOut = new MainWindow();
            LogOut.Show();
            this.Close();
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            List<Book> cartBooks = FetchCartBooksFromDatabase();
            int totalBooks = cartBooks.Count;
            decimal totalPrice = cartBooks.Sum(b => b.Price);

            CheckoutWindow checkoutWindow = new CheckoutWindow(totalBooks, totalPrice, username);
            checkoutWindow.Show();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Use the search term to filter the books fetched from the database
                List<Book> filteredBooks = SearchBooksFromDatabase(searchTerm);

                // Update the UI to display the filtered books
                BooksWrapPanel.Children.Clear();
                foreach (var book in filteredBooks)
                {
                    var bookItem = CreateBookItem(book);
                    bookItem.MouseDown += (s, evt) => OnBookClick(book);
                    BooksWrapPanel.Children.Add(bookItem);
                }
            }
            else
            {
                MessageBox.Show("Please enter a search term.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ACTIVE SIDEBAR BUTTON
        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                SetActiveButton(clickedButton);
                // Handle navigation or other logic here
            }
        }

        private void SetActiveButton(Button button)
        {
            if (activeButton != null)
            {
                // Reset the previous active button to its original state
                activeButton.Style = (Style)FindResource("SidebarButtonStyle");
                activeButton.Tag = buttonImagePaths[activeButton];
            }

            // Set the new active button
            button.Style = (Style)FindResource("ActiveSidebarButtonStyle");
            button.Tag = activeButtonImagePaths[button];
            activeButton = button;
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public int Sold { get; set; }
        public byte[] Image { get; set; }
        public int Pages { get; set; }
        public double Ratings { get; set; }
        public string Description { get; set; }
        public string Synopsis { get; set; }
        public decimal Price { get; set; }
        public string Language { get; set; }
    }
}
