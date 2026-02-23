using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject playerPrefab;
    public Transform spawnPointA;
    public Transform spawnPointB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(playerPrefab, spawnPointA.position, Quaternion.identity);
        Instantiate(playerPrefab, spawnPointB.position, Quaternion.identity);
    }
}