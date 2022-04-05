using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Enemies/FiringPattern")]
public class FiringPattern : ScriptableObject {
    public RangedFloat fireTime;
    public RangedFloat cooldownTime;
    public RangedFloat inaccuracy;
}
