using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace NerdStore.WebApp.Tests.Config
{
    public static class TestsExtensions
    {
        public static decimal ApenasNumeros(this string value)
        {
            return Convert.ToDecimal(new string(value.Where(char.IsDigit).ToArray()));
        }

        public static void AtribuirToken(this HttpClient client, string token)
        {
            client.AtribuirJsonMediaType();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public static void AtribuirJsonMediaType(this HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
