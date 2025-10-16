using AuthService.Domain.Entities;

namespace AuthService.Tests.Domain.Entities;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenValidData()
    {
        var email = "empresa@teste.com";
        var password = "SenhaForte123";
        
        var result = User.Create(email, password);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(email, result.Value!.Email.Value);
    }

    [TestMethod]
    public void Create_ShouldFail_WhenEmailIsInvalid()
    {
        var email = "email_invalido";
        var password = "SenhaForte123";
        
        var result = User.Create(email, password);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Forneça um email válido", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public void Create_ShouldFail_WhenPasswordIsInvalid()
    {
        var email = "empresa@teste.com";
        var password = "";
        
        var result = User.Create(email, password);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A senha deve ser informada", result.Errors!.FirstOrDefault());
    }
}