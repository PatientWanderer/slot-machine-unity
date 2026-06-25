// =============================================================================
// SymbolData.cs
// ScriptableObject for one reel symbol. Create one asset per symbol:
//   Cherry  (slot-symbol2) · Bell (slot-symbol3)
//   BAR×3   (slot-symbol4) · Lucky 7 (slot-symbol1)
//
// Create via: Assets → Create → SlotGame → Symbol Data
// =============================================================================

using UnityEngine;

namespace SlotGame.Data
{
    [CreateAssetMenu(menuName = "SlotGame/Symbol Data", fileName = "Symbol_New")]
    public class SymbolData : ScriptableObject
    {
        // ── Identity ──────────────────────────────────────────────────────────
        [Header("Identity")]
        [Tooltip("Unique ID. Use Constants.SymbolId_* values for safety.")]
        public int    symbolId;

        [Tooltip("Display name shown in debug and the paytable UI.")]
        public string symbolName;

        // ── Visuals ───────────────────────────────────────────────────────────
        [Header("Visuals")]
        [Tooltip("Sprite from Assets/Resources/Symbols/ – assign the matching slot-symbol PNG.")]
        public Sprite symbolSprite;

        [Tooltip("Optional tint (white = no tint).")]
        public Color  tintColor = Color.white;

        // ── Payout ────────────────────────────────────────────────────────────
        [Header("Payout")]
        [Tooltip("Multiplier applied to the current bet on 3-of-a-kind on the centre payline.\n" +
                 "Cherry=2  Bell=5  BAR=15  Lucky7=50")]
        public int    payoutThreeOfAKind;

        // ── RNG weight ────────────────────────────────────────────────────────
        [Header("RNG Weight")]
        [Tooltip("How many times this symbol appears in the weighted pool.\n" +
                 "Suggested: Cherry=10  Bell=7  BAR=4  Lucky7=1")]
        [Range(1, 20)]
        public int    reelWeight = 5;

        // ── Bonus flags ───────────────────────────────────────────────────────
        [Header("Bonus Flags")]
        [Tooltip("Three BAR symbols in a row triggers the free-spin bonus.")]
        public bool   triggersFreeSpins;

        [Tooltip("If true, landing 3 of this symbol awards the JACKPOT win sound + effect.")]
        public bool   isJackpot;
    }
}
