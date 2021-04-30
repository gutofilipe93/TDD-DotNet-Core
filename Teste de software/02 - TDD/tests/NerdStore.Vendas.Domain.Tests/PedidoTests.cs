using NerdStore.Core.DomainObjects;
using System;
using System.Linq;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoTests
    {
        [Fact(DisplayName = "Adicionar Item Novo Pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_NovoPedido_DeveAtualizarValor()
        {
            //Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto testes", 2, 100);

            //Act
            pedido.AdicionarItem(pedidoItem);

            //Assert   
            Assert.Equal(200, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Adicionar Item Pedido Existente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistente_DeveIncrementarUnidadesSomarValores()
        {
            //Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto testes", 2, 100);
            pedido.AdicionarItem(pedidoItem);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto teste", 1, 100);


            //Act
            pedido.AdicionarItem(pedidoItem2);

            //Assert   
            Assert.Equal(300, pedido.ValorTotal);
            Assert.Equal(1, pedido.PedidoItems.Count);
            Assert.Equal(3, pedido.PedidoItems.FirstOrDefault(x => x.ProdutoId == produtoId).Quantidade);
        }

        [Fact(DisplayName = "Adicionar item pedido acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_UnidadesItemAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM + 1, 100);
            // Act & Assert
            Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem));
        }

        [Fact(DisplayName = "Adicionar item pedido existente acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistenteSomaUnidadesAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM, 100);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", 1, 100);
            pedido.AdicionarItem(pedidoItem);
            // Act & Assert
            Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem2));
        }


        [Fact(DisplayName = "Atualizar item pedido inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemNaoExisteNaLista_DeveRetornarException()
        {
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItemAtualizado = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 100);
            // Act & Assert
            Assert.Throws<DomainException>(() => pedido.AtualizarItem(pedidoItemAtualizado));
        }

        [Fact(DisplayName = "Atualizar item pedido valido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemValido_DeveAtualizarQuantidade()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem);
            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            var novaQuantidade = pedidoItemAtualizado.Quantidade;

            // Act 
            pedido.AtualizarItem(pedidoItemAtualizado);

            // Assert 
            Assert.Equal(novaQuantidade, pedido.PedidoItems.FirstOrDefault(p => p.ProdutoId == produtoId).Quantidade);
        }


        [Fact(DisplayName = "Atualizar item pedido valido total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_PedidoComProdutosDiferentes_DeveAtualizarValorTotal()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItemExistente1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);
            var pedidoItemExistente2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(pedidoItemExistente1);
            pedido.AdicionarItem(pedidoItemExistente2);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            var totalPedido = (pedidoItemExistente1.Quantidade * pedidoItemExistente1.ValorUnitario) + (pedidoItemAtualizado.Quantidade * pedidoItemAtualizado.ValorUnitario);

            // Act 
            pedido.AtualizarItem(pedidoItemAtualizado);

            // Assert 
            Assert.Equal(totalPedido, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Atualizar item pedido quantidade acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemUnidadesAcimaDoPedido_DeveRetornarException()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItemExistente1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);            
            pedido.AdicionarItem(pedidoItemExistente1);
        
            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM+1, 100);
 
            // Act & Assert 
            Assert.Throws<DomainException>(() => pedido.AtualizarItem(pedidoItemAtualizado));
        }

        [Fact(DisplayName = "Remover item pedido inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemNaoExisteNaLista_DeveRetornarException()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);            
            
            // Act & Assert 
            Assert.Throws<DomainException>(() => pedido.RemoverItem(pedidoItem));
        }

        [Fact(DisplayName = "Remover item pedido deve calcular valor total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemExistente_DeveAtualizarValorTotal()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItemExistente1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);
            var pedidoItemExistente2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(pedidoItemExistente1);
            pedido.AdicionarItem(pedidoItemExistente2);

            var totalPedido = pedidoItemExistente2.Quantidade * pedidoItemExistente2.ValorUnitario;

            // Act 
            pedido.RemoverItem(pedidoItemExistente1);

            // Assert 
            Assert.Equal(totalPedido, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar voucher valido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void Pedido_AplicarVoucherValido_DeveRetornarSemErros()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var voucher = new Voucher("PROMO-15-REAIS",15,null,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Porcentagem);
            
            // Act
            var result = pedido.AplicarVoucher(voucher);

            // Assert 
            Assert.True(result.IsValid);            
        }

        [Fact(DisplayName = "Aplicar voucher invalido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void Pedido_AplicarVoucherInvalido_DeveRetornarComErros()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var voucher = new Voucher("",null,null,0,DateTime.Now.AddDays(-1),false,true,TipoDescontoVoucher.Valor);
            
            // Act
            var result = pedido.AplicarVoucher(voucher);

            // Assert 
            Assert.False(result.IsValid);            
        }

        [Fact(DisplayName = "Aplicar voucher tipo valor desconto")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_VoucherTipoValorDesconto_DeveDescontarDoValorTotal()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 3, 15);
            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);
            
            var voucher = new Voucher("PROMO-15-REAIS",null,15,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Valor);            
            var valorComDesconto = pedido.ValorTotal - voucher.ValorDesconto;
            // Act
            pedido.AplicarVoucher(voucher);

            // Assert 
            Assert.Equal(valorComDesconto,pedido.ValorTotal);            
        }

        [Fact(DisplayName = "Aplicar voucher tipo percentual desconto")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_VoucherTipoPercentualDesconto_DeveDescontarDoValorTotal()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);
            var pedidoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 3, 15);
            pedido.AdicionarItem(pedidoItem1);
            pedido.AdicionarItem(pedidoItem2);
            
            var voucher = new Voucher("PROMO-15-REAIS",10,null,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Porcentagem);            
            var valorComDesconto = pedido.ValorTotal - ((pedido.ValorTotal * voucher.PercentualDesconto) / 100);
            // Act
            pedido.AplicarVoucher(voucher);

            // Assert 
            Assert.Equal(valorComDesconto,pedido.ValorTotal);            
        }

        [Fact(DisplayName = "Aplicar voucher desconto excede valor total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_DescontoExcedeValorTotalPedido_PedidoDeveTerValorZero()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem1);
            
            
            var voucher = new Voucher("PROMO-15-REAIS",null,300,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Valor);            
            
            pedido.AplicarVoucher(voucher);

            // Assert 
            Assert.Equal(0,pedido.ValorTotal);            
        }

        [Fact(DisplayName = "Aplicar voucher recalcular desconto na modificação do pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_ModificarItensPedido_DeveCalcularDescontoValorTotal()
        {
            // Arrange 
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem1);
            
            
            var voucher = new Voucher("PROMO-15-REAIS",null,50,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Valor);                        
            pedido.AplicarVoucher(voucher);

            var pedidoItem2 = new PedidoItem(Guid.NewGuid(),"Produto Teste",4,25);

            //Act
            pedido.AdicionarItem(pedidoItem2);

            // Assert 
            var totalEsperado = pedido.PedidoItems.Sum(x => x.Quantidade * x.ValorUnitario) - voucher.ValorDesconto; 
            Assert.Equal(totalEsperado,pedido.ValorTotal);       
        }

    }
}