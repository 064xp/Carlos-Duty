using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnPlanes;
    [SerializeField]
    private Horde[] hordes;
    [SerializeField]
    private int currentHorde = 0;
    [SerializeField]
    private int enemiesLeft;

    public GameObject targetPlane;
    public Transform player;
    public Transform church;
    public HUDManager hud;
    public float gameOverSlowDownTime = 3f;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public MouseLook mouseLook;
    public FadeTransition fadeTransition;
    public float fadeInDuration;
    public float delayBetweenHordes;

    private void Start() {
        Time.timeScale = 1f;
        LoadHorde();
        fadeTransition.FadeIn(fadeInDuration);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
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
            AIScript.gameManager = this;
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
        print("enemy killed");
        enemiesLeft--;

        if (enemiesLeft == 0) {
            if(currentHorde == hordes.Length - 1) {
                print("Game over!");
                GameOver("¡Haz eliminado a todos los atacantes!");
            } else {
                print($"Horde {currentHorde} ended");
                currentHorde++;
                StartCoroutine(DelayedNextHorde());
            }
        }
    }

    IEnumerator DelayedNextHorde() {
        yield return new WaitForSeconds(delayBetweenHordes);
        GameObject[] equipables = GameObject.FindGameObjectsWithTag("Equipable");

        foreach(GameObject item in equipables) {
            if(item.GetComponent<Equipable>().UsedByAI)
                Destroy(item);
        }

        LoadHorde();
    }

    public void TogglePause() {
        bool isPaused = pauseMenu.activeInHierarchy || settingsMenu.activeInHierarchy;
        if (isPaused) {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            mouseLook.enabled = true;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            mouseLook.enabled = false;
        }
    }

    public void GameOver(string reason) {
        Cursor.lockState = CursorLockMode.None;
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
