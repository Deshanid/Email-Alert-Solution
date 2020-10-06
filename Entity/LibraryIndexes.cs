using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpiryEmailAlertSolution.Entity
{
    class LibraryIndexes
    {
        public string Agreement_ID { get; set; }
        public string Agreement_Name { get; set; }
        public string Supplier_Name { get; set; }
        public string Contract_Period { get; set; }
        public DateTime Signed_Date { get; set; }
        public DateTime Expiry_Date { get; set; }

    }
}
