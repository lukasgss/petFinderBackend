using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Images;
using Application.Services.General.Images;
using Domain.Entities;
using Domain.ValueObjects;
using NSubstitute;
using Tests.EntityGenerators;

namespace Tests.UnitTests.Images;

public class PetImageSubmissionServiceTests
{
	private readonly IAwsS3Client _awsS3ClientMock;
	private readonly IIdConverterService _idConverterServiceMock;
	private readonly IPetImageSubmissionService _sut;

	private static readonly Pet Pet = PetGenerator.GeneratePet();
	private static readonly CreatePetRequest CreatePetRequest = PetGenerator.GenerateCreatePetRequest();

	private static readonly AwsS3ImageResponse S3FailImageResponse =
		AwsS3ImageGenerator.GenerateFailS3ImageResponse();

	public PetImageSubmissionServiceTests()
	{
		IImageProcessingService imageProcessingServiceMock = Substitute.For<IImageProcessingService>();
		_awsS3ClientMock = Substitute.For<IAwsS3Client>();
		_idConverterServiceMock = Substitute.For<IIdConverterService>();
		_sut = new PetImageSubmissionService(imageProcessingServiceMock, _awsS3ClientMock, _idConverterServiceMock);
	}

	[Fact]
	public async Task Failed_Pet_Image_Upload_Throws_InternalServerErrorException()
	{
		_awsS3ClientMock
			.UploadPetImageAsync(Arg.Any<MemoryStream>(), CreatePetRequest.Images.First(), Arg.Any<string>())
			.Returns(S3FailImageResponse);

		async Task Result() => await _sut.UploadPetImageAsync(Pet.Id, CreatePetRequest.Images);

		var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
		Assert.Equal("Não foi possível fazer upload da imagem, tente novamente mais tarde.", exception.Message);
	}

	[Fact]
	public async Task Failed_Delete_Pet_Image_Throws_InternalServerErrorException()
	{
		const string hashedId = "hashedId";
		_idConverterServiceMock.ConvertGuidToShortId(Pet.Id, Arg.Any<int>()).Returns(hashedId);
		_awsS3ClientMock.DeletePetImageAsync(hashedId).Returns(S3FailImageResponse);
		List<PetImage> petImages = new()
		{
			new PetImage() { Id = 1, Pet = PetGenerator.GeneratePet(), ImageUrl = "imageUrl" }
		};

		async Task Result() => await _sut.DeletePetImageAsync(Pet.Id, petImages);

		var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
		Assert.Equal("Não foi possível excluir a imagem do animal, tente novamente mais tarde.", exception.Message);
	}
}