// =============================================================================
// UIManager.cs
// Purely display-layer: listens to GameEvents and updates UI.
//
// Asset assignments:
//   balancePanel / betPanel       ← slot_machine_Middle_box.png (orange info box)
//   winPanel background           ← popup.png (dark navy rounded rect)
//   freeSpinPanel background      ← popup.png
//   background Image              ← bg_gradient.png
// =============================================================================

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SlotGame.Utils;

namespace SlotGame.UI
{
    public class UIManager : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────────
        [Header("HUD – Balance Box (slot_machine_Middle_box)")]
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI betText;

        [Header("Win Panel (popup.png background)")]
        [SerializeField] private GameObject      winPanel;
        [SerializeField] private TextMeshProUGUI winTitleText;    // "YOU WIN!" / "JACKPOT!"
        [SerializeField] private TextMeshProUGUI winAmountText;   // "+500"
        [SerializeField] private float           winPanelDuration = 3f;

        [Header("Free Spin Panel (popup.png background)")]
        [SerializeField] private GameObject      freeSpinPanel;
        [SerializeField] private TextMeshProUGUI freeSpinText;    // "FREE SPINS\n5"

        [Header("Machine State Images")]
        [Tooltip("slot-machine1.png – idle/ready state cabinet sprite.")]
        [SerializeField] private Image           machineIdleImage;

        [Tooltip("slot-machine4.png – result/landed state cabinet sprite.")]
        [SerializeField] private Image           machineResultImage;

        [Header("Colours")]
        [SerializeField] private Color jackpotTitleColour = new Color(1f, 0.85f, 0f);
        [SerializeField] private Color normalWinColour    = Color.white;

        // ─── Unity lifecycle ──────────────────────────────────────────────────
        private void OnEnable()
        {
            GameEvents.OnBalanceChanged     += UpdateBalance;
            GameEvents.OnBetChanged         += UpdateBet;
            GameEvents.OnSpinStarted        += OnSpinStart;
            GameEvents.OnSpinComplete       += OnSpinEnd;
            GameEvents.OnWin                += OnWin;
            GameEvents.OnLoss               += OnLoss;
            GameEvents.OnFreeSpinsTriggered += ShowFreeSpinPanel;
            GameEvents.OnFreeSpinUsed       += UpdateFreeSpinCount;
            GameEvents.OnFreeSpinsEnded     += HideFreeSpinPanel;
        }

        private void OnDisable()
        {
            GameEvents.OnBalanceChanged     -= UpdateBalance;
            GameEvents.OnBetChanged         -= UpdateBet;
            GameEvents.OnSpinStarted        -= OnSpinStart;
            GameEvents.OnSpinComplete       -= OnSpinEnd;
            GameEvents.OnWin                -= OnWin;
            GameEvents.OnLoss               -= OnLoss;
            GameEvents.OnFreeSpinsTriggered -= ShowFreeSpinPanel;
            GameEvents.OnFreeSpinUsed       -= UpdateFreeSpinCount;
            GameEvents.OnFreeSpinsEnded     -= HideFreeSpinPanel;
        }

        private void Start()
        {
            winPanel?.SetActive(false);
            freeSpinPanel?.SetActive(false);

            // Start in idle/ready visual state
            SetMachineState(idle: true);
        }

        // ─── Event handlers ───────────────────────────────────────────────────

        private void UpdateBalance(int balance)
        {
            if (balanceText) balanceText.text = $"{balance:N0}G";
        }

        private void UpdateBet(int bet)
        {
            if (betText) betText.text = $"BET {bet}G";
        }

        private void OnSpinStart()
        {
            winPanel?.SetActive(false);
            SetMachineState(idle: true);    // idle = blue-window state (spinning)
        }

        private void OnSpinEnd()
        {
            SetMachineState(idle: false);   // result = white-window state (landed)
        }

        private void OnWin(int payout, bool isJackpot, bool isFree)
        {
            winPanel?.SetActive(true);

            if (winTitleText)
            {
                winTitleText.text  = isJackpot ? "★ JACKPOT! ★" : (isFree ? "FREE WIN!" : "YOU WIN!");
                winTitleText.color = isJackpot ? jackpotTitleColour : normalWinColour;
            }

            if (winAmountText) winAmountText.text = $"+{payout:N0}G";

            StopAllCoroutines();
            StartCoroutine(AutoHideWin());
        }

        private void OnLoss()
        {
            winPanel?.SetActive(false);
        }

        private void ShowFreeSpinPanel(int awarded)
        {
            freeSpinPanel?.SetActive(true);
            if (freeSpinText) freeSpinText.text = $"FREE SPINS\n{awarded}";
        }

        private void UpdateFreeSpinCount(int remaining)
        {
            if (freeSpinText) freeSpinText.text = $"FREE SPINS\n{remaining}";
        }

        private void HideFreeSpinPanel()
        {
            freeSpinPanel?.SetActive(false);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Swaps between the two machine cabinet images:
        ///   idle=true  → slot-machine1 (blue windows, spinning feel)
        ///   idle=false → slot-machine4 (white windows, landed/result)
        /// </summary>
        private void SetMachineState(bool idle)
        {
            if (machineIdleImage)   machineIdleImage.gameObject.SetActive(idle);
            if (machineResultImage) machineResultImage.gameObject.SetActive(!idle);
        }

        private IEnumerator AutoHideWin()
        {
            yield return new WaitForSeconds(winPanelDuration);
            winPanel?.SetActive(false);
        }
    }
}
