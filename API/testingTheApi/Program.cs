// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using testingTheApi;

HttpClient client = new();
client.BaseAddress = new Uri("https://localhost:7083");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

HttpResponseMessage response = await client.GetAsync("api/post");
response.EnsureSuccessStatusCode();
if (response.IsSuccessStatusCode)
{
    var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
    foreach (var post in posts)
    {
        Console.WriteLine(post.Title + post.User);
    }
}
else
{
    Console.WriteLine("No results");
}
Console.ReadLine();
