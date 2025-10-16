using AuthService.Domain.ValueObjects;

namespace AuthService.Tests.Domain.ValueObjects;

[TestClass]
public class NameTests
{
    [TestMethod]
    public void Create_DeveRetornarSucesso_QuandoNomeForValido()
    {
        var nome = "Minha Empresa";
        
        var result = Name.Create(nome);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(nome, result.Value.Value);
    }

    [TestMethod]
    public void Create_DeveRetornarFalha_QuandoNomeForVazio()
    {
        var nome = "";
        
        var result = Name.Create(nome);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Adicione um nome válido", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public void Create_DeveRetornarFalha_QuandoNomeForNulo()
    {
        string? nome = null;
        
        var result = Name.Create(nome!);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Adicione um nome válido", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public void Create_DeveRetornarFalha_QuandoNomeForEspacosEmBranco()
    {
        var nome = "    ";
        
        var result = Name.Create(nome);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Adicione um nome válido", result.Errors!.FirstOrDefault());
    }
}