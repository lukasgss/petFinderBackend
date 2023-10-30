using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjections;

public static class AwsSetup
{
    public static void ConfigureAws(this IServiceCollection services, IConfiguration configuration)
    {
        AWSOptions awsOptions = configuration.GetAWSOptions();
        AwsCredentialData credentialData = configuration.GetSection("AWSCredentials").Get<AwsCredentialData>();
        awsOptions.Credentials =
            new BasicAWSCredentials(credentialData.AccessKey, credentialData.SecretKey);

        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();
    }
}