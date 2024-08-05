using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nunuSnowBalling.Main {
    public class HistoryUI : ItemSpawner_Remote<HistoryItem> {

        [SerializeField] int MaxItemCount = 10;

        public void Add(float _odds) {
            var item = Spawn();
            string str = $"{_odds:0.00}x";
            item.SetItem(null, str);
            if (_odds < 1.2f) item.SetImgColor(Color.white);
            else item.SetImgColor(Color.yellow);
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