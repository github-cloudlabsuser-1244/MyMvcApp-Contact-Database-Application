using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using System.Text.RegularExpressions;

namespace MyMvcApp.Controllers;

public class UserController : Controller
{
    public static System.Collections.Generic.List<User> userlist = new System.Collections.Generic.List<User>();

        // GET: User
        public ActionResult Index()
        {
            return View(userlist);
        }

    // GET: User with search functionality
    public ActionResult Search(string searchString)
    {
        var users = from u in userlist
                    select u;

        if (!string.IsNullOrEmpty(searchString))
        {
            users = users.Where(u => u.Name.Contains(searchString) || u.Email.Contains(searchString));
        }

        ViewBag.CurrentFilter = searchString;

        return View("Index", users.ToList());
    }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            var user = userlist.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
            return NotFound();
            }
            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(User user)
        {
            // Validate name
            if (string.IsNullOrEmpty(user.Name))
            {
                ModelState.AddModelError("Name", "Name cannot be null or empty.");
            }

            // Validate email
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (string.IsNullOrEmpty(user.Email) || !emailRegex.IsMatch(user.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format... Please enter a valid email address.");
            }

            // Check if email already exists
            var existingUser = userlist.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", $"Email already exists for user {existingUser.Name}. Please enter a different email address.");
            }

            if (ModelState.IsValid)
            {
                user.Id = userlist.Any() ? userlist.Max(u => u.Id) + 1 : 1;
                userlist.Add(user);
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            var user = userlist.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            var existingUser = userlist.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound();
            }
                    
            // Validate name
            if (string.IsNullOrEmpty(user.Name))
            {
                ModelState.AddModelError("Name", "Name cannot be null or empty.");
            }
            
            // Validate email
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(user.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format. Please enter a valid email address.");
            }

            if (ModelState.IsValid)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            var user = userlist.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
            return NotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var user = userlist.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            userlist.Remove(user);
            return RedirectToAction(nameof(Index));
        }
}
