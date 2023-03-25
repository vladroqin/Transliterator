namespace Transliterate;

public interface ITrans
{
  /// <summary>
  /// Транслитерировать строку
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>Результат</returns>
  public string DoTrans(string s);
}