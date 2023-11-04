using System.Text;
using Newtonsoft.Json;

namespace FlashcardsApp.Services
{
    public static class ObjectSerialiser
    {
        public static T? Deserialise<T>(HttpResponseMessage response)
        {
            string data = response.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrEmpty(data))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        public static StringContent? Serialise<T>(T model)
        {
            string data = JsonConvert.SerializeObject(model);

            if (string.IsNullOrEmpty(data))
            {
                return default;
            }

            return new StringContent(data, Encoding.UTF8, "application/json");
        }
    }
}
