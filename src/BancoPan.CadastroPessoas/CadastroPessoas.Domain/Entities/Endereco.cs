namespace CadastroPessoas.Domain.Entities
{
    /// <summary>
    /// Representa um endereço completo no domínio da aplicação.
    /// </summary>
    /// <remarks>
    /// Esta entidade armazena todos os componentes de um endereço brasileiro,
    /// incluindo CEP, logradouro, bairro, cidade, estado, número e complemento.
    /// É utilizada tanto para pessoas físicas quanto jurídicas.
    /// 
    /// A implementação utiliza propriedades imutáveis (com setters privados) para 
    /// garantir a integridade dos dados após a criação.
    /// </remarks>
    public class Endereco
    {
        /// <summary>
        /// Obtém o CEP do endereço (Código de Endereçamento Postal).
        /// </summary>
        /// <example>01310-100</example>
        public string Cep { get; private set; }
        
        /// <summary>
        /// Obtém o logradouro do endereço (rua, avenida, praça, etc.).
        /// </summary>
        /// <example>Avenida Paulista</example>
        public string Logradouro { get; private set; }
        
        /// <summary>
        /// Obtém o bairro do endereço.
        /// </summary>
        /// <example>Bela Vista</example>
        public string Bairro { get; private set; }
        
        /// <summary>
        /// Obtém a cidade do endereço.
        /// </summary>
        /// <example>São Paulo</example>
        public string Cidade { get; private set; }
        
        /// <summary>
        /// Obtém o estado do endereço (UF).
        /// </summary>
        /// <example>SP</example>
        public string Estado { get; private set; }
        
        /// <summary>
        /// Obtém o número do endereço.
        /// </summary>
        /// <example>1000</example>
        /// <remarks>
        /// O número é representado como string para acomodar números com complementos
        /// como "s/n", "km 2", etc.
        /// </remarks>
        public string Numero { get; private set; }
        
        /// <summary>
        /// Obtém o complemento do endereço.
        /// </summary>
        /// <example>Apto 42</example>
        /// <remarks>
        /// O complemento pode incluir informações adicionais como andar, sala, 
        /// apartamento, bloco, etc.
        /// </remarks>
        public string Complemento { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Endereco"/> com todos os dados.
        /// </summary>
        /// <param name="cep">CEP do endereço.</param>
        /// <param name="logradouro">Logradouro (rua, avenida, etc.).</param>
        /// <param name="bairro">Bairro.</param>
        /// <param name="cidade">Cidade.</param>
        /// <param name="estado">Estado (UF).</param>
        /// <param name="numero">Número do endereço.</param>
        /// <param name="complemento">Complemento do endereço.</param>
        /// <remarks>
        /// Este construtor permite a criação de um endereço completo com todos os seus componentes.
        /// É utilizado principalmente após a consulta a serviços de CEP e complementação manual
        /// dos dados específicos (número e complemento).
        /// </remarks>
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

        /// <summary>
        /// Construtor protegido sem parâmetros para uso do Entity Framework Core.
        /// </summary>
        /// <remarks>
        /// Este construtor é necessário para que o Entity Framework Core possa criar
        /// instâncias da entidade durante a leitura do banco de dados. Não deve ser
        /// utilizado diretamente no código da aplicação.
        /// </remarks>
        protected Endereco() { }
    }
}