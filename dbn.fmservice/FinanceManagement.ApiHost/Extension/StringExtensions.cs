namespace FinanceManagement.ApiHost.Extension
{
    public static class StringExtensions
    {
        public static bool IsValidNumer(this string value)
        {
            return !string.IsNullOrEmpty(value) && value != "0" && value != "null" && value != "undefine";
        }
    }
}
