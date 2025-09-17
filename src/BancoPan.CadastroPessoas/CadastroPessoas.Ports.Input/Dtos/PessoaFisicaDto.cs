using System;

namespace CadastroPessoas.Ports.Input.Dtos
{
    public class PessoaFisicaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
        public DateTime DataCadastro { get; set; }
    }
}