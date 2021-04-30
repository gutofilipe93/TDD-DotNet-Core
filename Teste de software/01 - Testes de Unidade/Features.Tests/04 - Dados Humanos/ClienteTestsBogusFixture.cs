using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using Features.Clientes;
using Xunit;

namespace Features.Tests
{
    /* Link da documentação do Bogus -> https://github.com/bchavez/Bogus
     OBS: essa classe utiliza o Bogus para preencher os dados do objeto e o Fixture para reaproveitamento do objeto 
     Bogus é um framework usado para teste onde ele simula dados mais parecidos com a realidade de forma aleatória, 
     ou seja ao inves de passar sempre o mesmo teste correndo o risco de não ter testado tudo, 
     vaí passar sempre dados diferentes cada vez que o teste for executado */
    [CollectionDefinition(nameof(ClienteBogusCollection))]
    public class ClienteBogusCollection : ICollectionFixture<ClienteTestsBogusFixture>
    {}

    public class ClienteTestsBogusFixture : IDisposable
    {
        // Gera varios mas só retorna um
        public Cliente GerarClienteValido()
        {
            return GerarClientes(1, true).FirstOrDefault();
        }

        public IEnumerable<Cliente> ObterClientesVariados()
        {
            var clientes = new List<Cliente>();

            clientes.AddRange(GerarClientes(50, true).ToList());
            clientes.AddRange(GerarClientes(50, false).ToList());

            return clientes;
        }

        // Esta gerando varios cliente aleatorios para realizar os teste de formar mais parecida com a realidade
        public IEnumerable<Cliente> GerarClientes(int quantidade, bool ativo)
        {
            var genero = new Faker().PickRandom<Name.Gender>();
            var clientes = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(f => new Cliente(
                    Guid.NewGuid(), 
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(80,DateTime.Now.AddYears(-18)),
                    "",
                    ativo,
                    DateTime.Now))
                .RuleFor(c=>c.Email, (f,c) => 
                    f.Internet.Email(c.Nome.ToLower(), c.Sobrenome.ToLower()));

            return clientes.Generate(quantidade);
        }

        public Cliente GerarClienteInvalido()
        {
            var genero = new Faker().PickRandom<Name.Gender>();

            var cliente = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(f => new Cliente(
                    Guid.NewGuid(),
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(1, DateTime.Now.AddYears(1)),
                    "",
                    false,
                    DateTime.Now));

            return cliente;
        }

        public void Dispose()
        {
        }
    }
}