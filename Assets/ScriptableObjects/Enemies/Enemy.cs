using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Enemies/Enemy")]
public class Enemy : ScriptableObject {
    public int health = 100;
    public GameObject enemyPrefab;
    public GameObject gunPrefab;
    public FiringPattern firingPattern;
    public int updateFrequency = 1;
    public float shootDistance = 25f;
    public float followDistance = 125f;
}
