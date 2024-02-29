using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class PetServiceTests
{
	private readonly IPetRepository _petRepositoryMock;
	private readonly IBreedRepository _breedRepositoryMock;
	private readonly ISpeciesRepository _speciesRepositoryMock;
	private readonly IColorRepository _colorRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IVaccineRepository _vaccineRepositoryMock;
	private readonly IImageSubmissionService _imageSubmissionServiceMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IPetService _sut;

	private static readonly Pet Pet = PetGenerator.GeneratePet();
	private static readonly PetResponse ExpectedPetResponse = Pet.ToPetResponse(Pet.Owner, Pet.Colors, Pet.Breed);
	private static readonly User User = UserGenerator.GenerateUser();
	private static readonly Breed Breed = BreedGenerator.GenerateBreed();
	private static readonly Species Species = SpeciesGenerator.GenerateSpecies();
	private static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
	private static readonly CreatePetRequest CreatePetRequest = PetGenerator.GenerateCreatePetRequest();
	private static readonly EditPetRequest EditPetRequest = PetGenerator.GenerateEditPetRequest();

	private static readonly PetVaccinationRequest PetVaccinationRequest =
		VaccinationGenerator.GeneratePetVaccinationRequest();

	private static readonly List<Vaccine> Vaccines = VaccinesGenerator.GenerateListOfVaccines();

	public PetServiceTests()
	{
		_petRepositoryMock = Substitute.For<IPetRepository>();
		_breedRepositoryMock = Substitute.For<IBreedRepository>();
		_speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
		_colorRepositoryMock = Substitute.For<IColorRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_vaccineRepositoryMock = Substitute.For<IVaccineRepository>();
		_imageSubmissionServiceMock = Substitute.For<IImageSubmissionService>();
		_valueProviderMock = Substitute.For<IValueProvider>();

		_sut = new PetService(
			_petRepositoryMock,
			_breedRepositoryMock,
			_speciesRepositoryMock,
			_colorRepositoryMock,
			_userRepositoryMock,
			_vaccineRepositoryMock,
			_imageSubmissionServiceMock,
			_valueProviderMock);
	}

	[Fact]
	public async Task Get_Non_Existent_Pet_By_Id_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).ReturnsNull();

		async Task Result() => await _sut.GetPetBydIdAsync(Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Animal com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Get_Pet_By_Id_Returns_PetResponse()
	{
		_petRepositoryMock.GetPetByIdAsync(Arg.Any<Guid>()).Returns(Pet);

		PetResponse petResponse = await _sut.GetPetBydIdAsync(Pet.Id);

		Assert.Equivalent(ExpectedPetResponse, petResponse);
	}

	[Fact]
	public async Task Create_Pet_With_Non_Existent_Breed_Throws_NotFoundException()
	{
		_breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).ReturnsNull();

		async Task Result() => await _sut.CreatePetAsync(CreatePetRequest, Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Raça especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Pet_With_Non_Existent_Species_Throws_NotFoundException()
	{
		_breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(CreatePetRequest.SpeciesId).ReturnsNull();

		async Task Result() => await _sut.CreatePetAsync(CreatePetRequest, Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Espécie especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Pet_With_Non_Existent_Colors_Throws_NotFoundException()
	{
		_breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(CreatePetRequest.SpeciesId).Returns(Species);
		List<Color> emptyColorList = new();
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreatePetRequest.ColorIds).Returns(emptyColorList);

		async Task Result() => await _sut.CreatePetAsync(CreatePetRequest, Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Pet_While_Authenticated_Returns_Pet_Response_With_Logged_In_User_As_Owner()
	{
		_breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(CreatePetRequest.SpeciesId).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreatePetRequest.ColorIds).Returns(Colors);
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_valueProviderMock.NewGuid().Returns(Pet.Id);
		_imageSubmissionServiceMock.UploadPetImageAsync(Pet.Id, CreatePetRequest.Image).Returns(Pet.Image);

		PetResponse petResponse = await _sut.CreatePetAsync(CreatePetRequest, userId: User.Id);

		Assert.Equivalent(ExpectedPetResponse, petResponse);
	}

	[Fact]
	public async Task Edit_Pet_With_Route_Id_Different_Than_Specified_On_Request_Throws_BadRequestException()
	{
		async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, routeId: Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
	}

	[Fact]
	public async Task Edit_Non_Existent_Pet_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).ReturnsNull();

		async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Animal com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Pet_With_Non_Existent_Breed_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
		_breedRepositoryMock.GetBreedByIdAsync(EditPetRequest.BreedId).ReturnsNull();

		async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Raça especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Pet_With_Non_Existent_Species_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(EditPetRequest.SpeciesId).ReturnsNull();

		async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Espécie especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Pet_With_Non_Existent_Colors_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);
		List<Color> emptyColorsList = new();
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(EditPetRequest.ColorIds).Returns(emptyColorsList);

		async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Pet_Without_Being_Owner_Throws_UnauthorizedException()
	{
		_petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(EditPetRequest.ColorIds).Returns(Colors);
		_userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).Returns(User);
		Guid nonOwnerUserId = Guid.NewGuid();
		_userRepositoryMock.GetUserByIdAsync(nonOwnerUserId).Returns(new User());

		async Task Result() =>
			await _sut.EditPetAsync(EditPetRequest, userId: nonOwnerUserId, routeId: EditPetRequest.Id);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Você não possui permissão para editar dados desse animal.", exception.Message);
	}

	[Fact]
	public async Task Edit_Pet_Returns_Edited_Pet_Response()
	{
		_petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
		_speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(EditPetRequest.ColorIds).Returns(Colors);
		_userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).Returns(User);
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_imageSubmissionServiceMock.UploadPetImageAsync(EditPetRequest.Id, EditPetRequest.Image).Returns(Pet.Image);

		PetResponse editedPetResponse = await _sut.EditPetAsync(EditPetRequest, User.Id, EditPetRequest.Id);

		Assert.Equivalent(ExpectedPetResponse, editedPetResponse);
	}

	[Fact]
	public async Task Delete_Non_Existent_Pet_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).ReturnsNull();

		async Task Result() => await _sut.DeletePetAsync(Constants.PetData.Id, Constants.UserData.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Animal com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Delete_Pet_Without_Being_Its_Owner_Throws_UnauthorizedException()
	{
		_petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);

		async Task Result() => await _sut.DeletePetAsync(Pet.Id, userId: Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Você não possui permissão para excluir o animal.", exception.Message);
	}

	[Fact]
	public async Task Update_Vaccination_Of_Non_Existent_Pet_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(Pet.Id).ReturnsNull();

		async Task Result() => await _sut.UpdateVaccinations(PetVaccinationRequest, Pet.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Animal com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Update_Vaccination_Of_Unowned_Pet_Throws_ForbiddenException()
	{
		Guid differentUserId = Guid.NewGuid();
		_petRepositoryMock.GetPetByIdAsync(Pet.Id).Returns(Pet);

		async Task Result() => await _sut.UpdateVaccinations(PetVaccinationRequest, Pet.Id, differentUserId);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Você não possui permissão para adicionar vacinas ao animal.", exception.Message);
	}

	[Fact]
	public async Task Update_Vaccination_With_Non_Existent_Vaccine_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(Pet.Id).Returns(Pet);
		List<Vaccine> emptyVaccinesList = new();
		_vaccineRepositoryMock.GetMultipleByIdAsync(PetVaccinationRequest.VaccinationIds).Returns(emptyVaccinesList);

		async Task Result() => await _sut.UpdateVaccinations(PetVaccinationRequest, Pet.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma vacina com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Update_Vaccination_With_Vaccine_From_Different_Species_Throws_BadRequestException()
	{
		_petRepositoryMock.GetPetByIdAsync(Pet.Id).Returns(Pet);
		_vaccineRepositoryMock.GetMultipleByIdAsync(PetVaccinationRequest.VaccinationIds).Returns(Vaccines);
		List<Vaccine> vaccinesFromDifferentSpecies = new()
		{
			new Vaccine()
			{
				Species = new List<Species>() { new() { Id = 55 } }
			}
		};
		_vaccineRepositoryMock.GetMultipleByIdAsync(PetVaccinationRequest.VaccinationIds)
			.Returns(vaccinesFromDifferentSpecies);

		async Task Result() => await _sut.UpdateVaccinations(PetVaccinationRequest, Pet.Id, User.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Não é possível adicionar vacinas de outras espécies.", exception.Message);
	}

	[Fact]
	public async Task Update_Vaccination_Returns_Vaccinated_Pet()
	{
		_petRepositoryMock.GetPetByIdAsync(Pet.Id).Returns(Pet);
		_vaccineRepositoryMock.GetMultipleByIdAsync(PetVaccinationRequest.VaccinationIds).Returns(Vaccines);

		PetResponse vaccinatedPet = await _sut.UpdateVaccinations(PetVaccinationRequest, Pet.Id, User.Id);

		Assert.Equivalent(ExpectedPetResponse, vaccinatedPet);
	}
}