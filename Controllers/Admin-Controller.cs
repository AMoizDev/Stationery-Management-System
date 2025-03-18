using DocumentFormat.OpenXml.InkML;
using System.Dynamic;
using Fingers10.ExcelExport.ActionResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Stationery_Management_System.db_context;
using Stationery_Management_System.Models;
using System.Net.Mail;
using System.Net;
using DocumentFormat.OpenXml.Spreadsheet;
using NuGet.Versioning;

namespace Stationery_Management_System.Controllers
{
    public class Admin_Controller : Controller
    {

        sqldb db;
        IWebHostEnvironment env;

        public Admin_Controller(sqldb db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }



        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Name") != null)
            {
                return RedirectToAction("Index", HttpContext.Session.GetString("Role") + "_");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(users us, string UserEmail, string UserPassword)
        {
            var user = db.users.FirstOrDefault(x => x.UserEmail == UserEmail);

            if (user == null || user.UserPassword != UserPassword)
            {
                ViewBag.loginerror = "Invalid User Email or Incorrect Password";
                return View();
            }

            HttpContext.Session.SetInt32("Id", user.userId);
            HttpContext.Session.SetString("Name", user.UserName);
            HttpContext.Session.SetInt32("Userlimits", user.UserLimits);
            HttpContext.Session.SetInt32("Role", user.UserRole);
            HttpContext.Session.SetInt32("SuperiorId", Convert.ToInt32(user.Add_By));

            if (user is not null)
            {

                var role = HttpContext.Session.GetString("Role");
                TempData["roles"] = role;
                return RedirectToAction("Index", "Admin_");
            }

            ViewBag.loginerror = "Invalid User Role";
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Login");

        }


        public IActionResult Index()
        {

            Console.WriteLine("✅ Index() method is running..."); // Debugging log

            var userName = HttpContext.Session.GetString("Name");
            var userLimits = HttpContext.Session.GetInt32("Userlimits");
            var userRoleId = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName != null)
            {
                if (userRoleId == 1)
                {
                    ViewBag.username = userName;
                    ViewBag.userlimit = userLimits;
                    ViewBag.userRoleId = userRoleId;



                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRoleId)?.UserRoleName;
                    //var userlimit = db.users.FirstOrDefault(r => r.uesrId == userId)?.UserLimits;


                    TempData["userroles"] = userRoleName;
                    //TempData["userlimits"] = userlimit;

                    var recentlyAddedStock = db.Stationeries
     .OrderByDescending(s => s.Stationery_Id) // Sorting by latest added ID
     .Take(7)
     .Select(s => new
     {
         s.Stationery_Name,
         s.Stationery_Quantity,
         s.Stationery_Price,
                            s.Stationery_Image

     })
     .ToList();


                    // 2️⃣ Fetch Low Stock Items (Jinki quantity 10 se kam hai)
                    var lowStockItems = db.Stationeries
                        .Where(s => s.Stationery_Quantity < 10)
                        .Select(s => new
                        {
                            s.Stationery_Name,
                            s.Stationery_Quantity,
                            s.Stationery_Price,
                            s.Stationery_Image
                        })
                        .ToList();


                    var totalusers = db.users.Count();
                    var AvailableStock = db.Stationeries.Sum(s => s.Stationery_Quantity);
                    var StationeryUsage = db.Requests
       .Where(r => r.status.ToLower() == "accepted")
       .Sum(r => r.quantity);
                    var TotalPurchaseBills = db.Requests
     .Where(r => r.status.ToLower() == "accepted")
     .Sum(r => r.amount);


                    // Fetch total purchase amount for this user
                    var totalPurchaseAmount = db.Requests
                        .Where(r => r.userId == userId)
                        .Sum(r => r.amount);

                    // Fetch total requests count for this user
                    var totalRequests = db.Requests.Count();

                    // Fetch pending requests count for this user
                    var pendingRequests = db.Requests
                        .Count(r => r.status.ToLower() == "pending");

                    // Send values to View
                    ViewBag.TotalPurchaseAmount = totalPurchaseAmount;
                    ViewBag.TotalRequests = totalRequests;
                    ViewBag.PendingRequests = pendingRequests;
                    ViewBag.totalusers = totalusers;
                    ViewBag.AvailableStock = AvailableStock;
                    ViewBag.StationeryUsage = StationeryUsage;
                    ViewBag.TotalPurchaseBills = TotalPurchaseBills;
                    ViewBag.RecentlyAddedStock = recentlyAddedStock;
                    ViewBag.lowStockItems = lowStockItems;


                    return View();
                }
                if (userRoleId == 2)
                {
                    ViewBag.username = userName;
                    ViewBag.userRoleId = userRoleId;


                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRoleId)?.UserRoleName;


                    TempData["userroles"] = userRoleName;


                    var recentlyAddedStock = db.Stationeries
    .Where(s => s.Assign_to == 2 || s.Assign_to == null) // Sorting by latest added ID
    .Take(7)
    .Select(s => new
    {
        s.Stationery_Name,
        s.Stationery_Quantity,
        s.Stationery_Price,
        s.Stationery_Image

    })
    .ToList();

                    var lowStockItems = db.Stationeries
     .Where(s => s.Stationery_Quantity < 10 &&
                (s.Assign_to == 2 || s.Assign_to == null))
     .OrderBy(s => s.Stationery_Quantity) // Sabse kam stock pehle aaye
     .Select(s => new
     {
         s.Stationery_Name,
         s.Stationery_Quantity,
         s.Stationery_Price,
         s.Stationery_Image
     })
     .ToList();



                    var totalusers = db.users.Count();
                    var AvailableStock = db.Stationeries.Sum(s => s.Stationery_Quantity);
                    var StationeryUsage = db.Requests



       .Where(r => r.status.ToLower() == "accepted")
       .Sum(r => r.quantity);
                    var TotalPurchaseBills = db.Requests
     .Where(r => r.status.ToLower() == "accepted")
     .Sum(r => r.amount);

                    // Fetch total purchase amount for this user
                    var totalPurchaseAmount = db.Requests
                         .Where(r => r.userId == userId && r.status.ToLower() == "accepted")
                        .Sum(r => r.amount);

                    // Apni aur apni team ki total requests count
                    var totalRequests = db.Requests.Count(r => r.userId == userId || r.superior_id == userId);
                    ViewBag.TotalRequests = totalRequests;

                    // Apni aur apni team ki pending requests count
                    var pendingRequests = db.Requests.Count(r => (r.userId == userId || r.superior_id == userId) && r.status.ToLower() == "pending");
                    ViewBag.PendingRequests = pendingRequests;

                    // Send values to View
                    ViewBag.TotalPurchaseAmount = totalPurchaseAmount;
                    ViewBag.TotalRequests = totalRequests;
                    ViewBag.PendingRequests = pendingRequests;
                    ViewBag.totalusers = totalusers;
                    ViewBag.AvailableStock = AvailableStock;
                    ViewBag.StationeryUsage = StationeryUsage;
                    ViewBag.TotalPurchaseBills = TotalPurchaseBills;
                    ViewBag.recentlyAddedStock = recentlyAddedStock;
                    ViewBag.lowStockItems = lowStockItems;


                    return View();
                }
                if (userRoleId == 3)
                {
                    ViewBag.username = userName;


                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRoleId)?.UserRoleName;


