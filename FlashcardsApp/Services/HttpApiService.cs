using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace FlashcardsApp.Services
{
    public static class HttpApiService
    {
        public static HttpResponseMessage PostToAPI<T>(HttpClient httpClient, string dirrection, T model, string? token = null)
        {
            StringContent? content = ObjectSerialiser.Serialise(model);

            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient.PostAsync(httpClient.BaseAddress + dirrection, content).Result;
        }

        public static HttpResponseMessage PutToAPI<T>(HttpClient httpClient, string dirrection, T model, string? token = null)
        {
            StringContent? content = ObjectSerialiser.Serialise(model);
            
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient.PutAsync(httpClient.BaseAddress + dirrection, content).Result;
        }

        public static T? GetFromAPI<T>(HttpClient httpClient, string dirrection, int? id = null, string? token = null)
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage response = httpClient.GetAsync(httpClient.BaseAddress + dirrection + (id == null ? "" : id)).Result;
            
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            return ObjectSerialiser.Deserialise<T>(response);
        }

        public async static Task<HttpResponseMessage> DeleteFromAPI(HttpClient httpClient, string dirrection, int id, string? token = null)
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await httpClient.DeleteAsync(httpClient.BaseAddress + dirrection + id);
        }
    }
}
