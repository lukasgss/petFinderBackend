using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Middlewares;

public class CheckTokenTypeMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<CheckTokenTypeMiddleware> _logger;
	private const string RefreshTokenEndpoint = "/api/users/refresh";

	public CheckTokenTypeMiddleware(RequestDelegate next, ILogger<CheckTokenTypeMiddleware> logger)
	{
		_next = next ?? throw new ArgumentNullException(nameof(next));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task InvokeAsync(HttpContext context)
	{
		PathString path = context.Request.Path;
		ClaimsPrincipal user = context.User;

		if (path.StartsWithSegments(RefreshTokenEndpoint))
		{
			string? tokenType = user.FindFirst("token_type")?.Value;

			if (tokenType != TokenTypeNames.RefreshToken)
			{
				_logger.LogInformation("Token com tipo {TokenType} utilizado para renovação", tokenType);
				throw new BadRequestException("Utilize o refresh token para a renovação.");
			}

			await _next(context);
			return;
		}

		if (user.Identity is not null && user.Identity.IsAuthenticated)
		{
			string? tokenType = user.FindFirst("token_type")?.Value;

			if (tokenType != TokenTypeNames.AccessToken)
			{
				_logger.LogInformation("Token com tipo {TokenType} utilizado para acesso", tokenType);
				throw new BadRequestException("Utilize o token de acesso para acessar o recurso.");
			}
		}

		await _next(context);
	}
}