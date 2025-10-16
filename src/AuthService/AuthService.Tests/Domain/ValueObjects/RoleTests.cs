using AuthService.Domain.ValueObjects;

namespace AuthService.Tests.Domain.ValueObjects;

[TestClass]
public class RoleTests
{
    [TestMethod]
        public void Create_DeveRetornarSucesso_QuandoNomeForValido()
        {
            var result = Role.Create("admin");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("ADMIN", result.Value.Name);
        }

        [TestMethod]
        public void Create_DeveRetornarFalha_QuandoNomeForNuloOuVazio()
        {
            var result1 = Role.Create(null!);
            var result2 = Role.Create("");
            var result3 = Role.Create("   ");

            Assert.IsFalse(result1.IsSuccess);
            Assert.AreEqual("O nome da role é necessária", result1.Errors!.FirstOrDefault());

            Assert.IsFalse(result2.IsSuccess);
            Assert.AreEqual("O nome da role é necessária", result2.Errors!.FirstOrDefault());

            Assert.IsFalse(result3.IsSuccess);
            Assert.AreEqual("O nome da role é necessária", result3.Errors!.FirstOrDefault());
        }

        [TestMethod]
        public void IsActive_DeveRetornarTrue_QuandoValidUntilForNulo()
        {
            var role = Role.Create("user").Value;

            Assert.IsTrue(role?.IsActive());
        }

        [TestMethod]
        public void IsActive_DeveRetornarTrue_QuandoValidUntilForFuturo()
        {
            var role = Role.Create("manager", DateTime.UtcNow.AddDays(1)).Value;

            Assert.IsTrue(role?.IsActive());
        }

        [TestMethod]
        public void IsActive_DeveRetornarFalse_QuandoValidUntilForPassado()
        {
            var role = Role.Create("viewer", DateTime.UtcNow.AddDays(-1)).Value;

            Assert.IsFalse(role?.IsActive());
        }

        [TestMethod]
        public void ExtendValidity_DeveAtualizar_QuandoNovaDataForMaior()
        {
            var role = Role.Create("editor", DateTime.UtcNow.AddDays(1)).Value;
            var novaData = DateTime.UtcNow.AddDays(10);

            role?.ExtendValidity(novaData);

            Assert.AreEqual(novaData, role?.ValidUntil);
        }

        [TestMethod]
        public void ExtendValidity_NaoDeveAtualizar_QuandoNovaDataForMenor()
        {
            var validadeAntiga = DateTime.UtcNow.AddDays(10);
            var role = Role.Create("auditor", validadeAntiga).Value;
            var novaData = DateTime.UtcNow.AddDays(1);

            role?.ExtendValidity(novaData);

            Assert.AreEqual(validadeAntiga, role?.ValidUntil);
        }

        [TestMethod]
        public void ExtendValidity_DeveDefinirValidade_QuandoNaoTiverValidadeAnterior()
        {
            var role = Role.Create("temp").Value;
            var novaData = DateTime.UtcNow.AddDays(5);

            role?.ExtendValidity(novaData);

            Assert.AreEqual(novaData, role?.ValidUntil);
        }
}