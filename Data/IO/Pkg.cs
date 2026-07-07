using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

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
    public Dictionary<string, Effect>? Effects {get; set;}
    public Dictionary<string, Equipment>? Equipment {get; set;}
    public Dictionary<string, Ploy>? Ploys {get; set;}
    public Dictionary<string, Unit>? Units {get; set;}
    public Dictionary<string, Team>? Teams {get; set;}

    public Dictionary<string, Objective>? Objectives {get; set;}

    // Rule aliasing (if a rule is known by other ids as well)
    public Dictionary<string, IEnumerable<string>?>? Aliases {get; set;}

    public IPackagedContent? GetPackagedContent(string id)
    {
        if (Definitions?.TryGetValue(id, out var def) ?? false)
            return def;
        if (Actions?.TryGetValue(id, out var act) ?? false)
            return act;
        if (Effects?.TryGetValue(id, out var eff) ?? false)
            return eff;
        if (Equipment?.TryGetValue(id, out var eqp) ?? false)
            return eqp;
        if (Ploys?.TryGetValue(id, out var ply) ?? false)
            return ply;
        if (Units?.TryGetValue(id, out var unt) ?? false)
            return unt;
        if (Teams?.TryGetValue(id, out var tam) ?? false)
            return tam;

        return null;
    }

    public IEnumerable<IPackagedContent> Provides()
    {
        return 
        (Definitions?.Values ?? Enumerable.Empty<IPackagedContent>())
        .Concat((Teams?.Values ?? Enumerable.Empty<IPackagedContent>()))
        .Concat((Equipment?.Values ?? Enumerable.Empty<IPackagedContent>()))
        .Concat((Ploys?.Values ?? Enumerable.Empty<IPackagedContent>()))
        .Concat((Units?.Values ?? Enumerable.Empty<IPackagedContent>()))
        .Concat((Actions?.Values ?? Enumerable.Empty<IPackagedContent>()))
        .Concat((Effects?.Values ?? Enumerable.Empty<IPackagedContent>()))
        .Concat((Objectives?.Values ?? Enumerable.Empty<IPackagedContent>()))
        ;
    }

    public IEnumerable<Rule> AllRules() => 
        (Definitions?.Values ?? Enumerable.Empty<Rule>())
        .Concat((Actions?.Values ?? Enumerable.Empty<Rule>()))
        .Concat((Effects?.Values ?? Enumerable.Empty<Rule>()))
        .Concat((Equipment?.Values ?? Enumerable.Empty<Rule>()))
        .Concat((Ploys?.Values ?? Enumerable.Empty<Rule>()))
        .Concat((Objectives?.Values ?? Enumerable.Empty<Rule>()));


    public class PkgIndex
    {
        // Metadata vv
        public string? Name {get; set;}
        public string? Author {get; set;}
        public string? Updated {get; set;}

        // Provides vv
        public List<string>? Definitions {get; set;}
        public List<string>? Actions {get; set;}
        public List<string>? Effects {get; set;}
        public List<string>? Equipment {get; set;}
        public List<string>? Ploys {get; set;}
        public List<string>? Units {get; set;}
        public List<string>? Teams {get; set;}
        public List<string>? Objectives {get; set;}

        public Dictionary<string, IEnumerable<string>?>? Aliases {get; set;}
    }

    private static JsonSerializerOptions json = SerializationOptions;

    public static JsonSerializerOptions SerializationOptions => new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true
    };

    private static List<string> EMPTY = new (0);

    private static async Task<HttpResponseMessage> GetOneOfAsync(HttpClient client, params string[] urls)
    {
        foreach (var url in urls)
        {
            var resp = await client.GetAsync(url);
            if (resp.IsSuccessStatusCode)
                return resp;
        }
        throw new AggregateException(urls.Select(url => new FileNotFoundException(url)));
    }

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
        PkgIndex? index = null;
        try {
            var resp = await GetOneOfAsync(client, url + "/index.json", url + "/index.jsonc"); //await client.GetAsync(url + "/index.json");
            index = JsonSerializer.Deserialize<PkgIndex>(resp.Content.ReadAsStream(), json);
            if (index is null)
                return pkg;
        } catch (AggregateException ex)
        {
            throw new FileNotFoundException($"Could not find package index at {url}/index.json or {url}/index.jsonc", ex);
        }

        pkg.Name = index.Name;
        pkg.Author = index.Author;
        pkg.Updated = index.Updated;

        // Convert the paths in the package index into actual resources by parsing them one at a time
        var definitionTask = ParseResourcesFromRelativeUrl<Rule>   (pkg, client, url, index.Definitions ?? EMPTY);
        var actionTask     = ParseResourcesFromRelativeUrl<Action> (pkg, client, url, index.Actions ?? EMPTY);
        var effectTask     = ParseResourcesFromRelativeUrl<Effect> (pkg, client, url, index.Effects ?? EMPTY);
        var equipmentTask  = ParseResourcesFromRelativeUrl<Equipment>(pkg, client, url, index.Equipment ?? EMPTY);
        var ployTask       = ParseResourcesFromRelativeUrl<Ploy>   (pkg, client, url, index.Ploys ?? EMPTY);
        var unitTask       = ParseResourcesFromRelativeUrl<Unit>   (pkg, client, url, index.Units ?? EMPTY);           
        var teamTask       = ParseResourcesFromRelativeUrl<Team>   (pkg, client, url, index.Teams ?? EMPTY);
        var objTask        = ParseResourcesFromRelativeUrl<Objective>(pkg, client, url, index.Objectives ?? EMPTY);

        await Task.WhenAll(definitionTask, actionTask, effectTask, equipmentTask, ployTask, unitTask, teamTask, objTask);

        pkg.Definitions = await definitionTask;
        pkg.Actions     = await actionTask;
        pkg.Effects     = await effectTask;
        pkg.Equipment   = await equipmentTask;
        pkg.Ploys       = await ployTask;
        pkg.Units       = await unitTask;
        pkg.Teams       = await teamTask;
        pkg.Objectives  = await objTask;

        pkg.Aliases     = index.Aliases;

        // Resolve ID references to objects for the TEAMs 
        ResolveTeamIds(pkg);

        return pkg;
    }

    /// <summary>
    /// Read a package from embedded resources where the supplied path points to the package's index resource.
    /// </summary>
    /// <param name="path">Path to the package index resource, for example "Homebrew".</param>
    /// <returns>package</returns>
    public static Task<Pkg> FromEmbeddedResources(string path)
        => FromEmbeddedResources(typeof(Pkg).Assembly, path);

    public static async Task<Pkg> FromEmbeddedResources(Assembly assembly, string path)
    {
        var pkg = new Pkg();
        pkg.Author = Environment.UserName;
        pkg.Name = Path.GetFileNameWithoutExtension(path);

        PkgIndex? index = null;
        try
        {
            string? indexJson = null;
            if (!TryGetEmbeddedResource(assembly, path + "/index.json", out indexJson))
            {
                TryGetEmbeddedResource(assembly, path + "/index.jsonc", out indexJson);
            }
            if (indexJson is null)
                throw new FileNotFoundException($"Could not find package index at {path}/index.json or {path}/index.jsonc");
                
            index = JsonSerializer.Deserialize<PkgIndex>(indexJson, json);
            if (index is null)
                return pkg;
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"Could not read package index at {path}/index.json or {path}/index.jsonc", ex);
        }

        pkg.Name = index.Name;
        pkg.Author = index.Author;
        pkg.Updated = index.Updated;

        var definitionTask = ParseResourcesFromEmbeddedResources<Rule>(pkg, assembly, path, index.Definitions ?? EMPTY);
        var actionTask = ParseResourcesFromEmbeddedResources<Action>(pkg, assembly, path, index.Actions ?? EMPTY);
        var effectTask = ParseResourcesFromEmbeddedResources<Effect>(pkg, assembly, path, index.Effects ?? EMPTY);
        var equipmentTask = ParseResourcesFromEmbeddedResources<Equipment>(pkg, assembly, path, index.Equipment ?? EMPTY);
        var ployTask = ParseResourcesFromEmbeddedResources<Ploy>(pkg, assembly, path, index.Ploys ?? EMPTY);
        var unitTask = ParseResourcesFromEmbeddedResources<Unit>(pkg, assembly, path, index.Units ?? EMPTY);
        var teamTask = ParseResourcesFromEmbeddedResources<Team>(pkg, assembly, path, index.Teams ?? EMPTY);
        var objTask = ParseResourcesFromEmbeddedResources<Objective>(pkg, assembly, path, index.Objectives ?? EMPTY);

        await Task.WhenAll(definitionTask, actionTask, effectTask, equipmentTask, ployTask, unitTask, teamTask, objTask);

        pkg.Definitions = await definitionTask;
        pkg.Actions = await actionTask;
        pkg.Effects = await effectTask;
        pkg.Equipment = await equipmentTask;
        pkg.Ploys = await ployTask;
        pkg.Units = await unitTask;
        pkg.Teams = await teamTask;
        pkg.Objectives = await objTask;

        pkg.Aliases = index.Aliases;

        ResolveTeamIds(pkg);

        return pkg;
    }

    private static bool TryGetEmbeddedResource(Assembly assembly, string resource, [NotNullWhen(true)] out string? contents)
    {
        contents = null;
        var normalizedResource = resource;
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault((name) => name.EndsWith(normalizedResource, StringComparison.InvariantCultureIgnoreCase));
        if (resourceName is null) {
            Console.WriteLine("Failed to load " + resource + "(" + normalizedResource + ")");  
            var debug_ext = Path.GetFileName(resource);     
            foreach (var item in assembly.GetManifestResourceNames())
            {
                if (!item.EndsWith(debug_ext))
                    continue;
                Console.WriteLine(item + " == " + item.EndsWith(normalizedResource));
            }
            return false;
        }

        using var contentStream = assembly.GetManifestResourceStream(resourceName);
        if (contentStream is null)
            return false;

        using var reader = new StreamReader(contentStream);
        contents = reader.ReadToEnd();
        return true;
    } 

    private static async Task<T?> LoadSingleResourceFromRelativeUrl<T>(Pkg currentPkg, HttpClient client, string url, string relative)
    where T:IPackagedContent
    {
        try {
            // Clean the relative path
            var relativeUri = relative;
            if (!relativeUri.StartsWith("/"))
                relativeUri = "/" + relativeUri;
            if (!relativeUri.EndsWith(".html"))
                relativeUri = Path.ChangeExtension(relativeUri, ".html");

            // Form final URL
            var uri = url + relativeUri;

            // Get resource
            var resp = await client.GetAsync(uri);
            var content = await resp.Content.ReadAsStringAsync();
            var doc = new FmHtmlDocument(content);
            var data = JsonSerializer.Deserialize<T>(doc.FrontMatter, json);
            if (data is null)
                return default(T);

            // Assign the id and description 
            data.SourcePackage = currentPkg;
            data.Id = Path.GetFileNameWithoutExtension(relative);
            data.Description = doc.Html.ToString();
            return data;
        } catch (Exception ex)
        {
            throw new FormatException($"Could not load package resource at {url}/{relative}", ex);
        }
    }

    private static async Task<Dictionary<string, T>> ParseResourcesFromRelativeUrl<T>(Pkg currentPkg, HttpClient client, string url, List<string> relatives)
    where T:IPackagedContent
    {
        var tasks = new Task<T?>[relatives.Count];
        for (var i = 0; i < relatives.Count; i++) {
            tasks[i] = LoadSingleResourceFromRelativeUrl<T>(currentPkg, client, url, relatives[i]);
        }
        var results = await Task.WhenAll(tasks);

        return results
            .Where(x => x is not null)
            .ToDictionary(x => x?.Id!, x => x!);
    }

    private static async Task<Dictionary<string, T>> ParseResourcesFromEmbeddedResources<T>(Pkg currentPkg, Assembly assembly, string url, List<string> relatives)
    where T : IPackagedContent
    {
        var results = new Dictionary<string, T>();

        foreach (var relative in relatives)
        {
            try
            {
                // Clean the relative path
                var relativeUri = relative;
                if (!relativeUri.StartsWith("/"))
                    relativeUri = "/" + relativeUri;
                if (!relativeUri.EndsWith(".html"))
                    relativeUri = Path.ChangeExtension(relativeUri, ".html");

                // Form final URL
                var uri = url + relativeUri;

                if (!TryGetEmbeddedResource(assembly, relativeUri, out var content))
                {
                    continue;
                }

                var doc = new FmHtmlDocument(content);
                var data = JsonSerializer.Deserialize<T>(doc.FrontMatter, json);
                if (data is null)
                    continue;

                data.SourcePackage = currentPkg;
                data.Id = Path.GetFileNameWithoutExtension(relative);
                data.Description = doc.Html.ToString();
                results[data.Id] = data;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Could not load package resource from embedded resources: {relative}", ex);
            }
        }

        return results;
    }

    /// <summary>
    /// Read a package from a local file where the path points to the package's index.json file
    /// </summary>
    /// <param name="stream">Stream containing a .zip file with package data</param>
    /// <returns>package</returns>
    public static Pkg FromFile(Stream stream)
    {
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        
        var pkg = new Pkg();
        pkg.Author = Environment.UserName;
        
        // Read the package index
        PkgIndex? index = null;
        try
        {
            // Try to find index.json or index.jsonc at the root
            var indexEntry = archive.GetEntry("index.json") ?? archive.GetEntry("index.jsonc");
            if (indexEntry is null)
                throw new FileNotFoundException("Could not find index.json or index.jsonc at the root of the archive");
            
            using var indexStream = indexEntry.Open();
            index = JsonSerializer.Deserialize<PkgIndex>(indexStream, json);
            if (index is null)
                return pkg;
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException("Could not find package index at root of archive (index.json or index.jsonc)", ex);
        }
        
        pkg.Name = index.Name;
        pkg.Author = index.Author;
        pkg.Updated = index.Updated;
        
        // Convert the paths in the package index into actual resources by parsing them one at a time
        pkg.Definitions = ParseResourcesFromZip<Rule>(pkg, archive, index.Definitions ?? EMPTY);
        pkg.Actions = ParseResourcesFromZip<Action>(pkg, archive, index.Actions ?? EMPTY);
        pkg.Effects = ParseResourcesFromZip<Effect>(pkg, archive, index.Effects ?? EMPTY);
        pkg.Equipment = ParseResourcesFromZip<Equipment>(pkg, archive, index.Equipment ?? EMPTY);
        pkg.Ploys = ParseResourcesFromZip<Ploy>(pkg, archive, index.Ploys ?? EMPTY);
        pkg.Units = ParseResourcesFromZip<Unit>(pkg, archive, index.Units ?? EMPTY);
        pkg.Teams = ParseResourcesFromZip<Team>(pkg, archive, index.Teams ?? EMPTY);
        pkg.Objectives = ParseResourcesFromZip<Objective>(pkg, archive, index.Objectives ?? EMPTY);
        
        pkg.Aliases = index.Aliases;
        
        // Resolve ID references to objects for the TEAMs 
        ResolveTeamIds(pkg);
        
        return pkg;
    }

    private static Dictionary<string, T> ParseResourcesFromZip<T>(Pkg currentPkg, ZipArchive archive, List<string> relatives)
    where T : IPackagedContent
    {
        var results = new Dictionary<string, T>();
        
        foreach (var relative in relatives)
        {
            try
            {
                // Clean the relative path
                var relativeUri = relative;
                if (relativeUri.StartsWith("/"))
                    relativeUri = relativeUri.Substring(1);
                if (!relativeUri.EndsWith(".html"))
                    relativeUri = Path.ChangeExtension(relativeUri, ".html");
                
                // Get the entry from the archive
                var entry = archive.GetEntry(relativeUri);
                if (entry is null)
                    continue;
                
                using var contentStream = entry.Open();
                using var reader = new StreamReader(contentStream);
                var content = reader.ReadToEnd();
                
                var doc = new FmHtmlDocument(content);
                var data = JsonSerializer.Deserialize<T>(doc.FrontMatter, json);
                if (data is null)
                    continue;
                
                // Assign the id and description
                data.SourcePackage = currentPkg;
                data.Id = Path.GetFileNameWithoutExtension(relative);
                data.Description = doc.Html.ToString();
                results[data.Id] = data;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Could not load package resource from archive: {relative}", ex);
            }
        }
        
        return results;
    }

    private static void ResolveTeamIds(Pkg pkg)
    {
        #nullable disable

        var allRulesDict = pkg.AllRules().Where(r => r.Id is not null).DistinctBy(r => r.Id).ToDictionary(r => r.Id ?? string.Empty, r => r);

        // Resolve ID references to objects for the TEAMs
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
            
            team.Value.Equipment = (team.Value.EquipmentIds ?? Enumerable.Empty<string>()).Select(id =>
            {
                if (pkg.Equipment.TryGetValue(id, out var eq))
                    return eq;
                return null;
            })
            .Where(eq => eq is not null)
            .Cast<Equipment>()
            .ToList();
            
            team.Value.Ploys = (team.Value.PloyIds ?? Enumerable.Empty<string>()).Select(id =>
            {
                if (pkg.Ploys.TryGetValue(id, out var ploy))
                    return ploy;
                return null;
            })
            .Where(ploy => ploy is not null)
            .Cast<Ploy>()
            .ToList();

            team.Value.FactionRules = (team.Value.FactionRuleIds ?? Enumerable.Empty<string>()).Select(id =>
            {
                if (allRulesDict.TryGetValue(id, out var rule))
                    return rule;
                return null;
            })
            .Where(rule => rule is not null)
            .Cast<Rule>()
            .ToList();
        }

        #nullable restore
    }
}