using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
app.UseAuthorization();
app.Run();

public partial class Program { }