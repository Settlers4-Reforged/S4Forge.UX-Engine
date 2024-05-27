using Forge.Logging;
using Forge.Native;
using Forge.S4;
using Forge.S4.Types;
using Forge.UX.Interfaces;
using Forge.UX.S4.Types.Native;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.S4 {
    public interface IUIManager {
        unsafe int* UIScenes { get; }
        GUIEventHandler? GUIEventHandler { get; }
        S4UIScreenId GetActiveScreen();
        S4UIMenuId GetActiveMenu();
        S4UIMenuId GetActiveSubmenu();
    }

    public class UIManager : IUIManager {
        /// <summary>
        /// A list of all known menu state class vtables and their corresponding screen IDs.
        /// </summary>
        private readonly Dictionary<Int32, S4UIScreenId> menuVTables = new Dictionary<Int32, S4UIScreenId>() {
            { 0xC57200, S4UIScreenId.MainMenu },
            { 0xC575BC, S4UIScreenId.Tutorial },
            { 0xC56118, S4UIScreenId.AOBriefing },
            { 0xC561B8, S4UIScreenId.AOSplash },
            { 0xC56168, S4UIScreenId.AOCampaigns },
            { 0xC5617C, S4UIScreenId.AOCampaignsSettle },
            { 0xC5612C, S4UIScreenId.AOCampaignBonus },
            { 0xC56140, S4UIScreenId.AOCampaignMayan },
            { 0xC56154, S4UIScreenId.AOCampaignRoman },
            { 0xC56190, S4UIScreenId.AOCampaignTrojan },
            { 0xC561A4, S4UIScreenId.AOCampaignViking },
            { 0xC561CC, S4UIScreenId.Briefing },
            { 0xC56434, S4UIScreenId.Campaign3X3 },
            { 0xC56448, S4UIScreenId.CampaignDark },
            { 0xC5645C, S4UIScreenId.Credits },
            { 0xC5762C, S4UIScreenId.VictoryScreen },
            { 0xC56470, S4UIScreenId.EcoStatistic },
            { 0xC564A4, S4UIScreenId.EndStatistic },
            { 0xC56A64, S4UIScreenId.LoadGame },
            { 0xC56A78, S4UIScreenId.LoadType },
            { 0xC56B2C, S4UIScreenId.LobbyConnect },
            { 0xC56B40, S4UIScreenId.LobbyGameSettings },
            { 0xC56DB4, S4UIScreenId.LobbyLoadMP },
            { 0xC56DD0, S4UIScreenId.LobbyMapSettings },
            { 0xC56E54, S4UIScreenId.LobbyMultiplayerType },
            { 0xC56E68, S4UIScreenId.LocalType },
            { 0xC57340, S4UIScreenId.MD2Briefing },
            { 0xC57354, S4UIScreenId.MD2Campaigns },
            { 0xC574FC, S4UIScreenId.MDBriefing },
            { 0xC57510, S4UIScreenId.MDCampaignMayan },
            { 0xC57524, S4UIScreenId.MDCampaignRoman },
            { 0xC57538, S4UIScreenId.MDCampaigns },
            { 0xC5754C, S4UIScreenId.MDCampaignsEcoConflict },
            { 0xC57560, S4UIScreenId.MDCampaignViking },
            { 0xC57574, S4UIScreenId.MDRandomMapParameters },
            { 0xC57588, S4UIScreenId.MessageBox },
            { 0xC575A8, S4UIScreenId.Slideshow },
            { 0xC57AF0, S4UIScreenId.Video },
            { 0xC57C58, S4UIScreenId.XMD3Briefing },
            { 0xC57C6C, S4UIScreenId.XMD3Campaigns },
            { 0xC5678C, S4UIScreenId.Ingame },
        };

        public S4UIScreenId GetActiveScreen() {
            unsafe {
                Int32* GameStateManager = (Int32*)GameValues.ReadValue<Int32>(0xE94804);
                if (GameStateManager == null) {
                    return S4UIScreenId.Unknown;
                }

                // The GameStateManager is defined by implementations of the various State classes (e.g. CStateMainMenu, CStateTutorial, etc.)
                // Identification is done by the vtable pointer of the class instance.
                Int32 currentScreen = *GameStateManager - GameValues.S4_Main;
                if (!menuVTables.ContainsKey(currentScreen)) {
                    Logger.LogWarn($"New screen detected: {currentScreen:X}!");
                }

                return menuVTables.GetValueOrDefault(currentScreen, S4UIScreenId.Unknown);
            }
        }

        private readonly Dictionary<Int32, S4UIMenuId> menuTranslation = new Dictionary<Int32, S4UIMenuId>() {
            { 0x22, S4UIMenuId.ExtrasSave },
            { 0x24, S4UIMenuId.ExtrasMission },
            { 0x3B, S4UIMenuId.ExtrasChatSettings },
            { 0x23, S4UIMenuId.ExtrasQuit },
            { 0x25, S4UIMenuId.SettingsGraphics },
            { 0x26, S4UIMenuId.SettingsAudio },
            { 0x27, S4UIMenuId.SettingsGame },
            { 0x28, S4UIMenuId.SettingsNotifications },
            { 0xA, S4UIMenuId.BuildingsFoundation },
            { 0xB, S4UIMenuId.BuildingsMetal },
            { 0xC, S4UIMenuId.BuildingsFood },
            { 0xD, S4UIMenuId.BuildingsMisc },
            { 0x35, S4UIMenuId.BuildingsMiscDecoSubmenu },
            { 0xE, S4UIMenuId.BuildingsMilitary },
            { 0x20, S4UIMenuId.SettlersSummary },
            { 0x21, S4UIMenuId.SettlersWorkers },
            { 0x1F, S4UIMenuId.SettlersSpecialists },
            { 0x1E, S4UIMenuId.SettlersSearch },
            { 0x1C, S4UIMenuId.GoodsSummary },
            { 0x1B, S4UIMenuId.GoodsDistribution },
            { 0x1A, S4UIMenuId.GoodsPriority },
            { 0x1D, S4UIMenuId.StatisticsWarriors },
            { 0x4C, S4UIMenuId.StatisticsLand },
            { 0x4D, S4UIMenuId.StatisticsGoods },
            //{ 0x2A, S4UIMenuId.UnitSelectionDonkey },
            { 0x2A, S4UIMenuId.UnitSelectionTradingVehicle },
            { 0x2E, S4UIMenuId.UnitSelectionMilitary },
            { 0x2D, S4UIMenuId.UnitSelectionSpecialists },
            { 0x29, S4UIMenuId.UnitSelectionVehicles },
            { 0x2B, S4UIMenuId.UnitSelectionFerry },
            { 0x39, S4UIMenuId.UnitSelectionSubSpells },
            { 0x38, S4UIMenuId.UnitSelectionSubGroupings },
            { 0x10, S4UIMenuId.SelectionSimpleBuilding },
            { 0x11, S4UIMenuId.SelectionTowerBuilding },
            { 0x12, S4UIMenuId.SelectionTraderBuilding },
            { 0x13, S4UIMenuId.SelectionResourceBuilding },
            { 0x14, S4UIMenuId.SelectionTransformingBuilding },
            { 0x17, S4UIMenuId.SelectionBarracksBuilding },
            { 0x18, S4UIMenuId.SelectionStorageBuilding },
            { 0x19, S4UIMenuId.SelectionVehicleBuilding },
        };
        private readonly Dictionary<Int32, S4UIMenuId> submenuTranslation = new Dictionary<Int32, S4UIMenuId>() {
            { 0x36, S4UIMenuId.SelectionSubTrade },
            { 0x33,  S4UIMenuId.SelectionSubStorage },
            { 0x34, S4UIMenuId.SelectionSubBuildVehicle },
            { 0x31, S4UIMenuId.SelectionSubBarracks },
        };


        public S4UIMenuId GetActiveMenu() {
            unsafe {
                if (UIScenes == null) {
                    return S4UIMenuId.Unknown;
                }

                int menuSceneSurfaceId = UIScenes[2];
                if (menuSceneSurfaceId != 0 && !menuTranslation.ContainsKey(menuSceneSurfaceId)) {
                    Logger.LogWarn($"New menu detected: {menuSceneSurfaceId:X}!");
                    return (S4UIMenuId)menuSceneSurfaceId;
                }

                return menuTranslation.GetValueOrDefault(menuSceneSurfaceId, S4UIMenuId.Unknown);
            }
        }

        public S4UIMenuId GetActiveSubmenu() {
            unsafe {
                if (UIScenes == null) {
                    return S4UIMenuId.Unknown;
                }

                int submenuSceneSurfaceId = UIScenes[3];
                if (submenuSceneSurfaceId != 0 && !submenuTranslation.ContainsKey(submenuSceneSurfaceId)) {
                    Logger.LogWarn($"New menu detected: {submenuSceneSurfaceId:X}!");
                    return (S4UIMenuId)submenuSceneSurfaceId;
                }

                return submenuTranslation.GetValueOrDefault(submenuSceneSurfaceId, S4UIMenuId.Unknown);
            }
        }

        // [0] is the menus (including the main menu) 
        // [1] is chatbox 
        // [2] this is the menu on the left side (shows selections, buildings etc)
        // [3] is the group selection/magic/barracks/deco/smiths etc subsubmenu
        // [4] the menu left under the usual menu. This usually shows a summary of your goods. Is NULL if not present
        // [5] the camera. Is NULL if not present
        // [6] is the submenu (building submenu, settlers, goods, graph). Is NULL if settlers or a building is selected
        // [7] unknown
        // [8] is tutorial related. Probably an arrow handler? That one that points you to the UI elements that you have to click.
        // [9..10] unknown
        // [11] is probably the "you have won/lost" window proc
        // [12] is the timer in the top right corner (use f7 for example)
        // [13] maybe exists ? 
        public unsafe Int32* UIScenes => GameValues.GetPointer<Int32>(0x1064C98);

        public GUIEventHandler? GUIEventHandler {
            get {
                unsafe {
                    var handler = (GUIEventHandler*)new IntPtr(GameValues.ReadValue<Int32>(0x10540CC)).ToPointer();

                    if (handler == null) {
                        return null;
                    }

                    return *handler;
                }
            }
        }
    }
}
