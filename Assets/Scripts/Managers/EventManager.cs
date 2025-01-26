using System;
using UnityEngine;

namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        public static Action<GameObject> HitMonster;
        public static Action<int> PlayerKeepHitting;
        public static Action<bool> PlayerGotHit;
        public static Action<bool> PlayerStopHitting;
        public static Action<bool> MonsterKilled;
    }
}