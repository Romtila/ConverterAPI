using Newtonsoft.Json.Linq;

namespace ConverterAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private string apiKey;

        public CurrencyService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
            apiKey = Configuration["Freecurrencyapi:API-Key"];
        }

        public async Task<string> GetCurrency(string from, string to, double numberFrom)
        {
            var convertedValue = await GetConvertedValue(from, to, numberFrom);

            return $"{from} {numberFrom} = {to} {convertedValue}";
        }

        public async Task<string> CurrenciesSummation(string from, string to, double numberFrom, double summand)
        {
            if (from == to)
            {
                var summ = numberFrom + summand;
                return $"{from} {numberFrom} + {to} {summand} = {to} {summ}";
            }

            var summFrom = await CurrencySumming(from, to, summand, numberFrom);

            var summSummand = await CurrencySumming(to, from, numberFrom, summand);

            return $"{from} {numberFrom} + {to} {summand} = {to} {summFrom} or {from} {summSummand}";
        }


        public async Task<string> CurrenciesSubtraction(string from, string to, double numberFrom, double subtractor)
        {
            if (from == to)
            {
                var summ = numberFrom - subtractor;

                if (summ < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(summ));
                }

                return $"{from} {numberFrom} - {to} {subtractor} = {to} {summ}";
            }

            var summFrom = await CurrencySubtraction(from, to, subtractor, numberFrom);

            if (summFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(summFrom));
            }

            var summSummand = await GetConvertedValue(to, from, summFrom);

            return $"{from} {numberFrom} - {to} {subtractor} = {to} {summFrom} or {from} {summSummand}";
        }

        private async Task<double> GetConvertedValue(string from, string to, double numberFrom)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://freecurrencyapi.net");
                    var response = await client.GetAsync($"/api/v2/latest?apikey={apiKey}&base_currency={from.Trim()}");

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var currenciesJson = JObject.Parse(stringResult)["data"];

                    var currencyValue =
                        (double)JObject.Parse(currenciesJson?.ToString() ?? "The conversion service is not available")[
                            $"{to.Trim()}"]!;
                    var convertedValue = numberFrom * currencyValue;

                    return convertedValue;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        private async Task<double> CurrencySubtraction(string from, string to, double numberFrom, double summand)
        {
            var convertedSummandValue = await GetConvertedValue(from, to, summand);
            var summSummand = convertedSummandValue - numberFrom;

            return summSummand;
        }

        private async Task<double> CurrencySumming(string from, string to, double numberFrom, double summand)
        {
            var convertedSummandValue = await GetConvertedValue(from, to, summand);
            var summSummand = convertedSummandValue + numberFrom;

            return summSummand;
        }
    }
}