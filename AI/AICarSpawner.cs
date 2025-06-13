using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AICarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] aiCarPrefabs; // Array of AI car prefabs to spawn

    GameObject[] aiCarPool = new GameObject[20];

    Transform playerCarTransform;

    //Timing

    WaitForSeconds wait = new WaitForSeconds(0.5f); // Wait time for coroutine

    float lastCarSpawnTime = 0; // Last time a car was spawned

    // Collision detection
    [SerializeField]
    LayerMask otherCarsLayerMask; // Layer mask for detecting other cars

    Collider[] hitColliders = new Collider[1]; // Array to hold detected colliders

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCarTransform = GameObject.FindWithTag("Player").transform;

        int prefabIndex = 0;

        for (int i = 0; i < aiCarPool.Length; i++)
        {
            // Instantiate AI car from the prefab array
            aiCarPool[i] = Instantiate(aiCarPrefabs[prefabIndex]);
            aiCarPool[i].SetActive(false); // Initially deactivate the car
            // Cycle through the prefabs
            prefabIndex = (prefabIndex + 1) % aiCarPrefabs.Length;
        }

        StartCoroutine(UpdateLessOftenCO());
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            CleanupCarsBeyondView(); // Clean up cars that are beyond the player's view
            SpawnNewCars(); // Call the method to spawn new cars
            yield return wait; // Wait for the specified time
        }
    }

    void SpawnNewCars()
    {
        if (Time.time - lastCarSpawnTime < 2) 
            return; // Prevent spawning too frequently

        GameObject carToSpawn = null;

        foreach (GameObject car in aiCarPool)
        {
            //Skip active cars
            if (car.activeInHierarchy) 
                continue;

            carToSpawn = car; // Get the first inactive car
            break;
        }

        if (carToSpawn == null) 
            return;

        Vector3 spawnPosition = new Vector3(0, 0, playerCarTransform.position.z + 100);

        if (Physics.OverlapBoxNonAlloc(spawnPosition, Vector3.one * 2, hitColliders, Quaternion.identity, otherCarsLayerMask) > 0)
            return; // Check for collisions with other cars at the spawn position

        carToSpawn.transform.position = spawnPosition; // Set the spawn position
        carToSpawn.SetActive(true);

        lastCarSpawnTime = Time.time; // Update the last spawn time
    }

    void CleanupCarsBeyondView()
    {
        foreach (GameObject car in aiCarPool)
        {
            if (!car.activeInHierarchy) 
                continue; // Skip inactive cars
            // Check if the car is beyond the player's view
            if (car.transform.position.z - playerCarTransform.position.z > 200)
            {
                car.SetActive(false); // Deactivate the car
            }
            if (car.transform.position.z - playerCarTransform.position.z < -50)
            {
                car.SetActive(false); // Deactivate the car if it's too far behind
            }
        }
    }
}
