using System;
using System.Collections.Generic;

namespace MahApps.Metro.IconPacks;

public static class PackIconMaterialDataFactory
{
    public static Lazy<IDictionary<PackIconMaterialKind, string>> DataIndex { get; }

    static PackIconMaterialDataFactory()
    {
        DataIndex = new Lazy<IDictionary<PackIconMaterialKind, string>>(Create);
    }

    public static IDictionary<PackIconMaterialKind, string> Create()
    {
        var assembly = typeof(PackIconMaterialDataFactory).Assembly;
        using var stream = assembly.GetManifestResourceStream("PackIconMaterialData.json");
        var dictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<PackIconMaterialKind, string>>(stream!);
        return dictionary;
    }
}
