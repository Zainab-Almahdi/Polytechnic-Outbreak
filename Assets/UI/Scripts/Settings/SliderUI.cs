using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.UI.Scripts
{
    public class SliderUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private string playerPrefsKey;
        [SerializeField] private float saveDelay = 0.25f;

        private Coroutine saveCoroutine;

        void Start()
        {
            if (slider == null)
            {
                Debug.LogWarning($"SliderUI: slider is not assigned on '{gameObject.name}'.");
                return;
            }

            slider.wholeNumbers = true;

            if (!string.IsNullOrWhiteSpace(playerPrefsKey) &&
                PlayerPrefs.HasKey(playerPrefsKey))
            {
                slider.value = PlayerPrefs.GetInt(
                    playerPrefsKey,
                    Mathf.RoundToInt(slider.value)
                );
            }

            UpdateValueText(Mathf.RoundToInt(slider.value));
        }

        void OnEnable()
        {
            if (slider != null)
            {
                slider.onValueChanged.AddListener(HandleSliderValueChanged);
            }
        }

        void OnDisable()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
            }
        }

        void OnDestroy()
        {
            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
                saveCoroutine = null;
            }
        }

        void HandleSliderValueChanged(float value)
        {
            int intValue = Mathf.RoundToInt(value);

            UpdateValueText(intValue);

            if (string.IsNullOrWhiteSpace(playerPrefsKey))
            {
                return;
            }

            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
            }

            saveCoroutine = StartCoroutine(SaveAfterDelay(intValue));
        }

        void UpdateValueText(int value)
        {
            if (valueText != null)
            {
                valueText.text = value.ToString();
            }
        }

        IEnumerator SaveAfterDelay(int value)
        {
            yield return new WaitForSecondsRealtime(saveDelay);

            PlayerPrefs.SetInt(playerPrefsKey, value);
            PlayerPrefs.Save();
            Debug.Log($"SliderUI: Saved '{playerPrefsKey}' with value {value}.");
            Debug.Log($"SliderUI: Verified '{playerPrefsKey}' with value {PlayerPrefs.GetInt(playerPrefsKey, value)}.");
            saveCoroutine = null;
        }
    }
}