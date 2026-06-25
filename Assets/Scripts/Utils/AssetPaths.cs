// =============================================================================
// AssetPaths.cs
// Single source of truth for every asset filename provided in the asset pack.
// Use these constants anywhere you load a sprite/texture at runtime so that
// renaming a file only requires a change here.
//
// Assumed folder layout inside Assets/Resources/ :
//   Resources/
//     Symbols/           ← slot-symbol1..4
//     UI/
//       Machine/         ← slot-machine1..5
//       Buttons/         ← slot_machine_buttons-02..04, Yes_No_Btn
//       Panels/          ← slot_machine_Middle_box, popup, bg_gradient
// =============================================================================

namespace SlotGame.Utils
{
    public static class AssetPaths
    {
        // ── Slot symbols ──────────────────────────────────────────────────────
        /// <summary>slot-symbol1.png  – Lucky 7 (rarest, jackpot)</summary>
        public const string Symbol_Seven   = "Symbols/slot-symbol1";

        /// <summary>slot-symbol2.png  – Cherry (most common)</summary>
        public const string Symbol_Cherry  = "Symbols/slot-symbol2";

        /// <summary>slot-symbol3.png  – Bell (medium frequency)</summary>
        public const string Symbol_Bell    = "Symbols/slot-symbol3";

        /// <summary>slot-symbol4.png  – BAR×3 (rare)</summary>
        public const string Symbol_Bar     = "Symbols/slot-symbol4";

        // ── Machine cabinet layers ─────────────────────────────────────────────
        /// <summary>slot-machine1.png – Cabinet with light-blue reel windows (idle/ready state).</summary>
        public const string Machine_Idle   = "UI/Machine/slot-machine1";

        /// <summary>slot-machine2.png – Full machine with lever handle visible.</summary>
        public const string Machine_Lever  = "UI/Machine/slot-machine2";

        /// <summary>slot-machine3.png – Lever knob only (used for pull animation frame).</summary>
        public const string Machine_LeverKnob = "UI/Machine/slot-machine3";

        /// <summary>slot-machine4.png – Cabinet with empty white reel windows (result/landed state).</summary>
        public const string Machine_Result = "UI/Machine/slot-machine4";

        /// <summary>slot-machine5.png – Three separate reel strip panels (used as reel background).</summary>
        public const string Machine_ReelStrips = "UI/Machine/slot-machine5";

        // ── Buttons ───────────────────────────────────────────────────────────
        /// <summary>slot_machine_buttons-02.png – Close (X) button sprite sheet.
        /// Contains 4 states top-to-bottom: Normal, Hover, Pressed, Disabled.</summary>
        public const string Btn_Close      = "UI/Buttons/slot_machine_buttons-02";

        /// <summary>slot_machine_buttons-03.png – Right-arrow (Next/Increase) button sprite sheet.
        /// 4 states: Normal, Hover, Pressed, Disabled.</summary>
        public const string Btn_ArrowRight = "UI/Buttons/slot_machine_buttons-03";

        /// <summary>slot_machine_buttons-04.png – Left-arrow (Prev/Decrease) button sprite sheet.
        /// 4 states: Normal, Hover, Pressed, Disabled.</summary>
        public const string Btn_ArrowLeft  = "UI/Buttons/slot_machine_buttons-04";

        /// <summary>Yes_No_Btn.png – YES / NO gold buttons (3 rows × 2 columns of states).</summary>
        public const string Btn_YesNo      = "UI/Buttons/Yes_No_Btn";

        // ── Panels ────────────────────────────────────────────────────────────
        /// <summary>slot_machine_Middle_box.png – Orange/red gradient info box
        /// (used for Balance and Bet displays on the machine face).</summary>
        public const string Panel_InfoBox  = "UI/Panels/slot_machine_Middle_box";

        /// <summary>popup.png – Dark navy rounded-rectangle popup background
        /// (used for the Bet Selection menu and Win/Lose overlays).</summary>
        public const string Panel_Popup    = "UI/Panels/popup";

        /// <summary>bg_gradient.png – Purple halftone-dot gradient scene background.</summary>
        public const string BG_Gradient    = "UI/Panels/bg_gradient";
    }
}
