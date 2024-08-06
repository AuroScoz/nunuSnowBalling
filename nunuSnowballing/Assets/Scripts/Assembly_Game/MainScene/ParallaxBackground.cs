using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nunuSnowBalling.Main {
    public class ParallaxBackground : MonoBehaviour {

        [System.Serializable]
        public class ParallaxLayer {
            public Transform[] ImgTrans;
            public float Spd;
            public float BGWidth;
            public int FirstIdx { get; private set; }
            public void SetFirstIdx(int _idx) {
                FirstIdx = _idx;
            }
            public void UpdateFirstIdx() {
                float rightmostX = float.MinValue;
                for (int i = 0; i < ImgTrans.Length; i++) {
                    if (ImgTrans[i].localPosition.x > rightmostX) {
                        rightmostX = ImgTrans[i].localPosition.x;
                        SetFirstIdx(i);
                    }
                }
            }
        }
        public float SpdRate { get; private set; } = 1;
        public ParallaxLayer[] layers;
        public bool Playing { get; private set; } = false;
        private void Start() {
            foreach (var layer in layers) {
                layer.UpdateFirstIdx();
            }
        }

        public void Play() {
            Playing = true;
            WriteLog.LogError("Play");
        }
        public void Stop() {
            Playing = false;
            WriteLog.LogError("Stop");
        }

        public void ResetSpdRate() {
            SpdRate = 1;
        }
        public void AddSpdRate(float _value) {
            SpdRate += _value;
        }

        void Update() {
            if (!Playing) return;
            foreach (var layer in layers) {
                for (int i = 0; i < layer.ImgTrans.Length; i++) {
                    var layerTrans = layer.ImgTrans[i];
                    layerTrans.Translate(Vector3.left * layer.Spd * SpdRate * Time.deltaTime);

                    if (layerTrans.localPosition.x <= -layer.BGWidth) {
                        layerTrans.localPosition = new Vector3(layer.ImgTrans[layer.FirstIdx].localPosition.x + layer.BGWidth, layerTrans.localPosition.y, layerTrans.localPosition.z);
                        layer.SetFirstIdx(i);
                    }
                }
            }
        }
    }
}