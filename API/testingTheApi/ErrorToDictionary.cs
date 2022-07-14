using System.Text.Json;

namespace testingTheApi
{
    public class ErrorToDictionary
    {
        public static Dictionary<string, List<string>> ExtractErrorFromAPIResponse(string body)
        {
            var response = new Dictionary<string, List<string>>();

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(body);
            Console.WriteLine(jsonElement);
            var errorsJsonElement = jsonElement.GetProperty("error");
            foreach(var fieldWithErrors in errorsJsonElement.EnumerateObject())
            {
                var field = fieldWithErrors.Name;
                var errors = new List<string>();
                foreach(var errorKind in fieldWithErrors.Value.EnumerateArray())
                {
                    var error = errorKind.GetString();
                    errors.Add(error);
                }
                response.Add(field, errors);
            }
            
            return response;
        }
    }
}
