using System;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    public class PessoaJuridicaResponse
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public EnderecoResponse Endereco { get; set; } = new EnderecoResponse();
        public DateTime DataCadastro { get; set; }
    }
}