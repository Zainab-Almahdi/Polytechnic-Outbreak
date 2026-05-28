using UnityEngine;

public class PlayerIKHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerWeapons playerWeapons;

    [Header("IK Settings")]
    [Range(0, 1)] public float rightHandWeight = 1f;
    [Range(0, 1)] public float leftHandWeight = 1f;

    [Header("Hand Offsets (Relative to Weapon)")]
    public Vector3 rightHandPosOffset = new Vector3(-0.05f, -0.05f, -0.1f);
    public Vector3 rightHandRotOffset = new Vector3(0, 0, 0);
    public Vector3 leftHandPosOffset = new Vector3(0.05f, -0.05f, 0.2f);
    public Vector3 leftHandRotOffset = new Vector3(0, 0, 0);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerWeapons = GetComponentInParent<PlayerWeapons>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || playerWeapons == null) return;

        var weaponInstance = playerWeapons.GetEquippedWeapon();
        if (weaponInstance == null || weaponInstance.SpawnedObject == null || !weaponInstance.SpawnedObject.activeInHierarchy)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            return;
        }

        Transform weaponTransform = weaponInstance.SpawnedObject.transform;

        // Right Hand
        Vector3 rPos = weaponTransform.TransformPoint(rightHandPosOffset);
        Quaternion rRot = weaponTransform.rotation * Quaternion.Euler(rightHandRotOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rPos);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rRot);

        // Left Hand
        Vector3 lPos = weaponTransform.TransformPoint(leftHandPosOffset);
        Quaternion lRot = weaponTransform.rotation * Quaternion.Euler(leftHandRotOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, lPos);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, lRot);
    }
}
