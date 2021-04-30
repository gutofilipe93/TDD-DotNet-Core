using System;
using Xunit;
using NerdStore.Vendas.Application.Commands;
using System.Linq;
using NerdStore.Vendas.Domain;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class AdicionaItemPedidoCommandTest
    {
        [Fact(DisplayName = "Adicionar item command valido")] 
        [Trait("Categoria", "Vendas - Pedido commands")] 
        public void AdicionaItemPedidoCommand_CommandoEstaValido_DevePassarNaValidacao() 
        { 
          // Arrange 
          var pedidoCommand = new AdicionaItemPedidoCommand(Guid.NewGuid(),Guid.NewGuid(), "Produto teste", 2,100);
          
          // Act 
          var result = pedidoCommand.EhValido();
          
          // Assert 
          Assert.True(result);
        } 

        [Fact(DisplayName = "Adicionar item command valido")] 
        [Trait("Categoria", "Vendas - Pedido commands")] 
        public void AdicionaItemPedidoCommand_CommandoEstaInvalido_NaoDevePassarNaValidacao() 
        { 
          // Arrange 
          var pedidoCommand = new AdicionaItemPedidoCommand(Guid.Empty,Guid.Empty, "", 0,0);
          
          // Act 
          var result = pedidoCommand.EhValido();
          
          // Assert 
          Assert.False(result);
          Assert.Contains(AdicionarItemPedidoValidation.IdClienteErroMsg, pedidoCommand.ValidationResult.Errors.Select(c => c.ErrorMessage));
          Assert.Contains(AdicionarItemPedidoValidation.IdProdutoErroMsg, pedidoCommand.ValidationResult.Errors.Select(c => c.ErrorMessage));
          Assert.Contains(AdicionarItemPedidoValidation.NomeErroMsg, pedidoCommand.ValidationResult.Errors.Select(c => c.ErrorMessage));
          Assert.Contains(AdicionarItemPedidoValidation.QtdMinErroMsg, pedidoCommand.ValidationResult.Errors.Select(c => c.ErrorMessage));
          Assert.Contains(AdicionarItemPedidoValidation.ValorErroMsg, pedidoCommand.ValidationResult.Errors.Select(c => c.ErrorMessage));
        } 
        [Fact(DisplayName = "Adicionar item command unidade a cima do permitido")] 
        [Trait("Categoria", "Vendas - Pedido commands")] 
        public void AdicionaItemPedidoCommand_QuantidadeUnidadesSuperiorAoPermitido_NaoDevePassarNaValidacao() 
        { 
          // Arrange 
          var pedidoCommand = new AdicionaItemPedidoCommand(Guid.NewGuid(),Guid.NewGuid(), "Produto teste", Pedido.MAX_UNIDADES_ITEM+1,100);
          
          // Act 
          var result = pedidoCommand.EhValido();
          
          // Assert 
          Assert.False(result);
          Assert.Contains(AdicionarItemPedidoValidation.QtdMaxErroMsg, pedidoCommand.ValidationResult.Errors.Select(c => c.ErrorMessage));          
        } 
    }

    
}