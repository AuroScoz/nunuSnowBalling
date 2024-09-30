using Cysharp.Threading.Tasks;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CrashBonus.Main {
    public enum GameState {
        Betting,
        Playing,
        End,
    }
    public class MainManager : MonoBehaviour {
        [SerializeField] Camera SceneCam;
        [SerializeField] int OddsAddMiliSecs;
        [SerializeField] float SpdRateAdd;

        public static MainManager Instance;
        float RTP = 0.95f;
        float defaultOdds = 1f;
        float oddsAdd = 0.01f;
        float adjustCoefficient = 2;


        public int PlayerBet { get { return PlayerInfoUI.GetInstance<PlayerInfoUI>().CurBet; } }
        float curOdds;
        public int curReward { get { return (int)(curOdds * (float)PlayerBet); } }

        public GameState CurState { get; private set; } = GameState.Betting;

        public void Init() {
            Instance = this;
            new GamePlayer();
            ResetGame();
            AddCamStack(UICam.Instance.MyCam);
        }



        /// <summary>
        /// 將指定camera加入到MyCam的CameraStack中
        /// </summary>
        void AddCamStack(Camera _cam) {
            //因為場景的攝影機有分為場景與UI, 要把場景攝影機設定為Base, UI設定為Overlay, 並在BaseCamera中加入Camera stack
            UICam.Instance.SetRendererMode(CameraRenderType.Overlay);
            if (_cam == null) return;
            var cameraData = SceneCam.GetUniversalAdditionalCameraData();
            if (cameraData == null) return;
            cameraData.cameraStack.Add(_cam);
        }

        async UniTask PlayLoop(float _resultOdds) {
            var ui = MainSceneUI.GetInstance<MainSceneUI>();
            float targetOdds = PlayerInfoUI.GetInstance<PlayerInfoUI>().TargetOdds;
            if (_resultOdds == 0) {
                Lose();
                return;
            }
            float epsilon = 0.001f;//浮點精度問題處理
            while (CurState == GameState.Playing) {
                if (Mathf.Abs(curOdds - targetOdds) < epsilon || curOdds > targetOdds) {
                    Win();
                    break;
                } else if (Mathf.Abs(curOdds - _resultOdds) < epsilon || curOdds > _resultOdds) {
                    Lose();
                    break;
                }
                curOdds += oddsAdd;
                ui.SetCurOddsText(curOdds);

                await UniTask.Delay(OddsAddMiliSecs);
            }
        }

        void ResetGame() {
            curOdds = defaultOdds;
            CurState = GameState.Betting;
            MainSceneUI.GetInstance<MainSceneUI>().RefreshUI();
        }
        void EndGame(bool _win) {
            HistoryUI.GetInstance<HistoryUI>().Add(curOdds, _win);
            CurState = GameState.End;
            UniTask.Void(async () => {
                await UniTask.Delay(1000);
                ResetGame();
            });
        }

        void Test() {
            float playerPT = 1000000;
            float curPT = playerPT;
            int count = 1000;
            float curRTP = RTP;
            float totalBet = 0;
            float totalWin = 0;
            int getTargetOddsCount = 0;
            float targetOdds = PlayerInfoUI.GetInstance<PlayerInfoUI>().TargetOdds;
            for (int i = 0; i < count; i++) {
                curPT -= 1;
                totalBet += 1;
                var weightTable = GetWeightTable(curRTP, targetOdds);
                //foreach (var key in weightTable.Keys) {
                //    WriteLog.Log($"{key}:{weightTable[key]}");
                //}
                var resultOdds = Prob.GetRndTKeyFromWeightDic(weightTable);
                if (resultOdds == targetOdds) {
                    curPT += resultOdds;
                    totalWin += resultOdds;
                    getTargetOddsCount++;
                }
                curRTP = totalWin / totalBet;
            }
            WriteLog.LogError($"curRTP={curRTP} totalBet={totalBet} totalWin={totalWin} getTargetOddsProb={(float)getTargetOddsCount / (float)count}");
        }

        public void Play() {
            if (CurState != GameState.Betting) return;
            float targetOdds = PlayerInfoUI.GetInstance<PlayerInfoUI>().TargetOdds;
            var weightTable = GetWeightTable(GamePlayer.Instance.PlayerRTP, targetOdds);
            float resultOdds = Prob.GetRndTKeyFromWeightDic(weightTable);
            GamePlayer.Instance.AddPt(-PlayerBet);
            GamePlayer.Instance.AddTotalBet(PlayerBet);
            PlayerInfoUI.GetInstance<PlayerInfoUI>().AddPlayerPT(-PlayerBet);
            CurState = GameState.Playing;
            MainSceneUI.GetInstance<MainSceneUI>().SetResultOdds(resultOdds);
            UniTask.Void(async () => await PlayLoop(resultOdds));
        }

        public void Win() {
            if (CurState != GameState.Playing) return;
            GamePlayer.Instance.AddPt(curReward);
            GamePlayer.Instance.AddTotalWin(curReward);
            PlayerInfoUI.GetInstance<PlayerInfoUI>().AddPlayerPT(curReward);
            EndGame(true);
        }
        void Lose() {
            EndGame(false);
        }
        Dictionary<float, int> GetWeightTable(float _rtp, float _targetOdds) {
            float targetRTP = RTP + (RTP - _rtp) * (adjustCoefficient / _targetOdds);
            Dictionary<float, int> weightTable = new Dictionary<float, int>();
            int weightBase = 1000000;
            float failProb = Mathf.Clamp(1 - targetRTP, 0, 1);
            //WriteLog.Log("failProb=" + failProb);
            int failWeight = Mathf.RoundToInt(failProb * weightBase);
            weightTable.Add(0, failWeight);
            float targetProb = Mathf.Clamp(targetRTP / _targetOdds, 0, 1);
            //WriteLog.Log("targetProb=" + targetProb);
            int targetWeight = Mathf.RoundToInt(targetProb * weightBase);
            int step = Mathf.RoundToInt((_targetOdds - defaultOdds) / oddsAdd) - 1;
            float avgProb = (1 - failProb - targetProb) / step;
            int avgWeight = Mathf.RoundToInt(avgProb * weightBase);
            float curOdds = defaultOdds;
            for (int i = 0; i < step; i++) {
                curOdds += oddsAdd;
                curOdds = MyMath.Round(curOdds, 2);
                weightTable.Add(curOdds, avgWeight);
            }
            weightTable.Add(_targetOdds, targetWeight);
            return weightTable;
        }



    }
}
