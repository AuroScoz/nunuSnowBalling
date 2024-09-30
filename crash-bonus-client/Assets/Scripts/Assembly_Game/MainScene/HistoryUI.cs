using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrashBonus.Main {
    public class HistoryUI : ItemSpawner_Remote<HistoryItem> {

        [SerializeField] int MaxItemCount = 10;

        public void Add(float _odds, bool _win) {
            var item = Spawn();
            string str = $"{_odds:0.00}x";
            item.SetItem(null, str);
            if (_win) item.SetImgColor(Color.yellow);
            else item.SetImgColor(Color.white);
            if (ItemList.Count > MaxItemCount) {
                RemoveItem(0);
            }
        }

        void RemoveItem(int _idx) {
            Destroy(ItemList[_idx].gameObject);
            ItemList.RemoveAt(_idx);
        }

    }
}