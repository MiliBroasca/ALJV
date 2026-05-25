using UnityEngine;

public enum MapVariant
{
    Original,
    Variant2,
}

public class MapConfig : MonoBehaviour
{
    public static MapConfig Instance { get; private set; }

    public MapVariant selectedMapVariant = MapVariant.Original;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}