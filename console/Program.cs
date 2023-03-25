using System.Configuration;
using Transliterate;

namespace ConsoleTrans;

public class Program
{
  private static Trans TRANS;
  private static string BEGIN;
  private static string END;
  private static int BEGIN_SIZE;
  private static int END_SIZE;

  /// <summary>
  /// Подсчитывает количество вхождений начала.
  /// </summary>
  /// <param name="s">Строка для поиска</param>
  /// <returns>Количество вхождений</returns>
  private static int CountBegins(string s)
  {
    int count = 0;
    int position = -1;
    while ((position = s.IndexOf(BEGIN, position + 1)) > -1)
      ++count;
    return count;
  }

  /// <summary>
  /// Производит транслитерацию во всём документе
  /// </summary>
  /// <param name="s">Документ</param>
  /// <returns>Результат</returns>
  private static string Work(string s)
  {
    if (String.IsNullOrEmpty(s))
      return string.Empty;

    var result = new List<string>(CountBegins(s) * 2 + 1);
    int size = s.Length;
    int position = 0;
    do
    {
      int begin = s.IndexOf(BEGIN, position);
      if (begin == -1)
      {
        string ss = s.Substring(position);
        result.Add(ss);
        break;
      }
      result.Add(s.Substring(position, begin - position));
      int end = s.IndexOf(END, begin + BEGIN_SIZE);
      if (end == -1)
      {
        // Ну значит до конца
        string ss = s.Substring(begin + BEGIN_SIZE);
        result.Add(TRANS.DoTrans(ss));
        break;
      }
      string forTrans = s.Substring(begin + BEGIN_SIZE, (end - begin) - 1);
      result.Add(TRANS.DoTrans(forTrans));
      position = end + END_SIZE;
    } while (position < size);

    return String.Join(null, result);
  }

  /// <summary>
  /// Воспользуюсь конфигом
  /// </summary>
  private static void CheckConfig()
  {
    // Если чего-то нет, то пусть лучше сразу вылетит
    TRANS = new Trans(ConfigurationManager.AppSettings["TRANS_DICT"]);
    BEGIN = ConfigurationManager.AppSettings["BEGIN"];
    END = ConfigurationManager.AppSettings["END"];
    BEGIN_SIZE = BEGIN.Length;
    END_SIZE = END.Length;
  }

  public static async Task Main(string[] args)
  {
    // Console.OutputEncoding = UnicodeEncoding.UTF8;

    #region checks
    if (args.Length < 2)
    {
      Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} inFile outFile");
      return;
    }

    if (!File.Exists(args[0]))
    {
      Console.Error.WriteLine("Where's the file?");
      return;
    }

    if (File.Exists(args[1]))
    {
      Console.Error.WriteLine($"The file {args[1]} exists!");
      return;
    }

    if (
        !File.Exists(
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath
        )
    )
    {
      Console.Error.WriteLine("Where's the config file?");
      return;
    }
    #endregion

    CheckConfig();

    var input = await File.ReadAllTextAsync(args[0]);
    var output = Work(input);
    using var sw = new StreamWriter(args[1]);
    await sw.WriteAsync(output);

    Console.ReadKey();
  }
}