using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Model.DbService;
using TaxCalculator.Model.Entities;

namespace TaxCalculator.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TaxManagementController : ControllerBase
    {
        private readonly TaxDbService _taxManagementService;

        public TaxManagementController(TaxDbService taxManagementService) =>
            _taxManagementService = taxManagementService;

        [HttpGet]
        public async Task<List<Tax>> Get() =>
            await _taxManagementService.GetAsync();

        [HttpGet("{id:length(12)}")]
        public async Task<ActionResult<Tax>> Get(string id)
        {
            var tax = await _taxManagementService.GetAsync(id);

            if (tax is null)
            {
                return NotFound();
            }

            return tax;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Tax newTax)
        {
            await _taxManagementService.CreateAsync(newTax);

            return CreatedAtAction(nameof(Get), new { id = newTax.Id }, newTax);
        }

        [HttpPut("{id:length(12)}")]
        public async Task<IActionResult> Update(string id, Tax updatedTax)
        {
            var tax = await _taxManagementService.GetAsync(id);

            if (tax is null)
            {
                return NotFound();
            }

            updatedTax.Id = tax.Id;

            await _taxManagementService.UpdateAsync(id, updatedTax);

            return NoContent();
        }

        [HttpDelete("{id:length(12)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var tax = await _taxManagementService.GetAsync(id);

            if (tax is null)
            {
                return NotFound();
            }

            await _taxManagementService.RemoveAsync(id);

            return NoContent();
        }
    }
}
