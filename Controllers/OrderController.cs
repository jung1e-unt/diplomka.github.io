using Microsoft.AspNetCore.Mvc;
using BeeGroup.Models;
using System.Threading.Tasks;

namespace BeeGroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] OrderCreateDto dto)
        {
            var order = new Order
            {
                Email = dto.Email,
                ServiceType = dto.ServiceType,
                ApartmentSize = dto.ApartmentSize,
                Price = dto.Price,
                Regularity = dto.Regularity,
                Status = "В обработке"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Заказ успешно оформлен!" });
    
        
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Заказ не найден." });
            }

            order.Status = "Отменен";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Заказ отменён." });
        }

    }

}
