using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    public Tile[] upNeighbour;
    public Tile[] downNeighbour;
    public Tile[] leftNeighbour;
    public Tile[] rightNeighbour;

    public Tile[] upLeftNeighbour;
    public Tile[] upRightNeighbour;
    public Tile[] downLeftNeighbour;
    public Tile[] downRightNeighbour;

    public void PrintNeighbors()
    {
        Debug.Log("Up Neighbors:");
        foreach (Tile tile in upNeighbour)
        {
            Debug.Log(tile.name);
        }

        Debug.Log("Down Neighbors:");
        foreach (Tile tile in downNeighbour)
        {
            Debug.Log(tile.name);
        }

        Debug.Log("Left Neighbors:");
        foreach (Tile tile in leftNeighbour)
        {
            Debug.Log(tile.name);
        }

        Debug.Log("Right Neighbors:");
        foreach (Tile tile in rightNeighbour)
        {
            Debug.Log(tile.name);
        }
    }



}
