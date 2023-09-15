using NUnit.Framework;
using RestService.Controllers;
using RestService.Data;
using RestService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace RestService_tests
{
    public class UserControllerTests
    {
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            // Initialize the controller to be tested
            _controller = new UserController();

            // Clear the DataStore for each test to ensure a fresh state
            DataStore.Users.Clear();
        }

        [Test]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var user1 = new User { UserId = Guid.NewGuid(), UserEmail = "test1@example.com", UserPassword = "pass1" };
            var user2 = new User { UserId = Guid.NewGuid(), UserEmail = "test2@example.com", UserPassword = "pass2" };
            DataStore.Users.Add(user1);
            DataStore.Users.Add(user2);

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var users = ((OkObjectResult)result).Value as List<User>;
            Assert.AreEqual(2, users.Count);
        }

        [Test]
        public void GetUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserEmail = "test@example.com", UserPassword = "pass" };
            DataStore.Users.Add(user);

            // Act
            var result = _controller.GetUser(user.UserId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var returnedUser = ((OkObjectResult)result).Value as User;
            Assert.AreEqual(user.UserId, returnedUser.UserId);
        }

        [Test]
        public void GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = _controller.GetUser(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void AddUser_ReturnsCreated_WhenDataIsValid()
        {
            // Arrange
            var user = new User { UserEmail = "test@example.com", UserPassword = "pass" };

            // Act
            var result = _controller.AddUser(user);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            Assert.AreEqual(1, DataStore.Users.Count);
        }

        [Test]
        public void AddUser_ReturnsBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var user = new User { UserEmail = "test@example.com" };  // Missing password

            // Act
            var result = _controller.AddUser(user);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(0, DataStore.Users.Count);
        }

        [Test]
        public void UpdateUser_ReturnsOk_WhenDataIsValidAndUserExists()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserEmail = "test@example.com", UserPassword = "pass" };
            DataStore.Users.Add(user);
            var updatedUser = new User { UserEmail = "updated@example.com", UserPassword = "updatedPass" };

            // Act
            var result = _controller.UpdateUser(user.UserId, updatedUser);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var storedUser = DataStore.Users.FirstOrDefault(u => u.UserId == user.UserId);
            Assert.AreEqual("updated@example.com", storedUser.UserEmail);
            Assert.AreEqual("updatedPass", storedUser.UserPassword);
        }

        [Test]
        public void UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updatedUser = new User { UserEmail = "updated@example.com", UserPassword = "updatedPass" };

            // Act
            var result = _controller.UpdateUser(userId, updatedUser);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void DeleteUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserEmail = "test@example.com", UserPassword = "pass" };
            DataStore.Users.Add(user);

            // Act
            var result = _controller.DeleteUser(user.UserId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            Assert.IsFalse(DataStore.Users.Any(u => u.UserId == user.UserId));
        }

        [Test]
        public void DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = _controller.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserEmail = "test@example.com", UserPassword = "pass" };
            DataStore.Users.Add(user);

            // Act
            var result = _controller.Login("test@example.com", "pass");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserEmail = "test@example.com", UserPassword = "pass" };
            DataStore.Users.Add(user);

            // Act
            var result = _controller.Login("test@example.com", "wrongPass");

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
    }
}
