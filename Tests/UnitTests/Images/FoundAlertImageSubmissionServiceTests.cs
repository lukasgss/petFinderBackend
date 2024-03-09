using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Application.Services.General.Images;
using Domain.Entities.Alerts;
using NSubstitute;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;

namespace Tests.UnitTests.Images;

public class FoundAlertImageSubmissionServiceTests
{
	private readonly IFileUploadClient _fileUploadClientMock;
	private readonly IFoundAlertImageSubmissionService _sut;

	private static readonly FoundAnimalAlert FoundAnimalAlert = FoundAnimalAlertGenerator.GenerateFoundAnimalAlert();

	private static readonly CreateFoundAnimalAlertRequest CreateFoundAnimalAlertRequest =
		FoundAnimalAlertGenerator.GenerateCreateFoundAnimalAlertRequest();

	private static readonly AwsS3ImageResponse S3FailImageResponse =
		AwsS3ImageGenerator.GenerateFailS3ImageResponse();

	private static readonly AwsS3ImageResponse S3SuccessImageResponse =
		AwsS3ImageGenerator.GenerateSuccessS3ImageResponse();

	public FoundAlertImageSubmissionServiceTests()
	{
		IImageProcessingService imageProcessingServiceMock = Substitute.For<IImageProcessingService>();
		_fileUploadClientMock = Substitute.For<IFileUploadClient>();
		IIdConverterService idConverterServiceMock = Substitute.For<IIdConverterService>();
		_sut = new FoundAlertImageSubmissionService(
			imageProcessingServiceMock, _fileUploadClientMock, idConverterServiceMock);
	}

	[Fact]
	public async Task Failed_Found_Alert_Image_Upload_Throws_InternalServerErrorException()
	{
		_fileUploadClientMock
			.UploadFoundAlertImageAsync(Arg.Any<MemoryStream>(), CreateFoundAnimalAlertRequest.Images.First(),
				Arg.Any<string>())
			.Returns(S3FailImageResponse);

		async Task Result() =>
			await _sut.UploadImagesAsync(FoundAnimalAlert.Id, CreateFoundAnimalAlertRequest.Images);

		var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
		Assert.Equal("Não foi possível fazer upload da imagem, tente novamente mais tarde.", exception.Message);
	}

	[Fact]
	public async Task Found_Alert_Image_Upload_Returns_Uploaded_Image_Url()
	{
		_fileUploadClientMock
			.UploadFoundAlertImageAsync(Arg.Any<MemoryStream>(), CreateFoundAnimalAlertRequest.Images.First(),
				Arg.Any<string>())
			.Returns(S3SuccessImageResponse);
		IReadOnlyList<string> expectedUrls = new List<string>() { S3SuccessImageResponse.PublicUrl! };

		var uploadedUrls =
			await _sut.UploadImagesAsync(FoundAnimalAlert.Id, CreateFoundAnimalAlertRequest.Images);

		Assert.Equal(expectedUrls, uploadedUrls);
	}
}