using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BooksStore.Models
{
    public class StockSaldo
    {

        public int StoreID { get; set; }
        public string ISBN { get; set; }
        public int Number { get; set; }

    }
}