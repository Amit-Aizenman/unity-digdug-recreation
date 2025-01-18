//using DefaultNamespace;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject hookPrefab;
    private GameObject _currentHook;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchHook();
        }
    }
    void LaunchHook()
        {
            if (_currentHook == null) 
            {
                _currentHook = Instantiate(hookPrefab, transform.position, Quaternion.identity);
                Hook hookComponent = _currentHook.GetComponent<Hook>();
                    //hookComponent.Initialize(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), OnHookReturn);
            }
        }

        void OnHookReturn()
        {
            Destroy(_currentHook);
            _currentHook = null;
        }
    }

