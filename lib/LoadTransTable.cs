namespace Transliterate;

public class LoadTransTable
{
  /// <summary>
  /// Загрузить строчку и обработать
  /// </summary>
  /// <param name="keyString">Исходный символ</param>
  /// <param name="valueString">Результат</param>
  /// <param name="dic">Словарь для проверки</param>
  /// <returns>Словарь</returns>
  private IDictionary<char, Entity> PairOfChar(
      string keyString,
      string valueString,
      IDictionary<char, Entity> dic
  )
  {
    int keyLength = keyString.Length;
    char firstCharacter = keyString[0];

    if (keyLength == 1)
    {
      if (dic.TryGetValue(firstCharacter, out Entity oldValue))
      {
        oldValue.Result = valueString;
        dic[firstCharacter] = oldValue;
      }
      else
      {
        dic.Add(firstCharacter, new() { Result = valueString });
      }
    }
    else
    {
      string subString = keyString.Substring(1);
      if (dic.TryGetValue(firstCharacter, out Entity oldValue))
      {
        oldValue.Results = oldValue.Results ?? new Dictionary<char, Entity>();
        oldValue.Results = PairOfChar(subString, valueString, oldValue.Results);
        dic[firstCharacter] = oldValue;
      }
      else
      {
        IDictionary<char, Entity> newDict = new Dictionary<char, Entity>();
        newDict = PairOfChar(subString, valueString, newDict);
        dic.Add(firstCharacter, new() { Results = newDict });
      }
    }
    return dic;
  }

  /// <summary>
  /// Загрузить tab-файл с правилами транслитерации
  /// </summary>
  /// <param name="fileName">Имя tab-файла</param>
  /// <returns>Словарь с правилами</returns>
  public IDictionary<char, Entity> Load(string fileName)
  {
    var fromFile = File.ReadAllLines(fileName);
    return Load(fromFile);
  }

  /// <summary>
  /// Загрузить множество строк с правилами транслитерации, разделёнными табуляциями
  /// </summary>
  /// <param name="strings">Множество строк с правилами транслитерации, разделёнными табуляциями</param>
  /// <returns>Словарь с правилами</returns>
  public IDictionary<char, Entity> Load(IReadOnlyCollection<string> strings)
  {
    var workpiece = new List<(string, string)>(strings.Count());
    foreach (var e in strings)
    {
      var im = e.Split('\t');
      workpiece.Add((im[0], im[1]));
    }
    return Load(workpiece);
  }

  /// <summary>
  /// Загрузить множество строк с правилами транслитерации и результатами
  /// </summary>
  /// <param name="coupleStrings">Множество строк с правилами транслитерации и результатами</param>
  /// <returns>Словарь с правилами</returns>
  public IDictionary<char, Entity> Load(IReadOnlyCollection<ValueTuple<string, string>> coupleStrings)
  {
    var result = new Dictionary<char, Entity>(coupleStrings.Count());
    foreach (var e in coupleStrings)
    {
      if (string.IsNullOrEmpty(e.Item1) || string.IsNullOrEmpty(e.Item2))
        throw new FormatException("Lines can't be empty");

      PairOfChar(e.Item1, e.Item2, result);
    }
    return result;
  }
}