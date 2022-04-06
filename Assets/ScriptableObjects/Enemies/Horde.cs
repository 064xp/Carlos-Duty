using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Enemies/Horde")]
public class Horde : ScriptableObject {
    public WeightedValue<Enemy>[] enemies;
    public RangedFloat spawnFrequency;
    public int totalEnemies;
}
