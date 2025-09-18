using System;

namespace CadastroPessoas.Ports.Input.Dtos
{
    /// <summary>
    /// Representa um objeto de transferência de dados (DTO) para pessoas físicas.
    /// Esta classe é utilizada para transferir informações de pessoa física entre as camadas da aplicação,
    /// principalmente para entrada de dados através das portas de entrada (input ports).
    /// </summary>
    /// <remarks>
    /// Este DTO é utilizado para:
    /// - Receber dados de pessoa física em requisições de API
    /// - Transferir informações de pessoa física entre a camada de aplicação e a camada de domínio
    /// - Desacoplar a representação externa da pessoa física da entidade de domínio PessoaFisica
    /// </remarks>
    public class PessoaFisicaDto
    {
        /// <summary>
        /// Obtém ou define o identificador único da pessoa física.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome completo da pessoa física.
        /// </summary>
        /// <example>João Silva</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o CPF (Cadastro de Pessoa Física) da pessoa.
        /// </summary>
        /// <remarks>
        /// O CPF deve ser um documento válido conforme as regras da Receita Federal do Brasil.
        /// Pode ser fornecido com ou sem formatação.
        /// </remarks>
        /// <example>123.456.789-00</example>
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o endereço completo da pessoa física.
        /// </summary>
        /// <seealso cref="EnderecoDto"/>
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();

        /// <summary>
        /// Obtém ou define a data em que o registro da pessoa física foi criado no sistema.
        /// </summary>
        /// <example>2025-09-18T14:30:00</example>
        public DateTime DataCadastro { get; set; }
    }
}