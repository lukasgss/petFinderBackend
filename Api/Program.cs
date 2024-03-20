using Api.ActionFilters;
using Api.Converters;
using Api.Extensions;
using Application;
using Application.Middlewares;
using Infrastructure;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging();

builder.Services.AddHttpLogging(logging =>
{
	// Customize HTTP logging here.
	logging.LoggingFields = HttpLoggingFields.All;
	logging.RequestHeaders.Add("sec-ch-ua");
	logging.MediaTypeOptions.AddText("application/javascript");
	logging.RequestBodyLogLimit = 4096;
	logging.ResponseBodyLogLimit = 4096;

	logging.CombineLogs = true;
});

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

app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<CheckTokenTypeMiddleware>();

app.MapControllers();

app.Run();