namespace Transliterate;

public struct Entity
{
  public string? Result { get; internal set; }
  // Думаю в будущем применить ReadOnly/Immutable/FrozenDictionary
  public IDictionary<char, Entity>? Results { get; internal set; }
}