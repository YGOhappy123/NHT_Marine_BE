namespace NHT_Marine_BE.Utilities
{
    public static class CapitalizeWords
    {
        public static string CapitalizeAllWords(this string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                return originalString;
            }

            var capitalizedWords = originalString.ToLower().Split(' ').Select(word => word.CapitalizeSingleWord()).ToArray();

            return string.Join(" ", capitalizedWords);
        }

        public static string CapitalizeSingleWord(this string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }

            return char.ToUpper(word[0]) + word[1..];
        }
    }
}
