using System.Numerics;

namespace Forge.UX.Rendering.Text {
    public interface ITextRenderer {
        /// <summary>
        /// Renders text at the given position and clips around size, when given.
        /// <br/>
        /// Text style is derived from the current renderer state
        /// </summary>
        /// <param name="text">Text to render</param>
        /// <param name="position">Position to render text at</param>
        /// <param name="size">Clipping rect parameters</param>
        void RenderText(string text, Vector2 position, Vector2? size = null);

        /// <summary>
        /// Renders text at the given position and scales the text around size.
        /// <br/>
        /// Text style is derived from the current renderer state, but TextSize is ignored
        /// </summary>
        /// <param name="text">Text to render</param>
        /// <param name="position">Position to render text at</param>
        /// <param name="size">Clipping rect parameters</param>
        void RenderTextScaled(string text, Vector2 position, Vector2 size);

        #region State
        void SetTextSize(TextSize size);
        void SetTextType(TextType type);
        void SetTextAlignment(Alignment alignment);
        void SetTextColor(int rgba);

        void SetTextStyle(TextStyle style);

        /// <summary>
        /// Resets the renderer to the default state
        /// <br/>
        /// Regular Sized, Normal Typed, Left Aligned, Black
        /// </summary>
        void ResetTextState();

        /// <summary>
        /// Save State Context Id Ranges are per game instance, making it possible for collisions to occur when using multiple mods
        /// <br/>
        /// This method returns the start index for a 20 long yet unused save range. This does reserves that range as well
        /// </summary>
        /// <returns>A starting index for a yet unused save state index range</returns>
        int GetUnusedSaveStateRange();
        /// <summary>
        /// Saves the current state to the given id - overwriting is not allowed and will throw an exception
        /// </summary>
        void SaveStateContext(int id);
        /// <summary>
        /// Loads the saved state into the current renderer context
        /// </summary>
        void LoadStateContext(int id);
        #endregion
    }
}
