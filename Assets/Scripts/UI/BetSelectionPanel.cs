// =============================================================================
// BetSelectionPanel.cs
// Implements the bet-selection popup seen in the GIF:
//   ┌──────────────────┐
//   │   Bet  10G       │  ← button 0
//   │   Bet  50G       │  ← button 1
//   │   Bet 100G       │  ← button 2
//   │   Exit           │  ← closes panel
//   └──────────────────┘
//
// Background: popup.png (dark navy rounded rectangle)
// Panel is shown when the machine is clicked / SpinButton is pressed while
// no bet has been set for this round. You can also wire a dedicated "BET" button.
//
// Uses the right-arrow triangle buttons (slot_machine_buttons-03) as decorative
// bullets next to each option, and the X close button (slot_machine_buttons-02)
// as the close/exit trigger.
// =============================================================================

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SlotGame.Core;
using SlotGame.Utils;

namespace SlotGame.UI
{
    public class BetSelectionPanel : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────────
        [Header("Panel Root")]
        [Tooltip("The GameObject containing popup.png as its background Image.")]
        [SerializeField] private GameObject panelRoot;

        [Header("Bet Option Buttons")]
        [Tooltip("Assign 3 Button components, one per bet tier (10G / 50G / 100G).")]
        [SerializeField] private Button[]            betButtons;    // length 3

        [Tooltip("TextMeshProUGUI labels inside each bet button.")]
        [SerializeField] private TextMeshProUGUI[]   betLabels;     // length 3

        [Header("Exit / Close")]
        [Tooltip("The X-button (slot_machine_buttons-02) that closes the panel.")]
        [SerializeField] private Button              closeButton;

        [Header("Current Bet Display")]
        [Tooltip("Optional: highlights the currently selected bet option.")]
        [SerializeField] private Color               selectedColour = new Color(1f, 0.85f, 0f);
        [SerializeField] private Color               normalColour   = Color.white;

        // ─── Unity lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            // Populate button labels from Constants
            for (int i = 0; i < betButtons.Length && i < Constants.BetOptions.Length; i++)
            {
                int capturedIndex = i;   // capture for lambda
                betLabels[i].text = $"Bet {Constants.BetOptions[i]}G";
                betButtons[i].onClick.AddListener(() => OnBetSelected(capturedIndex));
            }

            closeButton.onClick.AddListener(Close);
        }

        private void OnEnable()
        {
            GameEvents.OnBetMenuOpened += Open;
            GameEvents.OnBetMenuClosed += Close;
            RefreshHighlight();
        }

        private void OnDisable()
        {
            GameEvents.OnBetMenuOpened -= Open;
            GameEvents.OnBetMenuClosed -= Close;
        }

        private void Start()
        {
            panelRoot.SetActive(false);   // hidden on start
        }

        // ─── Public API ───────────────────────────────────────────────────────

        public void Open()
        {
            panelRoot.SetActive(true);
            RefreshHighlight();
        }

        public void Close()
        {
            panelRoot.SetActive(false);
        }

        // ─── Private ──────────────────────────────────────────────────────────

        private void OnBetSelected(int index)
        {
            GameManager.Instance.SetBetByIndex(index);
            RefreshHighlight();
            Close();

            // After the player picks a bet, kick off the spin immediately
            // (matches the GIF flow where picking Bet Xg starts the spin).
            FindObjectOfType<SlotMachine>()?.RequestSpin();
        }

        /// <summary>
        /// Tints the label of the active bet tier gold; others stay white.
        /// </summary>
        private void RefreshHighlight()
        {
            int current = GameManager.Instance != null ? GameManager.Instance.BetIndex : 0;
            for (int i = 0; i < betLabels.Length; i++)
                betLabels[i].color = (i == current) ? selectedColour : normalColour;
        }
    }
}
