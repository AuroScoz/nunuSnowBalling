using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scoz.Func;
namespace nunuSnowBalling.Main {
    public class PlayerInfoUI : BaseUI {

        [SerializeField] Dropdown BetDropDown;
        [SerializeField] Text Text_PlayerPT;
        [SerializeField] Text Text_AddPT;
        [SerializeField] Animator Ani_AddPT;


        List<int> BetTypes;
        public int CurBet { get; private set; }

        protected override void OnEnable() {
            base.OnEnable();
        }

        public override void RefreshText() {
            Text_PlayerPT.text = GamePlayer.Instance.Pt.ToString();
        }

        public override void Init() {
            base.Init();
            SetBetDropDown();
            RefreshText();
        }


        public void AddPlayerPT(int _value) {
            if (_value == 0) return;
            string aniTrigger = "add";
            if (_value < 0) {
                aniTrigger = "reduce";
            }
            Ani_AddPT.SetTrigger(aniTrigger);
            if (_value > 0) Text_AddPT.text = "+" + _value.ToString();
            else Text_AddPT.text = _value.ToString();
            RefreshText();
        }

        void SetBetDropDown() {
            BetDropDown.ClearOptions();
            BetTypes = new List<int>() { 10, 20, 30, 50, 100, 200 };
            List<Dropdown.OptionData> dropDwonDatas = new List<Dropdown.OptionData>();
            var betOptions = BetTypes.ConvertAll(a => a.ToString());
            BetDropDown.AddOptions(betOptions);
            CurBet = BetTypes[0];
        }

        public void DropdownValueChanged(Dropdown change) {
            CurBet = BetTypes[change.value];
        }
    }
}