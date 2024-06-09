using System.Collections;
using UnityEngine;

namespace TrophyRace.Architecture {
    public sealed class Coroutines : MonoBehaviour {

        private static Coroutines instance {
            get {
                if(_instance == null) {
                    var go = new GameObject("[COROUTINE MANAGER]");
                    _instance = go.AddComponent<Coroutines>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private static Coroutines _instance;

        public static Coroutine StartRoutine(IEnumerator enumerator) {
            return instance.StartCoroutine(enumerator);
        }

        public static void StopRoutine(Coroutine routine) {
            if(routine != null) {
                instance.StopCoroutine(routine);
            }
        }
    }
}