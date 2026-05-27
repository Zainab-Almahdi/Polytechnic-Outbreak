using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.UI.Scripts
{
    public class DisplayManager : MonoBehaviour
    {
        [Header("Post Processing")]
        [SerializeField] private Volume postProcessVolume;

        void Start()
        {
            PlayerStorageHandler.EnsureInitialized();
            ApplySettingsFromPrefs();
        }

        void ApplySettingsFromPrefs()
        {
            ApplyResolutionFromPrefs();
            ApplyVsyncFromPrefs();
            ApplyPostProcessFromPrefs();
            ApplyBrightnessFromPrefs();
        }

        void ApplyResolutionFromPrefs()
        {
            if (!PlayerPrefs.HasKey(PlayerStorageHandler.ResolutionKey))
            {
                return;
            }

            string saved = PlayerPrefs.GetString(PlayerStorageHandler.ResolutionKey);
            string[] split = saved.Split('x');

            if (split.Length != 2 ||
                !int.TryParse(split[0], out int width) ||
                !int.TryParse(split[1], out int height))
            {
                return;
            }

            Resolution[] resolutions = Screen.resolutions;
            int index = Array.FindIndex(
                resolutions,
                r => r.width == width && r.height == height
            );

            if (index < 0)
            {
                return;
            }

            Resolution resolution = resolutions[index];
            RefreshRate refreshRate = Screen.currentResolution.refreshRateRatio.value > 0
                ? Screen.currentResolution.refreshRateRatio
                : resolution.refreshRateRatio;

            Screen.SetResolution(
                resolution.width,
                resolution.height,
                Screen.fullScreenMode,
                refreshRate
            );
        }

        void ApplyVsyncFromPrefs()
        {
            int enabled = PlayerPrefs.GetInt(
                PlayerStorageHandler.VsyncKey,
                QualitySettings.vSyncCount > 0 ? 1 : 0
            );

            QualitySettings.vSyncCount = enabled != 0 ? 1 : 0;
        }

        void ApplyPostProcessFromPrefs()
        {
            if (postProcessVolume == null || postProcessVolume.profile == null)
            {
                return;
            }

            if (postProcessVolume.profile.TryGet(out MotionBlur motionBlur))
            {
                motionBlur.active = PlayerPrefs.GetInt(PlayerStorageHandler.MotionBlurKey, 1) != 0;
            }

            if (postProcessVolume.profile.TryGet(out FilmGrain filmGrain))
            {
                filmGrain.active = PlayerPrefs.GetInt(PlayerStorageHandler.FilmGrainKey, 1) != 0;
            }

            if (postProcessVolume.profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                chromaticAberration.active =
                    PlayerPrefs.GetInt(PlayerStorageHandler.ChromaticAberrationKey, 1) != 0;
            }
        }

        void ApplyBrightnessFromPrefs()
        {
            if (postProcessVolume == null || postProcessVolume.profile == null)
            {
                return;
            }

            if (!postProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                return;
            }

            float brightness = PlayerPrefs.GetFloat(PlayerStorageHandler.BrightnessKey, colorAdjustments.postExposure.value);
            colorAdjustments.postExposure.value = brightness;
        }
    }
}
