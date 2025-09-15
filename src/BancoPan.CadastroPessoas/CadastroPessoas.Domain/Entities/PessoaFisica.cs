using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Entities
{
    public class PessoaFisica
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string TipoPessoa { get; set; } = "F";
        public Endereco Endereco { get; set; } = new Endereco();

        public PessoaFisica()
        {
        }

        public PessoaFisica(string nome, string cpf, string tipoPessoa, Endereco endereco)
        {
            Nome = nome;
            CPF = cpf;
            TipoPessoa = tipoPessoa;
            Endereco = endereco;
        }

        public void AtualizarDados(string nome, string cpf, string tipoPessoa)
        {
            Nome = nome;
            CPF = cpf;
            TipoPessoa = tipoPessoa;
        }

        public void AtualizarEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public void AtualizarNumeroComplemento(string numero, string complemento)
        {
            if (Endereco == null) Endereco = new Endereco();
            Endereco.Numero = numero;
            Endereco.Complemento = complemento;
        }
    }
}
