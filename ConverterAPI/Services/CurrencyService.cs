using Newtonsoft.Json.Linq;

namespace ConverterAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        public readonly IConfiguration Configuration;

        public CurrencyService(IConfiguration configuration)
        {
            Configuration = configuration;
            apiKey = Configuration["Freecurrencyapi:API-Key"];
        }

        private string apiKey;

        public async Task<string> GetCurrency(string from, string to, double numberFrom)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var convertedValue = await GetConvertedValue(client, from, to, numberFrom);

                    return $"{from} {numberFrom} = {to} {convertedValue}";
                }
                catch (Exception)
                {
                    return "Error";
                }
            }
        }

        public async Task<string> CurrenciesSummation(string from, string to, double numberFrom, double summand)
        {
            if (from == to)
            {
                var summ = numberFrom + summand;
                return $"{from} {numberFrom} + {to} {summand} = {to} {summ}";
            }

            using (var client = new HttpClient())
            {
                try
                {
                    var summFrom = await CurrencySumming(client, from, to, summand, numberFrom);

                    var summSummand = await CurrencySumming(client, to, from, numberFrom, summand);

                    return $"{from} {numberFrom} + {to} {summand} = {to} {summFrom} or {from} {summSummand}";
                }
                catch (Exception)
                {
                    return "Error";
                }
            }
        }

        public async Task<string> CurrenciesSubtraction(string from, string to, double numberFrom, double subtractor)
        {
            if (from == to)
            {
                var summ = numberFrom - subtractor;

                return summ < 0 ? "Number cannot be negative" : $"{from} {numberFrom} - {to} {subtractor} = {to} {summ}";
            }

            using (var client = new HttpClient())
            {
                try
                {
                    var summFrom = await CurrencySubtraction(client, from, to, subtractor, numberFrom);

                    if (summFrom < 0)
                    {
                        return "Number cannot be negative";
                    }
                    
                    var summSummand = await GetConvertedValue(client, to, from, summFrom);

                    return $"{from} {numberFrom} - {to} {subtractor} = {to} {summFrom} or {from} {summSummand}";
                }
                catch (Exception)
                {
                    return "Error";
                }
            }
        }

        private async Task<double> GetConvertedValue(HttpClient client, string from, string to, double numberFrom)
        {
            var response = await client.GetAsync(
                $"https://freecurrencyapi.net/api/v2/latest?apikey={apiKey}&base_currency={from.Trim()}");

            var stringResult = await response.Content.ReadAsStringAsync();
            var currenciesJson = JObject.Parse(stringResult)["data"];

            var currencyValue =
                (double)JObject.Parse(currenciesJson?.ToString() ?? "The conversion service is not available")[
                    $"{to.Trim()}"]!;
            var convertedValue = numberFrom * currencyValue;

            return convertedValue;
        }

        private async Task<double> CurrencySubtraction(HttpClient client, string from, string to, double numberFrom, double summand)
        {
            var convertedSummandValue = await GetConvertedValue(client, from, to, summand);
            var summSummand = convertedSummandValue - numberFrom;
            
            return summSummand;
        }

        private async Task<double> CurrencySumming(HttpClient client, string from, string to, double numberFrom, double summand)
        {
            var convertedSummandValue = await GetConvertedValue(client, from, to, summand);
            var summSummand = convertedSummandValue + numberFrom;

            return summSummand;
        }
    }
}
