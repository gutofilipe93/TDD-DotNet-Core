using Xunit;

namespace Features.Tests
{
    public class TesteNaoPassandoMotivoEspecifico
    {
        // Skip é usado para quando você quer pular um teste, seja por qual motivo for; Feito isso ele fica uma mensagem de alerta no codigo;
        [Fact(DisplayName = "Novo Cliente 2.0", Skip = "Nova versão 2.0 quebrando")]
        [Trait("Categoria", "Escapando dos Testes")]
        public void Teste_NaoEstaPassando_VersaoNovaNaoCompativel()
        {
            Assert.True(false);
        }
    }
}