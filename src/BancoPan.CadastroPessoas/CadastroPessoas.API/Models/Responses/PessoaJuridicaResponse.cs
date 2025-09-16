namespace CadastroPessoas.API.Models.Responses
{
    public class PessoaJuridicaResponse
    {
        public int Id { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public string Documento { get; set; }
        public string TipoPessoa { get; set; }
        public EnderecoDto Endereco { get; set; }
    }
}
