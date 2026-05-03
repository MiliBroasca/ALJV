using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUtils
{
    public static Genome TournamentSelect(List<Genome> population, int tournamentSize)
    {
        Genome best = null;

        for (int i = 0; i < tournamentSize; i++)
        {
            Genome candidate = population[Random.Range(0, population.Count)];

            if (best == null || candidate.fitness > best.fitness)
                best = candidate;
        }

        return best;
    }

    public static Genome Crossover(Genome parentA, Genome parentB)
    {
        Genome child = new Genome();
        int cut = Random.Range(0, parentA.genes.Count);

        for (int i = 0; i < parentA.genes.Count; i++)
        {
            if (i < cut)
                child.genes.Add(parentA.genes[i]);
            else
                child.genes.Add(parentB.genes[i]);
        }

        return child;
    }

    public static void Mutate(Genome genome, float mutationRate)
    {
        for (int i = 0; i < genome.genes.Count; i++)
        {
            if (Random.value < mutationRate)
            {
                genome.genes[i] = (MoveGene)Random.Range(0, 4);
            }
        }
    }
}
