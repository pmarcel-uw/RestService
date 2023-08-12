using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using rest_service_project.Models;
using rest_service_project.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace rest_service_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET all users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(DataStore.Users);
        }

        // GET user by id
        [HttpGet("{id}")]
        public IActionResult GetUser(Guid id)
        {
            var user = DataStore.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST to add a user
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (string.IsNullOrEmpty(user.UserEmail) || string.IsNullOrEmpty(user.UserPassword))
                return BadRequest("Required fields are missing.");

            user.UserId = Guid.NewGuid();
            user.CreatedDate = DateTime.UtcNow;

            DataStore.Users.Add(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // PUT to update user
        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, User updatedUser)
        {
            var user = DataStore.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(updatedUser.UserEmail) || string.IsNullOrEmpty(updatedUser.UserPassword))
                return BadRequest("Required fields are missing.");

            user.UserEmail = updatedUser.UserEmail;
            user.UserPassword = updatedUser.UserPassword;

            return Ok(user);
        }

        // DELETE user by id
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = DataStore.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            DataStore.Users.Remove(user);
            return Ok();
        }

        // Custom Login Endpoint
        [HttpGet("login/{userEmail}/{userPassword}")]
        public IActionResult Login(string userEmail, string userPassword)
        {
            // Simple check, in real life, use hash comparison and stronger checks
            var user = DataStore.Users.FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == userPassword);
            if (user == null) return Unauthorized();

            // Return a dummy token. In real life, generate JWT or similar token.
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return Ok(token);
        }
    }
}
