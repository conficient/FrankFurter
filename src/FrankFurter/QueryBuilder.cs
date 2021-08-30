using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FrankFurter
{
    /// <summary>
    /// Helper class for constructing URLs
    /// </summary>
    public class QueryBuilder
    {
        readonly string url;
        readonly NameValueCollection values;

        /// <summary>
        /// Create with a base URL
        /// </summary>
        /// <param name="url">base URL, e.g. /latest</param>
        public QueryBuilder(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            // ensure url has / prefix
            this.url = "/" + url.Trim().TrimStart('/');
            // this uses an internal class that URL encodes values
            // see https://stackoverflow.com/questions/829080/how-to-build-a-query-string-for-a-url-in-c
            values = System.Web.HttpUtility.ParseQueryString(string.Empty);
        }

        /// <summary>
        /// Add a query string value - if the value is the default it won't be added
        /// </summary>
        /// <param name="name">name of the query param</param>
        /// <param name="value">value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns></returns>
        public QueryBuilder Add(string name, string value, string defaultValue = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(value) && value != defaultValue)
                    values.Add(name, value);
            }
            return this;
        }

        /// <summary>
        /// Add a query decimal value - if the value is the default it won't be added
        /// </summary>
        /// <param name="name">name of the query param</param>
        /// <param name="value">value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns></returns>
        public QueryBuilder Add(string name, decimal value, decimal defaultValue = 1m)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (value != defaultValue)
                    return Add(name, value.ToString());
            }
            return this;
        }
        
        /// <summary>
        /// Add multiple query string values in CSV format
        /// </summary>
        /// <param name="name">name of the query param</param>
        /// <param name="value">value</param>
        /// <param name="defaultValue">default value</param>
        /// <returns></returns>
        public QueryBuilder AddRange(string name, IEnumerable<string> values, string defaultValue = null)
        {
            // convert values to CSV and add
            return Add(name, GetCsvList(values), defaultValue);
        }


        /// <summary>
        /// Convert set of values to a CSV list
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string GetCsvList(IEnumerable<string> values)
        {
            if (values is null || !values.Any())
                return string.Empty;
            return string.Join(",", values.Select(c => c.Trim().ToUpper()));
        }

        /// <summary>
        /// Return the full encoded query string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // construct full url
            var qs = values.ToString();
            if (string.IsNullOrEmpty(qs))
                return url;
            else
                return $"{url}?{qs}";
        }

    }
}