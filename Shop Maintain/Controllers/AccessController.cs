using Microsoft.AspNetCore.Mvc;
using Shop_Maintain.Models;

namespace Shop_Maintain.Controllers
{
    public class AccessController : Controller
    {
        private readonly QlbanVaLiContext db;

        public AccessController(QlbanVaLiContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") ==  null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult Login(TUser user)
        {
            if(HttpContext.Session.GetString("UserName") == null)
            {
                var u = db.TUsers.Where(x => x.Username.Equals(user.Username) 
                                         && x.Password.Equals(user.Password)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("UserName", u.Username.ToString());
                    HttpContext.Session.SetInt32("UserType", u.LoaiUser ?? 0); // Thêm dòng này
                    
                    if (u.LoaiUser == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            TempData["ErrorMessage"] = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "Access");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SignUp(TUser user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                TempData["ErrorMessage"] = "Please enter username and password";
                return View(user);
            }

            try
            {
                // Check if username exists
                var existingUser = db.TUsers.FirstOrDefault(x => x.Username == user.Username);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "Username already exists";
                    return View(user);
                }

                // Create new user
                var newUser = new TUser
                {
                    Username = user.Username,
                    Password = user.Password,
                    LoaiUser = 0,  // Default user type
                };

                // Add to database
                db.TUsers.Add(newUser);
                var result = db.SaveChanges();

                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to save user";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Error in SignUp: {ex.Message}");
                TempData["ErrorMessage"] = "Registration failed: " + ex.Message;
                return View(user);
            }
        }
    }
}
