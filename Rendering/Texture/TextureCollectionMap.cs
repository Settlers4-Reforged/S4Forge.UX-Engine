using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.Rendering.Texture {
    public enum TextureCollectionMap : int {
        #region UI
        MainUI = 0,
        AddonUI = 40,
        GameUI = 7,
        Icons = 1,
        RomanUI = 9,
        VikingUI = 19,
        MayanUI = 29,
        TrojanUI = 39,

        ForgeUI = 100,
        #endregion

        #region Effects
        Effects = 4,
        Effects2 = 6,
        MagicEffects = 36,
        MagicEffects2 = 37,
        #endregion

        Terrain = 2,
        Piles = 3,
        Nature = 5,
        Animals = 8,

        RomanBuildings = 10,
        VikingBuildings = 11,
        MayanBuildings = 12,
        DarkTribeBuildings = 13,
        TrojanBuildings = 14,
        RomanSettlers = 20,
        VikingSettlers = 21,
        MayanSettlers = 22,
        DarkTribeSettlers = 23,
        TrojanSettlers = 24,
        RomanVehicles = 30,
        VikingVehicles = 31,
        MayanVehicles = 32,
        TrojanVehicles = 34,
    }
}
