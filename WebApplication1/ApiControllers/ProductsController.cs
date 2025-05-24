using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Models;

namespace WebApplication1.ApiControllers
{
    /// <summary>
    /// Controller to manage grocery products including CRUD operations, stock updates, and filtering by category.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        // Database context
        GroceryDBEntities db = new GroceryDBEntities();

        /// <summary>
        /// Get all products from the database.
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllProducts()
        {
            try
            {
                var products = db.Groceries.ToList();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Add a new product to the database.
        /// </summary>
        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddProduct(Grocery product)
        {
            try
            {
                db.Groceries.Add(product);
                db.SaveChanges();
                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Update an existing product by ID.
        /// </summary>
        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateProduct(Grocery product)
        {
            try
            {
                var existing = db.Groceries.Find(product.ProductID);
                if (existing == null)
                    return NotFound();

                // Update product properties
                existing.ProductName = product.ProductName;
                existing.ProductImg = product.ProductImg;
                existing.ProductPrice = product.ProductPrice;
                existing.ProductQuantity = product.ProductQuantity;
                existing.ProductDescription = product.ProductDescription;
                existing.ProductCatID = product.ProductCatID;

                db.SaveChanges();
                return Ok("Product updated.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Delete a product by ID.
        /// </summary>
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteProduct(int id)
        {
            try
            {
                var exist = db.Groceries.Find(id);
                if (exist == null)
                    return NotFound();

                db.Groceries.Remove(exist);
                db.SaveChanges();
                return Ok("Product deleted.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get products by category ID.
        /// </summary>
        [HttpGet]
        [Route("category/{catId:int}")]
        public IHttpActionResult GetGroceriesByCategory(int catId)
        {
            try
            {
                var exist = db.Groceries.Where(p => p.ProductCatID == catId).ToList();
                if (exist == null || !exist.Any())
                    return NotFound();

                return Ok(exist);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get all product categories.
        /// </summary>
        [HttpGet]
        [Route("categories")]
        public IHttpActionResult GetCategories()
        {
            try
            {
                var categories = db.Categories.Select(c => new { c.CategoryID, c.CategoryName }).ToList();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Update stock quantities after a purchase.
        /// </summary>
        /// <param name="purchasedItems">List of purchased product IDs and quantities.</param>
        [HttpPost]
        [Route("update-stock")]
        public IHttpActionResult UpdateStockAfterPurchase(List<ProductPurchaseDto> purchasedItems)
        {
            try
            {
                foreach (var item in purchasedItems)
                {
                    var product = db.Groceries.FirstOrDefault(p => p.ProductID == item.ProductID);
                    if (product != null)
                    {
                        if (product.ProductQuantity >= item.Quantity)
                        {
                            product.ProductQuantity -= item.Quantity; // Reduce stock
                        }
                        else
                        {
                            return BadRequest($"Not enough stock for product ID: {item.ProductID}");
                        }
                    }
                }

                db.SaveChanges();
                return Ok("Stock updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// DTO class used for stock update operation.
        /// </summary>
        public class ProductPurchaseDto
        {
            public int ProductID { get; set; }
            public int Quantity { get; set; }
        }
    }
}
