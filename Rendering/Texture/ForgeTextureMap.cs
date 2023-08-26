using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.Rendering.Texture {
    public enum ForgeTextureMap {
        #region Checkboxes

        CheckboxUnchecked = 0,
        CheckboxChecked = 1,
        RadiobuttonUnchecked = 4,
        RadiobuttonChecked = 5,

        #endregion


        #region Buttons

        Button = 2,
        ButtonPressed = 3,
        LongButton = 6,
        LongButtonPressed = 7,

        #endregion

        #region Menus

        SettingsMenu = 41,
        SettingsMenuFull = 42,
        CreditsMenu = 43,

        #endregion
    }
}
