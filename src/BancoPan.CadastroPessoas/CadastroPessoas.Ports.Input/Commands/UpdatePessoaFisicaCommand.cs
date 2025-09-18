namespace CadastroPessoas.Ports.Input.Commands
{
    /// <summary>
    /// Comando que representa a solicitação de atualização de uma pessoa física existente.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Este comando segue o padrão Command na arquitetura hexagonal, sendo uma porta de entrada
    /// que transporta os dados para atualização parcial de uma pessoa física identificada pelo ID.
    /// </para>
    /// <para>
    /// Implementa o conceito de atualização parcial, onde apenas os campos informados 
    /// (não nulos ou vazios) serão atualizados. Os demais campos manterão seus valores atuais.
    /// </para>
    /// <para>
    /// Note que o CPF não está presente, pois não é permitido alterar este documento de identificação
    /// após o cadastro da pessoa física, sendo ele um identificador natural imutável.
    /// </para>
    /// <para>
    /// Se um novo CEP for informado, o endereço será reconsultado e atualizado automaticamente,
    /// mantendo apenas o número e complemento informados (ou os atuais, caso não sejam informados).
    /// </para>
    /// </remarks>
    public class UpdatePessoaFisicaCommand
    {
        /// <summary>
        /// Novo nome completo da pessoa física (opcional).
        /// </summary>
        /// <example>João da Silva Santos</example>
        /// <remarks>
        /// Se não informado ou vazio, o nome atual será mantido.
        /// </remarks>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Novo CEP para atualização do endereço (opcional).
        /// </summary>
        /// <example>04538-132</example>
        /// <remarks>
        /// Se informado, o endereço completo será atualizado com base neste CEP.
        /// Se não informado ou vazio, o CEP e endereço atuais serão mantidos.
        /// Pode ser informado com ou sem hífen.
        /// </remarks>
        public string CEP { get; set; } = string.Empty;

        /// <summary>
        /// Novo número do endereço (opcional).
        /// </summary>
        /// <example>1500</example>
        /// <remarks>
        /// Se não informado ou vazio, o número atual será mantido.
        /// Se o CEP for alterado, este número será associado ao novo endereço.
        /// </remarks>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Novo complemento do endereço (opcional).
        /// </summary>
        /// <example>Bloco B, Apto 45</example>
        /// <remarks>
        /// Se não informado ou vazio, o complemento atual será mantido.
        /// Se o CEP for alterado, este complemento será associado ao novo endereço.
        /// </remarks>
        public string Complemento { get; set; } = string.Empty;
    }
}