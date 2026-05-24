using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticAlgRunner : MonoBehaviour
{
    public int populationSize = 50;
    public int genomeLength = 80;
    public int generations = 50;
    public float mutationRate = 0.08f;
    public int eliteCount = 2;

    public RoomManager roomManager;
    public GenomePlayback genomePlayback;

    private void OnEnable()
    {
        if (roomManager != null)
            roomManager.generatedGrid += StartAlg;
    }

    private void OnDisable()
    {
        if (roomManager != null)
            roomManager.generatedGrid -= StartAlg;
    }

    public void StartAlg()
    {
        Vector2Int startPos = new Vector2Int(0, 0);
        string startRoom = "RoomA";

        DungeonSimulator simulator = new DungeonSimulator(startRoom, startPos, 100);
        List<Genome> population = GeneticUtils.CreatePopulation(populationSize, genomeLength);

        Genome bestEver = null;
        List<GenerationLogEntry> generationStats = new List<GenerationLogEntry>();

        for (int generation = 0; generation < generations; generation++)
        {
            foreach (Genome genome in population)
            {
                SimulationResult result = simulator.EvaluateGenome(genome);
                genome.fitness = result.fitness;
            }

            population = population.OrderByDescending(g => g.fitness).ToList();

            if (bestEver == null || population[0].fitness > bestEver.fitness)
                bestEver = CloneGenome(population[0]);

            Debug.Log($"Generation {generation}: Best fitness = {population[0].fitness}");

            generationStats.Add(new GenerationLogEntry
            {
                generation = generation,
                bestFitness = population[0].fitness
            });

            List<Genome> nextPopulation = new List<Genome>();

            for (int i = 0; i < eliteCount; i++)
                nextPopulation.Add(CloneGenome(population[i]));

            while (nextPopulation.Count < populationSize)
            {
                Genome parentA = SelectionUtils.TournamentSelect(population, 3);
                Genome parentB = SelectionUtils.TournamentSelect(population, 3);

                Genome child = SelectionUtils.Crossover(parentA, parentB);
                SelectionUtils.Mutate(child, mutationRate);

                nextPopulation.Add(child);
            }

            population = nextPopulation;
        }

        Debug.Log("Best ever fitness: " + bestEver.fitness);
        DebugBestGenome(bestEver);

        SimulationResult bestResult = simulator.EvaluateGenome(bestEver);
        SaveLog(bestEver, bestResult, generationStats, startRoom);

        if (genomePlayback != null)
            genomePlayback.PlayGenome(bestEver.genes);
    }

    private void SaveLog(Genome bestEver, SimulationResult bestResult, List<GenerationLogEntry> generationStats, string startRoom)
    {
        GALog log = new GALog
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            startRoom = startRoom,
            populationSize = populationSize,
            genomeLength = genomeLength,
            generations = generations,
            mutationRate = mutationRate,
            eliteCount = eliteCount,

            bestFitness = bestEver.fitness,
            finalScore = bestResult.finalScore,
            remainingHealth = bestResult.remainingHealth,
            stepsUsed = bestResult.stepsUsed,
            died = bestResult.died,
            reachedGoal = bestResult.reachedGoal,

            bestGenome = bestEver.genes.Select(g => g.ToString()).ToList(),
            generationStats = generationStats
        };

        GAJsonLogger.SaveRunLog(log);
    }

    private Genome CloneGenome(Genome original)
    {
        return new Genome
        {
            fitness = original.fitness,
            genes = new List<MoveGene>(original.genes)
        };
    }

    private void DebugBestGenome(Genome genome)
    {
        string path = string.Join(", ", genome.genes);
        Debug.Log("Best genome: " + path);
    }
}
