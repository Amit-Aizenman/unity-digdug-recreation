using System;
using System.Collections;
using Player;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static readonly int Horizontal = Animator.StringToHash("horizontal");
        private static readonly int Vertical = Animator.StringToHash("vertical");
        [SerializeField] private GameObject player;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Animator animator;
        private readonly Vector3Int _leftTile = new (0, 2);
        private readonly Vector3Int _downTile = new (0, -5);
        private readonly float _playerSpeed = 2;
        private bool _goingLeft = true;
        private bool _startedCoroutine;
        private bool _finishStarting;
        private float _timeToStart = 1.8f;
        private float _initialAnimatorSpeed;
        private bool _startedWalking;
        private Vector3 _initialPlayerPosition;
        private int _monstersCounter;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            EventManager.GameStart?.Invoke(true);
            _initialAnimatorSpeed = animator.speed;
            _initialPlayerPosition = player.transform.position;
            initializeNumberOfMonsters();
        }

       

        // Update is called once per frame
        void Update()
        {
            if (!_finishStarting)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            
            if (_goingLeft)
            {
                animator.SetFloat(Horizontal, -1);
                if (Math.Abs(player.transform.position.x - tilemap.GetCellCenterWorld(_leftTile).x) > 0.1)
                {
                    player.transform.position += new Vector3(-1, 0, 0) * (Time.deltaTime * _playerSpeed);
                    if (Math.Abs(player.transform.position.x - _initialPlayerPosition.x) > 0.05)
                    {
                        PlayStartingMusic();
                    }
                }
                else
                {
                    player.transform.position = tilemap.GetCellCenterWorld(_leftTile);
                    _goingLeft = false;
                    animator.SetFloat(Horizontal, 0);
                }
            }
            else
            {
                animator.SetFloat(Vertical, -1);
                if (Math.Abs(player.transform.position.y - tilemap.GetCellCenterWorld(_downTile).y) > 0.1)
                {
                    player.transform.position += new Vector3(0, -1, 0) * (Time.deltaTime * _playerSpeed);
                }
                else
                {
                    player.transform.position = tilemap.GetCellCenterWorld(_downTile);
                    animator.SetFloat(Horizontal, 1);
                    animator.SetFloat(Vertical, 0);
                    animator.speed = 0;
                    if (!_startedCoroutine)
                    {
                        StartCoroutine(StopStart(_timeToStart));
                        _startedCoroutine = true;
                    }
                }
            }
        }

        private void PlayStartingMusic()
        {
            if (!_startedWalking)
            {
                FindAnyObjectByType<SoundManager>().Play("gameStart");
                _startedWalking = true;
            }
        }

        private IEnumerator StopStart (float seconds)
        {
            yield return new WaitForSeconds(seconds);
            animator.SetFloat(Horizontal, 0);
            animator.speed = _initialAnimatorSpeed;
            EventManager.FinishGameStart?.Invoke(true);
        }

        private void OnEnable()
        {
            EventManager.FinishGameStart += ChangeFinishFlag;
            EventManager.MonsterKilled += CheckMonsterCount;
        }

        private void OnDisable()
        {
            EventManager.FinishGameStart -= ChangeFinishFlag;
            EventManager.MonsterKilled -= CheckMonsterCount;

        }

        private void CheckMonsterCount(bool obj)
        {
            _monstersCounter--;
            if (_monstersCounter == 0)
            {
                EventManager.FinishLevel?.Invoke(true);
                EndLevel();
            }
        }

        private void EndLevel()
        {
            FindAnyObjectByType<SoundManager>().Play("stageClear");
            StartCoroutine(FinishLevel(3.2f));
        }

        private void ChangeFinishFlag(bool obj)
        {
            _finishStarting = true;
        }
        
        private IEnumerator FinishLevel (float seconds)
        {
            yield return new WaitForSeconds(seconds);
            SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex + 1)%3);
        }
        
        private void initializeNumberOfMonsters()
        {
            var pookaArray = GameObject.FindGameObjectsWithTag("Pooka");
            foreach (var pooka in pookaArray)
            {
                if (pooka.activeInHierarchy)
                {
                    
                    _monstersCounter++;
                }
            }
        }

        
    }
}
