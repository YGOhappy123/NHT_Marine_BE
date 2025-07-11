using NHT_Marine_BE.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddJsonOptionsForControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsPolicies();
builder.Services.AddApplicationServices();
builder.Services.AddBackgroundServices();
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.ConfigureApiBehavior();

var app = builder.Build();

app.UsePathBase("/api/v1");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
