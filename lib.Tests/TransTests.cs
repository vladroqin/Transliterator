using Xunit;
using Transliterate;

namespace lib.Tests;

public class TransTests
{
  private const string GOST1_FN = "gost.tab";
  private const string GOST2_FN = "gost_lc.tab";
  private const string string1 = "Съешь же ещё этих мягких французских булок да выпей чаю";
  private const string string2 =
      "S''esh' zhe eshhjo ehtikh mjagkikh francuzskikh bulok da vypejj chaju";
  private const string string3 = "Веснушчатый";
  private const string string4 = "Vesnushchatyjj";

  [Theory]
  [InlineData(GOST1_FN, string1, string2)]
  [InlineData(GOST2_FN, string2, string1)]
  [InlineData(GOST1_FN, string3, string4)]
  [InlineData(GOST2_FN, string4, string3)]
  public void Try_transliterate(string dict, string source, string result)
  {
    var trans = new Trans(dict);
    var outString = trans.DoTrans(source);
    Console.Error.WriteLine($"<<{outString}>>");
    Assert.True(result == outString, "The string isn't transliterated correctly");
  }
}