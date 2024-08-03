using Cysharp.Threading.Tasks;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace nunuSnowBalling.Main {


    public class MainSceneUI : BaseUI {
        [SerializeField] AssetReference MainManagerAsset;
        [SerializeField] Text Text_PlayerPT;
        [SerializeField] Text Text_AddPT;
        [SerializeField] Animator Ani_AddPT;

        //LoadingProgress MyUILoadingProgress;

        private void Start() {
            Init();
            CreateMainManager();
        }

        void CreateMainManager() {
            AddressablesLoader.GetAssetRef<GameObject>(MainManagerAsset, prefab => {
                var go = Instantiate(prefab);
                go.GetComponent<MainManager>().Init();
            });
        }

        public override void Init() {
            base.Init();
            //MyUILoadingProgress = new LoadingProgress(OnUIFinishedLoad);

        }

        /// <summary>
        /// 所有UI都載入完跑這裡
        /// </summary>
        //void OnUIFinishedLoad() {

        //}


        public override void RefreshText() {

        }
    }
}
