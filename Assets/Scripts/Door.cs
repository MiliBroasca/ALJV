using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string nextScene;
    public string entryPoint; // "left", "right", "up", "down"

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger hit: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered door!");

            PlayerPrefs.SetString("EntryPoint", entryPoint);
            GridManager.instance.grid = null; // Clear grid to force reinitialization in new scene
            SceneManager.LoadScene("RoomB");
        }
    }
}