                    TempData["userroles"] = userRoleName;




                    var recentlyAddedStock = db.Stationeries
    .Where(s => s.Assign_to == 3 || s.Assign_to == null) // Sorting by latest added ID
    .Take(7)
    .Select(s => new
    {
        s.Stationery_Name,
        s.Stationery_Quantity,
        s.Stationery_Price,
        s.Stationery_Image

    })
    .ToList();

                    var lowStockItems = db.Stationeries
     .Where(s => s.Stationery_Quantity < 10 &&
                (s.Assign_to == 3 || s.Assign_to == null))
     .OrderBy(s => s.Stationery_Quantity) // Sabse kam stock pehle aaye
     .Select(s => new
     {
         s.Stationery_Name,
         s.Stationery_Quantity,
         s.Stationery_Price,
         s.Stationery_Image
     })
     .ToList();



                    // Fetch total purchase amount for this user
                    var totalPurchaseAmount = db.Requests
                         .Where(r => r.userId == userId && r.status.ToLower() == "accepted")
                        .Sum(r => r.amount);

                    // Fetch total requests count for this user
                    var totalRequests = db.Requests
                        .Count(r => r.userId == userId);

                    // Fetch pending requests count for this user
                    var pendingRequests = db.Requests
                        .Count(r => r.userId == userId && r.status.ToLower() == "pending");

                    // Send values to View
                    ViewBag.TotalPurchaseAmount = totalPurchaseAmount;
                    ViewBag.TotalRequests = totalRequests;
                    ViewBag.PendingRequests = pendingRequests;
                    ViewBag.recentlyAddedStock = recentlyAddedStock;
                    ViewBag.lowStockItems = lowStockItems;


                    return View();
                }
                if (userRoleId == 4)
                {
                    ViewBag.username = userName;


                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRoleId)?.UserRoleName;


                    TempData["userroles"] = userRoleName;


