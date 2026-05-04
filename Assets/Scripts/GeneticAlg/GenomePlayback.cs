using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenomePlayback : MonoBehaviour
{
    public Player player;
    public float stepDelay = 0.5f;
    private Coroutine playbackCoroutine;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (player != null)
            player.arrivedAtBoss += OnArrivedAtBoss;
    }

    private void OnDisable()
    {
        if (player != null)
            player.arrivedAtBoss -= OnArrivedAtBoss;
    }

    public void PlayGenome(List<MoveGene> genes)
    {
        playbackCoroutine = StartCoroutine(PlayGenomeRoutine(genes));
    }

    private IEnumerator PlayGenomeRoutine(List<MoveGene> genes)
    {
        foreach (MoveGene gene in genes)
        {
            yield return new WaitForSeconds(stepDelay);
            Vector2Int dir = GeneToDirection(gene);
            player.MoveFromGenome(dir);
        }
    }

    private Vector2Int GeneToDirection(MoveGene gene)
    {
        switch (gene)
        {
            case MoveGene.Up: return Vector2Int.up;
            case MoveGene.Down: return Vector2Int.down;
            case MoveGene.Left: return Vector2Int.left;
            case MoveGene.Right: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    private void OnArrivedAtBoss()
    {
        Debug.Log("Arrived at boss, stopping genome playback.");
        if (playbackCoroutine != null)
            StopCoroutine(playbackCoroutine);
    }
}