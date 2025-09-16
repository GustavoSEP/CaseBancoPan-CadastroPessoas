using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CadastroPessoas.Application.Helpers
{
    public static class DocumentoHelper
    {
        private static readonly Regex OnlyDigitsRegex = new(@"[^\d]", RegexOptions.Compiled);

        public static string NormalizeDigits(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return OnlyDigitsRegex.Replace(value, string.Empty);
        }

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

        public static string FormatCpf(string? cpf)
        {
            var digits = NormalizeDigits(cpf);
            if (digits.Length != 11) throw new ArgumentException("CPF deve conter 11 dígitos para formatação.", nameof(cpf));

            return $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9, 2)}";
        }
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
        public static string FormatCnpj(string? cnpj)
        {
            var digits = NormalizeDigits(cnpj);
            if (digits.Length != 14) throw new ArgumentException("CNPJ deve conter 14 dígitos para formatação.", nameof(cnpj));

            return $"{digits.Substring(0, 2)}.{digits.Substring(2, 3)}.{digits.Substring(5, 3)}/{digits.Substring(8, 4)}-{digits.Substring(12, 2)}";
        }
        public static bool IsValidTipoPessoa(string? tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo)) return false;
            var norm = NormalizeTipoPessoa(tipo);
            return norm == "F" || norm == "J";
        }
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
