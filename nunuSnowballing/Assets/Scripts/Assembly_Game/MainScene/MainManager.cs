using Cysharp.Threading.Tasks;
using nunuSnowBalling.Socket.Matchgame;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace nunuSnowBalling.Main {
    public enum GameState {
        Betting,
        Playing,
        End,
    }
    public class MainManager : MonoBehaviour {
        [SerializeField] Camera SceneCam;
        [SerializeField] ParallaxBackground MyParallaxBackground;
        [SerializeField] Role MyRole;
        [SerializeField] float OddsAdd;
        [SerializeField] int OddsAddMiliSecs;
        [SerializeField] float RTP;
        [SerializeField] float SpdRateAdd;

        float defaultOdds = 1f;
        public static MainManager Instance;
        public int PlayerBet { get { return PlayerInfoUI.GetInstance<PlayerInfoUI>().CurBet; } }

        float curOdds;
        public int curReward { get { return (int)(curOdds * (float)PlayerBet); } }
        float resultOdds;

        bool RoleSlide = false;

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

        async UniTask PlayLoop() {
            while (CurState == GameState.Playing) {

                MyParallaxBackground.AddSpdRate(SpdRateAdd);
                if (!RoleSlide && curOdds > 1.2f) {
                    RoleSlide = true;
                    MyRole.SetAni("slide");
                }
                curOdds += OddsAdd;
                if (curOdds > resultOdds) Lose();

                await UniTask.Delay(OddsAddMiliSecs);
            }
        }
        void Lose() {
            MyRole.SetAni("jump");
            EndGame();
        }

        void ResetGame() {
            RoleSlide = false;
            MyRole.SetAni("idle");
            curOdds = defaultOdds;
            MyParallaxBackground.ResetSpdRate();
            CurState = GameState.Betting;
            MainSceneUI.GetInstance<MainSceneUI>().RefreshUI();
        }
        void EndGame() {
            HistoryUI.GetInstance<HistoryUI>().Add(curOdds);
            MyParallaxBackground.Stop();
            CurState = GameState.End;
            UniTask.Void(async () => {
                await UniTask.Delay(1000);
                ResetGame();
            });
        }

        void Test() {
            float playerPT = 100000;
            float curPT = playerPT;
            float count = 10;
            float winRate = 0.5f;
            float curRTP = 0.95f;
            float totalBet = 0;
            float totalWin = 0;
            for (int i = 0; i < count; i++) {
                curPT -= 1;
                totalBet += 1;
                resultOdds = GetResultOdds(curRTP);
                if (Prob.GetResult(winRate)) {
                    curPT += resultOdds;
                    totalWin += resultOdds;
                }
                curRTP = totalWin / totalBet;
            }
            WriteLog.LogError("curRTP=" + curRTP);
        }

        public void Play() {
            if (CurState != GameState.Betting) return;
            Test();
            return;

            WriteLog.LogError("resultOdds=" + resultOdds);
            MyRole.SetAni("walk");
            MyParallaxBackground.Play();
            GamePlayer.Instance.AddPt(-PlayerBet);
            PlayerInfoUI.GetInstance<PlayerInfoUI>().AddPlayerPT(-PlayerBet);
            CurState = GameState.Playing;
            UniTask.Create(PlayLoop);
        }

        public void GetReward() {
            if (CurState != GameState.Playing) return;
            MyRole.SetAni("idle");
            GamePlayer.Instance.AddPt(curReward);
            PlayerInfoUI.GetInstance<PlayerInfoUI>().AddPlayerPT(curReward);
            EndGame();
        }
        float GetResultOdds(float _curRTP) {
            resultOdds = defaultOdds;
            float tmpOdds = defaultOdds;
            while (true) {
                var prob = RTP * tmpOdds / (tmpOdds + OddsAdd);
                tmpOdds += OddsAdd;
                if (prob >= 0.99) {
                    WriteLog.LogError($"prob={prob} _curRTP={_curRTP} tmpOdds={tmpOdds}");
                    break;
                }
                if (!Prob.GetResult(prob)) break;
                else resultOdds = tmpOdds;
            }
            return resultOdds;
        }




    }
}
