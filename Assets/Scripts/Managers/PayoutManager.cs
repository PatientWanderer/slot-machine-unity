// =============================================================================
// PayoutManager.cs
// Evaluates the 3-symbol centre payline for the 4-symbol game.
//
// Paytable (3-of-a-kind only, centre row):
//   Cherry  × 3  →  2  × bet   (most common)
//   Bell    × 3  →  5  × bet
//   BAR     × 3  → 15  × bet   + triggers Free Spins bonus
//   Lucky 7 × 3  → 50  × bet   JACKPOT
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using SlotGame.Data;

namespace SlotGame.Managers
{
    /// <summary>Outcome of a single spin evaluation.</summary>
    public class SpinResult
    {
        public bool       IsWin               { get; set; }
        public bool       IsJackpot           { get; set; }
        public bool       TriggeredFreeSpins  { get; set; }
        public int        Payout              { get; set; }
        public SymbolData MatchedSymbol       { get; set; }
    }

    public class PayoutManager : MonoBehaviour
    {
        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Evaluates the three centre-row symbols and returns the outcome.
        /// </summary>
        /// <param name="paylineSymbols">Exactly 3 SymbolData objects (left→right).</param>
        /// <param name="currentBet">Active bet in credits.</param>
        /// <param name="multiplier">Extra multiplier (1 = normal, 2 = free-spin).</param>
        public SpinResult Evaluate(List<SymbolData> paylineSymbols,
                                   int currentBet,
                                   float multiplier = 1f)
        {
            if (paylineSymbols == null || paylineSymbols.Count != 3)
            {
                Debug.LogError("[PayoutManager] Evaluate requires exactly 3 symbols.");
                return new SpinResult();
            }

            var result = new SpinResult();

            // ── Check 3-of-a-kind on centre payline ───────────────────────────
            var s0 = paylineSymbols[0];
            var s1 = paylineSymbols[1];
            var s2 = paylineSymbols[2];

            bool isThreeOfAKind = s0 != null
                                  && s0.symbolId == s1?.symbolId
                                  && s1.symbolId == s2?.symbolId;

            if (isThreeOfAKind)
            {
                result.IsWin        = true;
                result.MatchedSymbol = s0;
                result.Payout       = Mathf.RoundToInt(
                    s0.payoutThreeOfAKind * currentBet * multiplier);

                // Bonus flags from SymbolData
                result.IsJackpot          = s0.isJackpot;
                result.TriggeredFreeSpins = s0.triggersFreeSpins;
            }

            return result;
        }
    }
}
