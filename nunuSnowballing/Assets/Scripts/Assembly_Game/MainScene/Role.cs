using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nunuSnowBalling.Main {


    public class Role : MonoBehaviour {
        [SerializeField] Animator RoleAni;

        public void SetAni(string _triggerName) {
            RoleAni.SetTrigger(_triggerName);
        }
    }
}