using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using SimpleJSON;
using System.Reflection;

namespace nunuSnowBalling.Main {
    public class JsonEquip : JsonBase {
        public static string DataName { get; set; }
        public string Name {
            get {
                return JsonString.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public int Odds { get; private set; }
        public string Ref { get; private set; }
        public string AttackParticle { get; private set; }

        static Dictionary<int, List<JsonEquip>> EquipDic = new Dictionary<int, List<JsonEquip>>();

        protected override void SetDataFromJson(JsonData _item) {
            JsonData item = _item;
            //反射屬性
            var myData = JsonMapper.ToObject<JsonEquip>(item.ToJson());
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties()) {
                if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                    //下面這行如果報錯誤代表上方的sonMapper.ToObject<XXXXX>(item.ToJson());<---XXXXX忘了改目前Class名稱
                    var value = propertyInfo.GetValue(myData, null);
                    propertyInfo.SetValue(this, value, null);
                }
            }
            if (EquipDic.ContainsKey(myData.Odds)) EquipDic[myData.Odds].Add(myData);
            else EquipDic.Add(myData.Odds, new List<JsonEquip>() { myData });
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
            EquipDic.Clear();
        }
        public static JsonEquip GetRndEquip(int _odds) {
            if (!EquipDic.ContainsKey(_odds)) {
                WriteLog.LogErrorFormat("不包含此賠率的裝備:{0}", _odds);
                return null;
            }
            var rndJsonEquip = Prob.GetRandomTFromTList(EquipDic[_odds].ToList());
            return rndJsonEquip;
        }
    }

}
