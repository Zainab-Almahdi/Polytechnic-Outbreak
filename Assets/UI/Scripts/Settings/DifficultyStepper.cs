using UnityEngine;

namespace Assets.UI.Scripts
{
    public class DifficultyStepper : MonoBehaviour
    {
        [SerializeField]
        private string[] difficulties = new[] { "Easy", "Medium", "Hard" };
        [SerializeField]
        private string playerPrefsKey = "Difficulty.GameDifficulty";
        [SerializeField]
        private TMPro.TMP_Text difficultyText;
        [SerializeField]
        private UnityEngine.UI.Button nextButton;
        [SerializeField]
        private UnityEngine.UI.Button previousButton;


        private int currentIndex;

        void Start()
        {
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextDifficulty);
            }

            UISFXButtonHover.Ensure(nextButton);

            if (previousButton != null)
            {
                previousButton.onClick.AddListener(PreviousDifficulty);
            }

            UISFXButtonHover.Ensure(previousButton);

            LoadSavedDifficulty();
            UpdateDifficultyText();
        }

        void OnDestroy()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(NextDifficulty);
            }

            if (previousButton != null)
            {
                previousButton.onClick.RemoveListener(PreviousDifficulty);
            }
        }

        void LoadSavedDifficulty()
        {
            if (!PlayerPrefs.HasKey(playerPrefsKey))
            {
                currentIndex = 0;
                return;
            }

            string saved = PlayerPrefs.GetString(playerPrefsKey);
            int index = System.Array.FindIndex(
                difficulties,
                difficulty =>
                    string.Equals(difficulty, saved, System.StringComparison.OrdinalIgnoreCase)
            );

            currentIndex = index >= 0 ? index : 0;
        }

        public void NextDifficulty()
        {
            if (UISFXManager.Instance != null)
                UISFXManager.Instance.PlayClick();
            currentIndex++;

            if (currentIndex >= difficulties.Length)
            {
                currentIndex = 0;
            }

            ApplyDifficulty();
        }

        public void PreviousDifficulty()
        {
            if (UISFXManager.Instance != null)
                UISFXManager.Instance.PlayClick();
            currentIndex--;

            if (currentIndex < 0)
            {
                currentIndex = difficulties.Length - 1;
            }

            ApplyDifficulty();
        }

        void ApplyDifficulty()
        {
            UpdateDifficultyText();

            PlayerPrefs.SetString(playerPrefsKey, difficulties[currentIndex]);
            PlayerPrefs.Save();
        }

        void UpdateDifficultyText()
        {
            if (difficultyText == null || difficulties.Length == 0)
            {
                return;
            }

            currentIndex = Mathf.Clamp(currentIndex, 0, difficulties.Length - 1);
            difficultyText.text = difficulties[currentIndex];
        }
    }
}
