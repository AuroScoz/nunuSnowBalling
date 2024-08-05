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
        [SerializeField] HistoryUI MyHistoryUI;
        [SerializeField] PlayerInfoUI MyPlayerInfoUI;
        [SerializeField] Button PlayBtn;
        [SerializeField] Button RewardBtn;

        LoadingProgress MyUILoadingProgress;

        private void Start() {
            Init();
        }

        public override void Init() {
            base.Init();
            MyUILoadingProgress = new LoadingProgress(OnUIFinishedLoad);
            MyUILoadingProgress.AddLoadingProgress("MainManager");
            AddressablesLoader.GetAssetRef<GameObject>(MainManagerAsset, prefab => {
                var go = Instantiate(prefab);
                go.GetComponent<MainManager>().Init();
                MyUILoadingProgress.FinishProgress("MainManager");
            });


            MyUILoadingProgress.AddLoadingProgress("HistoryUI");
            MyHistoryUI.Init();
            MyHistoryUI.LoadItemAsset(() => {
                MyUILoadingProgress.FinishProgress("HistoryUI");
            });
            MyPlayerInfoUI.Init();
        }

        /// <summary>
        /// 所有UI都載入完跑這裡
        /// </summary>
        void OnUIFinishedLoad() {
            RefreshUI();
        }


        public override void RefreshText() {

        }

        public void OnPlayClick() {
            MainManager.Instance.Play();
            RefreshUI();
        }
        public void OnRewardClick() {
            MainManager.Instance.GetReward();
            RefreshUI();
        }
        public void RefreshUI() {
            PlayBtn.gameObject.SetActive(MainManager.Instance.CurState == GameState.Betting);
            RewardBtn.gameObject.SetActive(MainManager.Instance.CurState == GameState.Playing);
        }
    }
}
