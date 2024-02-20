using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly IValueProvider _valueProvider;
	private readonly JwtConfig _jwtConfig;

	public JwtTokenGenerator(IValueProvider valueProvider, IOptions<JwtConfig> jwtConfig)
	{
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_jwtConfig = jwtConfig.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
	}

	public string GenerateToken(Guid userId, string fullName)
	{
		SigningCredentials signingCredentials = new(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey)),
			SecurityAlgorithms.HmacSha256);

		Claim[] claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Name, fullName)
		};

		JwtSecurityToken jwtSecurityToken = new(
			issuer: _jwtConfig.Issuer,
			audience: _jwtConfig.Audience,
			claims: claims,
			expires: _valueProvider.Now().AddMinutes(_jwtConfig.ExpiryTimeInMin),
			signingCredentials: signingCredentials);

		return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
	}
}