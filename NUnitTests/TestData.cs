namespace NUnitTests
{
    public static class TestData
    {
        public const string Actual = @"
<?xml version=""1.0""?>
<PARTS>
   <TITLE>Computer Parts</TITLE>
   <PART>
      <ITEM>Motherboard</ITEM>
      <MANUFACTURER>ASUS</MANUFACTURER>
      <MODEL>P3B-F</MODEL>
      <COST>123.00</COST>
   </PART>
</PARTS>
";

        public const string Expected = @"
<?xml version=""1.0""?>
<PARTS>
   <TITLE>Computer Parts</TITLE>
   <PART>
      <ITEM>Motherboard</ITEM>
      <MANUFACTURER>ASWS</MANUFACTURER>
      <MODEL>P3B-F</MODEL>
      <COST>124.00</COSTT>
   </QWER>
</PARTS>
";
    }
}