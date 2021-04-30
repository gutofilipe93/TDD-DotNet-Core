using System;
using FluentValidation;
using FluentValidation.Results;

namespace NerdStore.Vendas.Domain
{
    public class Voucher
    {
        public string Codigo { get; private set; }
        public decimal? PercentualDesconto { get; private set; }
        public decimal? ValorDesconto { get; private set; }
        public int Quantidade { get; private set; }
        public DateTime DataValidade { get; private set; }
        public bool Ativo { get; private set; }
        public bool Utilizado { get; private set; }
        public TipoDescontoVoucher TipoDescontoVoucher { get; set; }

        public Voucher(string codigo, decimal? percentualDesconto, decimal? valorDesconto, int quantidade, DateTime dataValidade,
                        bool ativo, bool utilizado, TipoDescontoVoucher tipoDescontoVoucher)
        {
            Codigo = codigo;
            PercentualDesconto = percentualDesconto;
            ValorDesconto = valorDesconto;
            Quantidade = quantidade;
            DataValidade = dataValidade;
            Ativo = ativo;
            Utilizado = utilizado;
            TipoDescontoVoucher = tipoDescontoVoucher;
        }

        public ValidationResult ValidarSeAplicavel()
        {
            return new VoucherAplicavelValidation().Validate(this);
        }
    }

    public class VoucherAplicavelValidation : AbstractValidator<Voucher>
    {
        public static string CodigoErroMsg => "Voucher sem código valido";
        public static string DataValidadeErroMsg => "Este voucher está expirado";
        public static string AtivoErroMsg => "Este voucher não é mais valido";
        public static string UtilizadoErroMsg => "Este voucher já foi utilizado";
        public static string QuantidadeErroMsg => "Este voucher não esta mais disponivel";
        public static string ValorDescontoErroMsg => "O valor do desconto precisa ser superior a Zero";
        public static string PercentualDescontoErroMsg => "O valor da porcentagem precisa ser superior a Zero";

        public VoucherAplicavelValidation()
        {
            RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage(CodigoErroMsg);

            RuleFor(x => x.DataValidade)
            .Must(DataVencimentoSuperioAtual)
            .WithMessage(DataValidadeErroMsg);

            RuleFor(x => x.Ativo)
            .Equal(true)
            .WithMessage(AtivoErroMsg);

            RuleFor(x => x.Utilizado)
            .Equal(false)
            .WithMessage(UtilizadoErroMsg);

            RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage(QuantidadeErroMsg);

            When(x => x.TipoDescontoVoucher == TipoDescontoVoucher.Porcentagem, () =>
            {
                RuleFor(x => x.PercentualDesconto)
                .NotNull()
                .WithMessage(PercentualDescontoErroMsg)
                .GreaterThan(0)
                .WithMessage(PercentualDescontoErroMsg);
            });

            When(x => x.TipoDescontoVoucher == TipoDescontoVoucher.Valor, () =>
            {
                RuleFor(x => x.ValorDesconto)
                .NotNull()
                .WithMessage(ValorDescontoErroMsg)
                .GreaterThan(0)
                .WithMessage(ValorDescontoErroMsg);
            });
        }

        protected static bool DataVencimentoSuperioAtual(DateTime dataValidade)
        {
            return dataValidade >= DateTime.Now;
        }

    }
}