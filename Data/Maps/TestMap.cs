using static Kt.Layout.Map;

namespace Kt.Maps;

public partial class UserMaps {
// https://www.reddit.com/r/killteam/comments/1gfwwum/mapsterrains_and_symbols/
public static readonly MapData VolkusOne = new MapData {
  Name = "Example Map",
  Orientation = MapOrientation.LeftToRight,
  Objectives = new List<ObjectiveMarker> {
    new ObjectiveMarker(15, 11, "Center Objective"),
    new ObjectiveMarker(12, 18, "Allied Objective"),
    new ObjectiveMarker(18, 4, "Enemy Objective"),
  },
  Terrain = new List<Terrain> {
    new Terrain(12.079899f, 7.1528215f, 0, 1, "art/terrain/building.08.svg"),
    new Terrain(22, 12, 0, 1, "art/terrain/building.09.svg"),
    new Terrain(2, 13, 0, 1, "art/terrain/building.07.svg"),
    new Terrain(2, 2, 0, 1, "art/terrain/building.06.svg"),
    new Terrain(9.2073555f, 0.30437538f, 0, 1, "art/terrain/barricade.02.svg"),
    new Terrain(15.142676f, 17.19721f, 0, 1, "art/terrain/rubble.01.svg"),
    new Terrain(16, 1, 0, 0.5f, "art/terrain/barricade.02.svg"),
    new Terrain(25, 5, 90, 0.5f, "art/terrain/barricade.02.svg"),
  }
};
}
