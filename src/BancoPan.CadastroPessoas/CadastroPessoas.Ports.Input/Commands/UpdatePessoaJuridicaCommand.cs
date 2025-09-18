namespace CadastroPessoas.Ports.Input.Commands
{
    /// <summary>
    /// Comando que representa a solicitação de atualização de uma pessoa jurídica existente.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Este comando segue o padrão Command na arquitetura hexagonal, sendo uma porta de entrada
    /// que transporta os dados para atualização parcial de uma pessoa jurídica identificada pelo ID.
    /// </para>
    /// <para>
    /// Implementa o conceito de atualização parcial, onde apenas os campos informados 
    /// (não nulos ou vazios) serão atualizados. Os demais campos manterão seus valores atuais.
    /// </para>
    /// <para>
    /// Note que o CNPJ não está presente, pois não é permitido alterar este documento de identificação
    /// após o cadastro da pessoa jurídica, sendo ele um identificador natural imutável.
    /// </para>
    /// <para>
    /// Se um novo CEP for informado, o endereço será reconsultado e atualizado automaticamente,
    /// mantendo apenas o número e complemento informados (ou os atuais, caso não sejam informados).
    /// </para>
    /// </remarks>
    public class UpdatePessoaJuridicaCommand
    {
        /// <summary>
        /// Nova razão social da empresa (opcional).
        /// </summary>
        /// <example>Empresa XYZ Comércio e Serviços Ltda.</example>
        /// <remarks>
        /// Se não informada ou vazia, a razão social atual será mantida.
        /// A razão social é o nome oficial da empresa, conforme registrado nos órgãos competentes.
        /// </remarks>
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Novo nome fantasia da empresa (opcional).
        /// </summary>
        /// <example>XYZ Solutions</example>
        /// <remarks>
        /// Se não informado ou vazio, o nome fantasia atual será mantido.
        /// O nome fantasia é o nome comercial da empresa, podendo ser diferente da razão social.
        /// </remarks>
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// Novo CEP para atualização do endereço (opcional).
        /// </summary>
        /// <example>04711-130</example>
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
        /// <example>Torre B, 15º andar</example>
        /// <remarks>
        /// Se não informado ou vazio, o complemento atual será mantido.
        /// Se o CEP for alterado, este complemento será associado ao novo endereço.
        /// Pode incluir informações como torre, andar, sala, etc.
        /// </remarks>
        public string Complemento { get; set; } = string.Empty;
    }
}