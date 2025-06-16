using BeeGroup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace BeeGroup.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] MonkeyAccount account)
        {
            try
            {
                if (account == null)
                    return Json(new { success = false, message = "Данные не получены" });

                if (string.IsNullOrWhiteSpace(account.MonkeyEmail) || string.IsNullOrWhiteSpace(account.MonkeyPassword))
                    return Json(new { success = false, message = "Email или пароль пустые" });

                if (_context.MonkeyAccount.Any(a => a.MonkeyEmail == account.MonkeyEmail))
                {
                    return Json(new { success = false, message = "Email уже зарегистрирован" });
                }
                if (_context.MonkeyAccount.Any(a => a.PhoneNumber == account.PhoneNumber))
                {
                    return Json(new { success = false, message = "Номер телефона уже зарегистрирован" });
                }

                // Присваиваем роль "user" по умолчанию
                account.Role = "user";

                _context.MonkeyAccount.Add(account);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("MonkeyEmail", account.MonkeyEmail);
                HttpContext.Session.SetString("UserRole", account.Role);

                return Json(new { success = true, role = account.Role });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Login([FromBody] MonkeyAccount input)
        {
            if (string.IsNullOrWhiteSpace(input.MonkeyEmail) || string.IsNullOrWhiteSpace(input.MonkeyPassword))
            {
                return Json(new { success = false, message = "Email и пароль обязательны." });
            }

            var user = _context.MonkeyAccount
                .FirstOrDefault(a =>
                    a.MonkeyEmail == input.MonkeyEmail &&
                    a.MonkeyPassword == input.MonkeyPassword);

            if (user == null)
            {
                return Json(new { success = false, message = "Неверный email или пароль" });
            }

            HttpContext.Session.SetString("MonkeyEmail", user.MonkeyEmail);
            HttpContext.Session.SetString("UserRole", user.Role ?? "user");

            return Json(new { success = true, role = user.Role ?? "user" });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
