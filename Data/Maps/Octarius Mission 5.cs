using static Kt.Layout.Map;

namespace Kt.Maps;

public partial class UserMaps {
public static readonly MapData OctariusMission5 = new MapData {
  Name = "Octarius Mission 5",
  Orientation = MapOrientation.TopToBottom,
  Objectives = new List<ObjectiveMarker> {
    new ObjectiveMarker(18f, 8f, "Center Objective"),
    new ObjectiveMarker(5f, 11f, "Allied Objective"),
    new ObjectiveMarker(26f, 13f, "Enemy Objective"),
  },
  Terrain = new List<Terrain> {
    new Terrain(3f, 0f, 90f, 1f, "art/terrain/building.01.svg"),
    new Terrain(14f, 1f, 90f, 1f, "art/terrain/barricade.02.svg"),
    new Terrain(24f, 0f, 90f, 1f, "art/terrain/barricade.01.svg"),
    new Terrain(2f, 9f, 90f, 1f, "art/terrain/barricade.02.svg"),
    new Terrain(24f, 10f, 0f, 1f, "art/terrain/building.05.svg"),
    new Terrain(13f, 12f, 314f, 1f, "art/terrain/rubble.01.svg"),
    new Terrain(3f, 15f, 272f, 1f, "art/terrain/building.01.svg"),
    new Terrain(17.5f, 9f, 0f, 1f, "art/terrain/building.02.svg"),
    new Terrain(13f, 7f, 270f, 1f, "art/terrain/building.02.svg"),
    new Terrain(23f, 16f, 90f, 1f, "art/terrain/rubble.03.svg"),
    new Terrain(13f, 17f, 0f, 1f, "art/terrain/rubble.02.svg"),
  }
};
}
