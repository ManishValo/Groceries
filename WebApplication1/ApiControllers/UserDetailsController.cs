using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;
using System.Web.Http.Cors;
namespace WebApplication1.ApiControllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "SampleHeader")]
    [RoutePrefix("api/user")]
    public class UserDetailsController : ApiController
    {
        GroceryDBEntities db = new GroceryDBEntities();

        [HttpGet]
        [Route("get-all")]
        //get all user details
        public IHttpActionResult GetUserDetail()
        {
            List<UserDetail> user = db.UserDetails.ToList();
            return Ok(user);
        }

        [HttpPost]
        [Route("register")]
        //register user
        public IHttpActionResult PostRegisterUserDetail(UserDetail details)
        {
            try
            {
                var exist = db.UserDetails.FirstOrDefault(u => u.Email == details.Email);
                if (exist == null)
                {
                    db.UserDetails.Add(new UserDetail
                    {
                        Name = details.Name,
                        Email = details.Email,
                        Password = details.Password,
                        TypeId = details.TypeId,
                        // Other fields remain null
                    });

                    db.SaveChanges();
                    return Ok("Registered successfully");
                }
                else
                {
                    return BadRequest("User already exists with this email");
                }
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Server Error"));
            }
        }


        [HttpPost]
        [Route("login")]
        //login user 
        public IHttpActionResult PostLoginuserDetail(UserDetail user)
        {
            try {
                var logindata = db.UserDetails.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if (logindata != null)
                {
                    return Ok(new
                    {
                        UserID = logindata.UserID,
                        email = logindata.Email,
                        name = logindata.Name,
                        password = logindata.Password,
                        TypeID = logindata.TypeId
                    });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Server Error"));
            }

        }


        [HttpPut]
        [Route("update-contact/{id}")]
        public IHttpActionResult UpdateContactDetails(int id, [FromBody] UserDetail updatedDetails)
        {
            try
            {
                var user = db.UserDetails.FirstOrDefault(u => u.UserID == id);
                if (user == null)
                {
                    return NotFound();
                }

                user.MobileNo = updatedDetails.MobileNo;
                user.Address = updatedDetails.Address;
                user.City = updatedDetails.City;
                user.Pincode = updatedDetails.Pincode;

                db.SaveChanges();
                return Ok("Contact details updated successfully.");
            }
            catch (Exception)
            {
                return InternalServerError(new Exception("Failed to update user details."));
            }
        }


        //[HttpDelete]
        //[Route("delete/{id}")]
        //public IHttpActionResult DeleteUserDetail(int id)
        //{
        //    var user = db.UserDetails.Find(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    db.UserDetails.Remove(user);
        //    db.SaveChanges();
        //    return Ok("User deleted");
        //}

        //[HttpPost]
        //[Route("add")]
        //public IHttpActionResult AddUserDetail(UserDetail user)
        //{
        //    try
        //    {
        //        var exist = db.UserDetails.FirstOrDefault(u => u.Email == user.Email);
        //        if (exist == null)
        //        {
        //            db.UserDetails.Add(new UserDetail
        //            {
        //                Name = user.Name,
        //                Email = user.Email,
        //                Password = user.Password,
        //                TypeId = user.TypeId,
        //                MobileNo=user.MobileNo,
        //                Address=user.Address,
        //                City=user.City,
        //                Pincode=user.Pincode
        //            });

        //            db.SaveChanges();
        //            return Ok("Added successfully");
        //        }
        //        else
        //        {
        //            return BadRequest("User already exists with this email");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return InternalServerError(new Exception("Server Error"));
        //    }

        //}

    }
}

    
