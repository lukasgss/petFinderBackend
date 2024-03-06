using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Application.Common.Exceptions;
using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjections;

public static class AwsSetup
{
	public static void ConfigureAws(this IServiceCollection services, IConfiguration configuration)
	{
		AWSOptions awsOptions = configuration.GetAWSOptions();
		AwsCredentialData? credentialData = configuration.GetSection("AWSCredentials").Get<AwsCredentialData>();
		if (credentialData is null)
		{
			throw new InternalServerErrorException("Não foi possível obter credenciais AWS");
		}

		awsOptions.Credentials =
			new BasicAWSCredentials(credentialData.AccessKey, credentialData.SecretKey);

		services.AddDefaultAWSOptions(awsOptions);
		services.AddAWSService<IAmazonS3>();
	}
}