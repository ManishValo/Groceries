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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        GroceryDBEntities db = new GroceryDBEntities();

        // GET: api/products
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllProducts()
        {
            var products = db.Groceries.ToList();
            return Ok(products);
        }


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


        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateProduct(Grocery product)
        {
            var existing = db.Groceries.Find(product.ProductID);
            if (existing == null)
                return NotFound();

            existing.ProductName = product.ProductName;
            existing.ProductImg = product.ProductImg;
            existing.ProductPrice = product.ProductPrice;
            existing.ProductQuantity= product.ProductQuantity;
            existing.ProductDescription = product.ProductDescription;
            existing.ProductCatID = product.ProductCatID;

            db.SaveChanges();
            return Ok("Products updated.");
        }


        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteProduct(int id)
        {
            var exist = db.Groceries.Find(id);
            if (exist == null)
                return NotFound();

            db.Groceries.Remove(exist);
            db.SaveChanges();
            return Ok("Products deleted.");
        }


        [HttpGet]
        [Route("category/{catId:int}")]
        public IHttpActionResult GetGroceriesByCategory(int catId)
        {
            var exist = db.Groceries.Where(p => p.ProductCatID == catId).ToList();

            if (exist == null || !exist.Any())
                return NotFound();

            return Ok(exist);
        }

        [HttpGet]
        [Route("categories")]
        public IHttpActionResult GetCategories()
        {
            var categories = db.Categories.Select(c => new { c.CategoryID, c.CategoryName }).ToList();
            return Ok(categories);
        }


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
                            product.ProductQuantity -= item.Quantity;
                        }
                        else
                        {
                            return BadRequest($"Not enough stock for Product");
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

        public class ProductPurchaseDto
        {
            public int ProductID { get; set; }
            public int Quantity { get; set; }
        }


    }
}
