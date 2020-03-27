using System.Xml;
using NUnit;

namespace Ghpr.NUnit.Extensions
{
    public static class XmlNodeExtensions
    {
        public static string Val(this XmlNode node) => node.GetAttribute("value");
    }
}