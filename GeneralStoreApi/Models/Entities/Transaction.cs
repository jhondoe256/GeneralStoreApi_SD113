using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStoreApi.Models.Entities
{
    public class Transaction
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey(nameof(Product))]
        public string ProductSKU { get; set; }
        public Product Product { get; set; }
        [Required]
        public int ItemCount { get; set; }

        public DateTime DateOfTransaction { get; set; }
    }
}