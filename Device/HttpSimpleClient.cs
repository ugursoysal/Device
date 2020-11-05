using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Device
{
    class HttpSimpleClient
    {
        static string url = File.ReadAllText("url.txt").TrimEnd('\n');

        public static async Task SendRequestAsync(int id, string p, string info)
        {
            var client = new HttpClient();

            // Create the HttpContent for the form to be posted.
            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("p", p),
                new KeyValuePair<string, string>("info", info), });

            // Get the response.
            HttpResponseMessage response = await client.PostAsync(
                url,
                requestContent);

            // Get the response content.
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                // Write the output.
                Console.WriteLine(await reader.ReadToEndAsync());
            }
        }
    }
}