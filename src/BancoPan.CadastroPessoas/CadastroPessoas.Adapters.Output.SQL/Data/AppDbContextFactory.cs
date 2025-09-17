using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json; 
using System;
using System.IO;

namespace CadastroPessoas.Adapters.Output.SQL.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string projectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "CadastroPessoas.API");

            if (!Directory.Exists(projectPath))
            {
                projectPath = Path.GetFullPath(Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "..",
                    "CadastroPessoas.API")); 
            }

            if (!Directory.Exists(projectPath))
            {
                throw new DirectoryNotFoundException(
                    $"Não foi possível encontrar o diretório do projeto API em: {projectPath}");
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "A string de conexão 'DefaultConnection' não foi encontrada no arquivo appsettings.json do projeto API.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}