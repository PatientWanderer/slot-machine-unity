// =============================================================================
// GameEvents.cs
// Static pub/sub event bus – all game systems communicate through here,
// keeping direct coupling between components to zero.
// =============================================================================

using System;

namespace SlotGame.Utils
{
    public static class GameEvents
    {
        // ── Spin lifecycle ────────────────────────────────────────────────────
        public static event Action        OnSpinStarted;
        public static event Action        OnSpinComplete;

        // ── Outcomes ──────────────────────────────────────────────────────────
        /// <summary>int payout, bool isJackpot, bool isFreeSpin</summary>
        public static event Action<int, bool, bool> OnWin;

        public static event Action        OnLoss;

        // ── Balance / bet ─────────────────────────────────────────────────────
        /// <summary>int newBalance</summary>
        public static event Action<int>   OnBalanceChanged;

        /// <summary>int newBetAmount (e.g. 10, 50, 100)</summary>
        public static event Action<int>   OnBetChanged;

        // ── Lever ─────────────────────────────────────────────────────────────
        public static event Action        OnLeverPulled;

        // ── Bet selection menu ────────────────────────────────────────────────
        public static event Action        OnBetMenuOpened;
        public static event Action        OnBetMenuClosed;

        // ── Free spins ────────────────────────────────────────────────────────
        public static event Action<int>   OnFreeSpinsTriggered;   // int = awarded
        public static event Action<int>   OnFreeSpinUsed;         // int = remaining
        public static event Action        OnFreeSpinsEnded;

        // ── Raise helpers ─────────────────────────────────────────────────────
        public static void RaiseSpinStarted()                             => OnSpinStarted?.Invoke();
        public static void RaiseSpinComplete()                            => OnSpinComplete?.Invoke();
        public static void RaiseWin(int p, bool jackpot, bool free)       => OnWin?.Invoke(p, jackpot, free);
        public static void RaiseLoss()                                    => OnLoss?.Invoke();
        public static void RaiseBalanceChanged(int bal)                   => OnBalanceChanged?.Invoke(bal);
        public static void RaiseBetChanged(int bet)                       => OnBetChanged?.Invoke(bet);
        public static void RaiseLeverPulled()                             => OnLeverPulled?.Invoke();
        public static void RaiseBetMenuOpened()                           => OnBetMenuOpened?.Invoke();
        public static void RaiseBetMenuClosed()                           => OnBetMenuClosed?.Invoke();
        public static void RaiseFreeSpinsTriggered(int n)                 => OnFreeSpinsTriggered?.Invoke(n);
        public static void RaiseFreeSpinUsed(int remaining)               => OnFreeSpinUsed?.Invoke(remaining);
        public static void RaiseFreeSpinsEnded()                          => OnFreeSpinsEnded?.Invoke();
    }
}