                    var recentlyAddedStock = db.Stationeries
.Where(s => s.Assign_to == 4 || s.Assign_to == null) // Sorting by latest added ID
.Take(7)
.Select(s => new
{
    s.Stationery_Name,
    s.Stationery_Quantity,
    s.Stationery_Price,
    s.Stationery_Image

})
.ToList();

                    var lowStockItems = db.Stationeries
     .Where(s => s.Stationery_Quantity < 10 &&
                (s.Assign_to == 4 || s.Assign_to == null))
     .OrderBy(s => s.Stationery_Quantity) // Sabse kam stock pehle aaye
     .Select(s => new
     {
         s.Stationery_Name,
         s.Stationery_Quantity,
         s.Stationery_Price,
         s.Stationery_Image
     })
     .ToList();



                    // Fetch total purchase amount for this user
                    var totalPurchaseAmount = db.Requests
                         .Where(r => r.userId == userId && r.status.ToLower() == "accepted")
                        .Sum(r => r.amount);

                    // Fetch total requests count for this user
                    var totalRequests = db.Requests
                        .Count(r => r.userId == userId);

                    // Fetch pending requests count for this user
                    var pendingRequests = db.Requests
                        .Count(r => r.userId == userId && r.status.ToLower() == "pending");

                    // Send values to View
                    ViewBag.TotalPurchaseAmount = totalPurchaseAmount;
                    ViewBag.TotalRequests = totalRequests;
                    ViewBag.PendingRequests = pendingRequests;
                    ViewBag.recentlyAddedStock = recentlyAddedStock;
                    ViewBag.lowStockItems = lowStockItems;


                    return View();
                }
                if (userRoleId == 5)
                {
                    ViewBag.username = userName;


                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRoleId)?.UserRoleName;


                    TempData["userroles"] = userRoleName;

                    var recentlyAddedStock = db.Stationeries
