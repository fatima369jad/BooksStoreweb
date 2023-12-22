using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using BooksStore.Models;

namespace BooksStore.Controllers
{
    public class StockController : Controller
    {
        private readonly DBACCESS db = new DBACCESS();

        [HttpGet]
        public ActionResult Index()
        {
            List<StockSaldo> stockItems = new List<StockSaldo>();

            db.OpenConnection();

            string query = "SELECT * FROM StockSaldo";

            using (var command = new SqlCommand(query, db.con))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    StockSaldo stockItem = new StockSaldo
                    {
                        StoreID = Convert.ToInt32(reader["StoreID"]),
                        ISBN = reader["ISBN"].ToString(),
                        Number = Convert.ToInt32(reader["Number"])
                    };

                    stockItems.Add(stockItem);
                }
            }

            db.CloseConnection();

            return View(stockItems);
        }

        [HttpGet]
        public ActionResult Edit(string storeId, string isbn)
        {
            var stockItem = GetStockItem(storeId, isbn);

            if (stockItem == null)
            {
                return HttpNotFound();
            }

            return View(stockItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockSaldo stockItem)
        {
            if (ModelState.IsValid)
            {
                db.OpenConnection();

                if (StockItemExists(stockItem.StoreID, stockItem.ISBN))
                {
                    string query = $"UPDATE StockSaldo SET Number = {stockItem.Number} " +
                                   $"WHERE StoreID = {stockItem.StoreID} AND ISBN = '{stockItem.ISBN}'";

                    db.IUD(query);

                    db.CloseConnection();

                    return RedirectToAction("Index");
                }
                else
                {
                    db.CloseConnection();
                    return HttpNotFound();
                }
            }

            return View(stockItem);
        }

        private bool StockItemExists(int storeId, string isbn)
        {
            string query = $"SELECT COUNT(*) FROM StockSaldo WHERE StoreID = {storeId} AND ISBN = '{isbn}'";

            using (var command = new SqlCommand(query, db.con))
            {
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private StockSaldo GetStockItem(string storeId, string isbn)
        {
            db.OpenConnection();

            string query = $"SELECT * FROM StockSaldo WHERE StoreID = {storeId} AND ISBN = '{isbn}'";

            using (var command = new SqlCommand(query, db.con))
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    StockSaldo stockItem = new StockSaldo
                    {
                        StoreID = Convert.ToInt32(reader["StoreID"]),
                        ISBN = reader["ISBN"].ToString(),
                        Number = Convert.ToInt32(reader["Number"])
                    };

                    db.CloseConnection();
                    return stockItem;
                }
            }

            db.CloseConnection();
            return null;
        }

        // ... (previous code)

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StockSaldo stockItem)
        {
            if (ModelState.IsValid)
            {
                db.OpenConnection();

             
                    string query = $"INSERT INTO StockSaldo (StoreID, ISBN, Number) " +
                                   $"VALUES ({stockItem.StoreID}, '{stockItem.ISBN}', {stockItem.Number})";

                    db.IUD(query);

                    db.CloseConnection();

                    return RedirectToAction("Index");
              
            }

            return View(stockItem);
        }

        [HttpGet]
        public ActionResult Delete(string storeId, string isbn)
        {
            var stockItem = GetStockItem(storeId, isbn);

            if (stockItem == null)
            {
                return HttpNotFound();
            }

            return View(stockItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string storeId, string isbn)
        {
            db.OpenConnection();

            if (StockItemExists(Convert.ToInt32(storeId), isbn))
            {
                string query = $"DELETE FROM StockSaldo WHERE StoreID = {storeId} AND ISBN = '{isbn}'";
                db.IUD(query);

                db.CloseConnection();

                return RedirectToAction("Index");
            }
            else
            {
                db.CloseConnection();
                return HttpNotFound();
            }
        }

        // ... (remaining code)

    }
}
