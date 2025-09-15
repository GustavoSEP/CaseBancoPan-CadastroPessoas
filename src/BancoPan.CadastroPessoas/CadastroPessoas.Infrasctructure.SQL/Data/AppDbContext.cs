using CadastroPessoas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Infrasctructure.SQL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
        }
        public DbSet<CadastroPessoas.Domain.Entities.PessoaFisica> PessoasFisicas { get; set; }
        public DbSet<CadastroPessoas.Domain.Entities.Endereco> Enderecos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PessoaFisica
            modelBuilder.Entity<PessoaFisica>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.Property(p => p.Nome).HasMaxLength(200);
                b.Property(p => p.CPF).HasMaxLength(20);
                b.Property(p => p.TipoPessoa).HasMaxLength(1);

                b.HasIndex(p => p.CPF).IsUnique();

                b.OwnsOne(p => p.Endereco, e =>
                {
                    e.Property(x => x.Cep).HasColumnName("Endereco_Cep").HasMaxLength(20);
                    e.Property(x => x.Logradouro).HasColumnName("Endereco_Logradouro").HasMaxLength(200);
                    e.Property(x => x.Bairro).HasColumnName("Endereco_Bairro").HasMaxLength(100);
                    e.Property(x => x.Cidade).HasColumnName("Endereco_Cidade").HasMaxLength(100);
                    e.Property(x => x.Estado).HasColumnName("Endereco_Estado").HasMaxLength(50);
                    e.Property(x => x.Numero).HasColumnName("Endereco_Numero").HasMaxLength(20);
                    e.Property(x => x.Complemento).HasColumnName("Endereco_Complemento").HasMaxLength(200);
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
