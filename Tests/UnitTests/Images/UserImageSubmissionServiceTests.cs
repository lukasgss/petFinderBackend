using System.IO;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Users.DTOs;
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
	private readonly IFileUploadClient _fileUploadClientMock;
	private readonly IUserImageSubmissionService _sut;

	private static readonly User User = UserGenerator.GenerateUser();
	private static readonly CreateUserRequest CreateUserRequest = UserGenerator.GenerateCreateUserRequest();

	private static readonly AwsS3ImageResponse S3FailImageResponse =
		AwsS3ImageGenerator.GenerateFailS3ImageResponse();

	private static readonly AwsS3ImageResponse S3SuccessImageResponse =
		AwsS3ImageGenerator.GenerateSuccessS3ImageResponse();

	public UserImageSubmissionServiceTests()
	{
		IImageProcessingService imageProcessingServiceMock = Substitute.For<IImageProcessingService>();
		_fileUploadClientMock = Substitute.For<IFileUploadClient>();
		IIdConverterService idConverterServiceMock = Substitute.For<IIdConverterService>();
		_sut = new UserImageSubmissionService(imageProcessingServiceMock, _fileUploadClientMock,
			idConverterServiceMock);
	}

	[Fact]
	public async Task Failed_User_Image_Upload_Throws_InternalServerErrorException()
	{
		_fileUploadClientMock
			.UploadUserImageAsync(Arg.Any<MemoryStream>(), Constants.UserData.ImageFile, Arg.Any<string>())
			.Returns(S3FailImageResponse);

		async Task Result() => await _sut.UploadUserImageAsync(User.Id, Constants.UserData.ImageFile);

		var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
		Assert.Equal("Não foi possível fazer upload da imagem, tente novamente mais tarde.", exception.Message);
	}

	[Fact]
	public async Task User_Image_Upload_Returns_Uploaded_Image_Url()
	{
		_fileUploadClientMock
			.UploadUserImageAsync(Arg.Any<MemoryStream>(), CreateUserRequest.Image,
				Arg.Any<string>())
			.Returns(S3SuccessImageResponse);

		string uploadedUrl = await _sut.UploadUserImageAsync(User.Id, CreateUserRequest.Image);

		Assert.Equal(S3SuccessImageResponse.PublicUrl, uploadedUrl);
	}
}