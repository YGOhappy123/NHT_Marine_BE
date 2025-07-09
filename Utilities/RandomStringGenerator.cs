using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Utilities
{
    public class RandomStringGenerator
    {
        private static readonly char[] Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        public static string GenerateRandomString(int length = 16)
        {
            if (length < 1)
            {
                throw new ArgumentException("Length must be greater than 0.");
            }

            var random = new Random();
            return new string([.. Enumerable.Range(0, length).Select(_ => Characters[random.Next(Characters.Length)])]);
        }
    }
}
