using System.Text.Json;

namespace Kt.Data.IO;

public class Pkg
{
    // Metadata vv
    public string? Name {get; set;}
    public string? Author {get; set;}
    public string? Updated {get; set;}

    // Provides vv
    public Dictionary<string, Rule>? Definitions {get; set;}
    public Dictionary<string, Action>? Actions {get; set;}
    public Dictionary<string, Unit>? Units {get; set;}
    public Dictionary<string, Team>? Teams {get; set;}

    public IEnumerable<Rule> AllRules() => (Definitions?.Values ?? Enumerable.Empty<Rule>()).Concat((Actions?.Values ?? Enumerable.Empty<Rule>()));


    internal class PkgIndex
    {
        // Metadata vv
        public string? Name {get; set;}
        public string? Author {get; set;}
        public string? Updated {get; set;}

        // Provides vv
        public List<string>? Definitions {get; set;}
        public List<string>? Actions {get; set;}
        public List<string>? Units {get; set;}
        public List<string>? Teams {get; set;}
    }

    private static JsonSerializerOptions json = SerializationOptions;

    public static JsonSerializerOptions SerializationOptions => new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private static List<string> EMPTY = new (0);

    /// <summary>
    /// Read a package from a network location where the URL points to the package's index.json file
    /// </summary>
    /// <param name="url">Path to package index</param>
    /// <returns>package</returns>
    public static async Task<Pkg> FromUrl(HttpClient client, string url)
    {
        var pkg = new Pkg();
        pkg.Author = Environment.UserName;
        pkg.Name = Path.GetFileNameWithoutExtension(url);

        // Read the package index
        var resp = await client.GetAsync(url + "/index.json");
        var index = JsonSerializer.Deserialize<PkgIndex>(resp.Content.ReadAsStream(), json);
        if (index is null)
            return pkg;

        pkg.Name = index.Name;
        pkg.Author = index.Author;
        pkg.Updated = index.Updated;

        // Convert the paths in the package index into actual resources by parsing them one at a time
        pkg.Definitions   = await ParseResourcesFromRelativeUrl<Rule>   (client, url, index.Definitions ?? EMPTY);
        pkg.Actions = await ParseResourcesFromRelativeUrl<Action> (client, url, index.Actions ?? EMPTY);
        pkg.Units   = await ParseResourcesFromRelativeUrl<Unit>   (client, url, index.Units ?? EMPTY);
        pkg.Teams   = await ParseResourcesFromRelativeUrl<Team>   (client, url, index.Teams ?? EMPTY);

        var allRulesDict = pkg.AllRules().Where(r => r.Id is not null).ToDictionary(r => r.Id ?? string.Empty, r => r);

        foreach (var team in pkg.Teams)
        {
            team.Value.Units = (team.Value.UnitIds ?? Enumerable.Empty<string>()).Select(id =>
            {
                if (pkg.Units.TryGetValue(id, out var unit))
                    return unit;
                return null;
            })
            .Where(unit => unit is not null)
            .Cast<Unit>()
            .ToList();

            team.Value.Rules = allRulesDict;
        }

        return pkg;
    }

    private static async Task<Dictionary<string, T>> ParseResourcesFromRelativeUrl<T>(HttpClient client, string url, List<string> relatives)
    where T:IPackagedContent
    {
        Dictionary<string, T> lst = new Dictionary<string, T>(relatives.Count);

        for (var i = 0; i < relatives.Count; i++)
        {
            var uri = url + relatives[i];
            var resp = await client.GetAsync(uri);
            var content = await resp.Content.ReadAsStringAsync();
            var doc = new FmHtmlDocument(content);
            var data = JsonSerializer.Deserialize<T>(doc.FrontMatter, json);
            if (data is null)
                continue;

            // Assign the id and description 
            data.Id = Path.GetFileNameWithoutExtension(relatives[i]);
            data.Description = doc.Html.ToString();
            lst[data.Id] = data;
        }

        return lst;
    }

    /// <summary>
    /// Read a package from a local file where the path points to the package's index.json file
    /// </summary>
    /// <param name="url">Path to package file *.40pkg</param>
    /// <returns>package</returns>
    public static Pkg FromFile(string path)
    {
        throw new NotImplementedException();
    }
}