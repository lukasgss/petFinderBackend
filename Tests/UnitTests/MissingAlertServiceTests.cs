using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.DTOs;
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

namespace Tests.UnitTests;

public class MissingAlertServiceTests
{
    private readonly IMissingAlertRepository _missingAlertRepositoryMock;
    private readonly IPetRepository _petRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IGuidProvider _guidProviderMock;
    private readonly IDateTimeProvider _dateTimeProviderMock;
    private readonly IMissingAlertService _sut;

    public MissingAlertServiceTests()
    {
        _missingAlertRepositoryMock = Substitute.For<IMissingAlertRepository>();
        _petRepositoryMock = Substitute.For<IPetRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _guidProviderMock = Substitute.For<IGuidProvider>();
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

        _sut = new MissingAlertService(
            _missingAlertRepositoryMock,
            _petRepositoryMock,
            _userRepositoryMock,
            _guidProviderMock,
            _dateTimeProviderMock);
    }

    [Fact]
    public async Task Get_Non_Existent_Missing_Alert_Throws_NotFoundException()
    {
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetMissingAlertByIdAsync(missingAlert.Id).ReturnsNull();

        async Task Result() => await _sut.GetMissingAlertByIdAsync(missingAlert.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de desaparecimento com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Missing_Alert_By_Id_Returns_Missing_Alert()
    {
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetMissingAlertByIdAsync(missingAlert.Id).Returns(missingAlert);
        MissingAlertResponse expectedMissingAlert = missingAlert.ConvertToMissingAlertResponse();

        MissingAlertResponse missingAlertResponse = await _sut.GetMissingAlertByIdAsync(missingAlert.Id);

        Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
    }

    [Fact]
    public async Task Create_Missing_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlert();
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).ReturnsNull();

        async Task Result() => await _sut.CreateMissingAlertAsync(createMissingAlertRequest, userId: null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Missing_Alert_For_Other_Users_Throws_ForbiddenException()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlert();
        Pet searchedPet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).Returns(searchedPet);

        async Task Result() => await _sut.CreateMissingAlertAsync(createMissingAlertRequest, userId: Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
        Assert.Equal("Não é possível criar alertas para outros usuários.", exception.Message);
    }

    [Fact]
    public async Task Create_Missing_Alert_Without_Owner_Returns_MissingAlert()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlertWithoutOwner();
        var expectedMissingAlert =
            MissingAlertGenerator.GenerateMissingAlertResponseFromCreateRequest(createMissingAlertRequest);
        Pet returnedPet = PetGenerator.GeneratePetWithId(expectedMissingAlert.Pet.Id);
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).Returns(returnedPet);
        _dateTimeProviderMock.UtcNow().Returns(expectedMissingAlert.RegistrationDate);
        _guidProviderMock.NewGuid().Returns(expectedMissingAlert.Id);

        MissingAlertResponse missingAlertResponse =
            await _sut.CreateMissingAlertAsync(createMissingAlertRequest, userId: null);

        Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
    }

    [Fact]
    public async Task Create_Missing_Alert_With_Owner_Returns_Missing_Alert_With_Owner()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlert();
        var expectedMissingAlert =
            MissingAlertGenerator.GenerateMissingAlertResponseFromCreateRequest(createMissingAlertRequest);
        Pet returnedPet = PetGenerator.GeneratePetWithId(expectedMissingAlert.Pet.Id);
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).Returns(returnedPet);
        User returnedUser = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(Arg.Any<Guid>()).Returns(returnedUser);
        _dateTimeProviderMock.UtcNow().Returns(expectedMissingAlert.RegistrationDate);
        _guidProviderMock.NewGuid().Returns(expectedMissingAlert.Id);

        MissingAlertResponse missingAlertResponse =
            await _sut.CreateMissingAlertAsync(createMissingAlertRequest, userId: createMissingAlertRequest.UserId);

        Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
    }
}