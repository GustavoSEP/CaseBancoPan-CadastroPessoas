namespace CadastroPessoas.Ports.Input.Commands
{
    /// <summary>
    /// Comando que representa a solicitação de criação de uma nova pessoa jurídica.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Este comando segue o padrão Command na arquitetura hexagonal, sendo uma porta de entrada
    /// que transporta os dados necessários para a operação de criação de pessoa jurídica (empresa).
    /// </para>
    /// <para>
    /// Os dados são recebidos das interfaces externas (como APIs ou UIs) e encaminhados
    /// para os casos de uso apropriados, mantendo o domínio isolado das camadas externas.
    /// </para>
    /// <para>
    /// O CNPJ será validado no caso de uso correspondente, podendo ser informado com ou sem formatação.
    /// O endereço será complementado automaticamente a partir do CEP informado, sendo necessário
    /// apenas o número e complemento (opcional) para completar o endereço.
    /// </para>
    /// </remarks>
    public class CreatePessoaJuridicaCommand
    {
        /// <summary>
        /// Razão social da empresa.
        /// </summary>
        /// <example>Empresa XYZ Ltda.</example>
        /// <remarks>
        /// A razão social é o nome oficial da empresa, conforme registrado nos órgãos competentes.
        /// Este campo é obrigatório.
        /// </remarks>
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Nome fantasia da empresa.
        /// </summary>
        /// <example>XYZ Tecnologia</example>
        /// <remarks>
        /// O nome fantasia é o nome comercial da empresa, podendo ser diferente da razão social.
        /// Este campo pode ser opcional em alguns contextos, dependendo do tipo de empresa.
        /// </remarks>
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da empresa.
        /// </summary>
        /// <example>12.345.678/0001-90</example>
        /// <remarks>
        /// Pode ser informado com ou sem formatação (pontos, barra e traço).
        /// A validação e formatação serão realizadas no caso de uso.
        /// O CNPJ deve ser único no sistema.
        /// </remarks>
        public string CNPJ { get; set; } = string.Empty;

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
        /// Para endereços sem número, pode ser informado "s/n".
        /// </remarks>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endereço (opcional).
        /// </summary>
        /// <example>Sala 1001, 10º andar</example>
        /// <remarks>
        /// Informações adicionais sobre o endereço, como sala, andar, bloco, etc.
        /// Este campo é opcional e pode ser deixado em branco.
        /// </remarks>
        public string Complemento { get; set; } = string.Empty;
    }
}