using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticUtils
{
    public static Genome CreateRandomGenome(int length)
    {
        Genome genome = new Genome();

        for (int i = 0; i < length; i++)
        {
            genome.genes.Add((MoveGene)Random.Range(0, 4));
        }

        return genome;
    }

    public static List<Genome> CreatePopulation(int populationSize, int genomeLength)
    {
        List<Genome> population = new List<Genome>();

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(CreateRandomGenome(genomeLength));
        }

        return population;
    }
}
