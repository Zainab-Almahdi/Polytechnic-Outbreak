using System;
using UnityEngine;

namespace Assets.UI.Scripts
{
    public class PlayerStorageHandler
    {
        public const string InitializedKey = "PlayerStorageInitialized";

        public const string MoveForwardKey = "Keybind.MoveForward";
        public const string MoveBackwardKey = "Keybind.MoveBackward";
        public const string MoveLeftKey = "Keybind.MoveLeft";
        public const string MoveRightKey = "Keybind.MoveRight";
        public const string ShootKey = "Keybind.Shoot";
        public const string ReloadKey = "Keybind.Reload";
        public const string InteractKey = "Keybind.Interact";
        public const string MouseSensitivityKey = "Keybind.MouseSensitivity";
        public const string SwitchWeaponKey = "Keybind.SwitchWeapon";
        public const string SprintKey = "Keybind.Sprint";

        public const string GameDifficultyKey = "Difficulty.GameDifficulty";
        public const string FriendlyFireKey = "Difficulty.FriendlyFire";
        public const string PermaDeathKey = "Difficulty.PermaDeath";

        public const string ResolutionKey = "Display.Resolution";
        public const string VsyncKey = "Display.Vsync";
        public const string MotionBlurKey = "Display.MotionBlur";
        public const string FilmGrainKey = "Display.FilmGrain";
        public const string ChromaticAberrationKey = "Display.ChromaticAberration";
        public const string BrightnessKey  = "Display.Brightness";

        public static float GetMouseSensitivity()
        {
            // Sensitivity is stored as an int (default 50). 
            // We map this to a usable multiplier (e.g. 50 -> 1.0, 100 -> 2.0).
            return PlayerPrefs.GetInt(MouseSensitivityKey, 50) / 50f;
        }

        public static void EnsureInitialized()
        {
            if (PlayerPrefs.HasKey(InitializedKey))
            {
                return;
            }

            PlayerPrefs.SetString(MoveForwardKey, KeyCode.W.ToString());
            PlayerPrefs.SetString(MoveBackwardKey, KeyCode.S.ToString());
            PlayerPrefs.SetString(MoveLeftKey, KeyCode.A.ToString());
            PlayerPrefs.SetString(MoveRightKey, KeyCode.D.ToString());
            PlayerPrefs.SetString(ShootKey, KeyCode.Mouse0.ToString());
            PlayerPrefs.SetString(ReloadKey, KeyCode.R.ToString());
            PlayerPrefs.SetString(InteractKey, KeyCode.E.ToString());
            PlayerPrefs.SetInt(MouseSensitivityKey, 50);
            PlayerPrefs.SetString(SwitchWeaponKey, KeyCode.Q.ToString());
            PlayerPrefs.SetString(SprintKey, KeyCode.LeftShift.ToString());

            PlayerPrefs.SetString(GameDifficultyKey, "medium");
            PlayerPrefs.SetInt(FriendlyFireKey, 0);
            PlayerPrefs.SetInt(PermaDeathKey, 0);

            PlayerPrefs.SetString(ResolutionKey, "1920x1080");
            PlayerPrefs.SetInt(VsyncKey, 1);
            PlayerPrefs.SetInt(MotionBlurKey, 0);
            PlayerPrefs.SetInt(FilmGrainKey, 0);
            PlayerPrefs.SetInt(ChromaticAberrationKey, 0);
            PlayerPrefs.SetFloat(BrightnessKey, 0f);

            PlayerPrefs.SetInt(InitializedKey, 1);
            PlayerPrefs.Save();
        }
    }
}
