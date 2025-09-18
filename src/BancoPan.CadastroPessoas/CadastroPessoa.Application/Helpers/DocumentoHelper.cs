using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CadastroPessoas.Application.Helpers
{
    /// <summary>
    /// Fornece métodos utilitários para validação e formatação de documentos brasileiros (CPF e CNPJ).
    /// </summary>
    /// <remarks>
    /// Esta classe contém métodos estáticos que implementam os algoritmos oficiais de validação
    /// de CPF e CNPJ, conforme as regras da Receita Federal do Brasil, bem como métodos para
    /// formatação destes documentos no padrão visual brasileiro.
    /// </remarks>
    public static class DocumentoHelper
    {
        /// <summary>
        /// Valida um número de CPF aplicando o algoritmo oficial.
        /// </summary>
        /// <param name="cpf">O número de CPF a ser validado, com ou sem formatação.</param>
        /// <returns>
        /// <c>true</c> se o CPF for válido de acordo com o algoritmo de validação; caso contrário, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// O método verifica:
        /// <list type="bullet">
        /// <item>Se o CPF tem 11 dígitos após a remoção de caracteres especiais</item>
        /// <item>Se o CPF não é composto por dígitos repetidos (ex: 111.111.111-11)</item>
        /// <item>Se os dígitos verificadores estão corretos conforme o algoritmo da Receita Federal</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// bool isValid = DocumentoHelper.IsValidCpf("123.456.789-09");
        /// </code>
        /// </example>
        public static bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        /// <summary>
        /// Formata um número de CPF no padrão brasileiro: XXX.XXX.XXX-XX.
        /// </summary>
        /// <param name="cpf">O número de CPF a ser formatado, com ou sem formatação prévia.</param>
        /// <returns>
        /// O CPF formatado no padrão XXX.XXX.XXX-XX se tiver 11 dígitos;
        /// caso contrário, retorna o valor original limpo.
        /// </returns>
        /// <remarks>
        /// O método remove qualquer formatação existente antes de aplicar o novo formato.
        /// Se o CPF não tiver exatamente 11 dígitos após a limpeza, ele será retornado sem formatação.
        /// </remarks>
        /// <example>
        /// <code>
        /// string formatted = DocumentoHelper.FormatCpf("12345678909"); // Retorna "123.456.789-09"
        /// </code>
        /// </example>
        public static string FormatCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Valida um número de CNPJ aplicando o algoritmo oficial.
        /// </summary>
        /// <param name="cnpj">O número de CNPJ a ser validado, com ou sem formatação.</param>
        /// <returns>
        /// <c>true</c> se o CNPJ for válido de acordo com o algoritmo de validação; caso contrário, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// O método verifica:
        /// <list type="bullet">
        /// <item>Se o CNPJ tem 14 dígitos após a remoção de caracteres especiais</item>
        /// <item>Se o CNPJ não é composto por dígitos repetidos (ex: 11.111.111/1111-11)</item>
        /// <item>Se os dígitos verificadores estão corretos conforme o algoritmo da Receita Federal</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// bool isValid = DocumentoHelper.IsValidCnpj("12.345.678/0001-95");
        /// </code>
        /// </example>
        public static bool IsValidCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();

            if (cnpj.Length != 14)
                return false;

            if (cnpj.Distinct().Count() == 1)
                return false;

            var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCnpj = cnpj.Substring(0, 12);
            var soma = 0;

            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        /// <summary>
        /// Formata um número de CNPJ no padrão brasileiro: XX.XXX.XXX/XXXX-XX.
        /// </summary>
        /// <param name="cnpj">O número de CNPJ a ser formatado, com ou sem formatação prévia.</param>
        /// <returns>
        /// O CNPJ formatado no padrão XX.XXX.XXX/XXXX-XX se tiver 14 dígitos;
        /// caso contrário, retorna o valor original limpo.
        /// </returns>
        /// <remarks>
        /// O método remove qualquer formatação existente antes de aplicar o novo formato.
        /// Se o CNPJ não tiver exatamente 14 dígitos após a limpeza, ele será retornado sem formatação.
        /// </remarks>
        /// <example>
        /// <code>
        /// string formatted = DocumentoHelper.FormatCnpj("12345678000195"); // Retorna "12.345.678/0001-95"
        /// </code>
        /// </example>
        public static string FormatCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return string.Empty;

            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();

            if (cnpj.Length != 14)
                return cnpj;

            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
    }
}