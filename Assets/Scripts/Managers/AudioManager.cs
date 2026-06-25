// =============================================================================
// AudioManager.cs
// Centralised sound playback. Assign AudioClip assets in the Inspector.
// =============================================================================

using System;
using UnityEngine;
using SlotGame.Utils;

namespace SlotGame.Managers
{
    public enum SoundEffect
    {
        SpinStart,        // plays when lever is pulled / spin begins
        ReelStop,         // plays as each reel lands
        CoinDrop,         // small win
        BigWin,           // large win (BAR × 3)
        Jackpot,          // Lucky 7 × 3 jackpot
        FreeSpinsTrigger, // BAR × 3 triggers bonus
        FreeSpinStart,    // start of each free spin
        ButtonClick,      // UI button generic
        LeverPull         // mechanical lever pull sound
    }

    [Serializable]
    public class SoundEntry
    {
        public SoundEffect effect;
        public AudioClip   clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public class AudioManager : MonoBehaviour
    {
        // ─── Singleton ────────────────────────────────────────────────────────
        public static AudioManager Instance { get; private set; }

        // ─── Inspector ────────────────────────────────────────────────────────
        [Header("Sound Library")]
        [SerializeField] private SoundEntry[] soundEntries;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        // ─── Unity lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnSpinStarted        += () => Play(SoundEffect.SpinStart);
            GameEvents.OnLeverPulled        += () => Play(SoundEffect.LeverPull);
            GameEvents.OnSpinComplete       += () => Play(SoundEffect.ReelStop);
            GameEvents.OnWin                += HandleWinSound;
            GameEvents.OnFreeSpinsTriggered += _ => Play(SoundEffect.FreeSpinsTrigger);
            GameEvents.OnFreeSpinUsed       += _ => Play(SoundEffect.FreeSpinStart);
        }

        private void OnDisable()
        {
            GameEvents.OnSpinStarted        -= () => Play(SoundEffect.SpinStart);
            GameEvents.OnLeverPulled        -= () => Play(SoundEffect.LeverPull);
            GameEvents.OnSpinComplete       -= () => Play(SoundEffect.ReelStop);
            GameEvents.OnWin                -= HandleWinSound;
            GameEvents.OnFreeSpinsTriggered -= _ => Play(SoundEffect.FreeSpinsTrigger);
            GameEvents.OnFreeSpinUsed       -= _ => Play(SoundEffect.FreeSpinStart);
        }

        // ─── Public API ───────────────────────────────────────────────────────
        public void Play(SoundEffect effect)
        {
            var entry = Array.Find(soundEntries, e => e.effect == effect);
            if (entry?.clip == null)
            {
                Debug.LogWarning($"[AudioManager] No clip assigned for {effect}");
                return;
            }
            sfxSource.PlayOneShot(entry.clip, entry.volume);
        }

        // ─── Private ──────────────────────────────────────────────────────────
        private void HandleWinSound(int payout, bool isJackpot, bool isFree)
        {
            if (isJackpot)
                Play(SoundEffect.Jackpot);
            else if (payout >= Constants.BetOptions[1] * 5)   // 5× the mid-bet = "big"
                Play(SoundEffect.BigWin);
            else
                Play(SoundEffect.CoinDrop);
        }
    }
}
