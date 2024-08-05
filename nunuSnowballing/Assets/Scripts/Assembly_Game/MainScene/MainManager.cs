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
        [SerializeField] float DefaultOdds;
        [SerializeField] float OddsAdd;
        [SerializeField] int OddsAddMiliSecs;
        [SerializeField] float RTP;
        [SerializeField] float SpdRateAdd;

        public static MainManager Instance;
        public int PlayerBet { get { return PlayerInfoUI.GetInstance<PlayerInfoUI>().CurBet; } }

        float CurOdds;
        float CurProb { get { return RTP / CurOdds; } }
        public int CurReward { get { return (int)(CurOdds * (float)PlayerBet); } }

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
                CurOdds += OddsAdd;
                MyParallaxBackground.AddSpdRate(SpdRateAdd);
                if (!RoleSlide && CurOdds > 0.8f) {
                    RoleSlide = true;
                    MyRole.SetAni("slide");
                }
                WriteLog.LogColor("CurProb=" + CurProb, WriteLog.LogType.Debug);
                if (!Prob.GetResult(CurProb)) Lose();
                await UniTask.Delay(OddsAddMiliSecs);
            }
        }
        void Lose() {
            MyRole.SetAni("jump");
            WriteHistory(false);
            EndGame();
        }

        void ResetGame() {
            RoleSlide = false;
            MyRole.SetAni("idle");
            CurOdds = DefaultOdds;
            MyParallaxBackground.ResetSpdRate();
            CurState = GameState.Betting;
            MainSceneUI.GetInstance<MainSceneUI>().RefreshUI();
        }
        void EndGame() {
            MyParallaxBackground.Stop();
            CurState = GameState.End;
            UniTask.Void(async () => {
                await UniTask.Delay(1000);
                ResetGame();
            });
        }

        public void Play() {
            if (CurState != GameState.Betting) return;
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
            GamePlayer.Instance.AddPt(CurReward);
            PlayerInfoUI.GetInstance<PlayerInfoUI>().AddPlayerPT(CurReward);
            WriteHistory(true);
            EndGame();
        }
        void WriteHistory(bool _win) {
            if (_win) {
                while (true) {
                    CurOdds += OddsAdd;
                    if (!Prob.GetResult(CurProb)) break;
                }
            }
            HistoryUI.GetInstance<HistoryUI>().Add(CurOdds);
        }




    }
}
