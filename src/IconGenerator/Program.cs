const string kindTemplate =
    """
    using System.ComponentModel;

    namespace MahApps.Metro.IconPacks;

    /// ******************************************
    /// This code is auto generated. Do not amend.
    /// ******************************************

    /// <summary>
    /// List of available icons for use with <see cref="PackIconMaterial" />.
    /// </summary>
    /// <remarks>
    /// All icons sourced from Material Design Icons Font <see><cref>https://materialdesignicons.com</cref></see>
    /// In accordance of <see><cref>https://github.com/Templarian/MaterialDesign/blob/master/LICENSE</cref></see>.
    /// </remarks>
    public enum PackIconMaterialKind
    {
    INSERT_HERE}

    """;

const string solutionDir = @"..\..\..\..\..\";
const string svgDir = solutionDir + @"..\..\MaterialDesignSvgo\svg";

var icons = new List<(string Name, string Path)> { ("None", string.Empty) };

Console.WriteLine("Reading icon data...");

foreach (var path in Directory.EnumerateFiles(svgDir))
{
    var fileName = Path.GetFileNameWithoutExtension(path);
    var iconName = ToCamelCase(fileName);
    if (iconName is "CheckBoxMultipleOutline" or "CheckBoxOutline")
    {
        continue;
    }

    var text = File.ReadAllText(path);
    var svgIndex = text.IndexOf("path d=\"", StringComparison.Ordinal);
    var svg = text[(svgIndex + 8)..^9];
    icons.Add((ToCamelCase(fileName), svg));

    if (icons.Count % 500 is 0)
    {
        Console.WriteLine($"Reading icon data {icons.Count}...");
    }
}

Console.WriteLine($"Total icons: {icons.Count}");
Console.WriteLine("\nGenerating icon kind...");

var stringBuilder = new System.Text.StringBuilder();

foreach (var (name, _) in icons)
{
    stringBuilder.AppendLine($"    {name},");
}

var contents = kindTemplate.Replace("INSERT_HERE", stringBuilder.ToString());
stringBuilder.Clear();
File.WriteAllText(solutionDir + @"src\MahApps.Metro.IconPacks\PackIconMaterialKind.cs", contents);

Console.WriteLine("Generating json...");
var dictionary = new Dictionary<int, string>();
for (var i = 0; i < icons.Count; ++i)
{
    dictionary.Add(i, icons[i].Path);
}

var json = System.Text.Json.JsonSerializer.Serialize(dictionary);
File.WriteAllText(solutionDir + @"src\MahApps.Metro.IconPacks\Resources\PackIconMaterialData.json", json);
return;

static string ToCamelCase(string source)
{
    source = char.ToUpperInvariant(source[0]) + source[1..];
    while (source.Contains('-'))
    {
        var index = source.IndexOf('-');
        if (index == source.Length - 1)
        {
            return source[..^1];
        }

        if (char.IsDigit(source[index + 1]))
        {
            source = source[..index] + source[(index + 1)..];
        }
        else if (char.IsLetter(source[index + 1]))
        {
            source = source[..index]
                     + source.Substring(index + 1, 1).ToUpperInvariant()
                     + source[(index + 2)..];
        }
        else
        {
            break;
        }
    }

    return source;
}
