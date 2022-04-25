using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnPlanes;
    [SerializeField]
    private Horde[] hordes;
    private int currentHorde = 0;
    private int enemiesLeft;

    public GameObject targetPlane;
    public Transform player;
    public Transform church;
    public HUDManager hud;
    public float gameOverSlowDownTime = 3f;

    private void Start() {
        //LoadHorde();
    }

    IEnumerator SpawnEnemies() {
        FindRandomPoint findRandomSpawnPoint = new FindRandomPoint();
        FindRandomPoint findRandomTarget = new FindRandomPoint(targetPlane);
        Horde horde = hordes[currentHorde];
        float delay;

        for(int i=0; i<horde.totalEnemies; i++) {
            // Generate random spawn point
            int planeIndex = Random.Range(0, spawnPlanes.Length);
            findRandomSpawnPoint.SetPlane(spawnPlanes[planeIndex]);
            Vector3 spawnPoint = findRandomSpawnPoint.CalculateRandomPoint();
            Vector3 navigationTarget = findRandomTarget.CalculateRandomPoint();

            Enemy toSpawn = WeightedRandomChoice.RandomChoice<Enemy>(horde.enemies);
            GameObject enemyPrefab = toSpawn.enemyPrefab;
            EnemyAI AIScript = enemyPrefab.GetComponent<EnemyAI>();

            // Initialize & Instantiate enemy
            AIScript.enemySettings = toSpawn;
            AIScript.navigationTarget = navigationTarget;
            AIScript.player = player;
            AIScript.shootingTarget = church;
            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);


            delay = Random.Range(
                horde.spawnFrequency.minValue,
                horde.spawnFrequency.maxValue
             );

            yield return new WaitForSeconds(delay);
        }
    }

    void LoadHorde() {
        print($"Horde {currentHorde} starting");
        enemiesLeft = hordes[currentHorde].totalEnemies;
        StartCoroutine(SpawnEnemies());
    }

    public void NotifyEnemyKilled() {
        enemiesLeft--;

        if (enemiesLeft == 0) {
            if(currentHorde == hordes.Length - 1) {
                print("Game over!");
            } else {
                print($"Horde {currentHorde} ended");
                currentHorde++;
                LoadHorde();
            }
        }
    }

    public void GameOver(string reason) {
        hud.OnGameOver(reason);
        player.GetComponent<PlayerController>().enabled = false;
        player.Find("FPSCamera").GetComponent<MouseLook>().enabled = false;
        StartCoroutine(LerpTimeScaleTo(0.0f));
    }

    public void OnChurchDestroyed() {
        GameOver("¡La iglesia fue infiltrada!");
    }

    IEnumerator LerpTimeScaleTo(float value) {
        float elapsedTime = 0f;
        float initialTimeScale = Time.timeScale;

        while(elapsedTime <= gameOverSlowDownTime) {
            elapsedTime += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(initialTimeScale, value, elapsedTime / gameOverSlowDownTime);
            yield return null;
        }
    }
}
