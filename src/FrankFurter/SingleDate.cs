using System;
using System.Collections.Generic;

namespace FrankFurter
{
    public class SingleDate
    {
        /// <summary>
        /// Amount of base currency being converted (1 for not specified)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Base currency (default is EUR)
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Date of the exchange rates
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// List of exchange rates
        /// </summary>
        public Dictionary<string, decimal> Rates { get; set; }

        /// <summary>
        /// Get rate for given currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public decimal? GetRate(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentNullException(nameof(currency));
            var key = currency.Trim().ToUpper();
            if (Rates.ContainsKey(key))
                return Rates[key];
            return null;
        }
    }
}
