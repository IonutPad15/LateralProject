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
HttpResponseMessage response2 = await client.GetAsync("api/user");
HttpResponseMessage response3 = await client.GetAsync("api/comment");
//response = await client.
response.EnsureSuccessStatusCode();
response2.EnsureSuccessStatusCode();
response3.EnsureSuccessStatusCode();
if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
{
    var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
    foreach (var post in posts)
    {
        Console.WriteLine(post.ToString());
    }
    var users = await response2.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
    foreach (var user in users)
    {
        Console.WriteLine(user.ToString());
    }
    var comments = await response3.Content.ReadFromJsonAsync<IEnumerable<CommentDto>>();
    foreach (var comment in comments)
    {
        Console.WriteLine(comment.ToString());
    }
}
else
{
    Console.WriteLine("No results");
}
Console.ReadLine();
