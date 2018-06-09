namespace NUnitTests
{
    public static class TestData
    {
        public const string Actual = @"
<?xml version=""1.0""?>
<PARTS>
   <TITLE>computer Parts</TITLE>
<PARTS>
";

        public const string Expected = @"
<?xml version=""1.0""?>
<PARTS>
   <TITLE>Computer Parts</TITLE>
</PARTS>
";
    }
}