using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Models;
namespace WebApplication1.ApiControllers
{
    /// <summary>
    /// API Controller for managing order details, including retrieving, adding, and filtering orders
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/bill")]
    public class OrderDetailsController : ApiController
    {
        //creating object of databse entities
        GroceryDBEntities db = new GroceryDBEntities();

        /// <summary>
        /// Retrieves all bills from the database.
        /// </summary>
        /// <returns>List of bills with OrderID, UserID, Customer Name, Order Date, and Order Amount.</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllBills()
        {
            try {
                var bills = db.Orders
                    .Select(b => new
                    {
                        b.OrderID,
                        b.UserID,
                        CustomerName = b.UserDetail.Name,
                        b.OrderDate,
                        b.OrderAmt,
                    })
                    .ToList();

                return Ok(bills);
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Server Error"));
            }
        }

        /// <summary>
        /// Adds a new bill with associated order details.
        /// </summary>
        /// <param name="billDto">DTO containing bill and order details.</param>
        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddBill(BillDTO billDto)
        {
            try {
                //validate input data 
                if (billDto == null || billDto.Details == null || !billDto.Details.Any())
                    return BadRequest("Invalid bill data.");

                // Create a new Order entity
                var bill = new Order
                {
                    UserID = billDto.UserID,
                    OrderAmt = billDto.BillAmt,
                    OrderDate = DateTime.Now
                };

                // Add the order to the database
                db.Orders.Add(bill);
                db.SaveChanges();

                // Iterate over the order details and insert them into the database
                foreach (var item in billDto.Details)
                {
                    var detail = new OrderDetail
                    {
                        OrderID = bill.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    };
                    db.OrderDetails.Add(detail);
                }

                db.SaveChanges();

                return Ok(new { Order = bill.OrderID });
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Server Error"));
            }

        }

        /// <summary>
        /// Retrieves details of a specific bill based on OrderID.
        /// </summary>
        /// <param name="orderId">Order ID for which details need to be retrieved.</param>
        /// <returns>List of bill details including product information.</returns>
        [HttpGet]
        [Route("details/{orderId}")]
        public IHttpActionResult GetBillDetails(int orderId)
        {
            try {
                var details = db.OrderDetails
                    .Where(od => od.OrderID == orderId)
                    .Select(od => new BillDetailDTO
                    {
                        ProductID = od.ProductID,
                        ProductName = od.Grocery.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.TotalPrice
                    })
                    .ToList();

                if (details.Count == 0)
                    return NotFound();

                return Ok(details);
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Server Error"));
            }
        }

        [HttpGet]
        [Route("user/{userId}")]
        public IHttpActionResult GetUserBills(int userId)
        {
            var bills = db.Orders
                .Where(b => b.UserID == userId)
                .OrderByDescending(b => b.OrderDate)
                .Select(b => new
                {
                    b.OrderID,
                    b.OrderDate,
                    b.OrderAmt,
                    Items = b.OrderDetails.Select(od => new
                    {
                        od.ProductID,
                        ProductName = od.Grocery.ProductName,
                        Description = od.Grocery.ProductDescription,
                        od.Quantity,
                        Price = od.TotalPrice
                    }).ToList()
                })
                .ToList();

            return Ok(bills);
        }

    }
}

