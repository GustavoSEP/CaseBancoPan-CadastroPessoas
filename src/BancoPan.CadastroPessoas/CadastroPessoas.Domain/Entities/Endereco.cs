using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Entities
{
    /// <summary>
    /// Representa um endereço físico brasileiro com seus componentes.
    /// Esta entidade armazena informações de endereçamento para pessoas físicas e jurídicas.
    /// </summary>
    public class Endereco
    {
        /// <summary>
        /// Obtém ou define o CEP (Código de Endereçamento Postal).
        /// </summary>
        /// <remarks>Formato padrão: 00000-000</remarks>
        public string Cep { get; set; } = "";

        /// <summary>
        /// Obtém ou define o logradouro (rua, avenida, etc).
        /// </summary>
        public string Logradouro { get; set; } = "";

        /// <summary>
        /// Obtém ou define o bairro.
        /// </summary>
        public string Bairro { get; set; } = "";

        /// <summary>
        /// Obtém ou define a cidade.
        /// </summary>
        public string Cidade { get; set; } = "";

        /// <summary>
        /// Obtém ou define o estado (UF).
        /// </summary>
        /// <remarks>Normalmente representado por duas letras (ex: SP, RJ, MG)</remarks>
        public string Estado { get; set; } = "";

        /// <summary>
        /// Obtém ou define o número do endereço.
        /// </summary>
        public string Numero { get; set; } = "";

        /// <summary>
        /// Obtém ou define o complemento do endereço.
        /// </summary>
        /// <remarks>Informações adicionais como apartamento, bloco, andar, etc.</remarks>
        public string Complemento { get; set; } = "";

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Endereco"/> com valores vazios.
        /// </summary>
        public Endereco() { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Endereco"/> com os dados básicos do endereço.
        /// </summary>
        /// <param name="cep">O CEP do endereço.</param>
        /// <param name="logradouro">O logradouro do endereço.</param>
        /// <param name="bairro">O bairro do endereço.</param>
        /// <param name="cidade">A cidade do endereço.</param>
        /// <param name="estado">O estado (UF) do endereço.</param>
        /// <remarks>O número e complemento serão inicializados como strings vazias.</remarks>
        public Endereco(string cep, string logradouro, string bairro, string cidade, string estado)
            : this(cep, logradouro, bairro, cidade, estado, string.Empty, string.Empty)
        { }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Endereco"/> com todos os dados do endereço.
        /// </summary>
        /// <param name="cep">O CEP do endereço.</param>
        /// <param name="logradouro">O logradouro do endereço.</param>
        /// <param name="bairro">O bairro do endereço.</param>
        /// <param name="cidade">A cidade do endereço.</param>
        /// <param name="estado">O estado (UF) do endereço.</param>
        /// <param name="numero">O número do endereço.</param>
        /// <param name="complemento">O complemento do endereço.</param>
        /// <remarks>Valores nulos serão convertidos para strings vazias.</remarks>
        public Endereco(string cep, string logradouro, string bairro, string cidade, string estado, string numero, string complemento)
        {
            Cep = cep ?? string.Empty;
            Logradouro = logradouro ?? string.Empty;
            Bairro = bairro ?? string.Empty;
            Cidade = cidade ?? string.Empty;
            Estado = estado ?? string.Empty;
            Numero = numero ?? string.Empty;
            Complemento = complemento ?? string.Empty;
        }

        /// <summary>
        /// Atualiza o número e o complemento do endereço.
        /// </summary>
        /// <param name="numero">O novo número do endereço.</param>
        /// <param name="complemento">O novo complemento do endereço.</param>
        /// <remarks>
        /// Apenas valores não nulos ou vazios serão aplicados.
        /// Se algum parâmetro for nulo ou vazio, o valor atual será mantido.
        /// </remarks>
        public void AtualizarNumeroComplemento(string numero, string complemento)
        {
            if (!string.IsNullOrWhiteSpace(numero))
                Numero = numero;
            if (!string.IsNullOrWhiteSpace(complemento))
                Complemento = complemento;
        }
    }
}