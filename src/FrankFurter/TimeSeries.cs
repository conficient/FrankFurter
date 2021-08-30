using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FrankFurter
{
    public class TimeSeries
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
        /// Start Date of the series
        /// </summary>
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End Date of the series
        /// </summary>
        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// a dictionary of dates + rate dictionary on that date
        /// </summary>
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; }

    }
}
