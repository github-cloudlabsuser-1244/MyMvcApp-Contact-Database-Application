using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Controllers;
using MyMvcApp.Models;
using Xunit;

namespace MyMvcApp.Tests
{
    public class UserControllerTests
    {
        private List<User> GetTestUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new User { Id = 2, Name = "Jane Doe", Email = "jane@example.com" },
                new User { Id = 3, Name = "Alice Smith", Email = "alice@example.com" }
            };
        }
        
        [Fact]
        public void Search_ValidSearchString_ReturnsMatchingUsers()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            string searchString = "Doe";

            // Act
            var result = controller.Search(searchString) as ViewResult;
            var model = result.Model as List<User>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(2, model.Count);
            Assert.Contains(model, u => u.Name == "John Doe");
            Assert.Contains(model, u => u.Name == "Jane Doe");
        }

        [Fact]
        public void Search_EmptySearchString_ReturnsAllUsers()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            string searchString = "";

            // Act
            var result = controller.Search(searchString) as ViewResult;
            var model = result.Model as List<User>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Equal(3, model.Count);
        }

        [Fact]
        public void Search_NoMatchingUsers_ReturnsEmptyList()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            string searchString = "NonExistent";

            // Act
            var result = controller.Search(searchString) as ViewResult;
            var model = result.Model as List<User>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.Empty(model);
        }

        [Fact]
        public void Create_ValidUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            var newUser = new User { Name = "New User", Email = "newuser@example.com" };

            // Act
            var result = controller.Create(newUser);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Contains(newUser, userlist);
        }

        [Fact]
        public void Create_InvalidEmail_ReturnsViewResultWithModelError()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            var newUser = new User { Name = "New User", Email = "invalid-email" };

            // Act
            var result = controller.Create(newUser);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains("Email", controller.ModelState.Keys);
        }

        [Fact]
        public void Edit_ValidUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            var updatedUser = new User { Id = 1, Name = "Updated User", Email = "updated@example.com" };

            // Act
            var result = controller.Edit(1, updatedUser);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            var user = userlist.FirstOrDefault(u => u.Id == 1);
            Assert.Equal("Updated User", user.Name);
            Assert.Equal("updated@example.com", user.Email);
        }

        [Fact]
        public void Edit_InvalidEmail_ReturnsViewResultWithModelError()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;
            var updatedUser = new User { Id = 1, Name = "Updated User", Email = "invalid-email" };

            // Act
            var result = controller.Edit(1, updatedUser);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains("Email", controller.ModelState.Keys);
        }

        [Fact]
        public void Delete_ExistingUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;

            // Act
            var result = controller.Delete(1, null);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.DoesNotContain(userlist, u => u.Id == 1);
        }

        [Fact]
        public void Delete_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var userlist = GetTestUsers();
            var controller = new UserController();
            UserController.userlist = userlist;

            // Act
            var result = controller.Delete(99, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}