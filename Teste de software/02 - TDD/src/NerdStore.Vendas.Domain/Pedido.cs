using FluentValidation.Results;
using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdStore.Vendas.Domain
{
    public class Pedido : Entity, IAggregateRoot
    {
        public static int MAX_UNIDADES_ITEM = 15;
        public static int MIM_UNIDADES_ITEM = 1;
        protected Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }
        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItens;
        public Voucher Voucher { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public decimal Desconto { get; set; }    
        public ValidationResult AplicarVoucher(Voucher voucher)
        {            
            var result =  voucher.ValidarSeAplicavel();
            if(!result.IsValid) return result;

            Voucher = voucher;
            VoucherUtilizado = true;

            CalcularValorTotalDescontoVoucher();
            return result;
        }

        public void CalcularValorTotalDescontoVoucher()
        {
            if(!VoucherUtilizado) return;

            decimal desconto = 0;
            if(Voucher.TipoDescontoVoucher == TipoDescontoVoucher.Valor)            
                desconto = Voucher.ValorDesconto.Value;
            else
                desconto = (ValorTotal * Voucher.PercentualDesconto.Value) / 100;

            ValorTotal -= desconto; 
            if(ValorTotal < 0) ValorTotal = 0;
            Desconto = desconto;   
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(x => x.CalcularValor());
            CalcularValorTotalDescontoVoucher();
        }

        public bool PedidoItemExistente(PedidoItem item)
        {
            return _pedidoItens.Any(x => x.ProdutoId == item.ProdutoId);
        }

        private void ValidarQuantidadeItemPermitida(PedidoItem item)
        {
            var quantidadeItems = item.Quantidade;
            if (PedidoItemExistente(item))
            {
                var itemExistente = _pedidoItens.FirstOrDefault(x => x.ProdutoId == item.ProdutoId);
                quantidadeItems += itemExistente.Quantidade;
            }

            if (quantidadeItems > MAX_UNIDADES_ITEM) throw new DomainException($"Maximo de {MAX_UNIDADES_ITEM} unidades por produto");
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            ValidarQuantidadeItemPermitida(pedidoItem);
            if (PedidoItemExistente(pedidoItem))
            {
                var itemExistente = _pedidoItens.FirstOrDefault(x => x.ProdutoId == pedidoItem.ProdutoId);
                itemExistente.AdicionarUnidade(pedidoItem.Quantidade);
                pedidoItem = itemExistente;
                _pedidoItens.Remove(itemExistente);
            }

            _pedidoItens.Add(pedidoItem);
            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            _pedidoItens.Remove(pedidoItem);
            CalcularValorPedido();
        }

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            ValidarQuantidadeItemPermitida(pedidoItem);
            var itemExistente = _pedidoItens.FirstOrDefault(x => x.ProdutoId == pedidoItem.ProdutoId);
            _pedidoItens.Remove(itemExistente);
            _pedidoItens.Add(pedidoItem);
            CalcularValorPedido();
        }

        public void ValidarPedidoItemInexistente(PedidoItem pedidoItem)
        {
            if (!PedidoItemExistente(pedidoItem)) throw new DomainException($"O item não pertence ao pedido");
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}