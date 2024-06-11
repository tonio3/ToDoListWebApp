using ToDoListWebApp.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Net;
using ToDoListWebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
 
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://10.0.1.50:5050");
app.Urls.Add("http://localhost:5050");

app.Run();
