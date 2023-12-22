using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Linq;
using System.Web.Mvc;
using BooksStore.Models;
using System.Web.UI.WebControls;

namespace BooksStore.Controllers
{
    public class BookController : Controller
    {
        private readonly DBACCESS db = new DBACCESS();

        // GET: Book

        [HttpGet]
        public ActionResult Index()
        {

            List<Books> books = new List<Books>();


            db.OpenConnection();

            string query = "SELECT * FROM Books";

            using (var command = new SqlCommand(query, db.con))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Books book = new Books
                    {
                        ISBN13 = reader["ISBN13"].ToString(),
                        Title = reader["Title"].ToString(),
                        Language = reader["Language"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
                        AuthorID = Convert.ToInt32(reader["AuthorID"])
                    };

                    books.Add(book);
                }
            }

            db.CloseConnection();


            return View(books);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            // Retrieve book details by ISBN
            var book = GetBookById(id);

            if (book == null)
            {
                // Handle the case where the book with the provided ISBN does not exist
                return HttpNotFound();
            }

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute for security
        public ActionResult Edit(Books book)
        {
            if (ModelState.IsValid)
            {
                // Validate the ModelState before proceeding with the update

                db.OpenConnection();

                // Check if the book with the provided ISBN exists
                if (BookExists(book.ISBN13))
                {
                    string query = $"UPDATE Books SET Title = '{book.Title}', Language = '{book.Language}', " +
                                   $"Price = {book.Price}, ReleaseDate = '{book.ReleaseDate}', AuthorID = {book.AuthorID} " +
                                   $"WHERE ISBN13 = '{book.ISBN13}'";

                    db.IUD(query);

                    db.CloseConnection();

                    // Redirect to the Details page after a successful update
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the case where the book with the provided ISBN does not exist
                    db.CloseConnection();
                    return HttpNotFound();
                }
            }

            // If ModelState is not valid, return the Edit view with validation errors
            return View(book);
        }

        // Helper method to check if a book with a given ISBN exists
        private bool BookExists(string ISBN)
        {
            string query = $"SELECT COUNT(*) FROM Books WHERE ISBN13 = '{ISBN}'";

            using (var command = new SqlCommand(query, db.con))
            {
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Helper method to retrieve a book by its ISBN
        private Books GetBookById(string ISBN)
        {
            db.OpenConnection();

            string query = $"SELECT * FROM Books WHERE ISBN13 = '{ISBN}'";

            using (var command = new SqlCommand(query, db.con))
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Books book = new Books
                    {
                        ISBN13 = reader["ISBN13"].ToString(),
                        Title = reader["Title"].ToString(),
                        Language = reader["Language"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
                        AuthorID = Convert.ToInt32(reader["AuthorID"])
                    };

                    db.CloseConnection();
                    return book;
                }
            }

            db.CloseConnection();
            return null;
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Retrieve book details by ISBN
            var book = GetBookById(id);

            if (book == null)
            {
                // Handle the case where the book with the provided ISBN does not exist
                return HttpNotFound();
            }

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            // Check if the book with the provided ISBN exists

            db.OpenConnection();

                string query = $"DELETE FROM Books WHERE ISBN13 = '{id}'";
                db.IUD(query);


                // Redirect to the Index page after a successful delete
                return RedirectToAction("Index");
         
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Books book)
        {
            if (ModelState.IsValid)
            {

                db.OpenConnection();


                string query = $"INSERT INTO Books (ISBN13, Title, Language, Price, ReleaseDate, AuthorID) " +
               $"VALUES ('{book.ISBN13}', '{book.Title}', '{book.Language}', {book.Price}, '{book.ReleaseDate}', {book.AuthorID})";


                db.IUD(query);
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return the Add view with validation errors
            return View(book);
        }


    }
}
