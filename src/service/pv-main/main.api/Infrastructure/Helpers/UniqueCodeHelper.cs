using System;
using System.Linq;

namespace OpenRIMS.PV.Main.API.Helpers
{
    public static class UniqueCodeHelper
    {
        private static long _lastTick;
        private static readonly char[] _characters =
            @"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_+-=[]{};':,./<>?~`|\"
            .ToCharArray();

        public static string BaseConvert(long number, char[] characters = null)
        {
            if (characters == null || !characters.Any()) characters = _characters;
            var b = new char[Math.Max((int)Math.Ceiling(Math.Log(number + 1, characters.Length)), 1)];
            var i = b.Length;
            for (; ; )
            {
                b[--i] = characters[number % characters.Length];
                number = number / characters.Length;
                if (number <= 0) break;
            }
            return new string(b, i, b.Length - i);
        }

        public static long BaseConvert(string value, char[] characters = null)
        {
            if (characters == null || !characters.Any()) characters = _characters;
            var cv = characters.Select((c, i) => new { Char = c, Index = i })
                .ToDictionary(c => c.Char, c => c.Index);

            var chrs = value.ToCharArray();
            var m = chrs.Length - 1;
            long result = 0;
            for (int i = 0; i < chrs.Length;)
                result += cv[chrs[i++]] * (long)Math.Pow(characters.Length, m--);
            return result;
        }

        public static long GenerateLong()
        {
            lock (_characters) while (_lastTick == DateTime.UtcNow.Ticks) { }
            var array = $"{_lastTick = DateTime.UtcNow.Ticks}".ToArray();
            Array.Reverse(array);
            return Convert.ToInt64(new string(array));
        }

        public static string SimpleCode() =>
            BaseConvert(GenerateLong(), "ABCDEFGHJKMNPQRSTUVWXYZ".ToArray());

        public static string NormalCode() =>
            BaseConvert(GenerateLong(), "23456789ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz".ToArray());

        public static string ComplexCode() =>
            BaseConvert(GenerateLong());

        public static string CustomCode(string characters = null) =>
            BaseConvert(GenerateLong(), characters?.ToArray());
    }
}
