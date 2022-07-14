// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using testingTheApi;
using System.Text.Json;
using System.Text;
using System.Net;

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
    Console.WriteLine();
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
    Console.WriteLine(); Console.WriteLine();
}
else
{
    Console.WriteLine("No results");
}


//post json
/*
var newPost = new PostDto()
{
    Title = "altu",
    User = "mgfgb",
    Description = "FdfgfbgfjgfgkgfhskjdsjhgfgjkgdhkgdhgSFJDSFVODSFJFGVOJVFOIJ",
    Created = DateTime.Now,
    Updated = DateTime.Now

};


string urii = "https://localhost:7083" + "/api/post";
response2 = await client.PostAsJsonAsync(urii, newPost);
if(response2.IsSuccessStatusCode)
{
    response = await client.GetAsync("api/post");
    if( response.IsSuccessStatusCode )
    {
        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
        foreach (var post in posts)
        {
            Console.WriteLine(post.ToString());
        }
    }
}*/

//post async
/*
var newPost = new PostDto()
{
    
    User = "mgfgb",
    Description = "FdfgfbgfjgfgkgfhskjdsjhgfgjkgdhkgdhgSFJDSFVODSFJFGVOJVFOIJ",
    Created = DateTime.Now,
    Updated = DateTime.Now

};
string url = "https://localhost:7083" + "/api/post";
var newPostSerialized = JsonSerializer.Serialize(newPost);
var stringContent = new StringContent(newPostSerialized, Encoding.UTF8, "application/json");
//poate fi si application/xml

response2 = await client.PostAsync(url, stringContent);
if (response2.IsSuccessStatusCode)
{
    response = await client.GetAsync("api/post");
    if (response.IsSuccessStatusCode)
    {
        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
        foreach (var post in posts)
        {
            Console.WriteLine(post.ToString());
        }
    }
   
}*/

/*PostDto newPost = new PostDto();
response2 = await client.PostAsJsonAsync(url, newPost);
if (response2.IsSuccessStatusCode)
{
    response = await client.GetAsync("api/post");
    if (response.IsSuccessStatusCode)
    {
        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
        foreach (var post in posts)
        {
            Console.WriteLine(post.ToString());
        }
    }

}
else
    {
        var body = await response.Content.ReadAsStringAsync();
        var errorsfromWebApi = ErrorToDictionary.ExtractErrorFromAPIResponse(body);
        foreach(var fieldWithError in errorsfromWebApi)
        {
            Console.WriteLine($"-{fieldWithError.Key}");
            foreach(var error in fieldWithError.Value)
            {
                Console.WriteLine($"   {error}");
            }
        }

    }
*/
/*
 
 //post with header
string url = "https://localhost:7083" + "/api/post";
using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
{
    requestMessage.Headers.Add("postAmount", "2");
    var responseMessage = await client.SendAsync(requestMessage);
    var posts = await  responseMessage.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
    foreach (var post in posts)
    {
        Console.WriteLine(post.ToString());
    }

}*/



//create user
/*
try
{
    
    
        var respuesta = await client.PostAsJsonAsync($"{urlAccounts}/create", credentials);
        if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
        {
            respuesta.EnsureSuccessStatusCode();
        }
    
}
catch (Exception e)
{
    Console.WriteLine(e);
}*/
UserDto credentials = new UserDto()
{
    Email = "ioan@ererf.comm",
    Password = "stringst",
    UserName = "Ionel"
};
PostDto newPost = new PostDto()
{
    Description = "eram undeva pe mail",
    Title = "Postare 12",
    User = "Ionel",
    Created = DateTime.Now,
    Updated = DateTime.Now
};
Console.WriteLine(newPost.ToString());
string url = "https://localhost:7083" + "/api/post";
string urlAccounts = "https://localhost:7083" + "/api/user";
/*
try
{


    var respuesta = await client.PostAsJsonAsync($"{urlAccounts}/create", credentials);
    if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
    {
        respuesta.EnsureSuccessStatusCode();
    }

}
catch (Exception e)
{
    Console.WriteLine(e);
}*/

var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
var httpResponseToken = await client.PostAsJsonAsync(urlAccounts +"/login", credentials);
var responseToken = JsonSerializer.Deserialize<UserToken>(await
    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
    responseToken.Token);

response = await client.PostAsJsonAsync(url, newPost);
response.EnsureSuccessStatusCode();
var content = await response.Content.ReadAsStringAsync();
PostDto postare = JsonSerializer.Deserialize<PostDto>(content);
Console.WriteLine(postare.ToString());
Guid postId = Guid.Parse(content); 
newPost.Description = "updated";
using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, url+$"{postId}"))
{
    requestMessage.Headers.Add("username", credentials.UserName);

    var responseMessage = await client.SendAsync(requestMessage);
    var posts = await responseMessage.Content.ReadFromJsonAsync<IEnumerable<PostDto>>();
    foreach (var post in posts)
    {
        Console.WriteLine(post.ToString());
    }

}
Console.WriteLine("Post created successfully");
Console.ReadLine();
