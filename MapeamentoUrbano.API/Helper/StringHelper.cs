namespace MapeamentoUrbano.API.Helper
{
    public class StringHelper
    {
        public static List<string> ValidateString(string str, string variable)
        {
            List<string> validationErrors = new();

            if (str.Trim().Length < 2)
                validationErrors.Add($"{variable} deve ter pelo menos 2 caracteres.");
            if (str.Length > 100)
                validationErrors.Add($"{variable} deve ter no máximo 100 caracteres.");
            if (NameHelper.VerifyIfNameContainsSpecialCharacters(str))
                validationErrors.Add($"Números e caracteres especiais não são permitidos em {variable}.");

            return validationErrors;
        }
    }
}
