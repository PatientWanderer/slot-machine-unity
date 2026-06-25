// =============================================================================
// LeverController.cs
// Animates the slot machine lever using the provided sprite assets:
//   • slot-machine2.png → full lever (normal/idle state)
//   • slot-machine3.png → lever knob only (pulled-down frame)
//
// Place this script on the Lever GameObject, which is a child of the machine.
// The lever rotates around its top pivot on pull and springs back after.
//
// The SpinButton (or a PointerDown handler) calls PullLever() to start.
// =============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SlotGame.Core;
using SlotGame.Utils;

namespace SlotGame.UI
{
    /// <summary>
    /// Handles player interaction with the lever and drives its animation.
    /// Also triggers the spin via SlotMachine.RequestSpin().
    /// </summary>
    public class LeverController : MonoBehaviour, IPointerDownHandler
    {
        // ─── Inspector ────────────────────────────────────────────────────────
        [Header("References")]
        [Tooltip("The Image that shows the full lever sprite (slot-machine2.png).")]
        [SerializeField] private Image      leverImage;

        [Tooltip("The RectTransform to rotate (should be anchored at lever top).")]
        [SerializeField] private RectTransform leverPivot;

        [SerializeField] private SlotMachine   slotMachine;

        [Header("Sprites")]
        [Tooltip("slot-machine2.png – normal lever sprite.")]
        [SerializeField] private Sprite        spriteIdle;

        [Tooltip("slot-machine3.png – pulled-down knob sprite (cropped lever).")]
        [SerializeField] private Sprite        spritePulled;

        // ─── State ────────────────────────────────────────────────────────────
        private bool _isAnimating;

        // ─── IPointerDownHandler ──────────────────────────────────────────────
        public void OnPointerDown(PointerEventData eventData)
        {
            PullLever();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>Starts the lever animation and triggers a spin request.</summary>
        public void PullLever()
        {
            if (_isAnimating || (slotMachine != null && GameManager.Instance.IsSpinning))
                return;

            StartCoroutine(LeverAnimation());
        }

        // ─── Animation coroutine ──────────────────────────────────────────────

        private IEnumerator LeverAnimation()
        {
            _isAnimating = true;
            GameEvents.RaiseLeverPulled();

            // ── Phase 1: Pull down ────────────────────────────────────────────
            if (spritePulled != null) leverImage.sprite = spritePulled;

            float elapsed = 0f;
            while (elapsed < Constants.LeverPullDuration)
            {
                elapsed += Time.deltaTime;
                float t  = elapsed / Constants.LeverPullDuration;
                float z  = Mathf.Lerp(0f, -Constants.LeverPullAngle, EaseOut(t));

                leverPivot.localRotation = Quaternion.Euler(0f, 0f, z);
                yield return null;
            }

            // Lever is fully down → trigger the spin
            slotMachine?.RequestSpin();

            // ── Phase 2: Spring back ──────────────────────────────────────────
            elapsed = 0f;
            while (elapsed < Constants.LeverReturnDuration)
            {
                elapsed += Time.deltaTime;
                float t  = elapsed / Constants.LeverReturnDuration;
                float z  = Mathf.Lerp(-Constants.LeverPullAngle, 0f, EaseOut(t));

                leverPivot.localRotation = Quaternion.Euler(0f, 0f, z);
                yield return null;
            }

            // Restore idle sprite and exact zero rotation
            leverPivot.localRotation = Quaternion.identity;
            if (spriteIdle != null) leverImage.sprite = spriteIdle;

            _isAnimating = false;
        }

        // ─── Easing ───────────────────────────────────────────────────────────
        private static float EaseOut(float t) => 1f - (1f - t) * (1f - t);
    }
}
