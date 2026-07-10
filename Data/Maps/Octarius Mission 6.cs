using static Kt.Layout.Map;

namespace Kt.Maps;

public partial class UserMaps {
public static readonly MapData OctariusMission6 = new MapData {
  Name = "Octarius Mission 6",
  Orientation = MapOrientation.TopToBottom,
  Objectives = new List<ObjectiveMarker> {
    new ObjectiveMarker(14f, 7f, "Center Objective"),
    new ObjectiveMarker(8f, 15f, "Allied Objective"),
    new ObjectiveMarker(23f, 11f, "Enemy Objective"),
  },
  Terrain = new List<Terrain> {
    new Terrain(6f, 1f, 270f, 1f, "art/terrain/building.01.svg"),
    new Terrain(4f, 6f, 90f, 1f, "art/terrain/barricade.02.svg"),
    new Terrain(9f, 8.5f, 0f, 1f, "art/terrain/rubble.02.svg"),
    new Terrain(15.5f, 1.5f, 275f, 1f, "art/terrain/rubble.03.svg"),
    new Terrain(22.5f, 1.5f, 0f, 1f, "art/terrain/building.05.svg"),
    new Terrain(15.5f, 7f, 297f, 1f, "art/terrain/building.02.svg"),
    new Terrain(26f, 9f, 90f, 1f, "art/terrain/barricade.02.svg"),
    new Terrain(6f, 11f, 312f, 1f, "art/terrain/building.02.svg"),
    new Terrain(22f, 14f, 90f, 1f, "art/terrain/building.01.svg"),
    new Terrain(13.5f, 16f, 80f, 1f, "art/terrain/rubble.01.svg"),
    new Terrain(5f, 15.5f, 270f, 1f, "art/terrain/barricade.01.svg"),
  }
};
}
