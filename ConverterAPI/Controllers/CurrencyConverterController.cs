using ConverterAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConverterAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyConverterController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        [Route("getCurrency")]
        public async Task<string> GetCurrency(string from, string to, double numberFrom)
        {
            return await _currencyService.GetCurrency(from, to, numberFrom);
        }

        [HttpGet]
        [Route("currenciesSummation")]
        public async Task<string> CurrenciesSummation(string from, string to, double numberFrom, double summand)
        {
            return await _currencyService.CurrenciesSummation(from, to, numberFrom, summand);
        }


        [HttpGet]
        [Route("currenciesSubtraction")]
        public async Task<string> CurrenciesSubtraction(string from, string to, double numberFrom, double subtractor)
        {
            return await _currencyService.CurrenciesSubtraction(from, to, numberFrom, subtractor);
        }
    }
}