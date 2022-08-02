// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using testingTheApi;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Web;
using Models.Request;
using Models.Response;

Console.WriteLine(DateTime.Now);
HttpClient client = new();
client.BaseAddress = new Uri("https://localhost:7083");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//HttpResponseMessage response = new HttpResponseMessage();
HttpResponseMessage response = await client.GetAsync("api/post");
HttpResponseMessage response2 = await client.GetAsync("api/user");
HttpResponseMessage response3 = await client.GetAsync("api/comment");
//response = await client.
response.EnsureSuccessStatusCode();
response2.EnsureSuccessStatusCode();
response3.EnsureSuccessStatusCode();
if (
    response.IsSuccessStatusCode &&
    response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
{
    Console.WriteLine("          Post list:   ");

    var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
    foreach (var post in posts)
    {
        Console.WriteLine(post.PostWithComments());
    }
    Console.WriteLine();
    Console.WriteLine("          Users List:   ");
    var users = await response2.Content.ReadFromJsonAsync<IEnumerable<UserInfo>>();
    foreach (var user in users)
    {
        Console.WriteLine(user.ToString());
    }
    var comments = await response3.Content.ReadFromJsonAsync<IEnumerable<CommentInfo>>();
    Console.WriteLine("          Comments list:   ");
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


////post json
///*
//var newPost = new Post()
//{
//    Title = "altu",
//    User = "mgfgb",
//    Description = "FdfgfbgfjgfgkgfhskjdsjhgfgjkgdhkgdhgSFJDSFVODSFJFGVOJVFOIJ",
//    Created = DateTime.Now,
//    Updated = DateTime.Now

//};


//string urii = "https://localhost:7083" + "/api/post";
//response2 = await client.PostAsJsonAsync(urii, newPost);
//if(response2.IsSuccessStatusCode)
//{
//    response = await client.GetAsync("api/post");
//    if( response.IsSuccessStatusCode )
//    {
//        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
//        foreach (var post in posts)
//        {
//            Console.WriteLine(post.ToString());
//        }
//    }
//}*/

////post async
///*
//var newPost = new Post()
//{

//    User = "mgfgb",
//    Description = "FdfgfbgfjgfgkgfhskjdsjhgfgjkgdhkgdhgSFJDSFVODSFJFGVOJVFOIJ",
//    Created = DateTime.Now,
//    Updated = DateTime.Now

//};
//string url = "https://localhost:7083" + "/api/post";
//var newPostSerialized = JsonSerializer.Serialize(newPost);
//var stringContent = new StringContent(newPostSerialized, Encoding.UTF8, "application/json");
////poate fi si application/xml

//response2 = await client.PostAsync(url, stringContent);
//if (response2.IsSuccessStatusCode)
//{
//    response = await client.GetAsync("api/post");
//    if (response.IsSuccessStatusCode)
//    {
//        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
//        foreach (var post in posts)
//        {
//            Console.WriteLine(post.ToString());
//        }
//    }

//}*/

///*Post newPost = new Post();
//response2 = await client.PostAsJsonAsync(url, newPost);
//if (response2.IsSuccessStatusCode)
//{
//    response = await client.GetAsync("api/post");
//    if (response.IsSuccessStatusCode)
//    {
//        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<Post>>();
//        foreach (var post in posts)
//        {
//            Console.WriteLine(post.ToString());
//        }
//    }

//}
//else
//    {
//        var body = await response.Content.ReadAsStringAsync();
//        var errorsfromWebApi = ErrorToDictionary.ExtractErrorFromAPIResponse(body);
//        foreach(var fieldWithError in errorsfromWebApi)
//        {
//            Console.WriteLine($"-{fieldWithError.Key}");
//            foreach(var error in fieldWithError.Value)
//            {
//                Console.WriteLine($"   {error}");
//            }
//        }

//    }
//*/
///*

// //post with header
//string url = "https://localhost:7083" + "/api/post";
//using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
//{
//    requestMessage.Headers.Add("postAmount", "2");
//    var responseMessage = await client.SendAsync(requestMessage);
//    var posts = await  responseMessage.Content.ReadFromJsonAsync<IEnumerable<Post>>();
//    foreach (var post in posts)
//    {
//        Console.WriteLine(post.ToString());
//    }

//}*/



////create user
///*
//try
//{


//        var respuesta = await client.PostAsJsonAsync($"{urlAccounts}/create", credentials);
//        if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
//        {
//            respuesta.EnsureSuccessStatusCode();
//        }

//}
//catch (Exception e)
//{
//    Console.WriteLine(e);
//}*/
//UserRequest credentials3 = new UserRequest()
//{
//    //Email = "ioan@ererf.comm",
//    Email = "isdark23@yahoo.com",
//    Password = "altaparolaaa",
//    UserName = "Ionutescu"
//    //UserName = "Ionel"
//};
//Credentials credentials = new Credentials()
//{
//    NameEmail = credentials3.UserName,
//    Password = credentials3.Password
//};


//string url = "https://localhost:7083" + "/api/post";
//string urlAccounts = "https://localhost:7083" + "/api/user";
//string urlComments = "https://localhost:7083" + "/api/comment";


////< update post >


////response3 = await client.GetAsync(url);
////response3.EnsureSuccessStatusCode();
////var posts4 = await response3.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
////var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
////var httpResponseToken = await client.PostAsJsonAsync(urlAccounts + "/login", credentials);
////var responseToken = JsonSerializer.Deserialize<UserToken>(await
////    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
////client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
////    responseToken.Token);
////Console.WriteLine(); Console.WriteLine(); Console.WriteLine();

////Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
////PostInfo post1 = posts4.LastOrDefault();
////post1.Body = "Updated again";
////string description = post1.Body;

//////response = await client.PutAsJsonAsync($"{url}/{post1.Id}", "Altceva domle mare, ce mare esti!");
//////response = await client.PutAsJsonAsync($"{url}/{post1.Id}", post1);

//////<delete post>
////response = await client.DeleteAsync($"{url}/{post1.Id}");
//////</delete post>
////response3 = await client.GetAsync(url);
////posts4 = await response3.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
////foreach (var post in posts4)
////{
////    Console.WriteLine(post.ToString());
////}


////</update post>


////<get all comments of a post>

////response = await client.GetAsync("api/post");
////if (response.IsSuccessStatusCode)
////{
////    var posts2 = await response.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
////    var post2 = posts2.LastOrDefault();
////    response = await client.GetAsync($"{url}/{post2.Id}/comments");
////    if (response.IsSuccessStatusCode)
////    {
////        var user3 = await response.Content.ReadFromJsonAsync<PostInfo>();
////        Console.WriteLine(user3.PostWithComments());
////    }
////}

////</get all comments of a post>

////<get all posts and comments of a user>

////response = await client.GetAsync("api/user");
////if (response.IsSuccessStatusCode)
////{
////    var users2 = await response.Content.ReadFromJsonAsync<IEnumerable<UserInfo>>();
////    var user2 = users2.LastOrDefault();
////    response = await client.GetAsync($"{urlAccounts}/{user2.Id}/postscomments");
////    if (response.IsSuccessStatusCode)
////    {
////        var user3 = await response.Content.ReadFromJsonAsync<UserPostInfo>();
////        Console.WriteLine(user3.ToString());
////    }
////}

////</get all posts and comments of a user>


////< create comment >


////response = await client.GetAsync("api/post");
////response2 = await client.GetAsync("api/user");
////if (response.IsSuccessStatusCode
////    && response2.IsSuccessStatusCode)
////{
////    var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
////    var post = posts.LastOrDefault();
////    response3 = await client.GetAsync("api/comment");


////    //var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
////    //var httpResponseToken = await client.PostAsJsonAsync($"{urlAccounts}/login", credentials);
////    //var responseToken = JsonSerializer.Deserialize<UserToken>(await
////    //    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
////    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
////    //    responseToken.Token);
////    ////<update comment>s
////    //var comments1 = await response3.Content.ReadFromJsonAsync<IEnumerable<CommentInfo>>();
////    //var comment3 = comments1.Last();
////    //comment3.Body = "Updated";
////    //var requestComm2 = await client.PutAsJsonAsync($"{urlComments}/{comment3.Id}", "Updated");
////    ////var requestComm3 = await client.DeleteAsync($"{urlComments}/{comment3.Id}");
////    //</update comment>
////    CommentRequest commentRequest = new CommentRequest()
////    {
////        Body = "Asa asa, chiar asa",
////        PostId = post.Id
////    };
////    //var requestComm = await client.PostAsJsonAsync($"{urlComments}/loggedin", commentRequest);
////    var requestComm = await client.PostAsJsonAsync($"{urlComments}/loggedout", commentRequest);
////    response3 = await client.GetAsync("api/comment");
////    if (response3.IsSuccessStatusCode)
////    {
////        var comments = await response3.Content.ReadFromJsonAsync<IEnumerable<CommentInfo>>();
////        foreach (var comment2 in comments)
////        {
////            Console.WriteLine(comment2.ToString());
////        }
////    }
////}

////</create comment>

////<create new post>

//response2 = await client.GetAsync("api/user");
//var users2 = await response2.Content.ReadFromJsonAsync<IEnumerable<UserInfo>>();
//var user2 = users2.LastOrDefault();

//PostRequest newPost = new PostRequest()
//{
//    Body = "BodyPostNou6666666",
//    Title = "PoST nou6666666"

//};

//var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
//var httpResponseToken = await client.PostAsJsonAsync($"{urlAccounts}/login", credentials);
//var responseToken = JsonSerializer.Deserialize<UserToken>(await
//    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
//client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
//    responseToken.Token);

//response2 = await client.PostAsJsonAsync(url, newPost);
//if (response2.IsSuccessStatusCode)
//{
//    response = await client.GetAsync("api/post");
//    if (response.IsSuccessStatusCode)
//    {
//        var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
//        foreach (var post in posts)
//        {
//            Console.WriteLine(post.ToString());
//        }
//    }
//}
////</create new post>

////<update password>

////var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
////var httpResponseToken = await client.PostAsJsonAsync($"{urlAccounts}/login", credentials);
////var responseToken = JsonSerializer.Deserialize<UserToken>(await
////    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
////client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
////    responseToken.Token);
////credentials.Password = "altaparolaaa";
////response = await client.PutAsJsonAsync(urlAccounts, credentials);
////if (response.IsSuccessStatusCode)
////{
////    Console.WriteLine("reusit");
////    response2 = await client.GetAsync("api/user");
////    if (response2.IsSuccessStatusCode)
////    {
////        var users = await response2.Content.ReadFromJsonAsync<IEnumerable<UserInfo>>();
////        foreach (var user in users)
////        {
////            Console.WriteLine(user.ToString());
////        }
////    }
////}

////</update password>

////<delete account>

////var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
////var httpResponseToken = await client.PostAsJsonAsync($"{urlAccounts}/login", credentials);
////var responseToken = JsonSerializer.Deserialize<UserToken>(await
////    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
////client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
////    responseToken.Token);

////try
////{
////    var respuesta = await client.GetAsync($"{urlAccounts}/deletecode?username={credentials3.UserName}&&email={credentials3.Email}");
////    if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
////    {
////        respuesta.EnsureSuccessStatusCode();
////    }
////    if (respuesta.IsSuccessStatusCode)
////    {
////        Console.WriteLine("merge?");
////        Console.WriteLine("Enter the code:");
////        string coderead = Console.ReadLine();
////        ValidationCode code = new ValidationCode()
////        {
////            Code = coderead,
////            Created = DateTime.Now
////        };
////        UserCode userCode = new UserCode()
////        {
////            Email = credentials3.Email,
////            Password = credentials3.Password,
////            UserName = credentials3.UserName,
////            Code = code
////        };
////        var respueste = await client.DeleteAsync($"{urlAccounts}/delete?username={userCode.UserName}&&codeFromUser={userCode.Code.Code}&&created={userCode.Code.Created.ToString()}");
////        if (respueste.StatusCode == HttpStatusCode.InternalServerError)
////        {
////            respueste.EnsureSuccessStatusCode();
////        }
////        Console.WriteLine("Deleted, soarele lui");
////        response2 = await client.GetAsync("api/user");
////        if (response2.IsSuccessStatusCode)
////        {
////            var users = await response2.Content.ReadFromJsonAsync<IEnumerable<User>>();
////            foreach (var user in users)
////            {
////                Console.WriteLine(user.ToString());
////            }
////        }
////    }


////}
////catch (Exception e)
////{
////    Console.WriteLine(e);
////}

////</delete account>


////<login >


////var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
////var httpResponseToken = await client.PostAsJsonAsync($"{urlAccounts}/login", credentials);
////var responseToken = JsonSerializer.Deserialize<UserToken>(await
////    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
////client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
////    responseToken.Token);

////</login>


////UserInfo userinfo = await response.Content.ReadFromJsonAsync<UserInfo>();
////Console.WriteLine(userinfo.ToString());
////($"{urlAccounts}/{credentials.UserName}");
////response = await client.DeleteAsync($"{urlAccounts}/{personId}")




////////<create user>

////try
////{
////    var respuesta = await client.GetAsync($"{urlAccounts}/registercode?username={credentials3.UserName}&&email={credentials3.Email}");
////    if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
////    {
////        respuesta.EnsureSuccessStatusCode();
////    }
////    if (respuesta.IsSuccessStatusCode)
////    {
////        Console.WriteLine("merge?");
////        Console.WriteLine("Enter the code:");
////        string coderead = Console.ReadLine();
////        ValidationCode code = new ValidationCode()
////        {
////            Code = coderead,
////            Created = DateTime.Now
////        };
////        UserCode userCode = new UserCode()
////        {
////            Email = credentials3.Email,
////            Password = credentials3.Password,
////            UserName = credentials3.UserName,
////            Code = code
////        };
////        var respueste = await client.PostAsJsonAsync($"{urlAccounts}/create", userCode);
////        if (respueste.StatusCode == HttpStatusCode.InternalServerError)
////        {
////            respueste.EnsureSuccessStatusCode();
////        }
////        Console.WriteLine("Created, soarele lui");
////        response2 = await client.GetAsync("api/user");
////        if (response2.IsSuccessStatusCode)
////        {
////            var users = await response2.Content.ReadFromJsonAsync<IEnumerable<User>>();
////            foreach (var user in users)
////            {
////                Console.WriteLine(user.ToString());
////            }
////        }

////    }

////}
////catch (Exception e)
////{
////    Console.WriteLine(e);
////}


////</create user>




//Console.WriteLine("Post created successfully");
//Console.ReadLine();
