// =============================================================================
// Constants.cs
// All magic numbers and configuration values in one place.
// Derived from the provided asset pack and GIF reference gameplay.
// =============================================================================

namespace SlotGame.Utils
{
    public static class Constants
    {
        // ── Reel animation ────────────────────────────────────────────────────
        /// <summary>Height (px) of a single symbol cell on the reel strip.</summary>
        public const float SymbolHeight      = 150f;

        /// <summary>Number of symbol rows visible at once per reel.</summary>
        public const int   VisibleRows       = 3;

        /// <summary>Blank padding cells added above and below the visible window.</summary>
        public const int   PaddingSymbols    = 2;

        /// <summary>Peak scroll speed in pixels per second during full-spin phase.</summary>
        public const float MaxSpinSpeed      = 2800f;

        /// <summary>Seconds for reel to reach full speed.</summary>
        public const float AccelerationTime  = 0.25f;

        /// <summary>Seconds for reel to decelerate and snap.</summary>
        public const float DecelerationTime  = 0.55f;

        /// <summary>Minimum additional full loops before landing on result.</summary>
        public const int   MinSpinLoops      = 3;

        /// <summary>Stagger delay (seconds) between each reel stopping.</summary>
        public const float ReelStopDelay     = 0.20f;

        // ── Lever animation ───────────────────────────────────────────────────
        /// <summary>Seconds for the lever pull-down animation.</summary>
        public const float LeverPullDuration = 0.18f;

        /// <summary>Seconds for the lever spring-back animation.</summary>
        public const float LeverReturnDuration = 0.28f;

        /// <summary>Degrees the lever rotates downward on pull.</summary>
        public const float LeverPullAngle    = 40f;

        // ── Economy – matches GIF bet options: 10G / 50G / 100G ──────────────
        public const int   StartingBalance   = 1000;

        // Fixed bet tiers shown in the bet selection menu
        public static readonly int[] BetOptions = { 10, 50, 100 };

        /// <summary>Default bet tier index (0 = 10G).</summary>
        public const int   DefaultBetIndex   = 0;

        // ── Symbol IDs (must match SymbolData.symbolId assets) ───────────────
        public const int   SymbolId_Cherry   = 0;
        public const int   SymbolId_Bell     = 1;
        public const int   SymbolId_Bar      = 2;
        public const int   SymbolId_Seven    = 3;

        // ── Payline payout multipliers (× bet) ───────────────────────────────
        public const int   Payout_Cherry3    = 2;    // 3× Cherry  =  2× bet
        public const int   Payout_Bell3      = 5;    // 3× Bell    =  5× bet
        public const int   Payout_Bar3       = 15;   // 3× BAR     = 15× bet
        public const int   Payout_Seven3     = 50;   // 3× Lucky 7 = JACKPOT (50× bet)

        // ── Free-spin bonus ───────────────────────────────────────────────────
        // (No scatter in this 4-symbol set; bonus triggered by 3× BAR)
        public const int   FreeSpinsAwarded      = 5;
        public const float FreeSpinMultiplier    = 2f;

        // ── Animator / trigger names ──────────────────────────────────────────
        public const string AnimTriggerSpin      = "Spin";
        public const string AnimTriggerWin       = "Win";
        public const string AnimTriggerJackpot   = "Jackpot";
        public const string AnimTriggerIdle      = "Idle";
        public const string AnimTriggerLeverPull = "Pull";

        // ── PlayerPrefs keys ──────────────────────────────────────────────────
        public const string PrefBalance          = "PlayerBalance";
        public const string PrefLastBetIndex     = "LastBetIndex";
    }
}
