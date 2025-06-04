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
    /// API controller to manage shopping cart operations such as adding, updating, retrieving, and deleting cart items.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/cart")]
    public class CartController : ApiController
    {
        // Entity Framework DB context to access Cart table
        GroceryDBEntities db = new GroceryDBEntities();

        /// <summary>
        /// Get all cart items from the database.
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllCarts()
        {
            try
            {
                var carts = db.Carts.ToList();
                return Ok(carts);
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to retrieve cart items."));
            }
        }

        /// <summary>
        /// Get cart items for a specific user by user ID.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns> cart items with product details for the specified user.</returns>
        [HttpGet]
        [Route("user/{userId:int}")]
        public IHttpActionResult GetCartByUserId(int userId)
        {
            try
            {
                var userCart = db.Carts
                    .Where(c => c.UserID == userId)
                    .Select(c => new
                    {
                        c.CartID,
                        c.UserID,
                        c.ProductID,
                        c.CartQty,
                        c.TotalPrice,
                        c.Grocery.ProductQuantity,
                        c.Grocery.ProductName,
                        c.Grocery.ProductImg,
                        c.Grocery.ProductPrice
                    })
                    .ToList();

                return Ok(userCart);
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to retrieve user cart."));
            }
        }

        /// <summary>
        /// Add a new item to the cart or update it if it already exists.
        /// </summary>
        /// <param name="cart">Cart object containing item details.</param>
        /// <returns>Success or error message.</returns>
        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddToCart(Cart cart)
        {
            try
            {
                var existingCartItem = db.Carts.FirstOrDefault(c =>
                    c.UserID == cart.UserID && c.ProductID == cart.ProductID);

                if (existingCartItem != null)
                {
                    // Item already in cart – update quantity and total price
                    existingCartItem.CartQty += cart.CartQty;
                    existingCartItem.TotalPrice += cart.TotalPrice;
                }
                else
                {
                    // New cart item – add to database
                    db.Carts.Add(cart);
                }

                db.SaveChanges();
                return Ok("Item added or updated in cart.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to add item to cart."));
            }
        }

        /// <summary>
        /// Update the quantity and total price of a cart item.
        /// </summary>
        /// <param name="cart">Cart object with updated details.</param>
        /// <returns>Success message or NotFound if item doesn't exist.</returns>
        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateCart(Cart cart)
        {
            try
            {
                var existing = db.Carts.FirstOrDefault(c => c.CartID == cart.CartID);
                if (existing == null)
                    return NotFound();

                existing.CartQty = cart.CartQty;
                existing.TotalPrice = cart.TotalPrice;

                db.SaveChanges();
                return Ok("Cart updated.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to update cart item."));
            }
        }

        /// <summary>
        /// Delete a specific cart item by its ID.
        /// </summary>
        /// <param name="id">Cart item ID.</param>
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteCartItem(int id)
        {
            try
            {
                var cart = db.Carts.Find(id);
                if (cart == null)
                    return NotFound();

                db.Carts.Remove(cart);
                db.SaveChanges();
                return Ok("Cart item deleted.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to delete cart item."));
            }
        }

        /// <summary>
        /// Clear all cart items for a specific user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        [HttpDelete]
        [Route("clear/user/{userId:int}")]
        public IHttpActionResult ClearCartByUser(int userId)
        {
            try
            {
                var cartItems = db.Carts.Where(c => c.UserID == userId).ToList();
                if (!cartItems.Any())
                    return NotFound();

                db.Carts.RemoveRange(cartItems);
                db.SaveChanges();
                return Ok("Cart cleared for user.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to clear user cart."));
            }
        }
    }
}
