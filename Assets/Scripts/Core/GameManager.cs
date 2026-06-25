// =============================================================================
// GameManager.cs
// Singleton. Owns balance and the currently selected bet tier.
// Bet is chosen from a fixed set (10G / 50G / 100G) matching the GIF UI.
// =============================================================================

using UnityEngine;
using SlotGame.Utils;

namespace SlotGame.Core
{
    public class GameManager : MonoBehaviour
    {
        // ─── Singleton ────────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ─── State ────────────────────────────────────────────────────────────
        public int  Balance         { get; private set; }

        /// <summary>Index into Constants.BetOptions (0=10G, 1=50G, 2=100G).</summary>
        public int  BetIndex        { get; private set; }

        /// <summary>Current bet in credits.</summary>
        public int  CurrentBet      => Constants.BetOptions[BetIndex];

        public bool IsSpinning      { get; private set; }

        // ─── Unity lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadProgress();
            GameEvents.RaiseBalanceChanged(Balance);
            GameEvents.RaiseBetChanged(CurrentBet);
        }

        private void OnApplicationQuit()              => SaveProgress();
        private void OnApplicationPause(bool paused)  { if (paused) SaveProgress(); }

        // ─── Event wiring ─────────────────────────────────────────────────────
        private void OnEnable()
        {
            GameEvents.OnSpinStarted  += () => IsSpinning = true;
            GameEvents.OnSpinComplete += () => IsSpinning = false;
            GameEvents.OnWin          += HandleWin;
        }

        private void OnDisable()
        {
            GameEvents.OnSpinStarted  -= () => IsSpinning = true;
            GameEvents.OnSpinComplete -= () => IsSpinning = false;
            GameEvents.OnWin          -= HandleWin;
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Deducts the current bet. Returns false if spinning or insufficient funds.
        /// </summary>
        public bool TryDeductBet()
        {
            if (IsSpinning)
            {
                Debug.LogWarning("[GameManager] Already spinning.");
                return false;
            }
            if (Balance < CurrentBet)
            {
                Debug.LogWarning("[GameManager] Insufficient balance.");
                return false;
            }

            SetBalance(Balance - CurrentBet);
            return true;
        }

        /// <summary>Credits added after a win.</summary>
        public void AddCredits(int amount)
        {
            if (amount > 0) SetBalance(Balance + amount);
        }

        /// <summary>
        /// Sets the active bet tier by index (0=10G, 1=50G, 2=100G).
        /// Called by BetSelectionPanel when the player picks an option.
        /// </summary>
        public void SetBetByIndex(int index)
        {
            BetIndex = Mathf.Clamp(index, 0, Constants.BetOptions.Length - 1);
            GameEvents.RaiseBetChanged(CurrentBet);
        }

        // ─── Private helpers ──────────────────────────────────────────────────

        private void SetBalance(int value)
        {
            Balance = Mathf.Max(0, value);
            GameEvents.RaiseBalanceChanged(Balance);
        }

        private void HandleWin(int payout, bool isJackpot, bool isFree)
        {
            SetBalance(Balance + payout);
        }

        // ─── Persistence ──────────────────────────────────────────────────────

        private void SaveProgress()
        {
            PlayerPrefs.SetInt(Constants.PrefBalance,     Balance);
            PlayerPrefs.SetInt(Constants.PrefLastBetIndex, BetIndex);
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            Balance  = PlayerPrefs.GetInt(Constants.PrefBalance,      Constants.StartingBalance);
            BetIndex = PlayerPrefs.GetInt(Constants.PrefLastBetIndex,  Constants.DefaultBetIndex);
            BetIndex = Mathf.Clamp(BetIndex, 0, Constants.BetOptions.Length - 1);
        }
    }
}
