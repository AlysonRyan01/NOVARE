using AuthService.Domain.ValueObjects;

namespace AuthService.Tests.Domain.ValueObjects;

[TestClass]
public class EmailTests
{
    [TestMethod]
        public void Create_DeveRetornarSucesso_QuandoEmailForValido()
        {
            var emailValido = "usuario@dominio.com";
            
            var result = Email.Create(emailValido);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(emailValido, result.Value.Value);
        }

        [TestMethod]
        public void Create_DeveRetornarFalha_QuandoEmailForNulo()
        {
            string? email = null;

            var result = Email.Create(email!);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Forneça um email válido", result.Error);
        }

        [TestMethod]
        public void Create_DeveRetornarFalha_QuandoEmailForVazio()
        {
            var result = Email.Create("");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Forneça um email válido", result.Error);
        }

        [TestMethod]
        public void Create_DeveRetornarFalha_QuandoEmailForEspacosEmBranco()
        {
            var result = Email.Create("   ");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Forneça um email válido", result.Error);
        }

        [TestMethod]
        [DataRow("usuario@")] 
        [DataRow("@dominio.com")]
        [DataRow("usuario.dominio.com")]
        [DataRow("usuario@dominio")] 
        public void Create_DeveRetornarFalha_QuandoEmailForInvalido(string emailInvalido)
        {
            var result = Email.Create(emailInvalido);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Forneça um email válido", result.Error);
        }

        [TestMethod]
        public void ToString_DeveRetornarMesmoValorDoEmail()
        {
            var emailValido = "usuario@teste.com";
            var result = Email.Create(emailValido);

            var valorString = result.Value?.ToString();

            Assert.AreEqual(emailValido, valorString);
        }
}