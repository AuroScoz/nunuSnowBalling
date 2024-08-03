using Cysharp.Threading.Tasks;
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
        public int PlayerBet { get; private set; }

        float CurOdds;
        float CurProb { get { return RTP / CurOdds; } }
        public long CurReward { get { return (long)(CurOdds * (float)PlayerBet); } }

        bool RoleSlide = false;

        public GameState CurState { get; private set; } = GameState.Betting;

        private void OnEnable() {

        }

        private void Start() {
            Init();
        }

        public void Init() {
            Instance = this;
            PlayerBet = 10;
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

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Q)) {
                Play();
            } else if (Input.GetKeyDown(KeyCode.W)) {
                GetReward();
            }

        }

        async UniTask PlayLoop() {
            while (CurState == GameState.Playing) {
                CurOdds += OddsAdd;
                MyParallaxBackground.AddSpdRate(SpdRateAdd);
                if (!RoleSlide && MyParallaxBackground.SpdRate > 1.3f) {
                    RoleSlide = true;
                    MyRole.SetAni("slide");
                }
                WriteLog.LogColor("CurProb=" + CurProb, WriteLog.LogType.Debug);
                //if (!Prob.GetResult(CurProb)) {
                //    ResetGame();
                //}
                await UniTask.Delay(OddsAddMiliSecs);
            }
        }


        void ResetGame() {
            MyRole.SetAni("idle");
            CurOdds = 1;
            MyParallaxBackground.Stop();
            MyParallaxBackground.ResetSpdRate();
            CurState = GameState.End;
        }

        public void Play() {
            MyRole.SetAni("walk");
            MyParallaxBackground.Play();
            GamePlayer.Instance.AddPt(-PlayerBet);
            CurState = GameState.Playing;
            UniTask.Create(PlayLoop);
        }

        public void GetReward() {
            GamePlayer.Instance.AddPt(CurReward);
            WriteLog.LogError($"贏得{CurReward}金幣");
            ResetGame();
        }



    }
}
