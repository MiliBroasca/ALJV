using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSimulator
{
    private readonly int width = 7;
    private readonly int height = 7;

    private string startRoom;
    private Vector2Int startPosition;
    private int startHealth;

    private Dictionary<string, CellType[,]> dungeonRooms = new Dictionary<string, CellType[,]>();


    public DungeonSimulator(string startRoom, Vector2Int startPos, int health)
    {
        this.startRoom = startRoom;
        this.startPosition = startPos;
        this.startHealth = health;

        // Define Rooms
        dungeonRooms["RoomA"] = RoomManager.RoomA();
        dungeonRooms["RoomB"] = RoomManager.RoomB();
        dungeonRooms["RoomC"] = RoomManager.RoomC();
        dungeonRooms["RoomD"] = RoomManager.RoomD();
        dungeonRooms["RoomE"] = RoomManager.RoomE();
        dungeonRooms["RoomF"] = RoomManager.RoomF();
        dungeonRooms["RoomG"] = RoomManager.RoomG();
        dungeonRooms["RoomH"] = RoomManager.RoomH();
        dungeonRooms["RoomI"] = RoomManager.RoomI();
        dungeonRooms["RoomJ"] = RoomManager.RoomJ();
        dungeonRooms["RoomK"] = RoomManager.RoomK();
        dungeonRooms["RoomL"] = RoomManager.RoomL();
    }

    public SimulationResult EvaluateGenome(Genome genome)
    {
        Dictionary<string, CellType[,]> simRooms = CopyRooms(dungeonRooms);

        SimState state = new SimState
        {
            currentRoom = startRoom,
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
                state.score -= 2;
                continue; // Ignore moves that go out of bounds
            }

            state.position = newPos;
            state.steps++;

            CellType[,] currentRoomGrid = simRooms[state.currentRoom];
            CellType cell = currentRoomGrid[newPos.x, newPos.y];

            switch(cell)
            {
                case CellType.Reward:
                    state.score += 10;
                    currentRoomGrid[newPos.x, newPos.y] = CellType.Empty; // Consume reward
                    break;

                case CellType.Trap:
                    state.score -= 5;
                    break;
                case CellType.Enemy:
                    // state.health -= Random.Range(5, 15);
                    state.health -= 10; // Fixed damage for simplicity
                    if (state.health > 0)
                    {
                        state.score += 15;
                        currentRoomGrid[newPos.x, newPos.y] = CellType.Empty; // Defeat enemy
                    }
                    break;
                case CellType.Boss:
                    // state.health -= Random.Range(10, 30);
                    state.health -= 20; // Fixed damage for simplicity
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
                    HandleDoorTransition(state, newPos);
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

    private void HandleDoorTransition(SimState state, Vector2Int doorPos)
    {
        string nextRoom = GridManager.instance.GetNextScene(state.currentRoom, doorPos);

        if (!string.IsNullOrEmpty(nextRoom))
        {
            state.score += 10;
            state.currentRoom = nextRoom;
            state.position = new Vector2Int(0, 0); // Reset position for new room
        }
        else
        {
            state.score -= 10;
        }
    }

    private Dictionary<string, CellType[,]> CopyRooms(Dictionary<string, CellType[,]> dungeonRooms)
    {
        return new Dictionary<string, CellType[,]>
        {
            { "RoomA", CopyGrid(dungeonRooms["RoomA"]) },
            { "RoomB", CopyGrid(dungeonRooms["RoomB"]) },
            { "RoomC", CopyGrid(dungeonRooms["RoomC"]) },
            { "RoomD", CopyGrid(dungeonRooms["RoomD"]) },
            { "RoomE", CopyGrid(dungeonRooms["RoomE"]) },
            { "RoomF", CopyGrid(dungeonRooms["RoomF"]) },
            { "RoomG", CopyGrid(dungeonRooms["RoomG"]) },
            { "RoomH", CopyGrid(dungeonRooms["RoomH"]) },
            { "RoomI", CopyGrid(dungeonRooms["RoomI"]) },
            { "RoomJ", CopyGrid(dungeonRooms["RoomJ"]) },
            { "RoomK", CopyGrid(dungeonRooms["RoomK"]) },
            { "RoomL", CopyGrid(dungeonRooms["RoomL"]) }
        };
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
