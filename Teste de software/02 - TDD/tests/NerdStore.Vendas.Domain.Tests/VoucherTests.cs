using System;
using System.Linq;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class VoucherTests
    {
        [Fact(DisplayName = "Validar voucher tipo valor valido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoValorValido_DeveEstarValido()
        {
            // Arrange 
            var voucher = new Voucher("PROMO-15-REAIS",null,15,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Valor);
            
            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert 
            Assert.True(result.IsValid);            
        }

        [Fact(DisplayName = "Validar voucher tipo valor inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoValorValido_DeveEstarInvalido()
        {
            // Arrange 
            var voucher = new Voucher("",null,null,0,DateTime.Now.AddDays(-1),false,true,TipoDescontoVoucher.Valor);

            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert 
            Assert.False(result.IsValid);  
            Assert.Equal(6,result.Errors.Count);      
            Assert.Contains(VoucherAplicavelValidation.AtivoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.CodigoErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.DataValidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.QuantidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.UtilizadoErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.ValorDescontoErroMsg, result.Errors.Select(c => c.ErrorMessage));      
        }

        [Fact(DisplayName = "Validar voucher tipo porcentagem valido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherPorcentagem_DeveEstarValido()
        {
            // Arrange 
            var voucher = new Voucher("PROMO-15-REAIS",15,null,1,DateTime.Now.AddDays(15),true,false,TipoDescontoVoucher.Porcentagem);
            
            // ActSem
            var result = voucher.ValidarSeAplicavel();

            // Assert 
            Assert.True(result.IsValid);            
        }

        [Fact(DisplayName = "Validar voucher tipo porcentagem inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherPorcentagem_DeveEstarInvalido()
        {
            // Arrange 
            var voucher = new Voucher("",null,null,0,DateTime.Now.AddDays(-1),false,true,TipoDescontoVoucher.Porcentagem);

            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert 
            Assert.False(result.IsValid);  
            Assert.Equal(6,result.Errors.Count);      
            Assert.Contains(VoucherAplicavelValidation.AtivoErroMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(VoucherAplicavelValidation.CodigoErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.DataValidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.QuantidadeErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.UtilizadoErroMsg, result.Errors.Select(c => c.ErrorMessage));  
            Assert.Contains(VoucherAplicavelValidation.PercentualDescontoErroMsg, result.Errors.Select(c => c.ErrorMessage));      
        }
    }
}