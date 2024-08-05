using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace nunuSnowBalling.Main {
    public class HistoryItem : MonoBehaviour, IItem {
        [SerializeField] Image MyImg;
        [SerializeField] Text MyText;

        public bool IsActive { get; set; }

        public void SetItem(Sprite _sprite, string _text) {
            MyImg.sprite = _sprite;
            MyText.text = _text;
            gameObject.SetActive(true);
        }
        public void SetImgColor(Color _color) {
            MyImg.color = _color;
        }
    }
}