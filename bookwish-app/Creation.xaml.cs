using Microsoft.Win32;
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
    /// Interaction logic for Creation.xaml
    /// </summary>
    public partial class Creation : Window
    {
        private string connectionString = "Server=localhost;Database=bookwish;Uid=root;Pwd=;";
        private byte[] selectedImageBytes;
        private Book editingBook;

        public event EventHandler BookSaved;

        public Creation()
        {
            InitializeComponent();
        }

        public Creation(Book book) : this()
        {
            editingBook = book;
            LoadBookData(book);
        }

        private void LoadBookData(Book book)
        {
            UsernameTextBlock.Text = book.Id.ToString();
            TitleTextBox.Text = book.Title;
            CategoryTextBox.Text = book.Category;
            PriceTextBox.Text = book.Price.ToString();
            AuthorTextBox.Text = book.Author;
            RatingTextBox.Text = book.Ratings.ToString();
            LanguageTextBox.Text = book.Language;
            DescriptionTextBox.Text = book.Description;
            SynopsisTextBox.Text = book.Synopsis;
            selectedImageBytes = book.Image;
            PagesTextBox.Text = book.Pages.ToString();

            if (book.Image != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(book.Image);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                BookCoverImage.Source = bitmap;
            }

            SaveBookButton.Content = "Update Book";
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                selectedImageBytes = File.ReadAllBytes(filePath);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                BookCoverImage.Source = bitmap;
            }
        }

        private bool BookTitleExists(string title)
        {
            string query = "SELECT COUNT(*) FROM books WHERE title = @title";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@title", title);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void SaveBook_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string category = CategoryTextBox.Text;
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }
            string author = AuthorTextBox.Text;
            if (!double.TryParse(RatingTextBox.Text, out double rating))
            {
                MessageBox.Show("Please enter a valid rating.");
                return;
            }
            string language = LanguageTextBox.Text;
            string description = DescriptionTextBox.Text;
            string synopsis = SynopsisTextBox.Text;
            int pages = int.Parse(PagesTextBox.Text);

            if (selectedImageBytes == null)
            {
                MessageBox.Show("Please select an image for the book cover.");
                return;
            }

            if (BookTitleExists(title))
            {
                MessageBox.Show("A book with this title already exists. Please choose a different title.");
                return;
            }

            Book newBook = new Book
            {
                Title = title,
                Category = category,
                Price = price,
                Author = author,
                Ratings = rating,
                Language = language,
                Description = description,
                Synopsis = synopsis,
                Pages = pages,
                Image = selectedImageBytes
            };

            if (editingBook == null)
            {
                SaveBookToDatabase(newBook);
            }
            else
            {
                UpdateBookInDatabase(newBook);
            }

            // Trigger the BookSaved event
            BookSaved?.Invoke(this, EventArgs.Empty);
        }


        private void SaveBookToDatabase(Book book)
        {
            string query = "INSERT INTO books (title, category, pages, price, author, ratings, language, description, synopsis, image) " +
                           "VALUES (@title, @category, @pages, @price, @author, @ratings, @language, @description, @synopsis, @image)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@title", book.Title);
                command.Parameters.AddWithValue("@category", book.Category);
                command.Parameters.AddWithValue("@price", book.Price);
                command.Parameters.AddWithValue("@author", book.Author);
                command.Parameters.AddWithValue("@ratings", book.Ratings);
                command.Parameters.AddWithValue("@language", book.Language);
                command.Parameters.AddWithValue("@description", book.Description);
                command.Parameters.AddWithValue("@synopsis", book.Synopsis);
                command.Parameters.AddWithValue("@image", book.Image);
                command.Parameters.AddWithValue("@pages", book.Pages);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            MessageBox.Show("Book saved successfully!");
            ClearFields();
        }

        private void UpdateBookInDatabase(Book book)
        {
            string query = "UPDATE books SET title = @title, category = @category, price = @price, pages = @pages, author = @author, ratings = @ratings, language = @language, description = @description, synopsis = @synopsis, image = @image " +
                           "WHERE id = @id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", int.Parse(UsernameTextBlock.Text));
                command.Parameters.AddWithValue("@title", book.Title);
                command.Parameters.AddWithValue("@category", book.Category);
                command.Parameters.AddWithValue("@price", book.Price);
                command.Parameters.AddWithValue("@author", book.Author);
                command.Parameters.AddWithValue("@ratings", book.Ratings);
                command.Parameters.AddWithValue("@language", book.Language);
                command.Parameters.AddWithValue("@description", book.Description);
                command.Parameters.AddWithValue("@synopsis", book.Synopsis);
                command.Parameters.AddWithValue("@image", book.Image);
                command.Parameters.AddWithValue("@pages", book.Pages);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            MessageBox.Show("Book updated successfully!");
            ClearFields();
        }

        private void ClearFields()
        {
            TitleTextBox.Clear();
            CategoryTextBox.Clear();
            PriceTextBox.Clear();
            AuthorTextBox.Clear();
            RatingTextBox.Clear();
            LanguageTextBox.Clear();
            DescriptionTextBox.Clear();
            SynopsisTextBox.Clear();
            BookCoverImage.Source = null;
            selectedImageBytes = null;
            this.Close();
        }
    }
}
