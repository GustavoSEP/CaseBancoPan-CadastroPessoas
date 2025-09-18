namespace CadastroPessoas.Ports.Input.Commands
{
    /// <summary>
    /// Comando que representa a solicitação de criação de uma nova pessoa física.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Este comando segue o padrão Command na arquitetura hexagonal, sendo uma porta de entrada
    /// que transporta os dados necessários para a operação de criação de pessoa física.
    /// </para>
    /// <para>
    /// Os dados são recebidos das interfaces externas (como APIs ou UIs) e encaminhados
    /// para os casos de uso apropriados, mantendo o domínio isolado das camadas externas.
    /// </para>
    /// <para>
    /// O CPF será validado no caso de uso correspondente, podendo ser informado com ou sem formatação.
    /// O endereço será complementado automaticamente a partir do CEP informado, sendo necessário
    /// apenas o número e complemento (opcional) para completar o endereço.
    /// </para>
    /// </remarks>
    public class CreatePessoaFisicaCommand
    {
        /// <summary>
        /// Nome completo da pessoa física.
        /// </summary>
        /// <example>João da Silva</example>
        /// <remarks>
        /// O nome deve ser informado completo e sem abreviações.
        /// </remarks>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CPF da pessoa física.
        /// </summary>
        /// <example>123.456.789-10</example>
        /// <remarks>
        /// Pode ser informado com ou sem formatação (pontos e traço).
        /// A validação e formatação serão realizadas no caso de uso.
        /// </remarks>
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// CEP para consulta e complementação do endereço.
        /// </summary>
        /// <example>01310-100</example>
        /// <remarks>
        /// Pode ser informado com ou sem hífen.
        /// O endereço será consultado automaticamente a partir deste CEP.
        /// </remarks>
        public string CEP { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço.
        /// </summary>
        /// <example>1000</example>
        /// <remarks>
        /// Complementa o logradouro obtido pela consulta do CEP.
        /// </remarks>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endereço (opcional).
        /// </summary>
        /// <example>Apto 42</example>
        /// <remarks>
        /// Informações adicionais sobre o endereço, como apartamento, bloco, sala, etc.
        /// Este campo é opcional e pode ser deixado em branco.
        /// </remarks>
        public string Complemento { get; set; } = string.Empty;
    }
}