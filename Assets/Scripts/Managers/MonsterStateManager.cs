using System;
using UnityEngine;
using Random = System.Random;

namespace Managers
{
    public class MonsterStateManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] monsters;
        [SerializeField] private GameObject[] ghostMonsters;
        public static MonsterStateManager Instance;

        private void Start()
        {
            foreach (var ghostMonster in ghostMonsters)
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
            int index = Array.IndexOf(monsters, monster);
            if (index != -1)
            {
                Vector3 monsterPos = monsters[index].transform.position;
                monsters[index].SetActive(false);
                ghostMonsters[index].SetActive(true);
                ghostMonsters[index].transform.position = monsterPos;
            }
            else
            {
                index = Array.IndexOf(ghostMonsters, monster);
                if (index != -1)
                {
                    Vector3 ghostMonsterPos = ghostMonsters[index].transform.position;
                    ghostMonsters[index].SetActive(false);
                    monsters[index].SetActive(true);
                    monsters[index].transform.position = ghostMonsterPos;
                }
            }
        }
    }
}
