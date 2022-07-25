using System.Net.Http.Headers;

namespace TheForestManMVC.Utils
{
    public class Client
    {
        public static HttpClient client { get; set; }
        private string url = "http://localhost:5083/api";
        public Client()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
        
    }
}
