using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Models;

namespace WebApplication1.ApiControllers
{
    /// <summary>
    /// API controller to manage grocery product categories.
    /// Supports CRUD operations: Create, Read, Update, and Delete.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        // Database context to access the Categories table
        GroceryDBEntities db = new GroceryDBEntities();

        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllCategories()
        {
            try
            {
                var categories = db.Categories.ToList();
                return Ok(categories);
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to retrieve categories."));
            }
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="category">Category object to be added.</param>
        [HttpPost]
        [Route("add")]
        public IHttpActionResult RegisterCategory(Category category)
        {
            try
            {
                // Check if category with the same name already exists
                var exists = db.Categories.Any(c => c.CategoryName == category.CategoryName);
                if (exists)
                {
                    return BadRequest("Category with the same name already exists.");
                }

                db.Categories.Add(category);
                db.SaveChanges();
                return Ok("Category added successfully.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Server error while adding category."));
            }
        }

        /// <summary>
        /// Updates an existing category by its ID.
        /// </summary>
        /// <param name="category">Category object with updated data.</param>
        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateCategory(Category category)
        {
            try
            {
                var existing = db.Categories.Find(category.CategoryID);
                if (existing == null)
                    return NotFound();

                existing.CategoryName = category.CategoryName;
                db.SaveChanges();
                return Ok("Category updated.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to update category."));
            }
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">ID of the category to delete.</param>
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteCategory(int id)
        {
            try
            {
                var category = db.Categories.Find(id);
                if (category == null)
                    return NotFound();

                db.Categories.Remove(category);
                db.SaveChanges();
                return Ok("Category deleted.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to delete category."));
            }
        }
    }
}
