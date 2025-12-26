using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            return Ok(await _customerService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CustomerDto dto)
        {
            await _customerService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, CustomerDto dto)
        {
            if (id != dto.Id)
                return BadRequest();
            await _customerService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _customerService.DeleteAsync(id);
            return NoContent();
        }
    }
}