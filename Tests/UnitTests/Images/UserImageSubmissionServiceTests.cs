using System.IO;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Application.Services.General.Images;
using Domain.Entities;
using NSubstitute;
using Tests.EntityGenerators;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests.Images;

public class UserImageSubmissionServiceTests
{
	private readonly IAwsS3Client _awsS3ClientMock;
	private readonly IUserImageSubmissionService _sut;

	private static readonly User User = UserGenerator.GenerateUser();

	private static readonly AwsS3ImageResponse S3FailImageResponse =
		AwsS3ImageGenerator.GenerateFailS3ImageResponse();

	public UserImageSubmissionServiceTests()
	{
		IImageProcessingService imageProcessingServiceMock = Substitute.For<IImageProcessingService>();
		_awsS3ClientMock = Substitute.For<IAwsS3Client>();
		IIdConverterService idConverterServiceMock = Substitute.For<IIdConverterService>();
		_sut = new UserImageSubmissionService(imageProcessingServiceMock, _awsS3ClientMock, idConverterServiceMock);
	}

	[Fact]
	public async Task Failed_User_Image_Upload_Throws_InternalServerErrorException()
	{
		_awsS3ClientMock
			.UploadUserImageAsync(Arg.Any<MemoryStream>(), Constants.UserData.ImageFile, Arg.Any<string>())
			.Returns(S3FailImageResponse);

		async Task Result() => await _sut.UploadUserImageAsync(User.Id, Constants.UserData.ImageFile);

		var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
		Assert.Equal("Não foi possível fazer upload da imagem, tente novamente mais tarde.", exception.Message);
	}
}