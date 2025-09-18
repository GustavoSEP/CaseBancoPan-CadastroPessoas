using System;

namespace CadastroPessoas.Ports.Input.Dtos
{
    /// <summary>
    /// Representa um objeto de transferência de dados (DTO) para pessoas jurídicas.
    /// Esta classe é utilizada para transferir informações de pessoa jurídica entre as camadas da aplicação,
    /// principalmente para entrada de dados através das portas de entrada (input ports).
    /// </summary>
    /// <remarks>
    /// Este DTO é utilizado para:
    /// - Receber dados de pessoa jurídica em requisições de API
    /// - Transferir informações de pessoa jurídica entre a camada de aplicação e a camada de domínio
    /// - Desacoplar a representação externa da pessoa jurídica da entidade de domínio PessoaJuridica
    /// </remarks>
    public class PessoaJuridicaDto
    {
        /// <summary>
        /// Obtém ou define o identificador único da pessoa jurídica.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a razão social da empresa.
        /// A razão social é o nome oficial da empresa, registrado nos órgãos competentes.
        /// </summary>
        /// <example>Empresa ABC Ltda.</example>
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o nome fantasia da empresa.
        /// O nome fantasia é o nome comercial pelo qual a empresa é conhecida no mercado.
        /// </summary>
        /// <example>ABC Tecnologia</example>
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o CNPJ (Cadastro Nacional da Pessoa Jurídica) da empresa.
        /// </summary>
        /// <remarks>
        /// O CNPJ deve ser um documento válido conforme as regras da Receita Federal do Brasil.
        /// Pode ser fornecido com ou sem formatação.
        /// </remarks>
        /// <example>12.345.678/0001-90</example>
        public string CNPJ { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o endereço completo da pessoa jurídica.
        /// </summary>
        /// <seealso cref="EnderecoDto"/>
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();

        /// <summary>
        /// Obtém ou define a data em que o registro da pessoa jurídica foi criado no sistema.
        /// </summary>
        /// <example>2025-09-18T14:30:00</example>
        public DateTime DataCadastro { get; set; }
    }
}