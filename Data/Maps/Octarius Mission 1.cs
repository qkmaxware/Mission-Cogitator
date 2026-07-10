using static Kt.Layout.Map;

namespace Kt.Maps;

public partial class UserMaps {
public static readonly MapData OctariusMission1 = new MapData {
  Name = "Octarius Mission 1",
  Orientation = MapOrientation.LeftToRight,
  Objectives = new List<ObjectiveMarker> {
    new ObjectiveMarker(15f, 11f, "Center Objective"),
    new ObjectiveMarker(8f, 13f, "Allied Objective"),
    new ObjectiveMarker(22f, 9f, "Enemy Objective"),
  },
  Terrain = new List<Terrain> {
    new Terrain(4f, 2f, 0f, 1f, "art/terrain/building.01.svg"),
    new Terrain(12f, 2f, 0f, 1f, "art/terrain/rubble.01.svg"),
    new Terrain(18f, 2f, 0f, 1f, "art/terrain/building.03.svg"),
    new Terrain(23.436905f, 3.3481293f, 0f, 1f, "art/terrain/rubble.03.svg"),
    new Terrain(22f, 12f, 0f, 1f, "art/terrain/building.04.svg"),
    new Terrain(12f, 7f, 0f, 1f, "art/terrain/building.05.svg"),
    new Terrain(15f, 16f, 0f, 1f, "art/terrain/rubble.02.svg"),
    new Terrain(8f, 12f, 0f, 1f, "art/terrain/building.02.svg"),
    new Terrain(4f, 13f, 0f, 1f, "art/terrain/barricade.01.svg"),
    new Terrain(18f, 11f, 0f, 1f, "art/terrain/barricade.02.svg"),
  }
};
}
