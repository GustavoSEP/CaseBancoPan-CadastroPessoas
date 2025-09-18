using System;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    /// <summary>
    /// Modelo de resposta para operações com pessoa jurídica
    /// </summary>
    /// <remarks>
    /// Este modelo é retornado nas operações de criação, consulta e atualização de pessoas jurídicas.
    /// Contém todos os dados cadastrais da empresa, incluindo seu endereço completo e data de cadastro.
    /// </remarks>
    public class PessoaJuridicaResponse
    {
        /// <summary>
        /// Identificador único da pessoa jurídica
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Razão social da empresa
        /// </summary>
        /// <example>Empresa XYZ Ltda.</example>
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Nome fantasia da empresa
        /// </summary>
        /// <example>XYZ Tecnologia</example>
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da empresa (formatado)
        /// </summary>
        /// <example>12.345.678/0001-90</example>
        public string CNPJ { get; set; } = string.Empty;

        /// <summary>
        /// Dados de endereço da empresa
        /// </summary>
        public EnderecoResponse Endereco { get; set; } = new EnderecoResponse();

        /// <summary>
        /// Data e hora de cadastro da empresa no sistema
        /// </summary>
        /// <example>2025-09-18T14:30:00Z</example>
        public DateTime DataCadastro { get; set; }
    }
}