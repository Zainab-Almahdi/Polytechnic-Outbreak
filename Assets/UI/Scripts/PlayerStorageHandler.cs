using System;
using UnityEngine;

namespace Assets.UI.Scripts
{
    internal class PlayerStorageHandler
    {
        private const string InitializedKey = "PlayerStorageInitialized";

        private const string MoveForwardKey = "Keybind.MoveForward";
        private const string MoveBackwardKey = "Keybind.MoveBackward";
        private const string MoveLeftKey = "Keybind.MoveLeft";
        private const string MoveRightKey = "Keybind.MoveRight";
        private const string ShootKey = "Keybind.Shoot";
        private const string ReloadKey = "Keybind.Reload";
        private const string InteractKey = "Keybind.Interact";
        private const string MouseSensitivityKey = "Keybind.MouseSensitivity";
        private const string SwitchWeaponKey = "Keybind.SwitchWeapon";
        private const string SprintKey = "Keybind.Sprint";

        private const string GameDifficultyKey = "Difficulty.GameDifficulty";
        private const string FriendlyFireKey = "Difficulty.FriendlyFire";
        private const string PermaDeathKey = "Difficulty.PermaDeath";

        private const string ResolutionKey = "Display.Resolution";
        private const string VsyncKey = "Display.Vsync";
        private const string MotionBlurKey = "Display.MotionBlur";
        private const string FilmGrainKey = "Display.FilmGrain";
        private const string ChromaticAberrationKey = "Display.ChromaticAberration";
        private const string BrightnessKey  = "Display.Brightness";

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
