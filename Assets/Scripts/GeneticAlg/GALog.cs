using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenerationLogEntry
{
    public int generation;
    public float bestFitness;
}

[Serializable]
public class GALog
{
    public string algorithm = "GeneticAlgorithm";
    public string mapVariant;
    public string timestamp;
    public string startRoom;
    public int populationSize;
    public int genomeLength;
    public int generations;
    public float mutationRate;
    public int eliteCount;

    public float bestFitness;
    public int finalScore;
    public int remainingHealth;
    public int stepsUsed;
    public bool died;
    public bool reachedGoal;

    public List<string> bestGenome;
    public List<GenerationLogEntry> generationStats;
}
