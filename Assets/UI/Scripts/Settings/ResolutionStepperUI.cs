using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Scripts
{
    public class ResolutionStepper : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text resolutionText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;

        [Header("Settings")]
        [SerializeField] private string playerPrefsKey = "Display.Resolution";

        private List<Resolution> resolutions = new();
        private int currentIndex;

        void Start()
        {
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextResolution);
            }

            UISFXButtonHover.Ensure(nextButton);

            if (previousButton != null)
            {
                previousButton.onClick.AddListener(PreviousResolution);
            }

            UISFXButtonHover.Ensure(previousButton);

            BuildResolutionList();

            LoadSavedResolution();

            UpdateResolutionText();
        }

        void OnDestroy()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(NextResolution);
            }

            if (previousButton != null)
            {
                previousButton.onClick.RemoveListener(PreviousResolution);
            }
        }

        void BuildResolutionList()
        {
            Resolution current = Screen.currentResolution;

            float targetAspect =
                Mathf.Round((current.width / (float)current.height) * 100f) / 100f;

            // Get unique resolutions with same aspect ratio
            resolutions = Screen.resolutions
                .Where(r =>
                {
                    float aspect =
                        Mathf.Round((r.width / (float)r.height) * 100f) / 100f;

                    return Mathf.Approximately(aspect, targetAspect);
                })
                .GroupBy(r => $"{r.width}x{r.height}")
                .Select(g => g.First())
                .OrderBy(r => r.width)
                .ThenBy(r => r.height)
                .ToList();

            // Find current resolution index
            currentIndex = resolutions.FindIndex(r =>
                r.width == Screen.width &&
                r.height == Screen.height);

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }
        }

        void LoadSavedResolution()
        {
            if (!PlayerPrefs.HasKey(playerPrefsKey))
            {
                return;
            }

            string saved = PlayerPrefs.GetString(playerPrefsKey);

            string[] split = saved.Split('x');

            if (split.Length != 2)
            {
                return;
            }

            if (!int.TryParse(split[0], out int width) ||
                !int.TryParse(split[1], out int height))
            {
                return;
            }

            int index = resolutions.FindIndex(r =>
                r.width == width &&
                r.height == height);

            if (index < 0)
            {
                return;
            }

            currentIndex = index;

            ApplyResolution(false);
        }

        public void NextResolution()
        {
            if (UISFXManager.Instance != null)
                UISFXManager.Instance.PlayClick();
            currentIndex++;

            if (currentIndex >= resolutions.Count)
            {
                currentIndex = 0;
            }

            ApplyResolution();
        }

        public void PreviousResolution()
        {
            if (UISFXManager.Instance != null)
                UISFXManager.Instance.PlayClick();
            currentIndex--;

            if (currentIndex < 0)
            {
                currentIndex = resolutions.Count - 1;
            }

            ApplyResolution();
        }

        void ApplyResolution(bool save = true)
        {
            Resolution resolution = resolutions[currentIndex];

            RefreshRate refreshRate = Screen.currentResolution.refreshRateRatio.value > 0
                ? Screen.currentResolution.refreshRateRatio
                : resolution.refreshRateRatio;

            Screen.SetResolution(
                resolution.width,
                resolution.height,
                Screen.fullScreenMode,
                refreshRate
            );

            UpdateResolutionText();

            if (save)
            {
                PlayerPrefs.SetString(
                    playerPrefsKey,
                    $"{resolution.width}x{resolution.height}"
                );

                PlayerPrefs.Save();
            }
        }

        void UpdateResolutionText()
        {
            Resolution resolution = resolutions[currentIndex];

            resolutionText.text =
                $"{resolution.width}x{resolution.height}";
        }
    }
}