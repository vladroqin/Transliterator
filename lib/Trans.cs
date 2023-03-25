using System.Text;

namespace Transliterate;

public class Trans : ITrans
{
  private IDictionary<char, Entity> _transTable;

  public Trans(IDictionary<char, Entity> transTable)
  {
    _transTable = transTable;
  }

  public Trans(string fileName)
  {
    LoadTransTable load = new();
    _transTable = load.Load(fileName);
  }

  public Trans(IEnumerable<string> strings)
  {
    LoadTransTable load = new();
    _transTable = load.Load(strings);
  }

  public Trans(IEnumerable<ValueTuple<string, string>> coupleStrings)
  {
    LoadTransTable load = new();
    _transTable = load.Load(coupleStrings);
  }

  /// <summary>
  /// Найти транслитерируемый символ
  /// </summary>
  /// <param name="s">Транслитерируемая строка</param>
  /// <param name="dic">Словарь для транслитерации</param>
  /// <param name="i">Порядковый номер</param>
  /// <param name="result">Результат</param>
  private void Do(string s, IDictionary<char, Entity> dic, ref int i, ref StringBuilder result)
  {
    if (dic.TryGetValue(s[i], out Entity foundEntity))
    {
      if (foundEntity.Results == null)
      {
        WorstCase(foundEntity, result, s[i]);
      }
      else
      {
        if (i + 1 < s.Length)
        {
          int x = Do(s, foundEntity, i, ref result);
          if (x != 0)
          {
            i = i + x;
          }
          else
          {
            WorstCase(foundEntity, result, s[i]);
          }
        }
        else
        {
          WorstCase(foundEntity, result, s[i]);
        }
      }
    }
    else
    {
      result.Append(s[i]);
    }
  }

  /// <summary>
  /// Найти возможные транслитерируемые символы
  /// </summary>
  /// <param name="s">Транслитерируемая строка</param>
  /// <param name="entity">Где надо искать транслитерируемые символы</param>
  /// <param name="i">Порядковый номер</param>
  /// <param name="result">Результат</param>
  /// <returns>Сколько символов использовано</returns>
  private int Do(string s, Entity entity, int i, ref StringBuilder result)
  {
    int x = i + 1;
    if (x < s.Length && entity.Results.TryGetValue(s[x], out Entity foundEntity))
    {
      if (foundEntity.Results == null)
      {
        result.Append(foundEntity.Result);
        return 1;
      }
      else
      {
        if (x + 1 > s.Length)
          return 1;

        int count = Do(s, foundEntity, x, ref result);

        if (count == 0 && foundEntity.Result != default)
        {
          result.Append(foundEntity.Result);
        }

        return count + 1;
      }
    }

    return 0;
  }

  /// <summary>
  /// Попытаться подобрать символ для транслитерации
  /// </summary>
  /// <param name="foundEntity">Ветка для транслитерации</param>
  /// <param name="result">Результат</param>
  /// <param name="si">Порядковый номер</param>
  private void WorstCase(Entity foundEntity, StringBuilder result, char si)
  {
    if (foundEntity.Result != default)
    {
      result.Append(foundEntity.Result);
    }
    else
    {
      result.Append(si);
    }
  }

  /// <summary>
  /// Транслитерировать строку
  /// </summary>
  /// <param name="s">Транслитерируямая строка</param>
  /// <returns>Транслитерируемая строка</returns>
  public string DoTrans(string s)
  {
    if(string.IsNullOrEmpty(s))
      return string.Empty;

    int stringLength = s.Length;
    StringBuilder result = new(stringLength * 21 / 20); // С запасом :)

    for (int i = 0; i < stringLength; i++)
    {
      Do(s, _transTable, ref i, ref result);
    }

    return result.ToString();
  }
}