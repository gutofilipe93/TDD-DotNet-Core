using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Data
{
    public class VendasContext : DbContext
    {
        private readonly IMediator _mediator;

        // quando eu uso a palavra BASE no construtor eu to falando para esse contrutor que eu quero passar esse parametro especificado para o construtor pai
        // No caso abaixo a a classe pai DbContext tem um construtor com o parametro DbContextOptions, então uso a palavra base para passa esse parametro para ela
        public VendasContext(DbContextOptions<VendasContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;
            if(sucesso) await _mediator.PublicarEventos(this);

            return sucesso;
        }
    }

    public static class MediatorExtension
    {
        public static async Task PublicarEventos(this IMediator mediator,VendasContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Notificacoes)
            .ToList();

            domainEntities.ToList()
            .ForEach(x => x.Entity.LimparEvento());

            var task = domainEvents.Select(async (domainEvent) => { await mediator.Publish(domainEvent);});
            await Task.WhenAll(task);
        }
    }
}