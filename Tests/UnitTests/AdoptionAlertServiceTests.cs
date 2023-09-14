using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using Domain.Entities.Alerts;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class AdoptionAlertServiceTests
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepositoryMock;
    private readonly IPetRepository _petRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IDateTimeProvider _dateTimeProviderMock;
    private readonly IGuidProvider _guidProviderMock;
    private readonly IAdoptionAlertService _sut;

    private static readonly AdoptionAlert AdoptionAlert = AdoptionAlertGenerator.GenerateNonAdoptedAdoptionAlert();
    private static readonly Pet Pet = PetGenerator.GeneratePet();
    private static readonly User User = UserGenerator.GenerateUser();

    private static readonly CreateAdoptionAlertRequest CreateAlertRequest =
        AdoptionAlertGenerator.GenerateCreateAdoptionAlertRequest();

    private static readonly AdoptionAlertResponse
        NonAdoptedAdoptionAlertResponse = AdoptionAlertGenerator.GenerateNonAdoptedAdoptionAlertResponse();

    public AdoptionAlertServiceTests()
    {
        _adoptionAlertRepositoryMock = Substitute.For<IAdoptionAlertRepository>();
        _petRepositoryMock = Substitute.For<IPetRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _guidProviderMock = Substitute.For<IGuidProvider>();

        _sut = new AdoptionAlertService(
            _adoptionAlertRepositoryMock,
            _petRepositoryMock,
            _userRepositoryMock,
            _dateTimeProviderMock,
            _guidProviderMock);
    }

    [Fact]
    public async Task Get_Non_Existent_Alert_By_Id_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(AdoptionAlert.Id).ReturnsNull();

        async Task Result() => await _sut.GetByIdAsync(AdoptionAlert.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de adoção com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Adoption_Alert_By_Id_Returns_Alert()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(AdoptionAlert.Id).Returns(AdoptionAlert);

        AdoptionAlertResponse adoptionAlertResponse = await _sut.GetByIdAsync(AdoptionAlert.Id);

        Assert.Equivalent(NonAdoptedAdoptionAlertResponse, adoptionAlertResponse);
    }

    [Fact]
    public async Task Create_Adoption_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).ReturnsNull();

        async Task Result() => await _sut.CreateAsync(CreateAlertRequest, Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Adoption_Alert_Not_Being_Owner_Of_Pet_Throws_UnauthorizedException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);
        Guid userIdThatDoesNotOwnPet = Guid.NewGuid();

        async Task Result() => await _sut.CreateAsync(CreateAlertRequest, userIdThatDoesNotOwnPet);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível cadastrar adoções para animais em que não é dono.", exception.Message);
    }

    [Fact]
    public async Task Create_Adoption_Alert_Returns_Created_Alert()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);
        _userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).Returns(User);
        _guidProviderMock.NewGuid().Returns(Constants.AdoptionAlertData.Id);
        _dateTimeProviderMock.UtcNow().Returns(Constants.AdoptionAlertData.RegistrationDate);

        AdoptionAlertResponse adoptionAlertResponse = await _sut.CreateAsync(CreateAlertRequest, Constants.UserData.Id);

        Assert.Equivalent(NonAdoptedAdoptionAlertResponse, adoptionAlertResponse);
    }
}