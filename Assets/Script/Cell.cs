using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Tile[] tileOptions;
    public Cell upNeighbor;
    public Cell downNeighbor;
    public Cell leftNeighbor;
    public Cell rightNeighbor;

    public void CreateCell(bool collapsedState, Tile[] tiles)
    {
        collapsed = collapsedState;
        tileOptions = tiles;
    }

    public void RecreateCell(Tile[] tiles)
    {
        tileOptions = tiles;
    }
}