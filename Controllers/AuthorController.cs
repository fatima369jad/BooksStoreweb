using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using BooksStore.Models;

namespace BooksStore.Controllers
{
    public class AuthorController : Controller
    {
        private readonly DBACCESS db = new DBACCESS();

        [HttpGet]
        public ActionResult Index()
        {
            List<Author> authors = new List<Author>();

            db.OpenConnection();

            string query = "SELECT * FROM Author";

            using (var command = new SqlCommand(query, db.con))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Author author = new Author
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])
                    };

                    authors.Add(author);
                }
            }

            db.CloseConnection();

            return View(authors);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Retrieve author details by ID
            var author = GetAuthorById(id);

            if (author == null)
            {
                // Handle the case where the author with the provided ID does not exist
                return HttpNotFound();
            }

            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                db.OpenConnection();

                // Check if the author with the provided ID exists
                if (AuthorExists(author.ID))
                {
                    string query = $"UPDATE Author SET FirstName = '{author.FirstName}', LastName = '{author.LastName}', " +
                                   $"DateOfBirth = '{author.DateOfBirth}' WHERE ID = {author.ID}";

                    db.IUD(query);

                    db.CloseConnection();

                    // Redirect to the Index page after a successful update
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the case where the author with the provided ID does not exist
                    db.CloseConnection();
                    return HttpNotFound();
                }
            }

            // If ModelState is not valid, return the Edit view with validation errors
            return View(author);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            // Retrieve author details by ID
            var author = GetAuthorById(id);

            if (author == null)
            {
                // Handle the case where the author with the provided ID does not exist
                return HttpNotFound();
            }

            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Check if the author with the provided ID exists

            db.OpenConnection();

            string query = $"DELETE FROM Author WHERE ID = {id}";
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
        public ActionResult Add(Author author)
        {
            if (ModelState.IsValid)
            {
                db.OpenConnection();

                string query = $"INSERT INTO Author (FirstName, LastName, DateOfBirth) " +
                               $"VALUES ('{author.FirstName}', '{author.LastName}', '{author.DateOfBirth}')";

                db.IUD(query);
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return the Add view with validation errors
            return View(author);
        }

        // Helper method to check if an author with a given ID exists
        private bool AuthorExists(int id)
        {
            string query = $"SELECT COUNT(*) FROM Author WHERE ID = {id}";

            using (var command = new SqlCommand(query, db.con))
            {
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Helper method to retrieve an author by their ID
        private Author GetAuthorById(int id)
        {
            db.OpenConnection();

            string query = $"SELECT * FROM Author WHERE ID = {id}";

            using (var command = new SqlCommand(query, db.con))
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Author author = new Author
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])
                    };

                    db.CloseConnection();
                    return author;
                }
            }

            db.CloseConnection();
            return null;
        }
    }
}
