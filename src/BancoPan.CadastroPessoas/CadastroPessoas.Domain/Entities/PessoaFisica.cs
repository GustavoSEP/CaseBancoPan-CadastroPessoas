using System;

namespace CadastroPessoas.Domain.Entities
{
    public class PessoaFisica
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string CPF { get; private set; }
        public string Tipo { get; private set; }
        public Endereco Endereco { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public PessoaFisica(int id, string nome, string cpf, string tipo, Endereco endereco)
        {
            Id = id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            CPF = cpf ?? throw new ArgumentNullException(nameof(cpf));
            Tipo = tipo ?? throw new ArgumentNullException(nameof(tipo));
            Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
            DataCadastro = DateTime.Now;
        }

        // Para EF Core
        protected PessoaFisica() { }
    }
}