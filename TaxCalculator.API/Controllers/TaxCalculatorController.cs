using Serilog;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Model.Entities;
using TaxCalculator.Model.Interfaces.Application;
using TaxCalculator.Model.Interfaces.Caching;
using Newtonsoft.Json;

namespace TaxCalculator.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ITaxCalculationService _taxCalculationService;
        private readonly ICachingService _cachingService;
       
        public TaxCalculatorController(ITaxCalculationService taxCalculationService, ICachingService cachingService)
        {
            _taxCalculationService = taxCalculationService;
            _cachingService = cachingService;
        }

        [HttpPost("Calculate")]
        public async Task<ActionResult<EmployeeRecord>> Calculate([FromBody] TaxEngine taxPayer)
        {
            string cacheSSN = taxPayer.SSN.ToString();
            EmployeeRecord employeeRecord = _taxCalculationService.Run(taxPayer);

            if (taxPayer == null)
            {
                Log.Error("TaxPayer is not instantiated.");
                return NotFound("TaxPayer cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                Log.Error("Invalid model state: ", ModelState);
                return BadRequest(ModelState);
            }
      
            if (await _cachingService.ExistsAsync(cacheSSN))
            {
                Log.Information("Tax Payer is cached: " + taxPayer.SSN);
                var cachedTaxes = await _cachingService.GetAsync<EmployeeRecord>(cacheSSN);
                return Ok(cachedTaxes);
            }

            Log.Information("New calculation is performed. " + JsonConvert.SerializeObject(cacheSSN));

            await _cachingService.SetAsync(cacheSSN, employeeRecord, TimeSpan.FromDays(1));
            return Ok(employeeRecord);
        }
    }
}
