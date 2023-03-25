using Xunit;
using Transliterate;

namespace lib.Tests;

public class LoadTransTableTests
{
  private const string GOST1_FN = "gost.tab";
  private const string GOST2_FN = "gost_lc.tab";
  private const int GOST1_COUNT = 66;
  private const int GOST2_COUNT = 45;

  [Theory]
  [InlineData(GOST1_FN, GOST1_COUNT)]
  [InlineData(GOST2_FN, GOST2_COUNT)]
  public void Load_simple_correct_table_by_fn(string fn, int count)
  {
    var forTest = new LoadTransTable();
    var preresult = forTest.Load(fn);
    int result = preresult.Count();
    Assert.True(result == count, "File not loading correctly");
  }

  [Theory]
  [InlineData(GOST1_FN, GOST1_COUNT)]
  [InlineData(GOST2_FN, GOST2_COUNT)]
  public void Load_simple_correct_table_by_array(string fn, int count)
  {
    var forTest = new LoadTransTable();
    var stringsArray = File.ReadAllLines(fn);
    var preresult = forTest.Load(stringsArray);
    int result = preresult.Count();
    Assert.True(result == count, "File not loading correctly");
  }

  [Theory]
  [InlineData(GOST1_FN, GOST1_COUNT)]
  [InlineData(GOST2_FN, GOST2_COUNT)]
  public void Load_simple_correct_table_by_tupple_array(string fn, int count)
  {
    var forTest = new LoadTransTable();
    var stringsArray = File.ReadAllLines(fn);
    var tuppleList = new List<(string, string)>(stringsArray.Length);
    foreach (var e in stringsArray)
    {
      var twoCells = e.Split('\t');
      tuppleList.Add((twoCells[0], twoCells[1]));
    }
    var preresult = forTest.Load(tuppleList);
    int result = preresult.Count();
    Assert.True(result == count, "File not loading correctly");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("aaa")]
  [InlineData("aaa\t")]
  [InlineData("\taaa")]
  public void Try_load_incorrect_string(string s)
  {
    var forLoadArray = new string[] { s };
    var forTest = new LoadTransTable();
    var result = true;
    try
    {
      forTest.Load(forLoadArray);
    }
    catch (Exception)
    {
      result = false;
    }
    Assert.False(result, "Can load incorrect string");
  }
}