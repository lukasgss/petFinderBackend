using Api.ActionFilters;
using Api.Converters;
using Api.Extensions;
using Application;
using Application.Middlewares;
using Infrastructure;
using Infrastructure.RealTimeCommunication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});

builder.Host.ConfigureLogging();

// Add services to the container.
builder.Services.AddControllers(options => { options.Filters.Add<CustomModelValidationAttribute>(); })
	.ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
	.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()); });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ConfigureSwagger();

builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddApplication(builder.Configuration)
	.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseCors();

app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<CheckTokenTypeMiddleware>();

app.MapControllers();
app.MapHub<ChatHub>("/api/chat-hub");

app.Run();