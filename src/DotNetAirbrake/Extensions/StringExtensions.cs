namespace DotNetAirbrake
{
    public static class StringExtensions
    {
        public static string ToUndercase(this string value)
        {
            return value.Replace('-', '_');
        }
    }
}