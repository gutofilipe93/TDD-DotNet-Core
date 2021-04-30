using System;
using Features.Clientes;
using Xunit;

namespace Features.Tests
{
    /* O Fixture é uma forma muito usada para economizar código, onde vc cria um objeto que pode ser usado em varios teste
     Dessa forma a outra vantagem é que esse objeto é instanciado somente uma vez.
     Já quando ele é criando dentro do construtor da classe teste ele é instanciado para todos os teste
     sem a possibilidade de reaproveitar um retorno de um teste para outro. 
     O Recomendado para seu uso é assim, quando um objeto for usado em mais de uma classe de teste usa-se Fixture, 
     caso seja usado só mente em uma classe cria-se o objeto no construtor mesmo */

    [CollectionDefinition(nameof(ClienteCollection))]
    public class ClienteCollection : ICollectionFixture<ClienteTestsFixture>
    {}

    public class ClienteTestsFixture : IDisposable
    {
        public Cliente GerarClienteValido()
        {
            var cliente = new Cliente(
                Guid.NewGuid(),
                "Eduardo",
                "Pires",
                DateTime.Now.AddYears(-30),
                "edu@edu.com",
                true,
                DateTime.Now);

            return cliente;
        }

        public Cliente GerarClienteInValido()
        {
            var cliente = new Cliente(
                Guid.NewGuid(),
                "",
                "",
                DateTime.Now,
                "edu2edu.com",
                true,
                DateTime.Now);

            return cliente;
        }

        public void Dispose()
        {
        }
    }
}