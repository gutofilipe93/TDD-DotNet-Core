using System;
using FluentValidation;
using NerdStore.Core.Messages;
using NerdStore.Vendas.Domain;

namespace NerdStore.Vendas.Application.Commands
{
    public class AdicionaItemPedidoCommand : Command
    {
        public Guid ClienteId { get; set; }
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }

        public AdicionaItemPedidoCommand(Guid clienteId, Guid produtoId, string nome, int quantidade, decimal valorUnitario)
        {
            ClienteId = clienteId;
            ProdutoId = produtoId;
            Nome= nome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarItemPedidoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class AdicionarItemPedidoValidation : AbstractValidator<AdicionaItemPedidoCommand>
    {
        public static string IdClienteErroMsg => "Id cliente inválido";
        public static string IdProdutoErroMsg => "Id produto inválido";
        public static string QtdMaxErroMsg => $"A quandtidade maxima de um item é {Pedido.MAX_UNIDADES_ITEM}";
        public static string QtdMinErroMsg => "A quantidade minima de um item é 1";
        public static string NomeErroMsg => "Nome do produto não foi informado";
        public static string ValorErroMsg => "O valor do item precisa ser maior que 0";

        public AdicionarItemPedidoValidation()
        {
            RuleFor(c => c.ClienteId)
            .NotEqual(Guid.Empty)
            .WithMessage(IdClienteErroMsg);

            RuleFor(c => c.ProdutoId)
            .NotEqual(Guid.Empty)
            .WithMessage(IdProdutoErroMsg);

            RuleFor(c => c.Nome)
            .NotEmpty()
            .WithMessage(NomeErroMsg);

            RuleFor(c => c.Quantidade)
            .GreaterThan(0)
            .WithMessage(QtdMinErroMsg)
            .LessThanOrEqualTo(Pedido.MAX_UNIDADES_ITEM)
            .WithMessage(QtdMaxErroMsg);

            RuleFor(c => c.ValorUnitario)
            .GreaterThan(0)
            .WithMessage(ValorErroMsg);
        }

    }
}