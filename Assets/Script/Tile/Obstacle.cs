using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int Hp=10;
    public Tile CurrentTile;
    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }
    
}
