using System.Collections.Generic;

[System.Serializable]
public class Genome
{
    public List<MoveGene> genes = new List<MoveGene>();
    public float fitness;
}