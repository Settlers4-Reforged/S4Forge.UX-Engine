using Forge.Config;
using Forge.Game.UI;
using Forge.Logging;
using Forge.Native;
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
        S4UIScreen GetActiveScreen();
        S4UIMenu GetActiveMenu();
        S4UISubmenu GetActiveSubmenu();
    }

    public class UIManager : IUIManager {
        private readonly CLogger Logger;
        private readonly IGameValues gameValues;

        public UIManager(CLogger logger, IGameValues gameValues) {
            Logger = logger.WithEnumCategory(ForgeLogCategory.UI);
            this.gameValues = gameValues;
        }

        /// <summary>
        /// A list of all known menu state class vtables and their corresponding screen IDs.
        /// </summary>
        private readonly Dictionary<Int32, S4UIScreen> menuVTables = new Dictionary<Int32, S4UIScreen>() {
            { 0xC57200, S4UIScreen.MainMenu },
            { 0xC575BC, S4UIScreen.Tutorial },
            { 0xC56118, S4UIScreen.AOBriefing },
            { 0xC561B8, S4UIScreen.AOSplash },
            { 0xC56168, S4UIScreen.AOCampaigns },
            { 0xC5617C, S4UIScreen.AOCampaignsSettle },
            { 0xC5612C, S4UIScreen.AOCampaignBonus },
            { 0xC56140, S4UIScreen.AOCampaignMayan },
            { 0xC56154, S4UIScreen.AOCampaignRoman },
            { 0xC56190, S4UIScreen.AOCampaignTrojan },
            { 0xC561A4, S4UIScreen.AOCampaignViking },
            { 0xC561CC, S4UIScreen.Briefing },
            { 0xC56434, S4UIScreen.Campaign3X3 },
            { 0xC56448, S4UIScreen.CampaignDark },
            { 0xC5645C, S4UIScreen.Credits },
            { 0xC5762C, S4UIScreen.VictoryScreen },
            { 0xC56470, S4UIScreen.EcoStatistic },
            { 0xC564A4, S4UIScreen.EndStatistic },
            { 0xC56A64, S4UIScreen.LoadGame },
            { 0xC56A78, S4UIScreen.LoadType },
            { 0xC56B2C, S4UIScreen.LobbyConnect },
            { 0xC56B40, S4UIScreen.LobbyGameSettings },
            { 0xC56DB4, S4UIScreen.LobbyLoadMP },
            { 0xC56DD0, S4UIScreen.LobbyMapSettings },
            { 0xC56E54, S4UIScreen.LobbyMultiplayerType },
            { 0xC56E68, S4UIScreen.LocalType },
            { 0xC57340, S4UIScreen.MD2Briefing },
            { 0xC57354, S4UIScreen.MD2Campaigns },
            { 0xC574FC, S4UIScreen.MDBriefing },
            { 0xC57510, S4UIScreen.MDCampaignMayan },
            { 0xC57524, S4UIScreen.MDCampaignRoman },
            { 0xC57538, S4UIScreen.MDCampaigns },
            { 0xC5754C, S4UIScreen.MDCampaignsEcoConflict },
            { 0xC57560, S4UIScreen.MDCampaignViking },
            { 0xC57574, S4UIScreen.MDRandomMapParameters },
            { 0xC57588, S4UIScreen.MessageBox },
            { 0xC575A8, S4UIScreen.Slideshow },
            { 0xC57AF0, S4UIScreen.Video },
            { 0xC57C58, S4UIScreen.XMD3Briefing },
            { 0xC57C6C, S4UIScreen.XMD3Campaigns },
            { 0xC5678C, S4UIScreen.Ingame },
        };

        public S4UIScreen GetActiveScreen() {
            unsafe {
                Int32* GameStateManager = (Int32*)gameValues.ReadValue<Int32>(0xE94804);
                if (GameStateManager == null) {
                    return S4UIScreen.Unknown;
                }

                // The GameStateManager is defined by implementations of the various State classes (e.g. CStateMainMenu, CStateTutorial, etc.)
                // Identification is done by the vtable pointer of the class instance.
                Int32 currentScreen = *GameStateManager - gameValues.S4_Main;
                if (!menuVTables.ContainsKey(currentScreen)) {
                    Logger.LogF(LogLevel.Warning, "New screen detected: {0:X}!", currentScreen);
                }

                return menuVTables.GetValueOrDefault(currentScreen, S4UIScreen.Unknown);
            }
        }

        private readonly Dictionary<Int32, S4UIMenu> menuTranslation = new Dictionary<Int32, S4UIMenu>() {
            { 0x22, S4UIMenu.ExtrasSave },
            { 0x2C, S4UIMenu.ConfirmDestroy },
            { 0x24, S4UIMenu.ExtrasMission },
            { 0x3B, S4UIMenu.ExtrasChatSettings },
            { 0x23, S4UIMenu.ExtrasQuit },
            { 0x25, S4UIMenu.SettingsGraphics },
            { 0x26, S4UIMenu.SettingsAudio },
            { 0x27, S4UIMenu.SettingsGame },
            { 0x28, S4UIMenu.SettingsNotifications },
            { 0xA, S4UIMenu.BuildingsFoundation },
            { 0xB, S4UIMenu.BuildingsMetal },
            { 0xC, S4UIMenu.BuildingsFood },
            { 0xD, S4UIMenu.BuildingsMisc },
            { 0xE, S4UIMenu.BuildingsMilitary },
            { 0x20, S4UIMenu.SettlersSummary },
            { 0x21, S4UIMenu.SettlersWorkers },
            { 0x1F, S4UIMenu.SettlersSpecialists },
            { 0x1E, S4UIMenu.SettlersSearch },
            { 0x1C, S4UIMenu.GoodsSummary },
            { 0x1B, S4UIMenu.GoodsDistribution },
            { 0x1A, S4UIMenu.GoodsPriority },
            { 0x1D, S4UIMenu.StatisticsWarriors },
            { 0x4C, S4UIMenu.StatisticsLand },
            { 0x4D, S4UIMenu.StatisticsGoods },
            //{ 0x2A, S4UIMenuId.UnitSelectionDonkey },
            { 0x2A, S4UIMenu.UnitSelectionTradingVehicle },
            { 0x2E, S4UIMenu.UnitSelectionMilitary },
            { 0x2D, S4UIMenu.UnitSelectionSpecialists },
            { 0x29, S4UIMenu.UnitSelectionVehicles },
            { 0x2B, S4UIMenu.UnitSelectionFerry },
            { 0x10, S4UIMenu.SelectionSimpleBuilding },
            { 0x11, S4UIMenu.SelectionTowerBuilding },
            { 0x12, S4UIMenu.SelectionTraderBuilding },
            { 0x13, S4UIMenu.SelectionResourceBuilding },
            { 0x14, S4UIMenu.SelectionTransformingBuilding },
            { 0x17, S4UIMenu.SelectionBarracksBuilding },
            { 0x18, S4UIMenu.SelectionStorageBuilding },
            { 0x19, S4UIMenu.SelectionVehicleBuilding },
        };
        private readonly Dictionary<Int32, S4UISubmenu> submenuTranslation = new Dictionary<Int32, S4UISubmenu>() {
            { 0x36, S4UISubmenu.SelectionSubTrade },
            { 0x35, S4UISubmenu.BuildingsMiscDecoSubmenu },
            { 0x33, S4UISubmenu.SelectionSubStorage },
            { 0x34, S4UISubmenu.SelectionSubBuildVehicle },
            { 0x31, S4UISubmenu.SelectionSubBarracks },
            { 0x39, S4UISubmenu.UnitSelectionSubSpells },
            { 0x38, S4UISubmenu.UnitSelectionSubGroupings },
        };


        public S4UIMenu GetActiveMenu() {
            unsafe {
                if (UIScenes == null) {
                    return S4UIMenu.Unknown;
                }

                int menuSceneSurfaceId = UIScenes[2];
                if (menuSceneSurfaceId != 0 && !menuTranslation.ContainsKey(menuSceneSurfaceId)) {
                    Logger.LogF(LogLevel.Warning, "New menu detected: {0:X}!", menuSceneSurfaceId);
                    return (S4UIMenu)menuSceneSurfaceId;
                }

                return menuTranslation.GetValueOrDefault(menuSceneSurfaceId, S4UIMenu.Unknown);
            }
        }

        public S4UISubmenu GetActiveSubmenu() {
            unsafe {
                if (UIScenes == null) {
                    return S4UISubmenu.Unknown;
                }

                int submenuSceneSurfaceId = UIScenes[3];
                if (submenuSceneSurfaceId != 0 && !submenuTranslation.ContainsKey(submenuSceneSurfaceId)) {
                    Logger.LogF(LogLevel.Warning, "New sub-menu detected: {0:X}!", submenuSceneSurfaceId);
                    return (S4UISubmenu)submenuSceneSurfaceId;
                }

                return submenuTranslation.GetValueOrDefault(submenuSceneSurfaceId, S4UISubmenu.Unknown);
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
        public unsafe Int32* UIScenes => gameValues.AddressAsPointer<Int32>(0x1064C98);

        public GUIEventHandler? GUIEventHandler {
            get {
                unsafe {
                    var handler = (GUIEventHandler*)new IntPtr(gameValues.ReadValue<Int32>(0x10540CC)).ToPointer();

                    if (handler == null) {
                        return null;
                    }

                    return *handler;
                }
            }
        }
    }
}
