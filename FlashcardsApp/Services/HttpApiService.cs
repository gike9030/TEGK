using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FlashcardsApp.Services
{
    public class HttpApiService
    {
        public static HttpResponseMessage PostToAPI<T>(HttpClient httpClient, string dirrection, T model)
        {
            StringContent? content = ObjectSerialiser.Serialise(model);
            return httpClient.PostAsync(httpClient.BaseAddress + dirrection, content).Result;

        }

        public static HttpResponseMessage PutToAPI<T>(HttpClient httpClient, string dirrection, T model)
        {
            StringContent? content = ObjectSerialiser.Serialise(model);
            
            return httpClient.PutAsync(httpClient.BaseAddress + dirrection, content).Result;
        }

        public static T? GetFromAPI<T>(HttpClient httpClient, string dirrection, int? id = null)
        {
            HttpResponseMessage response = httpClient.GetAsync(httpClient.BaseAddress + dirrection + (id == null ? "" : id)).Result;
            
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            return ObjectSerialiser.Deserialise<T>(response);
        }

        public async static Task<HttpResponseMessage> DeleteFromAPI(HttpClient httpClient, string dirrection, int id)
        {
            return await httpClient.DeleteAsync(httpClient.BaseAddress + dirrection + id);
        }
    }
}
