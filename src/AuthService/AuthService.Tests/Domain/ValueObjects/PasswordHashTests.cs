using AuthService.Domain.ValueObjects;

namespace AuthService.Tests.Domain.ValueObjects;

[TestClass]
public class PasswordHashTests
{
    [TestMethod]
    public void Create_DeveRetornarSucesso_QuandoSenhaForValida()
    {
        var senha = "MinhaSenhaSegura123";
        
        var result = PasswordHash.Create(senha);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(senha, result.Value.Value);
    }

    [TestMethod]
    public void Create_DeveRetornarFalha_QuandoSenhaForVazia()
    {
        var senha = "";
        
        var result = PasswordHash.Create(senha);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A senha deve ser informada", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public void Create_DeveRetornarFalha_QuandoSenhaForNula()
    {
        string? senha = null;
        
        var result = PasswordHash.Create(senha!);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A senha deve ser informada", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public void Create_DeveRetornarFalha_QuandoSenhaForEspacosEmBranco()
    {
        var senha = "     ";
        
        var result = PasswordHash.Create(senha);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A senha deve ser informada", result.Errors!.FirstOrDefault());
    }
}