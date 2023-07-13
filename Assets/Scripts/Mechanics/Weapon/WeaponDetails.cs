using UnityEngine;

[CreateAssetMenu(fileName ="Defualt Weapon", menuName ="Create Weapon")]
public class WeaponDetails : ScriptableObject
{
    [Space]
    public float Range;
    public float DelayToNexyShot;
    public int MaxAmmo;
    [Space]
    public AudioClip ShotSound;
}
