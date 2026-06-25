// =============================================================================
// Reel.cs
// Controls the spin animation for one reel using the infinite-strip approach.
//
// Asset mapping:
//   • symbolCellPrefab → a UI Image displaying one of the 4 slot-symbol PNGs
//   • reelStrip        → the scrolling RectTransform behind a Mask (slot-machine5 style)
//   • The Mask height  = SymbolHeight × VisibleRows (3 × 150 = 450 px)
// =============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotGame.Data;
using SlotGame.Utils;

namespace SlotGame.Core
{
    public class Reel : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────────
        [Header("References")]
        [Tooltip("The RectTransform containing all symbol cell Images – scrolled vertically.")]
        [SerializeField] private RectTransform reelStrip;

        [Tooltip("Prefab: RectTransform + Image, sized 150×150.")]
        [SerializeField] private GameObject    symbolCellPrefab;

        [SerializeField] private SymbolDatabase symbolDatabase;

        [Header("Feel")]
        [Tooltip("AnimationCurve: x=normalised progress (0→1), y=speed fraction (0→1). " +
                 "Use EaseIn for deceleration.")]
        [SerializeField] private AnimationCurve decelerationCurve =
            AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        // ─── State ────────────────────────────────────────────────────────────
        /// <summary>Symbol sitting on the centre payline after the reel stops.</summary>
        public SymbolData PaylineSymbol { get; private set; }

        public bool IsSpinning { get; private set; }

        /// <summary>Called once this reel fully stops.</summary>
        public event Action OnReelStopped;

        // ─── Private ──────────────────────────────────────────────────────────
        private List<SymbolData> _pool;          // weighted, shuffled symbol pool
        private List<Image>      _cells;         // instantiated Images
        private int              _stripLength;   // total cell count
        private float            _stripHeight;   // total pixel height of strip
        private float            _currentY;      // current scroll offset
        private int              _targetIndex;   // which cell lands on payline

        // ─── Unity lifecycle ──────────────────────────────────────────────────
        private void Awake() => BuildStrip();

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Spins this reel and snaps to <paramref name="result"/> after
        /// <paramref name="spinDuration"/> seconds at full speed.
        /// </summary>
        public IEnumerator Spin(SymbolData result, float spinDuration)
        {
            if (IsSpinning) yield break;
            IsSpinning = true;

            _targetIndex  = FindStripIndexFor(result);
            PaylineSymbol = _pool[_targetIndex % _pool.Count];

            yield return StartCoroutine(Accelerate());
            yield return StartCoroutine(FullSpeed(spinDuration));
            yield return StartCoroutine(Decelerate());

            SnapToTarget();
            IsSpinning = false;
            OnReelStopped?.Invoke();
        }

        // ─── Strip construction ───────────────────────────────────────────────

        private void BuildStrip()
        {
            _pool = symbolDatabase.BuildWeightedStrip();

            // Enough cells for MinSpinLoops full rotations plus padding
            int minCells  = _pool.Count * Constants.MinSpinLoops
                            + Constants.VisibleRows + Constants.PaddingSymbols * 2;
            _stripLength  = Mathf.Max(_pool.Count * 2, minCells);
            _stripHeight  = _stripLength * Constants.SymbolHeight;
            _cells        = new List<Image>(_stripLength);

            // Clear existing children (useful on Editor reload)
            foreach (Transform child in reelStrip) Destroy(child.gameObject);

            for (int i = 0; i < _stripLength; i++)
            {
                var go   = Instantiate(symbolCellPrefab, reelStrip);
                var img  = go.GetComponent<Image>();
                var rect = go.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(0f, -i * Constants.SymbolHeight);
                rect.sizeDelta        = new Vector2(rect.sizeDelta.x, Constants.SymbolHeight);

                // Assign sprite from pool (wraps around)
                var sym   = _pool[i % _pool.Count];
                img.sprite = sym.symbolSprite;
                img.color  = sym.tintColor;

                _cells.Add(img);
            }

            _currentY = 0f;
            ApplyPosition();
        }

        // ─── Animation phases ─────────────────────────────────────────────────

        private IEnumerator Accelerate()
        {
            float t = 0f;
            while (t < Constants.AccelerationTime)
            {
                t    += Time.deltaTime;
                float speed = Mathf.Lerp(0f, Constants.MaxSpinSpeed,
                                         t / Constants.AccelerationTime);
                Scroll(speed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator FullSpeed(float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                Scroll(Constants.MaxSpinSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator Decelerate()
        {
            // How far we need to scroll so targetIndex lands on the payline
            float targetY     = _targetIndex * Constants.SymbolHeight;
            float remaining   = CalculateForwardDistance(targetY);

            // Guarantee at least MinSpinLoops extra revolutions so it looks real
            float minExtra    = Constants.MinSpinLoops * _stripHeight;
            if (remaining < minExtra)
            {
                float loops    = Mathf.Ceil((minExtra - remaining) / _stripHeight);
                remaining     += _stripHeight * loops;
            }

            float totalDist   = remaining;
            float scrolled    = 0f;

            while (scrolled < totalDist)
            {
                float progress = Mathf.Clamp01(scrolled / totalDist);
                float speed    = decelerationCurve.Evaluate(progress) * Constants.MaxSpinSpeed;
                speed          = Mathf.Max(speed, 8f);      // never fully stop mid-scroll

                float delta    = Mathf.Min(speed * Time.deltaTime, totalDist - scrolled);
                Scroll(delta);
                scrolled      += delta;
                yield return null;
            }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void Scroll(float amount)
        {
            _currentY = (_currentY + amount) % _stripHeight;
            ApplyPosition();
        }

        private void ApplyPosition()
        {
            reelStrip.anchoredPosition = new Vector2(0f, _currentY);
        }

        private void SnapToTarget()
        {
            _currentY = (_targetIndex * Constants.SymbolHeight) % _stripHeight;
            ApplyPosition();
        }

        /// <summary>
        /// Calculates remaining forward scroll distance to align targetY with the payline.
        /// Always moves in the scroll direction (forward = positive Y).
        /// </summary>
        private float CalculateForwardDistance(float targetY)
        {
            float cur  = _currentY % _stripHeight;
            float diff = targetY - cur;
            if (diff <= 0f) diff += _stripHeight;
            return diff;
        }

        /// <summary>Finds the first cell index whose pool entry matches the given symbol.</summary>
        private int FindStripIndexFor(SymbolData sym)
        {
            for (int i = 0; i < _stripLength; i++)
            {
                if (_pool[i % _pool.Count].symbolId == sym.symbolId)
                    return i;
            }
            Debug.LogWarning($"[Reel] Symbol '{sym.symbolName}' not found on strip, using 0.");
            return 0;
        }
    }
}
