using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoItemTest
    {
        [Fact(DisplayName = "Novo item pedido com unidades abaixo do permitido")]
        [Trait("Categoria", "Vendas - Pedido Item")]
        public void AdicionarItemPedido_UnidadesAbaixoDoPermitido_DeveRetornarException()
        {
            // Arrange && Act & Assert            
            Assert.Throws<DomainException>(() => new PedidoItem(Guid.NewGuid(), "Produto Teste", 0, 100));
        }
    }
}
