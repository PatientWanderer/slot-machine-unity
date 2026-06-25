// =============================================================================
// WinEffect.cs
// Visual celebration on win/jackpot.
// Payline flash image matches the orange/gold colour from slot_machine_Middle_box.
// =============================================================================

using System.Collections;
using UnityEngine;
using SlotGame.Utils;

namespace SlotGame.UI
{
    public class WinEffect : MonoBehaviour
    {
        [Header("Particles")]
        [SerializeField] private ParticleSystem normalWinParticles;
        [SerializeField] private ParticleSystem jackpotParticles;   // extra burst for Lucky 7

        [Header("Payline Flash")]
        [Tooltip("Thin Image bar across the centre row; use the orange/gold colour from the info box.")]
        [SerializeField] private CanvasGroup paylineFlash;

        [SerializeField] private int   flashCount    = 4;
        [SerializeField] private float flashHalfTime = 0.12f;

        // ─── Unity lifecycle ──────────────────────────────────────────────────
        private void OnEnable()
        {
            GameEvents.OnWin  += HandleWin;
            GameEvents.OnLoss += ResetEffects;
        }

        private void OnDisable()
        {
            GameEvents.OnWin  -= HandleWin;
            GameEvents.OnLoss -= ResetEffects;
        }

        // ─── Handlers ─────────────────────────────────────────────────────────

        private void HandleWin(int payout, bool isJackpot, bool isFree)
        {
            ResetEffects();

            if (isJackpot && jackpotParticles != null)
                jackpotParticles.Play();
            else if (normalWinParticles != null)
                normalWinParticles.Play();

            if (paylineFlash != null)
                StartCoroutine(FlashPayline(isJackpot ? flashCount * 2 : flashCount));
        }

        private void ResetEffects()
        {
            normalWinParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            jackpotParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            StopAllCoroutines();
            if (paylineFlash) paylineFlash.alpha = 0f;
        }

        // ─── Payline flash ────────────────────────────────────────────────────
        private IEnumerator FlashPayline(int count)
        {
            paylineFlash.alpha = 0f;
            for (int i = 0; i < count; i++)
            {
                yield return Fade(0f, 1f, flashHalfTime);
                yield return Fade(1f, 0f, flashHalfTime);
            }
            paylineFlash.alpha = 0f;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                paylineFlash.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
        }
    }
}
