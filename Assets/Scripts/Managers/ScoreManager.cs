using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {

        private int _score; // Current score
        [SerializeField] private Sprite[] numberSprites; // Array of sprites for digits (0-9)
        [SerializeField] private GameObject digitPrefab; // Prefab for displaying each digit
        [SerializeField] private Transform scoreContainer; // Parent object to hold digit images
        public static ScoreManager Instance;

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

        void Start()
        {
            UpdateScoreDisplay(); // Display the initial score
        }

        public void UpdateScoreDisplay()
        {
            // Clear existing digits from the container
            foreach (Transform child in scoreContainer)
            {
                Destroy(child.gameObject);
            }

            // Convert the score to a string (e.g., 123 -> "123")
            string scoreString = _score == 0? "00" : _score.ToString();

            // Loop through each digit in the string and create corresponding digit objects
            foreach (char digitChar in scoreString)
            {
                int digit = int.Parse(digitChar.ToString()); // Convert character to integer

                // Instantiate a new digit object from the prefab
                GameObject newDigit = Instantiate(digitPrefab, scoreContainer);

                // Set the sprite for the digit
                newDigit.GetComponent<Image>().sprite = numberSprites[digit];
            }
        }

        public void AddScore(int amount)
        {
            _score += amount; // Update the score
            UpdateScoreDisplay(); // Refresh the display
        }
    }
}
