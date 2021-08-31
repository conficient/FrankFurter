# FrankFurter

> Frankfurter is an open-source API for current and historical foreign exchange rates published by the European Central Bank. 
> https://www.frankfurter.app/

This library is a C# API for accessing the Frankfurter foreign exchange rate API.

[![.NET Core build pack and push](https://github.com/conficient/FrankFurter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/conficient/FrankFurter/actions/workflows/dotnet.yml)

### Installation

Install the [Nuget package](https://www.nuget.org/packages/FrankFurter/) in your .NET application:
```cmd
nuget install FrankFurter
```
or in .NET Core applications:
```cmd
dotnet add package FrankFurter
```

### Quick Use

#### Client.GetLatestRateAsync()

If you just want to get a latest rate for a currency, use the static `GetLatestRateAsync()` method:
```c#
const string baseCurrency = "USD";
const string targetCurrency = "GBP";
// get the latest USD-GBP rate 
var rate = await Client.GetLatestRateAsync(baseCurrency, targetCurrency);
```
The date of the rate will be the most recent data available.

#### Client.GetRateForAsync()
If you need a rate for a specific date, use `GetRateForAsync()`:
```c#
var date = new DateTime(2021,8,2);
const string baseCurrency = "USD";
const string targetCurrency = "GBP";
// get the latest USD-GBP rate 
var rate = await Client.GetRateForAsync(date, baseCurrency, targetCurrency);
```
You can also use the `amount` parameter to perform conversions.

### Usage

Create a `Client` instance and call the appropriate method. Note that the client
implements `IDisposable` so should be in a `using` block:
```c#
using (var client = new Client())
{
    // call methods
}
```

#### [GetLatestAsync()](https://www.frankfurter.app/docs/#latest)

Gets the latest available exchange rates for all available currencies, using EUR as a base.
```c#
SingleDate latest = await client.GetLatestAsync();
```

The `SingleDate` has a helper method `GetRate()` which returns the rate for a given currency.
```c#
SingleDate latest = await client.LatestAsync();
decimal gbpRate = latest.GetRate("GBP");
```

#### [GetHistoricalAsync()](https://www.frankfurter.app/docs/#historical)

Get the rates for a specific date (going back to 4th Jan 1999). If you specify a date for which are not valid, e.g. weekend/holiday, the Frankfurter API will return the nearest date before that.

```c#
var forDate = new DateTime(2021,8,2);
SingleDate aug2nd = await client.GetHistoricalAsync(forDate);
```

#### [GetTimeSeriesAsync()](https://www.frankfurter.app/docs/#timeseries)

Get the rates for a range of dates. If you leave the `toDate` as null it returns all dates upto the latest available date. Note that the resulting date range may not be contiguous as rates are not available for every date.
```c#
var fromDate = new DateTime(2021,8,1);
TimeSeries rates = await client.GetTimeSeriesAsync(forDate);
```

**Important!** these calls can return a lot of data so you should normally specify just the target currencies you need, e.g.:
```c#
// get EUR-USD rate from 1st August 2021 to latest
var fromDate = new DateTime(2021,8,1);
var toFetch = new string[] {"USD"};
TimeSeries rates = await client.GetTimeSeriesAsync(fromDate, toCurrencies: toFetch);
```

For a specific set of dates, provide both a `fromDate` and `toDate`:
```c#
var fromDate = new DateTime(2021,8,2);
var toDate = new DateTime(2021,8,27);
var toFetch = new string[] {"USD"};
TimeSeries rates = await client.GetTimeSeriesAsync(fromDate, toDate, toCurrencies: toFetch);
```
#### [GetCurrenciesAsync()]()

https://www.frankfurter.app/docs/#timeseries

Returns a dictionary of the ISO 4217 currency codes and descriptions supported by the Frankfurter API.
```c#
Dictionary<string, string> currencies = await client.GetCurrenciesAsync();
// get name for USD:
var usdName = currencies["USD"];   // "United States Dollar"
```


#### Common Parameters

These parameters can be used in all methods except `GetCurrenciesAsync`

##### baseCurrency

To query for a different base currency use the `baseCurrency` parameter:
```c#
SingleDate latest = await client.LatestAsync(baseCurrency:"GBP");
```

##### toCurrencies

By default the method returns all available currencies: you should normally specify just the currency codes you need, e.g. to fetch just EUR-GBP and EUR-USD

```c#
var toFetch = new string[] {"GBP", "USD"};
SingleDate latest = await client.LatestAsync(toCurrencies: toFetch);
```

##### amount

The default amount of the base currency is normally 1. You can change this with the `amount` parameter.

```c#
// convert &euro;100 to all currencies
SingleDate latest = await client.LatestAsync(amount: 100m);
```


### Requirements

Any application that supports .NET Standard 2.0 - this includes .NET Framework 4.6.1 and later, .NET Core 2.0 + and .NET 5 and .NET 6
