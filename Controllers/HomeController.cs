using BeeGroup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BeeGroup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Sing()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("MonkeyEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var account = _context.MonkeyAccount.FirstOrDefault(x => x.MonkeyEmail == email);
            var orders = _context.Orders.Where(x => x.Email == email).ToList();

            var vm = new ProfileViewModel
            {
                Account = account,
                Orders = orders
            };

            return View("~/Views/Home/Profile.cshtml", vm);
        }

        public IActionResult Orders()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "admin")
                return Unauthorized("Нет доступа");

            var orders = _context.Orders.ToList();
            var cleaners = _context.Cleaners.ToList();
            var statuses = new List<string> { "В обработке", "Назначен клинер", "Завершён"};

            ViewBag.Orders = orders;
            ViewBag.Cleaners = cleaners;
            ViewBag.Statuses = statuses;

            return View(); // by default looks for Views/Home/Orders.cshtml
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string newStatus)
        {
            if (newStatus == "Отменен")
            {
                // Админ не может вручную установить "Отменен"
                return BadRequest("Установка статуса 'Отменен' из админ-панели запрещена.");
            }

            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;
            _context.SaveChanges();

            return RedirectToAction("Orders");
        }


        [HttpPost]
        public IActionResult AddCleaner(string LastName, string FirstName, string MiddleName, string PhoneNumber, string ServiceTypes)
        {
            var cleaner = new Cleaner
            {
                LastName = LastName,
                FirstName = FirstName,
                MiddleName = MiddleName,
                PhoneNumber = PhoneNumber,
                ServiceTypes = ServiceTypes
            };

            _context.Cleaners.Add(cleaner);
            _context.SaveChanges();

            return RedirectToAction("Orders");
        }

        [HttpPost]
        [HttpPost]
        public IActionResult UpdateCleaner(Cleaner updatedCleaner)
        {
            var cleaner = _context.Cleaners.FirstOrDefault(c => c.CleanerId == updatedCleaner.CleanerId);
            if (cleaner != null)
            {
                cleaner.LastName = updatedCleaner.LastName;
                cleaner.FirstName = updatedCleaner.FirstName;
                cleaner.MiddleName = updatedCleaner.MiddleName;
                cleaner.PhoneNumber = updatedCleaner.PhoneNumber;
                cleaner.ServiceTypes = updatedCleaner.ServiceTypes;
                _context.SaveChanges();
            }

            return RedirectToAction("Orders");
        }


        [HttpPost]
        public IActionResult DeleteCleaner([FromBody] int cleanerId)
        {
            var cleaner = _context.Cleaners.FirstOrDefault(c => c.CleanerId == cleanerId);
            if (cleaner != null)
            {
                _context.Cleaners.Remove(cleaner);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Клинер не найден" });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
