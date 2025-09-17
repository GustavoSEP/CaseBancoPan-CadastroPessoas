using System;

namespace CadastroPessoas.Domain.Entities
{
    public class PessoaJuridica
    {
        public int Id { get; private set; }
        public string RazaoSocial { get; private set; }
        public string NomeFantasia { get; private set; }
        public string CNPJ { get; private set; }
        public string Tipo { get; private set; }
        public Endereco Endereco { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public PessoaJuridica(int id, string razaoSocial, string nomeFantasia, string cnpj, string tipo, Endereco endereco)
        {
            Id = id;
            RazaoSocial = razaoSocial ?? throw new ArgumentNullException(nameof(razaoSocial));
            NomeFantasia = nomeFantasia;
            CNPJ = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
            Tipo = tipo ?? throw new ArgumentNullException(nameof(tipo));
            Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
            DataCadastro = DateTime.Now;
        }

        // Para EF Core
        protected PessoaJuridica() { }
    }
}