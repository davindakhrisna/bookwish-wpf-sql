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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        private string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
        private Dictionary<Button, string> buttonImagePaths;
        private Dictionary<Button, string> activeButtonImagePaths;
        private Button activeButton;
        private List<Book> allBooks;
        private Book selectedBook;

        public Admin(string username)
        {
            InitializeComponent();
            LoadData();

            buttonImagePaths = new Dictionary<Button, string>
            {
                { CreateButton, "pack://application:,,,/Resources/Admin/add.png" },
                { EditButton, "pack://application:,,,/Resources/Admin/edit.png" },
                { DeleteButton, "pack://application:,,,/Resources/Admin/delete.png" },
            };

            activeButtonImagePaths = new Dictionary<Button, string>
            {
                { CreateButton, "pack://application:,,,/Resources/Admin/active/add.png" },
                { EditButton, "pack://application:,,,/Resources/Admin/active/edit.png" },
                { DeleteButton, "pack://application:,,,/Resources/Admin/active/delete.png" },
            };

            AdminDataGrid.SelectionChanged += AdminDataGrid_SelectionChanged;
            UsernameTextBlock.Text = username;
        }

        // DATABASE FETCH
        private void LoadData()
        {
            allBooks = FetchBooksFromDatabase();
            AdminDataGrid.ItemsSource = allBooks;
        }

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

        // DATABASE DELETE FUNCTION
        private void DeleteBookFromDatabase(int bookId)
        {
            string query = "DELETE FROM books WHERE id = @id";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", bookId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Datagrid SelectionChanged
        private void AdminDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedBook = AdminDataGrid.SelectedItem as Book;
        }

        // BUTTON LOGIC
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            SidebarButton_Click(sender, e);
            Creation creationWindow = new Creation();
            creationWindow.BookSaved += CreationWindow_BookSaved;
            creationWindow.Show();
        }
        private void CreationWindow_BookSaved(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            
            if (AdminDataGrid.SelectedItem != null)
            {
                SidebarButton_Click(sender, e);
                Book selectedBook = (Book)AdminDataGrid.SelectedItem;
                OpenCreationWindow(selectedBook);
            }
            else
            {
                MessageBox.Show("Please select a book to edit.");
            }
        }
        private void OpenCreationWindow(Book book)
        {
            Creation creationWindow = new Creation(book);
            creationWindow.BookSaved += CreationWindow_BookSaved;
            creationWindow.ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook != null)
            {
                SidebarButton_Click(sender, e);
                DeleteBookFromDatabase(selectedBook.Id);
                LoadData(); // Refresh the DataGrid
                selectedBook = null;
            }
            else
            {
                MessageBox.Show("Please select a book to delete.", "No Book Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow LogOut = new MainWindow();
            LogOut.Show();
            this.Close();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.ToLower();
            var filteredBooks = allBooks.Where(b => b.Title.ToLower().Contains(searchTerm) ||
                                                    b.Author.ToLower().Contains(searchTerm) ||
                                                    b.Category.ToLower().Contains(searchTerm) ||
                                                    b.Description.ToLower().Contains(searchTerm) ||
                                                    b.Synopsis.ToLower().Contains(searchTerm)).ToList();
            AdminDataGrid.ItemsSource = filteredBooks;
            SearchTextBox.Text = null;
        }

        // BUTTON CLICK ACTIVE
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
}
