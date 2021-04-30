using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Moq.AutoMock;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Domain;
using Xunit;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class PedidoCommandHandlerTests
    {

        private readonly Guid _produtoId;
        private readonly Guid _clienteId;
        private readonly Pedido _pedido;
        private readonly AutoMocker _mocker;
        private readonly PedidoCommandHandler _pedidoHandle;

        public PedidoCommandHandlerTests()
        {
            _produtoId = Guid.NewGuid();
            _clienteId = Guid.NewGuid();
            _pedido = Pedido.PedidoFactory.NovoPedidoRascunho(_clienteId);
            _mocker = new AutoMocker();
            _pedidoHandle = _mocker.CreateInstance<PedidoCommandHandler>();
        }


        [Fact(DisplayName = "Adiionar item novo pedido com sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoPedido_DeveExecutarComSucesso()
        {
            // Arrange 
            var pedidoCommand = new AdicionaItemPedidoCommand(Guid.NewGuid(), Guid.NewGuid(), "Produto teste", 2, 100);            
            _mocker.GetMock<IPedidoRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _pedidoHandle.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.Adicionar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar novo item pedido rascunho com sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoItemAoPedidoRascunho_DeveExecutarComSucesso()
        {
            // Arrange 
            var pedidoItemExistente = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            _pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionaItemPedidoCommand(_clienteId, Guid.NewGuid(), "Produto teste", 2, 100);            
            _mocker.GetMock<IPedidoRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));
            _mocker.GetMock<IPedidoRepository>().Setup(r => r.ObterPedidoRascunhoPorCliente(_clienteId)).Returns(Task.FromResult(_pedido));

            // Act 
            var result = await _pedidoHandle.Handle(pedidoCommand, CancellationToken.None);

            // Assert 
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.AdicionarItem(It.IsAny<PedidoItem>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar item existente ao pedido rascunho com sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_ItemExistenteAoPedidoRascunho_DeveExecutarComSucesso()
        {
            // Arrange 
            var pedidoItemExistente = new PedidoItem(_produtoId, "Produto Xpto", 2, 100);
            _pedido.AdicionarItem(pedidoItemExistente);
            var pedidoCommand = new AdicionaItemPedidoCommand(_clienteId, _produtoId, "Produto Xpto", 2, 100);
            
            _mocker.GetMock<IPedidoRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));
            _mocker.GetMock<IPedidoRepository>().Setup(r => r.ObterPedidoRascunhoPorCliente(_clienteId)).Returns(Task.FromResult(_pedido));

            // Act 
            var result = await _pedidoHandle.Handle(pedidoCommand, CancellationToken.None);

            // Assert 
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.AtualizarItem(It.IsAny<PedidoItem>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar item commad inválido")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_CommandInvalido_DeveRetornarFalsoELancarEventosDeNotificacao()
        {
            // Arrange 
            var pedidoCommand = new AdicionaItemPedidoCommand(Guid.Empty, Guid.Empty, "", 0, 0);

            // Act 
            var result = await _pedidoHandle.Handle(pedidoCommand, CancellationToken.None);

            // Assert 
            Assert.False(result);
            _mocker.GetMock<IMediator>().Verify(x => x.Publish(It.IsAny<INotification>(),CancellationToken.None), Times.Exactly(5));
        }
    }
}