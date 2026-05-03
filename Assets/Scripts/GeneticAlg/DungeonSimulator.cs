using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class DungeonSimulator
{
    private CellType[,] originalGrid;
    private int width;
    private int height;
    private Vector2Int startPosition;
    private int startHealth;

    public DungeonSimulator(CellType[,] grid, Vector2Int startPos, int health)
    {
        originalGrid = CopyGrid(grid);
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        startPosition = startPos;
        startHealth = health;
    }

    public SimulationResult EvaluateGenome(Genome genome)
    {
        CellType[,] simGrid = CopyGrid(originalGrid);

        SimState state = new SimState
        {
            position = startPosition,
            health = startHealth,
            score = 0,
            steps = 0,
            dead = false,
            reachedGoal = false
        };

        foreach (MoveGene gene in genome.genes)
        {
            Vector2Int dir = GeneToDirection(gene);
            Vector2Int newPos = state.position + dir;

            if (!IsInsideGrid(newPos))
            {
                state.score -= 1;
                continue; // Ignore moves that go out of bounds
            }

            state.position = newPos;
            state.steps++;

            CellType cell = simGrid[newPos.x, newPos.y];

            switch(cell)
            {
                case CellType.Reward:
                    state.score += 10;
                    simGrid[newPos.x, newPos.y] = CellType.Empty; // Consume reward
                    break;

                case CellType.Trap:
                    state.score -= 5;
                    state.health -= 10;
                    break;
                case CellType.Enemy:
                    state.health -= 20;
                    if (state.health > 0)
                    {
                        state.score += 15;
                        simGrid[newPos.x, newPos.y] = CellType.Empty; // Defeat enemy
                    }
                    break;
                case CellType.Boss:
                    state.health -= 40;
                    if (state.health > 0)
                    {
                        state.score += 50;
                        state.reachedGoal = true; // Boss is the goal
                    }
                    break;
                case CellType.Exit:
                    state.reachedGoal = true;
                    state.score += 50;
                    break;
                case CellType.Door:
                    state.score += 25; // Bonus for passing through doors
                    state.reachedGoal = true; // Assume door leads to goal for simplicity
                    break;
            }

            if (state.health <= 0)
            {
                state.dead = true;
                break;
            }

            if (state.reachedGoal)
            {
                break;
            }
        }

        return BuildResult(state);
    }

    private SimulationResult BuildResult(SimState state)
    {
        float fitness = state.score;
        fitness -= state.steps * 0.5f; // Penalize for more steps
        if (state.reachedGoal)
        {
            fitness += 100; // Bonus for reaching goal
        }
        if (state.dead)
        {
            fitness -= 100; // Penalty for dying
        }
        else
        {
            fitness += state.health * 0.2f; // Bonus for remaining health
        }

        return new SimulationResult
        {
            fitness = fitness,
            finalScore = state.score,
            remainingHealth = state.health,
            stepsUsed = state.steps,
            died = state.dead,
            reachedGoal = state.reachedGoal
        };
    }

    private Vector2Int GeneToDirection(MoveGene gene)
    {
        switch(gene)
        {
            case MoveGene.Up: return Vector2Int.up;
            case MoveGene.Down: return Vector2Int.down;
            case MoveGene.Left: return Vector2Int.left;
            case MoveGene.Right: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private CellType[,] CopyGrid(CellType[,] source)
    {
        int w = source.GetLength(0);
        int h = source.GetLength(1);
        CellType[,] copy = new CellType[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                copy[x, y] = source[x, y];

        return copy;
    }
}