.Where(s => s.Assign_to == 5 || s.Assign_to == null) // Sorting by latest added ID
.Take(7)
.Select(s => new
{
   s.Stationery_Name,
   s.Stationery_Quantity,
   s.Stationery_Price,
   s.Stationery_Image

})
.ToList();

                    var lowStockItems = db.Stationeries
     .Where(s => s.Stationery_Quantity < 10 &&
                (s.Assign_to == 5 || s.Assign_to == null))
     .OrderBy(s => s.Stationery_Quantity) // Sabse kam stock pehle aaye
     .Select(s => new
     {
         s.Stationery_Name,
         s.Stationery_Quantity,
         s.Stationery_Price,
         s.Stationery_Image
     })
     .ToList();

                    // Fetch total purchase amount for this user
                    var totalPurchaseAmount = db.Requests
                         .Where(r => r.userId == userId && r.status.ToLower() == "accepted")
                        .Sum(r => r.amount);

                    // Fetch total requests count for this user
                    var totalRequests = db.Requests
                        .Count(r => r.userId == userId);

                    // Fetch pending requests count for this user
                    var pendingRequests = db.Requests
                        .Count(r => r.userId == userId && r.status.ToLower() == "pending");

                    // Send values to View
                    ViewBag.TotalPurchaseAmount = totalPurchaseAmount;
                    ViewBag.TotalRequests = totalRequests;
                    ViewBag.PendingRequests = pendingRequests;
                    ViewBag.recentlyAddedStock = recentlyAddedStock;
                    ViewBag.lowStockItems = lowStockItems;



                    return View();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult Addroles()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            // claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;
                    ViewBag.username = userName;
                    TempData["userroles"] = userRoleName;
                    return View();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]
        public IActionResult Addroles(UserRoles us)
        {


            db.UserRoles.Add(us);
            db.SaveChanges();
            ModelState.Clear();
            TempData["Addusers"] = "user added successfully";
            return RedirectToAction("roles");
        }


        public IActionResult roles()
        {

            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    //ViewBag.userrole = userRole;
                    return View(db.UserRoles.ToList());
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        public IActionResult Addusers()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id"); // Logged-in user's ID

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    ViewBag.UserRolesList = db.UserRoles.ToList();
                    return View();
                }
                if (userRole == 2)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    //ViewBag.UserRolesList = db.UserRoles.ToList();
                    ViewBag.UserRolesList = db.UserRoles.Where(r => r.UserRoleId > 2).ToList();


                    return View();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]
        public IActionResult Addusers(users us)
        {
            var userId = HttpContext.Session.GetInt32("Id");
            us.Add_By = userId;
            us.UserPassword = "123456789";
            db.users.Add(us);
            db.SaveChanges();
            ModelState.Clear();
            TempData["Addusers"] = "user added successfully";
            return RedirectToAction("users");
        }


        public IActionResult user_Edit(int id)
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id"); // Current logged-in user's ID

            // Clear login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            var user = db.users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserRolesList = db.UserRoles?.ToList() ?? new List<UserRoles>(); // ✅ Null handling fix

            // ✅ Admin can edit any user
            if (userRole == 1)
            {
                var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                TempData["userroles"] = userRoleName;
                ViewBag.username = userName;
                ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);
                if (user is not null)
                {
                    HttpContext.Session.SetString("userRoleid", user.Add_By.ToString());
                }
                return View(user);
            }

            // ✅ Manager can only edit users added by him
            if (userRole == 2 && user.Add_By == userId)
            {
                ViewBag.UserRolesList = db.UserRoles.Where(r => r.UserRoleId > 2).ToList();
                var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                TempData["userroles"] = userRoleName;
                ViewBag.username = userName;
                ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);

                if (user is not null)
                {
                    HttpContext.Session.SetString("userRoleid", user.Add_By.ToString());
                }
                return View(user);
            }

            // ❌ Unauthorized users get redirected
            return RedirectToAction("Unauthorized", "Home");
        }


        [HttpPost]
        public IActionResult user_Edit(users us)
        {
            //var userId = HttpContext.Session.GetInt32("Id");
            var addby = HttpContext.Session.GetString("userRoleid");
            us.Add_By = Convert.ToInt32(addby);
            db.users.Update(us);
            db.SaveChanges();
            ModelState.Clear();
            TempData["user_Edit"] = "User Updated successfully";
            return RedirectToAction("users");
        }

        public IActionResult user_Delete(int id)
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    var user = db.users.Find(id);
                    ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);
                    return View(user);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]
        public IActionResult user_Delete(users us)
        {
            var userToDelete = db.users.Find(us.userId);

            if (userToDelete == null)
            {
                return NotFound();
            }

            if (userToDelete.UserRole.ToString() == "Admin")
            {
                TempData["user_Delete"] = "Admin cannot be deleted!";
                return RedirectToAction("users");
            }

            db.users.Remove(userToDelete);
            db.SaveChanges();
            ModelState.Clear();
            TempData["user_Delete"] = "User Deleted successfully";
            return RedirectToAction("users");
        }


        public IActionResult user_Detail(int id)
        {

            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole  < 3)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    var user = db.users.Find(id);
                    ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);

                    return View(user);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]
        public IActionResult user_Detail(users us)
        {
            db.users.Find(us);
            db.SaveChanges();
            ModelState.Clear();
            return RedirectToAction("users");
        }

        public IActionResult users()
        {

            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id"); // Logged-in user's ID

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;

                    ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);
                    //var userList = db.users.Where(u => u.Add_By == userId).ToList();


                    //ViewBag.userrole = userRole;
                    return View(db.users.ToList());

                }
                if (userRole == 2)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;

                    ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);
                    var userList = db.users.Where(u => u.Add_By == userId).Include(u => u.AddedBy).ToList();




                    ViewBag.userrole = userRole;
                    return View(userList);

                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }




        public IActionResult Add_stationery()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    ViewBag.UserRolesList = db.UserRoles.ToList();
                    return View();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]

        public IActionResult Add_stationery(Stationery st, IFormFile Stationery_Image)
        {
            var userRole = HttpContext.Session.GetString("Role");

            string fileName = "";


            string location = Path.Combine(env.WebRootPath, "stationeyImages");

            // 🛠️ Fix: Folder exist nahi karta to create karo
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            fileName = Guid.NewGuid().ToString() + "_" + Stationery_Image.FileName;
            string filePath = Path.Combine(location, fileName);

            // 🛠️ Fix: FileStream ko properly handle karo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Stationery_Image.CopyTo(stream);
            }


            st.Stationery_Image = fileName;
            db.Stationeries.Add(st);
            db.SaveChanges();
            ModelState.Clear();
            TempData["Addstationery"] = "stationery added successfully";
            return RedirectToAction("stationery");
        }


        public IActionResult Edit(int id)
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    var stationery = db.Stationeries.Find(id);
                    ViewBag.UserRolesList = db.UserRoles.ToList();



                    if (stationery is not null)
                    {
                        HttpContext.Session.SetString("StationaryRoleId", stationery.Assign_to.ToString());
                    }

                    return View(stationery);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        //[HttpPost]
        //public IActionResult Edit(Stationery st)
        //{
        //    var stationaryRole = HttpContext.Session.GetString("StationaryRoleId");
        //    st.Assign_to = Convert.ToInt32(stationaryRole);
        //    db.Stationeries.Update(st);
        //    db.SaveChanges();
        //    ModelState.Clear();
        //    TempData["stat_Edit"] = "stationery Updated successfully";
        //    return RedirectToAction("stationery");
        //}

        
        [HttpPost]
        public IActionResult Edit(Stationery st, IFormFile? Stationery_Image)
        {
            var existingStationery = db.Stationeries.Find(st.Stationery_Id);
            if (existingStationery == null)
            {
                return NotFound();
            }

            // ✅ Assign_to should be updated from form, not session
            existingStationery.Assign_to = st.Assign_to;

            // ✅ Update other fields
            existingStationery.Stationery_Name = st.Stationery_Name;
            existingStationery.Stationery_Price = st.Stationery_Price;
            existingStationery.Stationery_Quantity = st.Stationery_Quantity;
            existingStationery.Stationery_Description = st.Stationery_Description;

            string location = Path.Combine(env.WebRootPath, "stationeyImages");

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            if (Stationery_Image != null && Stationery_Image.Length > 0)
            {
                // ✅ Delete old image
                if (!string.IsNullOrEmpty(existingStationery.Stationery_Image))
                {
                    string oldImagePath = Path.Combine(location, existingStationery.Stationery_Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // ✅ Save new image
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Stationery_Image.FileName);
                string filePath = Path.Combine(location, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Stationery_Image.CopyTo(stream);
                }

                existingStationery.Stationery_Image = fileName;
            }

            db.Stationeries.Update(existingStationery);
            db.SaveChanges();

            TempData["stat_Edit"] = "Stationery updated successfully";
            return RedirectToAction("stationery");
        }


        public IActionResult Delete(int id)
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    var stationery = db.Stationeries.Find(id);
                    return View(stationery);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            try
            {
                var stationery = db.Stationeries.Find(id);

                if (stationery == null)
                {
                    TempData["error"] = "Stationery not found!";
                    return RedirectToAction("stationery");
                }

                // ✅ Step 1: Find all requests related to this stationery and delete them
                var relatedRequests = db.Requests.Where(r => r.stationaryId == id).ToList();
                db.Requests.RemoveRange(relatedRequests);

                // ✅ Step 2: Now, delete the stationery
                db.Stationeries.Remove(stationery);
                db.SaveChanges();

                TempData["stat_Delete"] = "Stationery and related requests deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while deleting: " + ex.Message;
            }

            return RedirectToAction("stationery");
        }



        public IActionResult Detail(int id)
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");


            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    var stationery = db.Stationeries.Find(id);
                    return View(stationery);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }

        [HttpPost]
        public IActionResult Detail(Stationery st)
        {
            db.Stationeries.Find(st);
            db.SaveChanges();

            return RedirectToAction("stationery");
        }

        public IActionResult stationery()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            //claer login sessions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                if (userRole == 1)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                    TempData["userroles"] = userRoleName;
                    ViewBag.username = userName;
                    ViewBag.RolesDictionary = db.UserRoles.ToDictionary(r => r.UserRoleId, r => r.UserRoleName);
                    return View(db.Stationeries.ToList());

                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            };
        }





        public IActionResult All_stationery()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");

            // Clear login sessions to prevent caching issues
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userName is not null)
            {
                var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                TempData["userroles"] = userRoleName;
                ViewBag.username = userName;

                List<Stationery> stationeries = new List<Stationery>();

                if (userRole == 1) // Admin: Sab kuch dekh sakta hai
                {
                    stationeries = db.Stationeries.ToList();
                }
                else if (userRole == 2) // Manager: Sirf manager ki assigned stationery
                {
                    stationeries = db.Stationeries
                        .Where(s => s.Assign_to == 2 || s.Assign_to == null) // Manager ya "All"
                        .ToList();
                }
                else if (userRole == 3) // Assistant Manager: Sirf Assistant Manager ki assigned stationery
                {
                    stationeries = db.Stationeries
                        .Where(s => s.Assign_to == 3 || s.Assign_to == null) // Assistant Manager ya "All"
                        .ToList();
                }
                else if (userRole == 4) // Engineer: Sirf Engineer ki assigned stationery
                {
                    stationeries = db.Stationeries
                        .Where(s => s.Assign_to == 4 || s.Assign_to == null) // Engineer ya "All"
                        .ToList();
                }
                else if (userRole == 5) // Employee: Sirf Employee ki assigned stationery
                {
                    stationeries = db.Stationeries
                        .Where(s => s.Assign_to == 5 || s.Assign_to == null) // Employee ya "All"
                        .ToList();
                }
                else // Agar koi aur role ho jo defined nahi hai
                {
                    return NotFound();
                }

                return View(stationeries);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        public IActionResult help()
        {

            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            // Clear login sessions to prevent caching issues
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userId is not null)
            {
                var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                TempData["userroles"] = userRoleName;
                ViewBag.username = userName;
                return View();
            
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        public async Task<IActionResult> GetStationeryReport()

        {
            var Date_time = DateTime.Today;
            var results = db.Stationeries.ToList();
            return new ExcelResult<Stationery>(results, "Stationery List", "Stationeries Report" + Date_time);
        }







        public IActionResult Profile()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userId = HttpContext.Session.GetInt32("Id");
            var userRole = HttpContext.Session.GetInt32("Role");

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = db.users.Find(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

            TempData["userroles"] = userRoleName;
            ViewBag.username = userName;

            //return View(user);

            if (userRole == 1)
            {
                return View(user);

            }
            if (userRole == 2)
            {
                return View(user);

            }
            if (userRole == 3)
            {
                return View(user);

            }
            if (userRole == 4)
            {
                return View(user);

            }
            if (userRole == 5)
            {
                return View(user);

            }
            else
            {
                return NotFound();
            }

        }



        [HttpPost]
        public IActionResult Profile(users user)
        {
            var existingUser = db.users.Find(user.userId);

            if (existingUser == null)
            {
                return NotFound();
            }

            List<string> changes = new List<string>();

            if (existingUser.UserName != user.UserName)
            {
                changes.Add($"Name changed from '{existingUser.UserName}' to '{user.UserName}'");
            }

            if (existingUser.UserEmail != user.UserEmail)
            {
                changes.Add($"Email changed from '{existingUser.UserEmail}' to '{user.UserEmail}'");
            }

            if (existingUser.UserPhone != user.UserPhone)
            {
                changes.Add($"Phone number changed from '{existingUser.UserPhone}' to '{user.UserPhone}'");
            }

            if (existingUser.UserPassword != user.UserPassword)
            {
                changes.Add($"password changed from '{existingUser.UserPassword}' to '{user.UserPassword}'");
            }


            existingUser.UserName = user.UserName;
            existingUser.UserEmail = user.UserEmail;
            existingUser.UserPhone = user.UserPhone;
            existingUser.UserPassword = user.UserPassword;

            db.SaveChanges();

            if (changes.Count > 0)
            {
                string message = $"User {existingUser.UserName} updated profile: {string.Join(", ", changes)}";
                AddNotification(message);
            }
            TempData["ProfileUpdate"] = "Profile updated successfully";
            return RedirectToAction("Profile");
        }







        public IActionResult Requests()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (string.IsNullOrEmpty(userName) || userId == null)
            {
                return RedirectToAction("Login");
            }

            List<Request> requests = new List<Request>();

            if (userRole == 1)
            {
                requests = db.Requests.Include(x => x.Stationery).Include(x=>x.Users).ToList();
            }

            else
            {
                requests = db.Requests
                    .Include(x => x.Stationery).Include(x => x.Users)
                    .Where(r => r.userId == userId.Value)
                    .ToList();
            }

            var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

            TempData["userroles"] = userRoleName;
            ViewBag.username = userName;
            return View(requests);
        }

        public IActionResult TeamRequests()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";



            if (string.IsNullOrEmpty(userName) || userId == null)
            {
                return RedirectToAction("Login");
            }

            List<Request> requests = new List<Request>();


            if (userRole == 2) // Admin: Sabki requests dekh sakta hai
            {
                requests = db.Requests
                              .Include(x => x.Stationery)
                              .Where(r => r.superior_id == userId.Value)
                              .ToList();
            }
            else
            {
                requests = db.Requests
                    .Include(x => x.Stationery)
                    .Where(r => r.userId == userId.Value)
                    .ToList();
            }

            var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;
            TempData["userroles"] = userRoleName;

            ViewBag.username = userName;
            return View(requests);
        }



        public IActionResult CreateRequest()
        {
            var userid = HttpContext.Session.GetInt32("Id");

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userid is not null)
            {
                var stationary = db.Stationeries.ToList();
                ViewBag.Stationary = stationary;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public IActionResult CreateRequest(int stationaryId, int quantity, int amount)
        {
            var userid = HttpContext.Session.GetInt32("Id");

            if (userid is null)
                return Json(new { success = false, message = "User not found!" });

            var user = db.users.FirstOrDefault(x => x.userId == userid);
            var stationery = db.Stationeries.FirstOrDefault(x => x.Stationery_Id == stationaryId);


            if (user is null)
                return Json(new { success = false, message = "User not found!" });

            if (user.UserLimits < amount)
                return Json(new { success = false, message = "Insufficient balance!" });

            var superiorId = user.Add_By ?? userid;

            Request req = new Request
            {
                userId = userid.Value,
                stationaryId = stationaryId,
                quantity = quantity,
                amount = amount,
                status = "Pending",
                superior_id = superiorId.Value
            };

            var number = 0;

            if (req.status == "Accepted") {

                stationery.Stationery_Quantity -= quantity;
                user.UserLimits -= amount;
                db.Requests.Add(req);
            }
            else
            {
                stationery.Stationery_Quantity -= number;
                user.UserLimits -= number;
                db.Requests.Add(req);

            }


            Notification adminNotification = new Notification
            {
                Message = $"{user.UserName} requested {quantity} items for {amount} amount.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                UserId = null,
            };
            db.Notifications.Add(adminNotification);


            if (superiorId.Value != userid.Value)
            {
                Notification superiorNotification = new Notification
                {
                    Message = $"New request from {user.UserName} for {quantity} items.",
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    UserId = superiorId.Value
                };
                db.Notifications.Add(superiorNotification);
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Request submitted successfully!" });
        }


        [HttpPost]
        public IActionResult WithdrawRequest(int requestId)
        {


            try
            {
                var request = db.Requests.FirstOrDefault(r => r.requestId == requestId);

                if (request == null)
                    return Json(new { success = false, message = "Request not found!" });

                if (request.status != "Pending")
                    return Json(new { success = false, message = "Request cannot be withdrawn!" });

                // ✅ Update request status
                request.status = "Withdrawn";
                db.SaveChanges();



                Notification adminNotification = new Notification
                {
                    Message = $"Request {requestId} has been withdrawn by the user.",
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    UserId = null
                };
                db.Notifications.Add(adminNotification);
                db.SaveChanges();

                return Json(new { success = true, message = "Request withdrawn successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }




        [HttpPost]
        public IActionResult UpdateRequestStatus(int requestId, string status)
        {
            try
            {
                var request = db.Requests.FirstOrDefault(r => r.requestId == requestId);
                if (request == null)
                    return Json(new { success = false, message = "Request not found!" });

                var user = db.users.FirstOrDefault(x => x.userId == request.userId);
                var stationery = db.Stationeries.FirstOrDefault(x => x.Stationery_Id == request.stationaryId);

                if (user == null || stationery == null)
                    return Json(new { success = false, message = "User or Stationery not found!" });

                request.status = status;


                if (status == "Accepted")
                {
                    if (user.UserLimits < request.amount)
                        return Json(new { success = false, message = "Insufficient balance!" });

                    if (stationery.Stationery_Quantity < request.quantity)
                        return Json(new { success = false, message = "Not enough stationery available!" });

                    user.UserLimits -= request.amount;
                    stationery.Stationery_Quantity -= request.quantity;
                }

                db.SaveChanges();



                Notification userNotification = new Notification
                {
                    Message = $"request for {request.quantity} items has been {status}.",
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    UserId = request.userId
                };
                db.Notifications.Add(userNotification);

                db.SaveChanges();

                return Json(new { success = true, message = "Request status updated to " + status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }




        public IActionResult GetNotifications()
        {
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            if (userId == null)
                return Json(new { success = false, message = "User not found!" });

            List<Notification> notifications;

            if (userRole == 1) // ✅ Admin Case (Sirf Profile Updates & Requests)
            {
                notifications = db.Notifications
                                  .Where(n => n.UserId == null || n.Message.Contains("requested") || n.Message.Contains("updated profile") || n.Message.Contains("withdraw"))
                                  .OrderByDescending(n => n.CreatedAt)
                                  .Take(5)
                                  .ToList();
            }
            else // ✅ Normal User Case (Sirf Apni Notifications)
            {
                notifications = db.Notifications
                                  .Where(n => n.UserId == userId)
                                  .OrderByDescending(n => n.CreatedAt)
                                  .Take(5)
                                  .ToList();
            }

            int unreadCount = notifications.Count(n => !n.IsRead);
            return Json(new { notifications, unreadCount });
        }


        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            var notifications = db.Notifications.Where(n => !n.IsRead).ToList();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            db.SaveChanges();
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult ClearAllNotifications()
        {
            var userId = HttpContext.Session.GetInt32("Id"); // Get logged-in user ID

            if (userId == null)
                return Json(new { success = false, message = "User not found!" });

            var userNotifications = db.Notifications.Where(n => n.UserId == userId);
            db.Notifications.RemoveRange(userNotifications);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteSingleNotification(int id)
        {


            if (id == 0)
            {
                return Json(new { success = false, message = "Invalid ID!" });
            }

            var notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return Json(new { success = false, message = "Notification not found!" });
            }

            db.Notifications.Remove(notification);
            db.SaveChanges();

            return Json(new { success = true });
        }






        public IActionResult AllNotifications()
        {
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");
            var userName = HttpContext.Session.GetString("Name");



            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";


        
            if (userId is not null)
            {
                if (userRole == 1) // ✅ Admin Case (Sirf Profile Updates & Requests)
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;
                    TempData["userroles"] = userRoleName;

                    ViewBag.username = userName;
                    var allNotifications = db.Notifications
                                      .Where(n => n.UserId == null || n.Message.Contains("requested") || n.Message.Contains("updated profile") || n.Message.Contains("withdraw"))
                                      .OrderByDescending(n => n.CreatedAt)
                                      .Take(5)
                                      .ToList();
                    return View(allNotifications);

                }
                else
                {
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;
                    TempData["userroles"] = userRoleName;

                    ViewBag.username = userName;
                    var allNotifications = db.Notifications
                                    .Where(n => n.UserId == userId)
                                    .OrderByDescending(n => n.CreatedAt)
                                    .Take(5)
                                    .ToList();
                    return View(allNotifications);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }


            
         

        }


        public void AddNotification(string message, int? userId = null)
        {
            var notification = new Notification
            {
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false,
                UserId = userId
            };

            db.Notifications.Add(notification);
            db.SaveChanges();
        }


        public IActionResult StationeryStockReport()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            // Clear login sessions to prevent caching issues
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userId is not null)
            {

                if(userRole < 3) { 
                var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                TempData["userroles"] = userRoleName;
                ViewBag.username = userName;

                var stockData = db.Stationeries
     .Select(s => new
     {
         StationeryID = s.Stationery_Id,
         ItemName = s.Stationery_Name,
         TotalAdded = db.Stationeries
                       .Where(e => e.Stationery_Id == s.Stationery_Id)
                       .Sum(e => (int?)e.Stationery_Quantity) ?? 0,
         TotalUsed = db.Requests
                       .Where(r => r.stationaryId == s.Stationery_Id && r.status == "Accepted")
                       .Sum(r => (int?)r.quantity) ?? 0,
     })
     .ToList()
     .Select(s => new
     {
         StationeryID = s.StationeryID,
         ItemName = s.ItemName,
         TotalAdded = s.TotalAdded,
         TotalUsed = s.TotalUsed,
         RemainingStock = Math.Max(0, s.TotalAdded - s.TotalUsed)  // ❌ Negative values ko zero kar diya
     })
     .ToList();

                ViewBag.StockData = stockData;
                return View();

                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return RedirectToAction("Login");
            }


           }



        public IActionResult StationeryUsageReport()
        {
            var userName = HttpContext.Session.GetString("Name");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Id");

            // Clear login sessions to prevent caching issues
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (userId is not null)
            {
                if (userRole < 3)
                { 
                    var userRoleName = db.UserRoles.FirstOrDefault(r => r.UserRoleId == userRole)?.UserRoleName;

                TempData["userroles"] = userRoleName;
                ViewBag.username = userName;


                var reportData = db.Requests
               .Where(r => r.status == "Accepted") // ✅ Sirf Accepted Requests
               .GroupBy(r => r.stationaryId)
               .Select(g => new
               {
                   ItemName = g.First().Stationery.Stationery_Name,
                   TotalUsed = g.Sum(r => r.quantity),
                   TotalCost = g.Sum(r => r.quantity * r.Stationery.Stationery_Price),
                   UserCount = g.Select(r => r.userId).Distinct().Count()
               })
               .ToList();

                var totalCost = reportData.Sum(r => r.TotalCost);

                List<dynamic> finalReport = new List<dynamic>();

                foreach (var item in reportData)
                {
                    dynamic expando = new ExpandoObject();
                    expando.ItemName = item.ItemName;
                    expando.TotalUsed = item.TotalUsed;
                    expando.TotalCost = item.TotalCost;
                    expando.UserCount = item.UserCount;
                    expando.CostPercentage = totalCost > 0 ? (item.TotalCost / (double)totalCost) * 100 : 0;

                    finalReport.Add(expando);
                }

                ViewBag.ReportData = finalReport;
                ViewBag.TotalCost = totalCost;

                return View();

                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

           
        }


        public IActionResult ApproveMail(int id)
        {
            var user = db.users.FirstOrDefault(x => x.userId == id);
            if (user is not null)
            {
                var email = user.UserEmail;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("moiz92718@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Approval of Stationary Request";
                mail.Body = "Your request for stationary has been approved";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("moiz92718@gmail.com", "dkml esnw wpyi ehlx");
                smtp.Send(mail);
                return RedirectToAction("Requests");
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult RejectMail(int id)
        {
            var user = db.users.FirstOrDefault(x => x.userId == id);
            if (user is not null)
            {
                var email = user.UserEmail;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("moiz92718@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Rejection of Stationary Request";
                mail.Body = "Your request for stationary has been rejected";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("moiz92718@gmail.com", "dkml esnw wpyi ehlx");
                smtp.Send(mail);
                return RedirectToAction("Requests");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
