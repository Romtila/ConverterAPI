namespace ConverterAPI.Services;

public interface ICurrencyService
{
    Task<string> GetCurrency(string from, string to, double numberFrom);
    Task<string> CurrenciesSummation(string from, string to, double numberFrom, double summand);
    Task<string> CurrenciesSubtraction(string from, string to, double numberFrom, double subtractor);
}