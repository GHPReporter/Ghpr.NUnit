using System;
using System.Linq;

namespace Ghpr.NUnit.Extensions
{
    public static class StringExtensions
    {
        public static bool EndsWithAnyOf(this string s, params string[] values)
        {
            return values.Any(value => s.EndsWith(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool EndsWithImgExtension(this string s)
        {
            return s.EndsWithAnyOf(".png", "jpg", ".jpeg", ".bmp");
        }
    }
}