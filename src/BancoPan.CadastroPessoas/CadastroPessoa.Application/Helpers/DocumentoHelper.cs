using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CadastroPessoas.Application.Helpers
{
    /// <summary>
    /// Classe de utilidades para manipulação e validação de documentos brasileiros (CPF, CNPJ) 
    /// e tipos de pessoa.
    /// </summary>
    /// <remarks>
    /// Fornece métodos para validação, formatação e normalização de documentos como CPF e CNPJ,
    /// além de métodos para lidar com tipos de pessoa (física ou jurídica).
    /// </remarks>
    public static class DocumentoHelper
    {
        /// <summary>
        /// Expressão regular compilada para extrair apenas dígitos de uma string.
        /// </summary>
        private static readonly Regex OnlyDigitsRegex = new(@"[^\d]", RegexOptions.Compiled);

        /// <summary>
        /// Remove caracteres não numéricos de uma string.
        /// </summary>
        /// <param name="value">String a ser normalizada.</param>
        /// <returns>String contendo apenas os dígitos numéricos da entrada.</returns>
        /// <example>
        /// <code>
        /// string result = NormalizeDigits("123.456.789-01"); // Retorna "12345678901"
        /// </code>
        /// </example>
        public static string NormalizeDigits(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return OnlyDigitsRegex.Replace(value, string.Empty);
        }

        /// <summary>
        /// Verifica se um CPF é válido, utilizando o algoritmo de validação oficial.
        /// </summary>
        /// <param name="cpf">O CPF a ser validado, pode conter formatação.</param>
        /// <returns>True se o CPF for válido, False caso contrário.</returns>
        /// <remarks>
        /// A validação verifica:
        /// - Se o CPF tem 11 dígitos após normalização
        /// - Se não é um CPF com dígitos repetidos (ex: 111.111.111-11)
        /// - Se os dígitos verificadores são válidos de acordo com o algoritmo oficial
        /// </remarks>
        /// <example>
        /// <code>
        /// bool isValid = IsValidCpf("529.982.247-25"); // Valida um CPF com formatação
        /// </code>
        /// </example>
        public static bool IsValidCpf(string? cpf)
        {
            var digits = NormalizeDigits(cpf);
            if (digits.Length != 11) return false;

            if (digits.Distinct().Count() == 1) return false;

            var numbers = digits.Select(c => c - '0').ToArray();

            if (!VerifyCpfDigit(numbers, 9)) return false;
            if (!VerifyCpfDigit(numbers, 10)) return false;

            return true;
        }

        /// <summary>
        /// Verifica se um dígito verificador específico do CPF é válido.
        /// </summary>
        /// <param name="numbers">Array de inteiros representando os dígitos do CPF.</param>
        /// <param name="position">Posição do dígito verificador (9 para o primeiro, 10 para o segundo).</param>
        /// <returns>True se o dígito verificador na posição especificada for válido, False caso contrário.</returns>
        private static bool VerifyCpfDigit(int[] numbers, int position)
        {
            if (numbers == null) return false;
            if (position < 1 || position >= numbers.Length) return false;

            int sum = 0;
            int weight = position + 1; 
            for (int i = 0; i < position; i++)
            {
                int digit = numbers[i];
                if (digit < 0 || digit > 9) return false;
                sum += digit * (weight - i);
            }

            int remainder = sum % 11;
            int expected = remainder < 2 ? 0 : 11 - remainder;
            return numbers[position] == expected;
        }

        /// <summary>
        /// Formata um CPF adicionando a pontuação padrão brasileira.
        /// </summary>
        /// <param name="cpf">O CPF a ser formatado (apenas dígitos).</param>
        /// <returns>O CPF formatado no padrão XXX.XXX.XXX-XX.</returns>
        /// <exception cref="ArgumentException">Lançada quando o CPF não contém exatamente 11 dígitos após normalização.</exception>
        /// <example>
        /// <code>
        /// string formatted = FormatCpf("12345678901"); // Retorna "123.456.789-01"
        /// </code>
        /// </example>
        public static string FormatCpf(string? cpf)
        {
            var digits = NormalizeDigits(cpf);
            if (digits.Length != 11) throw new ArgumentException("CPF deve conter 11 dígitos para formatação.", nameof(cpf));

            return $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9, 2)}";
        }

        /// <summary>
        /// Verifica se um CNPJ é válido, utilizando o algoritmo de validação oficial.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser validado, pode conter formatação.</param>
        /// <returns>True se o CNPJ for válido, False caso contrário.</returns>
        /// <remarks>
        /// A validação verifica:
        /// - Se o CNPJ tem 14 dígitos após normalização
        /// - Se não é um CNPJ com dígitos repetidos (ex: 11.111.111/1111-11)
        /// - Se os dígitos verificadores são válidos de acordo com o algoritmo oficial
        /// </remarks>
        /// <example>
        /// <code>
        /// bool isValid = IsValidCnpj("11.444.777/0001-61"); // Valida um CNPJ com formatação
        /// </code>
        /// </example>
        public static bool IsValidCnpj(string? cnpj)
        {
            var digits = NormalizeDigits(cnpj);
            if (digits.Length != 14) return false;

            if (digits.Distinct().Count() == 1) return false;

            var numbers = digits.Select(c => c - '0').ToArray();

            if (!VerifyCnpjDigit(numbers, 12)) return false;
            if (!VerifyCnpjDigit(numbers, 13)) return false;

            return true;
        }

        /// <summary>
        /// Verifica se um dígito verificador específico do CNPJ é válido.
        /// </summary>
        /// <param name="numbers">Array de inteiros representando os dígitos do CNPJ.</param>
        /// <param name="position">Posição do dígito verificador (12 para o primeiro, 13 para o segundo).</param>
        /// <returns>True se o dígito verificador na posição especificada for válido, False caso contrário.</returns>
        private static bool VerifyCnpjDigit(int[] numbers, int position)
        {
            if (numbers == null) return false;
            if (position < 12 || position >= numbers.Length) return false;

            int[] weightsFirst = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] weightsSecond = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int sum = 0;

            if (position == 12)
            {
                for (int i = 0; i < 12; i++)
                {
                    int digit = numbers[i];
                    if (digit < 0 || digit > 9) return false;
                    sum += digit * weightsFirst[i];
                }
            }
            else 
            {
                for (int i = 0; i < 13; i++)
                {
                    int digit = numbers[i];
                    if (digit < 0 || digit > 9) return false;
                    sum += digit * weightsSecond[i];
                }
            }

            int remainder = sum % 11;
            int expected = remainder < 2 ? 0 : 11 - remainder;
            return numbers[position] == expected;
        }

        /// <summary>
        /// Formata um CNPJ adicionando a pontuação padrão brasileira.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser formatado (apenas dígitos).</param>
        /// <returns>O CNPJ formatado no padrão XX.XXX.XXX/XXXX-XX.</returns>
        /// <exception cref="ArgumentException">Lançada quando o CNPJ não contém exatamente 14 dígitos após normalização.</exception>
        /// <example>
        /// <code>
        /// string formatted = FormatCnpj("11444777000161"); // Retorna "11.444.777/0001-61"
        /// </code>
        /// </example>
        public static string FormatCnpj(string? cnpj)
        {
            var digits = NormalizeDigits(cnpj);
            if (digits.Length != 14) throw new ArgumentException("CNPJ deve conter 14 dígitos para formatação.", nameof(cnpj));

            return $"{digits.Substring(0, 2)}.{digits.Substring(2, 3)}.{digits.Substring(5, 3)}/{digits.Substring(8, 4)}-{digits.Substring(12, 2)}";
        }

        /// <summary>
        /// Verifica se um tipo de pessoa é válido (F ou J).
        /// </summary>
        /// <param name="tipo">String representando o tipo de pessoa.</param>
        /// <returns>True se o tipo for válido (F ou J após normalização), False caso contrário.</returns>
        /// <example>
        /// <code>
        /// bool isValid = IsValidTipoPessoa("Física"); // Retorna true
        /// </code>
        /// </example>
        public static bool IsValidTipoPessoa(string? tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) return false;
            
            try
            {
                var norm = NormalizeTipoPessoa(tipo);
                return norm == "F" || norm == "J";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Normaliza o tipo de pessoa para o formato padrão do sistema (F ou J).
        /// </summary>
        /// <param name="tipo">String representando o tipo de pessoa.</param>
        /// <returns>String normalizada "F" para pessoa física ou "J" para pessoa jurídica.</returns>
        /// <exception cref="ArgumentException">
        /// Lançada quando o tipo é vazio ou quando não é reconhecido como um tipo válido.
        /// </exception>
        /// <example>
        /// <code>
        /// string tipo = NormalizeTipoPessoa("Física"); // Retorna "F"
        /// string tipo = NormalizeTipoPessoa("JURÍDICA"); // Retorna "J"
        /// </code>
        /// </example>
        public static string NormalizeTipoPessoa(string? tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) throw new ArgumentException("TipoPessoa não pode ser vazio.", nameof(tipo));

            var t = tipo.Trim().ToUpperInvariant();
            t = RemoveDiacritics(t);

            if (t == "F" || t == "PF" || t == "PESSOAFISICA" || t == "FISICA")
                return "F";

            if (t == "J" || t == "PJ" || t == "PESSOAJURIDICA" || t == "JURIDICA")
                return "J";

            throw new ArgumentException($"TipoPessoa desconhecido: '{tipo}'. Valores válidos: 'F' ou 'J'.", nameof(tipo));
        }

        /// <summary>
        /// Remove acentos e caracteres diacríticos de uma string.
        /// </summary>
        /// <param name="input">String com possíveis acentos ou diacríticos.</param>
        /// <returns>String sem acentos ou diacríticos.</returns>
        /// <example>
        /// <code>
        /// string result = RemoveDiacritics("Física"); // Retorna "Fisica"
        /// </code>
        /// </example>
        private static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}