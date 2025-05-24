using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ApiControllers
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a user's bill.
    /// This object is used to send bill summary data from server to client.
    /// </summary>
    public class BillDTO
    {
        public int UserID { get; set; }
        public int BillAmt { get; set; }
        public List<BillDetailDTO> Details { get; set; }
    }

    // Data Transfer Object representing a single product/item in a bill
    public class BillDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
        public string ProductDescription { get; set; }

    }

}