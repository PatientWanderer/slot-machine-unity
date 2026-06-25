// =============================================================================
// SlotMachine.cs
// Orchestrates the full spin flow:
//   1. RNG picks one symbol per reel from the weighted pool.
//   2. Reels spin concurrently with staggered stops (left → centre → right).
//   3. PayoutManager evaluates the centre payline.
//   4. Jackpot (3× Lucky 7) fires a special event.
//   5. Three BAR symbols trigger the free-spin bonus.
// =============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Data;
using SlotGame.Managers;
using SlotGame.Utils;

namespace SlotGame.Core
{
    public class SlotMachine : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────────
        [Header("Reels (Left, Centre, Right)")]
        [SerializeField] private Reel[]          reels;              // must be length 3

        [Header("Data")]
        [SerializeField] private SymbolDatabase  symbolDatabase;

        [Header("Managers")]
        [SerializeField] private PayoutManager   payoutManager;
        [SerializeField] private BonusManager    bonusManager;

        [Header("Timing")]
        [Tooltip("Base full-speed spin duration for the first reel (seconds).")]
        [SerializeField] private float           baseSpinDuration = 1.0f;

        // ─── State ────────────────────────────────────────────────────────────
        private bool _isSpinning;

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Called by LeverController or BetSelectionPanel.
        /// Validates state, deducts bet, and runs the spin coroutine.
        /// </summary>
        public void RequestSpin()
        {
            if (_isSpinning) return;

            bool isFreeRound = bonusManager.IsFreeSpinActive;

            if (!isFreeRound)
            {
                if (!GameManager.Instance.TryDeductBet()) return;
            }
            else
            {
                bonusManager.TryConsumeFreeSpins();
            }

            StartCoroutine(SpinRoutine(isFreeRound));
        }

        // ─── Spin coroutine ───────────────────────────────────────────────────

        private IEnumerator SpinRoutine(bool isFreeRound)
        {
            _isSpinning = true;
            GameEvents.RaiseSpinStarted();

            // ── 1. RNG – pick one result per reel ─────────────────────────────
            var pool     = symbolDatabase.BuildWeightedStrip();
            var results  = new SymbolData[reels.Length];
            for (int i = 0; i < reels.Length; i++)
                results[i] = pool[Random.Range(0, pool.Count)];

            // ── 2. Start all reels (they run concurrently) ────────────────────
            for (int i = 0; i < reels.Length; i++)
            {
                float duration = baseSpinDuration + i * Constants.ReelStopDelay;
                StartCoroutine(reels[i].Spin(results[i], duration));
            }

            // ── 3. Wait for all reels to finish ───────────────────────────────
            bool allDone;
            do
            {
                yield return null;
                allDone = true;
                foreach (var reel in reels)
                    if (reel.IsSpinning) { allDone = false; break; }
            } while (!allDone);

            GameEvents.RaiseSpinComplete();

            // ── 4. Collect payline symbols ────────────────────────────────────
            var payline = new List<SymbolData>(reels.Length);
            foreach (var reel in reels)
                payline.Add(reel.PaylineSymbol);

            // ── 5. Evaluate ───────────────────────────────────────────────────
            float     multiplier = bonusManager.CurrentMultiplier;
            SpinResult outcome   = payoutManager.Evaluate(payline,
                                       GameManager.Instance.CurrentBet,
                                       multiplier);

            // ── 6. BAR free-spin trigger (before crediting win) ───────────────
            if (outcome.TriggeredFreeSpins && !bonusManager.IsFreeSpinActive)
            {
                bonusManager.TriggerFreeSpins();
                yield return null;   // let UI respond to trigger event
            }

            // ── 7. Apply outcome ──────────────────────────────────────────────
            if (outcome.IsWin)
            {
                GameManager.Instance.AddCredits(outcome.Payout);
                GameEvents.RaiseWin(outcome.Payout, outcome.IsJackpot, isFreeRound);

                Debug.Log($"[SlotMachine] WIN  payout={outcome.Payout}  " +
                          $"jackpot={outcome.IsJackpot}  freeSpin={isFreeRound}");
            }
            else
            {
                GameEvents.RaiseLoss();
                Debug.Log("[SlotMachine] No win.");
            }

            _isSpinning = false;
        }
    }
}
