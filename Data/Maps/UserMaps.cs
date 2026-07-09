using Kt.Layout;

namespace Kt.Maps;

public partial class UserMaps {

    public static IEnumerable<Map.MapData?> All => typeof(UserMaps)
    .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
    .Where(f => f.FieldType.IsAssignableTo(typeof(Map.MapData)))
    .Select(f => (Map.MapData?)f.GetValue(null));

}