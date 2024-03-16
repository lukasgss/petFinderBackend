using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Services.Authentication;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class CheckTokenTypeMiddleware
{
	private readonly RequestDelegate _next;
	private const string RefreshTokenEndpoint = "/api/users/refresh";

	public CheckTokenTypeMiddleware(RequestDelegate next)
	{
		_next = next;
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
				throw new BadRequestException("Utilize o token de acesso para acessar o recurso.");
			}
		}

		await _next(context);
	}
}