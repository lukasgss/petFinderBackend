using Application.Common.Interfaces.ExternalServices;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators;

public static class AwsS3ImageGenerator
{
    public static AwsS3ImageResponse GenerateSuccessS3ImageResponse()
    {
        return new AwsS3ImageResponse()
        {
            Success = Constants.S3ImagesData.Success,
            PublicUrl = Constants.S3ImagesData.PublicUrl
        };
    }

    public static AwsS3ImageResponse GenerateFailS3ImageResponse()
    {
        return new AwsS3ImageResponse()
        {
            Success = false,
            PublicUrl = null
        };
    }
}