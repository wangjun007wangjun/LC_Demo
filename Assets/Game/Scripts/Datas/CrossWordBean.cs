using System;
using BaseFramework;

[Serializable]
public class CrossWordBean
{
    public int rowIndex;
    public int colIndex;
    public CrossDirection direction;
    public string word;

    public CrossWordBean(string str)
    {
        if (str.IsNullOrEmpty())
        {
            Log.E(this, "parse error, input string is null");
            return;
        }

        string[] infos = str.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);

        if (infos.Length != 4)
        {
            Log.E(this, $"parse error, str:{str}, split length:{infos.Length}");
            return;
        }

        try
        {
            rowIndex = int.Parse(infos[1]);
            colIndex = int.Parse(infos[0]);
        }
        catch
        {
            Log.E(this, $"parse error, str:{str}, index parse error");
            return;
        }

        if (infos[2].ToLower() == "v"
            || infos[2].ToLower() == "h")
        {
            direction = infos[2].ToLower() == "v"
                            ? CrossDirection.Vertical
                            : CrossDirection.Horizontal;
        }
        else
        {
            Log.E(this, $"parse error, str:{str}, direction parse error");
            return;
        }

        word = infos[3];
    }
}