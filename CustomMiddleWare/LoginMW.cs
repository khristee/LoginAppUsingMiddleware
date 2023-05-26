using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace LoginAppUsingMW.CustomMiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoginMW
    {
        private readonly RequestDelegate _next;

        public LoginMW(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/" && httpContext.Request.Method == "POST")
            {

                //Read the response body as stream
                StreamReader reader = new StreamReader(httpContext.Request.Body);
                string body = await reader.ReadToEndAsync();

                //Parse the request body from string into dictionary
                Dictionary<string, StringValues> queryDict = QueryHelpers.ParseQuery(body);
                string? email = null, password = null;

                //read firstNumber if submitted in the request body
                if (queryDict.ContainsKey("email"))
                {
                    email = queryDict["email"][0].ToString();
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid input for 'email'\n");
                }

                //Read 'second number' if submitted in the body
                if (queryDict.ContainsKey("password"))
                {
                    password = queryDict["password"][0].ToString();
                }
                else
                {
                    if (httpContext.Response.StatusCode == 200)
                        httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid input for 'password'\n");
                }

                //if both mail and password are submitted in the request
                if (string.IsNullOrEmpty(email) == false && string.IsNullOrEmpty(password) == false)
                {
                    //valid email and password as per requirement specification
                    string validEmail = "admin@example.com", validPassword = "admin123";
                    bool isValidLogin;

                    if (email == validEmail && password == validPassword)
                    {
                        isValidLogin = true;
                    }
                    else
                    {
                        isValidLogin = false;
                    }

                    if (isValidLogin)
                    {
                        await httpContext.Response.WriteAsync("Successful login\n");
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 400;
                        await httpContext.Response.WriteAsync("Invalid login\n");
                    }
                }
            }
            else
            {
                await _next(httpContext);
            }
            
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoginMWExtensions
    {
        public static IApplicationBuilder UseLoginMW(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginMW>();
        }
    }
}
