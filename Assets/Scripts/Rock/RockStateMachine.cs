using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

namespace Rock
{

    public enum RockState
    {
        Idle,
        Wiggling,
        Falling,
        Breaking,
    }

    public class RockStateMachine : MonoBehaviour
    {
        private RockState _currentState;


        private static readonly int StartFalling = Animator.StringToHash("StartFalling");
        private static readonly int Falling = Animator.StringToHash("Falling");
        private static readonly int Breaking = Animator.StringToHash("Breaking");
        public static float RockSpeed = 5;
        private float _currentSpeed = 0;
        [SerializeField] private float wigglingTime = 1.5f;
        private bool _timeToWiggleStarted = false;
        [SerializeField] private Tilemap dugTilemap;
        [SerializeField] private Tilemap sandTilemap;
        [SerializeField] private Animator animator;
        private Vector3Int _initialTile;
        private bool _keepFalling = true;
        private bool _coroutineStarted;
        private bool _collided;
        private bool _didBreakSound;


        void Start()
        {
            _currentState = RockState.Idle;
            _initialTile = dugTilemap.WorldToCell(transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            switch (_currentState)
            {
                case RockState.Idle:
                    HandleIdleState();
                    break;
                case RockState.Wiggling:
                    HandleWigglingState();
                    break;
                case RockState.Falling:
                    HandleFallingState();
                    break;
                case RockState.Breaking:
                    HandleBreakingState();
                    break;
            }

        }

        private void HandleIdleState()
        {
            if (dugTilemap.HasTile(_initialTile + new Vector3Int(0, -1, 0)))
            {
                animator.SetBool(StartFalling, true);
                ChangeState(RockState.Wiggling);
            }
        }

        private void HandleWigglingState()
        {
            if (!_timeToWiggleStarted)
            {
                _timeToWiggleStarted = true;
                StartCoroutine(WigglingTime());
            }
        }

        private void HandleFallingState()
        {
            if (!_collided)
            {
                transform.position += Vector3.down * (Time.deltaTime * RockSpeed);
                if (!sandTilemap.HasTile(sandTilemap.WorldToCell(transform.position)))
                {
                    _collided = true;
                }
            }
            else
            {
                animator.SetBool(Falling, false);
                animator.SetBool(Breaking, true);
                ChangeState(RockState.Breaking);
            }
        }

        private void HandleBreakingState()
        {
            if (!_didBreakSound)
            {
                FindAnyObjectByType<SoundManager>().Play("rockBreaking");
                _didBreakSound = true;
            }
            EventManager.RockStoppedFalling?.Invoke(true);
            Destroy(gameObject,1.5f);
        }

        private void ChangeState(RockState newState)
        {
            _currentState = newState;
        }

        private IEnumerator WigglingTime()
        {
            yield return new WaitForSeconds(wigglingTime);
            animator.SetBool(StartFalling, false);
            animator.SetBool(Falling, true);
            ChangeState(RockState.Falling);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("SandTile") && sandTilemap.WorldToCell(transform.position) != _initialTile)
            {
                _collided = true;
            }

            if (other.gameObject.CompareTag("Player") && _currentState == RockState.Falling)
            {
                EventManager.PlayerHitByRock?.Invoke(true);
            }

            if (other.gameObject.CompareTag("Pooka") && _currentState == RockState.Falling)
            {
                EventManager.MonsterHitByRock?.Invoke(other.gameObject);
            }
        }
    }
}
