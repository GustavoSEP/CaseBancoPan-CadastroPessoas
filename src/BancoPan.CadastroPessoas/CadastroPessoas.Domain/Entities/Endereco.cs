namespace CadastroPessoas.Domain.Entities
{
    public class Endereco
    {
        public string Cep { get; private set; }
        public string Logradouro { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string Estado { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }

        public Endereco(string cep, string logradouro, string bairro, string cidade, string estado, string numero, string complemento)
        {
            Cep = cep;
            Logradouro = logradouro;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Numero = numero;
            Complemento = complemento;
        }

        // Para EF Core
        protected Endereco() { }
    }
}