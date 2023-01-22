using Microsoft.EntityFrameworkCore;
using NerdStore.Core.Commnunication.Mediator;
using NerdStore.Core.Data;
using NerdStore.Core.Messages;
using NerdStore.Pagamentos.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Pagamentos.Data
{
    public class PagamentoContext : DbContext, IUnitOfWork
    {
        private readonly IMediatrHandler _mediatrHandler;

        public PagamentoContext(DbContextOptions<PagamentoContext> options, IMediatrHandler mediatrHandler) : base(options)
        {
            _mediatrHandler = mediatrHandler;
        }

        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            {
                property.SetColumnType("varchar(100)");
            }

            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentoContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }


        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            var sucesso = await base.SaveChangesAsync() > 0;
            if (sucesso) await _mediatrHandler.PublicarEventos(this);

            return sucesso;
        }
    }
}
