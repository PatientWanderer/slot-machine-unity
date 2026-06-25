// =============================================================================
// BonusManager.cs
// Manages the free-spin bonus round triggered by landing 3× BAR symbols.
// =============================================================================

using UnityEngine;
using SlotGame.Utils;

namespace SlotGame.Managers
{
    public class BonusManager : MonoBehaviour
    {
        // ─── State ────────────────────────────────────────────────────────────
        public bool  IsFreeSpinActive    => _freeSpinsRemaining > 0;
        public int   FreeSpinsRemaining  => _freeSpinsRemaining;
        public float CurrentMultiplier   => IsFreeSpinActive ? Constants.FreeSpinMultiplier : 1f;

        private int _freeSpinsRemaining;

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Awards free spins. Re-triggering adds more on top of the current total.
        /// </summary>
        public void TriggerFreeSpins()
        {
            _freeSpinsRemaining += Constants.FreeSpinsAwarded;
            Debug.Log($"[BonusManager] Free spins triggered! Total: {_freeSpinsRemaining}");
            GameEvents.RaiseFreeSpinsTriggered(Constants.FreeSpinsAwarded);
        }

        /// <summary>
        /// Consumes one free spin. Returns false if none are active.
        /// </summary>
        public bool TryConsumeFreeSpins()
        {
            if (!IsFreeSpinActive) return false;

            _freeSpinsRemaining--;
            GameEvents.RaiseFreeSpinUsed(_freeSpinsRemaining);

            if (_freeSpinsRemaining == 0)
            {
                Debug.Log("[BonusManager] All free spins used.");
                GameEvents.RaiseFreeSpinsEnded();
            }

            return true;
        }

        public void EndFreeSpins()
        {
            _freeSpinsRemaining = 0;
            GameEvents.RaiseFreeSpinsEnded();
        }
    }
}
