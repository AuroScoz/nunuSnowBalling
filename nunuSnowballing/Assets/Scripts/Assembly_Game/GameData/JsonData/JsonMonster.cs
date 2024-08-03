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
    public class JsonMonster : JsonBase {
        public static string DataName { get; set; }
        public string Name {
            get {
                return JsonString.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public int Odds { get; private set; }
        public string Ref { get; private set; }
        public string MonsterType { get; private set; }
        public string AttackParticle { get; private set; }
        public enum MType {
            Minion,
            Elite,
            Boss,
        }

        static Dictionary<MType, List<JsonMonster>> TypeMonsterDic = new Dictionary<MType, List<JsonMonster>>();


        protected override void SetDataFromJson(JsonData _item) {
            JsonData item = _item;
            //反射屬性
            var myData = JsonMapper.ToObject<JsonMonster>(item.ToJson());
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties()) {
                if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                    //下面這行如果報錯誤代表上方的sonMapper.ToObject<XXXXX>(item.ToJson());<---XXXXX忘了改目前Class名稱
                    var value = propertyInfo.GetValue(myData, null);
                    propertyInfo.SetValue(this, value, null);
                }
            }
            if (MyEnum.TryParseEnum(myData.MonsterType, out MType type)) {
                if (TypeMonsterDic.ContainsKey(type)) TypeMonsterDic[type].Add(myData);
                else TypeMonsterDic[type] = new List<JsonMonster>() { myData };
            }

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
            TypeMonsterDic.Clear();
        }

        /// <summary>
        /// 取得該類怪物隨機一隻怪物
        /// </summary>
        public static JsonMonster GetRndMonster(MType _type) {
            if (!TypeMonsterDic.ContainsKey(_type)) return null;
            return Prob.GetRandomTFromTList(TypeMonsterDic[_type]);
        }
    }

}
