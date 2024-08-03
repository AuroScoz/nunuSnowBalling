using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using SimpleJSON;
using System.Reflection;
using Unity.VisualScripting;

namespace nunuSnowBalling.Main {
    public class JsonMap : JsonBase {
        public static string DataName { get; set; }
        public int MinionWeight { get; private set; }
        public int EliteWeight { get; private set; }
        public int BossWeight { get; private set; }
        public int ShopWeight { get; private set; }
        public string Bets { get; private set; }


        public List<int> BetList { get; private set; }

        public enum EventType {
            Minion,
            Elite,
            Boss,
            Shop,
        }
        Dictionary<EventType, int> EventWeights = new Dictionary<EventType, int>();

        protected override void SetDataFromJson(JsonData _item) {
            JsonData item = _item;
            //反射屬性
            var myData = JsonMapper.ToObject<JsonMap>(item.ToJson());
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties()) {
                if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                    //下面這行如果報錯誤代表上方的JsonMapper.ToObject<XXXXX>(item.ToJson());<---XXXXX忘了改目前Class名稱
                    var value = propertyInfo.GetValue(myData, null);
                    propertyInfo.SetValue(this, value, null);
                }
            }
            EventWeights.Add(EventType.Minion, MinionWeight);
            EventWeights.Add(EventType.Elite, EliteWeight);
            EventWeights.Add(EventType.Boss, BossWeight);
            EventWeights.Add(EventType.Shop, ShopWeight);
            BetList = TextManager.StringSplitToIntList(Bets, ',');

            //自定義屬性
            //foreach (string key in item.Keys) {
            //    switch (key) {
            //        case "ID":
            //            ID = int.Parse(item[key].ToString());
            //            break;
            //        default:
            //            WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
            //            break;
            //    }
            //}
        }
        protected override void ResetStaticData() {
        }

        /// <summary>
        /// 依據權重取得隨機事件
        /// </summary>
        public EventType GetRndEvent() {
            if (EventWeights.Count == 0) {
                WriteLog.LogError("EventWeights為空");
                return EventType.Minion;
            }
            return Prob.GetRndTKeyFromWeightDic(EventWeights);
        }


    }

}
