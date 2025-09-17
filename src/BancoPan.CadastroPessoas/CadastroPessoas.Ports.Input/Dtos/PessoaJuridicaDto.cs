using System;

namespace CadastroPessoas.Ports.Input.Dtos
{
    public class PessoaJuridicaDto
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
        public DateTime DataCadastro { get; set; }
    }
}