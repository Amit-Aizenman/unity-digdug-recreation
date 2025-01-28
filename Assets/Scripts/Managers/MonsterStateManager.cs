using System;
using UnityEngine;
using Random = System.Random;

namespace Managers
{
    public class MonsterStateManager : MonoBehaviour
    {
        private GameObject[] _monsters;
        private GameObject[] _ghostMonsters;
        public static MonsterStateManager Instance;

        private void Start()
        {
            _monsters = GameObject.FindGameObjectsWithTag("Pooka");
            _ghostMonsters = GameObject.FindGameObjectsWithTag("PookaGhost");
            foreach (var ghostMonster in _ghostMonsters)
            {
                ghostMonster.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void RollStateDice(GameObject monster, bool changeState = false)
        {
            if (changeState)
            {
                ChangeMonsterState(monster);
                return;
            }
            Random rand  = new Random();
            int dice = rand.Next(1, 7);
            if (dice == 6)
            {
                ChangeMonsterState(monster);
            }
        }
        private void ChangeMonsterState(GameObject monster)
        {
            int index = Array.IndexOf(_monsters, monster);
            if (index != -1)
            {
                Vector3 monsterPos = _monsters[index].transform.position;
                _monsters[index].SetActive(false);
                _ghostMonsters[index].SetActive(true);
                _ghostMonsters[index].transform.position = monsterPos;
            }
            else
            {
                index = Array.IndexOf(_ghostMonsters, monster);
                if (index != -1)
                {
                    Vector3 ghostMonsterPos = _ghostMonsters[index].transform.position;
                    _ghostMonsters[index].SetActive(false);
                    _monsters[index].SetActive(true);
                    _monsters[index].transform.position = ghostMonsterPos;
                }
            }
        }
    }
}
