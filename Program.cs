using LoginAppUsingMW.CustomMiddleWare;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseLoginMW();


app.Run(async context =>
{
    await context.Response.WriteAsync("No response");
});

app.Run();
