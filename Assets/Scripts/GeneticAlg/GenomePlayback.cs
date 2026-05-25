using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenomePlayback : MonoBehaviour
{
    [Header("Playback")]
    public Player player;
    public float stepDelay = 0.7f;
    private Coroutine playbackCoroutine;

    [Header("Optional JSON Input")]
    public TextAsset genomeJsonFile;
    public bool playFromJsonOnStart = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (playFromJsonOnStart)
        {
            PlayGenomeFromJson();
        }
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
        if (playbackCoroutine != null)
            StopCoroutine(playbackCoroutine);

        playbackCoroutine = StartCoroutine(PlayGenomeRoutine(genes));
    }

    public void PlayGenomeFromJson()
    {
        if (genomeJsonFile == null)
        {
            Debug.LogWarning("No genome JSON file assigned.");
            return;
        }

        GALog runLog = JsonUtility.FromJson<GALog>(genomeJsonFile.text);

        if (runLog == null || runLog.bestGenome == null || runLog.bestGenome.Count == 0)
        {
            Debug.LogWarning("JSON does not contain a valid bestGenome.");
            return;
        }

        List<MoveGene> genes = ConvertStringsToGenes(runLog.bestGenome);
        PlayGenome(genes);
    }

    private List<MoveGene> ConvertStringsToGenes(List<string> geneStrings)
    {
        List<MoveGene> genes = new List<MoveGene>();

        foreach (string geneStr in geneStrings)
        {
            if (Enum.TryParse(geneStr, out MoveGene gene))
            {
                genes.Add(gene);
            }
            else
            {
                Debug.LogWarning("Invalid gene string in JSON: " + geneStr);
            }
        }

        return genes;
    }

    private IEnumerator PlayGenomeRoutine(List<MoveGene> genes)
    {
        foreach (MoveGene gene in genes)
        {
            yield return new WaitForSeconds(stepDelay / 2);

            Vector2Int dir = GeneToDirection(gene);

            if (player == null)
                player = FindObjectOfType<Player>();

            if (player != null)
                player.MoveFromGenome(dir);

            yield return new WaitForSeconds(stepDelay / 2);
        }

        playbackCoroutine = null;
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
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }
    }
}