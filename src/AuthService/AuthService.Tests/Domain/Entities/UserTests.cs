using AuthService.Domain.Entities;

namespace AuthService.Tests.Domain.Entities;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var name = "Minha Empresa";
        var email = "empresa@teste.com";
        var password = "SenhaForte123";

        // Act
        var result = User.Create(name, email, password);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(name, result.Value!.Name.Value);
        Assert.AreEqual(email, result.Value!.Email.Value);
    }

    [TestMethod]
    public void Create_ShouldFail_WhenNameIsInvalid()
    {
        // Arrange
        var name = ""; // inválido
        var email = "empresa@teste.com";
        var password = "SenhaForte123";

        // Act
        var result = User.Create(name, email, password);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A empresa precisa ter um nome", result.Error);
    }

    [TestMethod]
    public void Create_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var name = "Minha Empresa";
        var email = "email_invalido";
        var password = "SenhaForte123";

        // Act
        var result = User.Create(name, email, password);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Forneça um email válido", result.Error);
    }

    [TestMethod]
    public void Create_ShouldFail_WhenPasswordIsInvalid()
    {
        // Arrange
        var name = "Minha Empresa";
        var email = "empresa@teste.com";
        var password = ""; // inválido

        // Act
        var result = User.Create(name, email, password);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A senha deve ser informada", result.Error);
    }
}