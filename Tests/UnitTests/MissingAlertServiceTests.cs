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
using Tests.TestUtils.Constants;

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
        _missingAlertRepositoryMock.GetByIdAsync(missingAlert.Id).ReturnsNull();

        async Task Result() => await _sut.GetByIdAsync(missingAlert.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de desaparecimento com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Missing_Alert_By_Id_Returns_Missing_Alert()
    {
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetByIdAsync(missingAlert.Id).Returns(missingAlert);
        MissingAlertResponse expectedMissingAlert = missingAlert.ConvertToMissingAlertResponse();

        MissingAlertResponse missingAlertResponse = await _sut.GetByIdAsync(missingAlert.Id);

        Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
    }

    [Fact]
    public async Task Create_Missing_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlert();
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).ReturnsNull();

        async Task Result() => await _sut.CreateAsync(createMissingAlertRequest, userId: null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Missing_Alert_For_Other_Users_Throws_ForbiddenException()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlert();
        Pet searchedPet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).Returns(searchedPet);

        async Task Result() => await _sut.CreateAsync(createMissingAlertRequest, userId: Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
        Assert.Equal("Não é possível criar alertas para outros usuários.", exception.Message);
    }

    [Fact]
    public async Task Create_Missing_Alert_Without_Owner_Returns_MissingAlert()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlertWithoutOwner();
        var expectedMissingAlert =
            MissingAlertGenerator.GenerateMissingAlertResponseWithoutOwner();
        Pet returnedPet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).Returns(returnedPet);
        _dateTimeProviderMock.UtcNow().Returns(expectedMissingAlert.RegistrationDate);
        _guidProviderMock.NewGuid().Returns(expectedMissingAlert.Id);

        MissingAlertResponse missingAlertResponse =
            await _sut.CreateAsync(createMissingAlertRequest, userId: null);

        Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
    }

    [Fact]
    public async Task Create_Missing_Alert_With_Owner_Returns_Missing_Alert_With_Owner()
    {
        var createMissingAlertRequest = MissingAlertGenerator.GenerateCreateMissingAlert();
        var expectedMissingAlert =
            MissingAlertGenerator.GenerateMissingAlertResponse();
        Pet returnedPet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(createMissingAlertRequest.PetId).Returns(returnedPet);
        User returnedUser = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(returnedUser.Id).Returns(returnedUser);
        _dateTimeProviderMock.UtcNow().Returns(expectedMissingAlert.RegistrationDate);
        _guidProviderMock.NewGuid().Returns(expectedMissingAlert.Id);

        MissingAlertResponse missingAlertResponse =
            await _sut.CreateAsync(createMissingAlertRequest, userId: createMissingAlertRequest.UserId);

        Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
    }

    [Fact]
    public async Task Edit_Alert_With_Route_Id_Different_Than_Specified_On_Request_Throws_BadRequestException()
    {
        EditMissingAlertRequest editMissingAlertRequest = MissingAlertGenerator.GenerateEditMissingAlertRequest();

        async Task Result() => await _sut.EditAsync(
            editMissingAlertRequest: editMissingAlertRequest,
            userId: Constants.UserData.Id,
            routeId: Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Edit_Non_Existent_Missing_Alert_Throws_NotFoundException()
    {
        EditMissingAlertRequest editMissingAlertRequest = MissingAlertGenerator.GenerateEditMissingAlertRequest();
        _missingAlertRepositoryMock.GetByIdAsync(editMissingAlertRequest.Id).ReturnsNull();

        async Task Result() => await _sut.EditAsync(
            editMissingAlertRequest: editMissingAlertRequest,
            userId: editMissingAlertRequest.UserId,
            routeId: editMissingAlertRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Missing_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
    {
        EditMissingAlertRequest editMissingAlertRequest = MissingAlertGenerator.GenerateEditMissingAlertRequest();
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetByIdAsync(editMissingAlertRequest.Id).Returns(missingAlert);
        _petRepositoryMock.GetPetByIdAsync(editMissingAlertRequest.PetId).ReturnsNull();

        async Task Result() => await _sut.EditAsync(
            editMissingAlertRequest: editMissingAlertRequest,
            userId: editMissingAlertRequest.UserId,
            routeId: editMissingAlertRequest.Id);
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Not_Owned_Missing_Alerts_Throws_ForbiddenException()
    {
        EditMissingAlertRequest editMissingAlertRequest = MissingAlertGenerator.GenerateEditMissingAlertRequest();
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetByIdAsync(editMissingAlertRequest.Id).Returns(missingAlert);
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editMissingAlertRequest.PetId).Returns(pet);

        async Task Result() => await _sut.EditAsync(
            editMissingAlertRequest: editMissingAlertRequest,
            userId: Guid.NewGuid(),
            routeId: editMissingAlertRequest.Id);
        
        var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
        Assert.Equal("Não é possível editar alertas de outros usuários.", exception.Message);
    }

    [Fact]
    public async Task Edit_Missing_Alert_With_Non_Existent_User_Throws_NotFoundException()
    {
        EditMissingAlertRequest editMissingAlertRequest = MissingAlertGenerator.GenerateEditMissingAlertRequest();
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetByIdAsync(editMissingAlertRequest.Id).Returns(missingAlert);
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editMissingAlertRequest.PetId).Returns(pet);
        User user = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(user.Id).ReturnsNull();
        
        async Task Result() => await _sut.EditAsync(
            editMissingAlertRequest: editMissingAlertRequest,
            userId: user.Id,
            routeId: editMissingAlertRequest.Id);
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Usuário com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Missing_Alert_Returns_Edited_Missing_Alert()
    {
        EditMissingAlertRequest editMissingAlertRequest = MissingAlertGenerator.GenerateEditMissingAlertRequest();
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetByIdAsync(editMissingAlertRequest.Id).Returns(missingAlert);
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editMissingAlertRequest.PetId).Returns(pet);
        User user = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(user.Id).Returns(user);
        MissingAlertResponse expectedMissingAlertResponse = MissingAlertGenerator.GenerateMissingAlertResponse();

        MissingAlertResponse missingAlertResponse = await _sut.EditAsync(
            editMissingAlertRequest: editMissingAlertRequest,
            userId: user.Id,
            routeId: editMissingAlertRequest.Id);
        
        Assert.Equivalent(expectedMissingAlertResponse, missingAlertResponse);
    }

    [Fact]
    public async Task Delete_Non_Existent_Missing_Alert_Throws_NotFoundException()
    {
        _missingAlertRepositoryMock.GetByIdAsync(Constants.MissingAlertData.Id).ReturnsNull();

        async Task Result() => await _sut.DeleteAsync(Constants.MissingAlertData.Id, Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Delete_Not_Owned_Missing_Alerts_Throws_ForbiddenException()
    {
        MissingAlert missingAlert = MissingAlertGenerator.GenerateMissingAlert();
        _missingAlertRepositoryMock.GetByIdAsync(Constants.MissingAlertData.Id).Returns(missingAlert);

        async Task Result() => await _sut.DeleteAsync(missingAlert.Id, userId: Guid.NewGuid());
        
        var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
        Assert.Equal("Não é possível excluir alertas de outros usuários.", exception.Message);
    }
}