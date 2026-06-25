// =============================================================================
// SymbolDatabase.cs
// Holds the complete list of 4 symbols. Drag all four SymbolData assets here.
// Create via: Assets → Create → SlotGame → Symbol Database
// =============================================================================

using System.Collections.Generic;
using UnityEngine;

namespace SlotGame.Data
{
    [CreateAssetMenu(menuName = "SlotGame/Symbol Database", fileName = "SymbolDatabase")]
    public class SymbolDatabase : ScriptableObject
    {
        [Header("All Symbols (Cherry, Bell, BAR, Lucky 7)")]
        public List<SymbolData> symbols = new List<SymbolData>();

        // ─── Weighted strip ───────────────────────────────────────────────────

        /// <summary>
        /// Builds a weighted pool and Fisher-Yates shuffles it.
        /// Cherry (w=10) → appears 10× more than Lucky 7 (w=1).
        /// </summary>
        public List<SymbolData> BuildWeightedStrip()
        {
            var pool = new List<SymbolData>();

            foreach (var sym in symbols)
            {
                int w = Mathf.Max(1, sym.reelWeight);
                for (int i = 0; i < w; i++)
                    pool.Add(sym);
            }

            // Fisher-Yates shuffle
            for (int i = pool.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (pool[i], pool[j]) = (pool[j], pool[i]);
            }

            return pool;
        }

        /// <summary>Look up a symbol by its unique ID. Returns null if missing.</summary>
        public SymbolData GetById(int id) =>
            symbols.Find(s => s.symbolId == id);
    }
}
