using AutoFixture;
using AutoFixture.AutoMoq;
using Features.Clientes;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Features.Tests.AutoFixture
{
    [Collection(nameof(ClienteBogusCollection))]
    public class ClienteServiceAutoFixtureTests
    {
        readonly ClienteTestsBogusFixture _clienteTestsBogus;
        private Fixture _fixture = new Fixture();
        public ClienteServiceAutoFixtureTests(ClienteTestsBogusFixture clienteTestsFixture)
        {
            _clienteTestsBogus = clienteTestsFixture;

            _fixture.Customize(new AutoMoqCustomization());

            //_policyRegistry = new PolicyRegistry
            //{
            //    { "getTeste", Policy.NoOpAsync<Pessoa>() },
            //};
            //_fixture.Inject<PolicyRegistry>(_policyRegistry); -> Quaso precise injetar um repositorio que não funcione corretamente no autoMocker
        }

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service AutoMock Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            // Arrange
            var cliente = _clienteTestsBogus.GerarClienteValido();
            
            var clienteService = _fixture.Create<ClienteService>();

            // Act
            clienteService.Adicionar(cliente);

            // Assert
            Assert.True(cliente.EhValido());
            _fixture.Freeze<Mock<IClienteRepository>>().Verify(r => r.Adicionar(cliente), Times.Once);
            _fixture.Freeze<Mock<IMediator>>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service AutoMock Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            // Arrange
            var cliente = _clienteTestsBogus.GerarClienteInvalido();
            var clienteService = _fixture.Create<ClienteService>();

            // Act
            clienteService.Adicionar(cliente);

            // Assert
            Assert.False(cliente.EhValido());
            _fixture.Freeze<Mock<IClienteRepository>>().Verify(r => r.Adicionar(cliente), Times.Never);
            _fixture.Freeze<Mock<IMediator>>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service AutoMock Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            // Arrange
            _fixture.Freeze<Mock<IClienteRepository>>().Setup(c => c.ObterTodos())
                                                       .Returns(_clienteTestsBogus.ObterClientesVariados());

            var clienteService = _fixture.Create<ClienteService>();

            // Act
            var clientes = clienteService.ObterTodosAtivos();

            // Assert 
            _fixture.Freeze<Mock<IClienteRepository>>().Verify(r => r.ObterTodos(), Times.Once);
            Assert.True(clientes.Any());
            Assert.False(clientes.Count(c => !c.Ativo) > 0);
        }
    }
}
