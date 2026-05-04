using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimState
{
    public string currentRoom;
    public Vector2Int position;
    public int health;
    public int score;
    public int steps;
    public bool dead;
    public bool reachedGoal;
}
