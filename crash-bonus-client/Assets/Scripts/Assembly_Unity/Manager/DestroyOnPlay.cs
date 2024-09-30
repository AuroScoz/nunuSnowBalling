using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scoz.Func {
    /// <summary>
    /// 加了這個的物件會在遊戲播放後被移除
    /// </summary>
    public class DestroyOnPlay : MonoBehaviour {
        private void Awake() {
            DestroyImmediate(gameObject);
        }
    }
}