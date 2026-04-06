using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform spawnLeft;
    public Transform spawnRight;
    public Transform spawnUp;
    public Transform spawnDown;

    void Start()
    {
        string entry = GameManager.instance.entryPoint;

        Transform spawnPoint = spawnLeft;

        if (entry == "left") spawnPoint = spawnLeft;
        if (entry == "right") spawnPoint = spawnRight;
        if (entry == "up") spawnPoint = spawnUp;
        if (entry == "down") spawnPoint = spawnDown;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = spawnPoint.position;
    }
}