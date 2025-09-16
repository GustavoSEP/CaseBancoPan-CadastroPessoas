namespace CadastroPessoas.API.Models.Responses
{
    public class PessoaFisicaResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public string TipoPessoa { get; set; }
        public EnderecoDto Endereco { get; set; }

    }
}
