using InvoiceService.Domain.ValueObjects.Customers;
[assembly: DoNotParallelize]

namespace InvoiceService.Tests.Domain.ValueObjects.Customers;

[TestClass]
public class DocumentTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_ForValidCPF()
    {
        var cpf = "123.456.789-01";
        
        var result = Document.Create(cpf);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("12345678901", result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_ForValidCNPJ()
    {
        var cnpj = "12.345.678/0001-90";
        
        var result = Document.Create(cnpj);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("12345678000190", result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForEmptyValue()
    {
        var empty = "";
        
        var result = Document.Create(empty);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Documento é obrigatório"));
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForInvalidLength()
    {
        var invalidDoc = "123456";
        
        var result = Document.Create(invalidDoc);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ)"));
    }

    [TestMethod]
    public void ToString_ShouldReturnValue()
    {
        var cpf = "12345678901";
        var result = Document.Create(cpf);
        
        var docString = result.Value!.ToString();
        
        Assert.AreEqual("12345678901", docString);
    }
}