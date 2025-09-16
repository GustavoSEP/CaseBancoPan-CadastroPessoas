using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Entities
{
    public class PessoaJuridica
    {
        public int Id { get; set; }

        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public string TipoPessoa { get; set; } = "J";
        public Endereco Endereco { get; set; } = new Endereco();

        public PessoaJuridica()
        {
        }

        public PessoaJuridica(string razaoSocial, string nomeFantasia, string cnpj, string tipoPessoa, Endereco endereco)
        {
            RazaoSocial = razaoSocial;
            NomeFantasia = nomeFantasia;
            CNPJ = cnpj;
            TipoPessoa = tipoPessoa;
            Endereco = endereco;
        }

        public void AtualizarDados(string razaoSocial, string nomeFantasia, string cnpj, string tipoPessoa)
        {
            RazaoSocial = razaoSocial;
            NomeFantasia = nomeFantasia;
            CNPJ = cnpj;
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
