using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Configuration;
using Transliterate;

namespace Avalon;

public partial class MainWindow : Window
{
  private readonly Trans _trans;
  public MainWindow()
  {
    var dict_fn = ConfigurationManager.AppSettings["TRANS_DICT"];
    if (!string.IsNullOrEmpty(dict_fn))
      _trans = new Trans(dict_fn);
    else
      throw new NullReferenceException("Where's is the file?");

    InitializeComponent();
  }
  public void MakeTrans(object sender, KeyEventArgs e)
  {
    if (e.Key != Key.Enter)
      return;
    Output.Text = _trans.DoTrans(Input.Text);
  }
}