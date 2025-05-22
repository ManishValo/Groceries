using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Models;

namespace WebApplication1.ApiControllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        GroceryDBEntities db = new GroceryDBEntities();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllCategories()
        {
            var categories = db.Categories.ToList();
            return Ok(categories);
        }

        //[HttpGet]
        //[Route("{id:int}")]
        //public IHttpActionResult GetCategoryById(int id)
        //{
        //    var category = db.Categories.Find(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(category);
        //}

        [HttpPost]
        [Route("add")]
        public IHttpActionResult RegisterCategory(Category category)
        {
            try
            {
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

        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateCategory(Category category)
        {
            var existing = db.Categories.Find(category.CategoryID);
            if (existing == null)
                return NotFound();

            existing.CategoryName = category.CategoryName;
            db.SaveChanges();
            return Ok("Category updated.");
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteCategory(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return NotFound();

            db.Categories.Remove(category);
            db.SaveChanges();
            return Ok("Category deleted.");
        }
    }
}
