namespace Chat.Utility
{
    public static class StringExtensions
    {
        public static int Utf8WorstCaseLength(this string s) => s.Length * 4;
    }
}