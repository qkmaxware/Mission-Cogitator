using static Kt.Layout.Map;

namespace Kt.Maps;

public partial class UserMaps {
public static readonly MapData OctariusMission2 = new MapData {
  Name = "Octarius Mission 2",
  Orientation = MapOrientation.LeftToRight,
  Objectives = new List<ObjectiveMarker> {
    new ObjectiveMarker(15f, 6f, "Center Objective"),
    new ObjectiveMarker(10f, 17f, "Allied Objective"),
    new ObjectiveMarker(20f, 17f, "Enemy Objective"),
  },
  Terrain = new List<Terrain> {
    new Terrain(4f, 1f, 29f, 1f, "art/terrain/building.02.svg"),
    new Terrain(13f, 1f, 180f, 1f, "art/terrain/rubble.01.svg"),
    new Terrain(8f, 12f, 305f, 1f, "art/terrain/building.01.svg"),
    new Terrain(12f, 10f, 216f, 1f, "art/terrain/rubble.03.svg"),
    new Terrain(14.800254f, 5f, 183f, 1f, "art/terrain/building.05.svg"),
    new Terrain(18f, 12f, 213f, 1f, "art/terrain/building.01.svg"),
    new Terrain(21f, 2f, 151f, 1f, "art/terrain/building.02.svg"),
    new Terrain(3f, 14f, 0f, 1f, "art/terrain/barricade.01.svg"),
    new Terrain(15f, 17f, 0f, 1f, "art/terrain/barricade.02.svg"),
    new Terrain(26f, 17f, 0f, 1f, "art/terrain/barricade.02.svg"),
    new Terrain(23.5f, 2f, 0f, 1f, "art/terrain/rubble.02.svg"),
  }
};
}
