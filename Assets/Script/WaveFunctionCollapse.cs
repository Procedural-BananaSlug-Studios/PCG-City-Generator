using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    public Tile backupTile;

    private int iteration;
    public GameObject umbrellaPrefab; // Public variable for the umbrella prefab
    public Tile curve0Tile; // set curve0 here
    public Tile curve90Tile; // set curve90 here
    public Tile curve180Tile; //    set curve180 here
    public Tile curve270Tile; // set curve270 here


    private void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();

    }

    void InitializeGrid()
    {
        float cellSize = 3.0f; // Replace with the cell size
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3(x * cellSize, 0, y * cellSize), cellObj.transform.rotation);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);

            }
        }
        StartCoroutine(CheckEntropy());
        SetNeighboringCells();
    }

    public void InitializeAdjacentGrid()
    {
        float cellSize = 3.0f;
        int newGridXOffset = dimensions;
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3((x + newGridXOffset) * cellSize, 0, y * cellSize), Quaternion.identity);
                newCell.CreateCell(true, tileObjects);  // Consider existing grid's edge tiles
                gridComponents.Add(newCell);
            }
        }
        StartCoroutine(CheckEntropy());
        SetNeighboringCells(true);  // Set neighbors considering the extended grid
    }

    void SetNeighboringCells(bool includeExistingGrid = false)
    {
        int totalDimensions = includeExistingGrid ? dimensions * 2 : dimensions;
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < totalDimensions; x++)  // Adjust to handle wider range
            {
                int index = x + y * totalDimensions;
                Cell currentCell = gridComponents[index];
                // Ensure neighbors are set correctly considering the boundary conditions
                currentCell.upNeighbor = GetNeighborCell(x, y + 1, totalDimensions);
                currentCell.downNeighbor = GetNeighborCell(x, y - 1, totalDimensions);
                currentCell.leftNeighbor = GetNeighborCell(x - 1, y, totalDimensions);
                currentCell.rightNeighbor = GetNeighborCell(x + 1, y, totalDimensions);
            }
        }
    }

    Cell GetNeighborCell(int x, int y, int totalDimensions)
    {
        if (x >= 0 && x < totalDimensions && y >= 0 && y < dimensions)
        {
            return gridComponents[x + y * totalDimensions];
        }
        return null;
    }


    IEnumerator CheckEntropy()
    {
        while (true)
        {
            List<Cell> tempGrid = new List<Cell>(gridComponents);
            tempGrid.RemoveAll(c => c.collapsed);
            if (tempGrid.Count == 0) break;  // Exit if all cells are collapsed

            tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length);
            if (tempGrid.Count == 0) yield break;  // Safety check
            tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length);


            yield return new WaitForSeconds(0.025f);

            CollapseCell(tempGrid);
        }

        if (iteration == dimensions * dimensions)
        {
            // InitializeAdjacentGrid(); // Can comment this out and only have it trigger with a button press
            DetectAndPlaceUmbrella();
        }

    }

    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        try
        {
            Tile selectedTile = WeightedTile(cellToCollapse.tileOptions);
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }
        catch
        {
            Tile selectedTile = backupTile;
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }

        Tile foundTile = cellToCollapse.tileOptions[0];
        // Rotate buildings a random direction, currently does not work due to pivot point being off-centered
        // if (foundTile.tag == "Building")
        // {
        //     System.Random random = new System.Random();
        //     Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range.Next(4) * 90, 0);
        //     foundTile.transform.rotation = foundTile.transform.rotation * randomRotation;
        // }
        Instantiate(foundTile, cellToCollapse.transform.position, foundTile.transform.rotation);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;

                if (gridComponents[index].collapsed)
                {
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }

                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in up.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].downNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (x < dimensions - 1)
                    {
                        Cell left = gridComponents[x + 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in left.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].rightNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in down.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].upNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    if (x > 0)
                    {
                        Cell right = gridComponents[x - 1 + y * dimensions];
                        List<Tile> validOptions = new List<Tile>();

                        foreach (Tile possibleOptions in right.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                            var valid = tileObjects[validOption].leftNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iteration++;

        if (iteration < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }

    //umbrella stuff that starts at bottom corner of grid so you check up and right and then up right
    void DetectAndPlaceUmbrella()
    {
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell currentCell = gridComponents[x + y * dimensions];

                // check if the current cell is collapsed and is curve180
                if (currentCell.collapsed && currentCell.tileOptions[0] == curve180Tile)
                {
                    // check if the right neighbor is available and is curve 90 
                    Cell rightNeighbor = currentCell.rightNeighbor;
                    if (rightNeighbor != null && rightNeighbor.collapsed && rightNeighbor.tileOptions[0] == curve90Tile)
                    {
                        // check if the up neighbor is available and contains curve270
                        Cell upNeighbor = currentCell.upNeighbor;
                        if (upNeighbor != null && upNeighbor.collapsed && upNeighbor.tileOptions[0] == curve270Tile)
                        {
                            // Check if the right neighbor of the up neighbor is available and contains curve0
                            Cell rightNeighborofTop = upNeighbor.rightNeighbor;
                            if (rightNeighborofTop != null && rightNeighborofTop.collapsed && rightNeighborofTop.tileOptions[0] == curve0Tile)
                            {
                                // Calculate the position of the center of the circle
                                Vector3 circleCenter = (currentCell.transform.position +
                                                        rightNeighbor.transform.position +
                                                        upNeighbor.transform.position +
                                                           rightNeighborofTop.transform.position) / 4f;

                                // instantiate the umbrella prefab at the center of the circle
                                float xOffset = 0.0f;
                                float yOffset = 0.0f; //dont need if scale is 2
                                circleCenter += new Vector3(xOffset, 0, yOffset);
                                Instantiate(umbrellaPrefab, circleCenter, Quaternion.identity);
                            }
                        }
                    }
                }
            }
        }
    }

    Tile WeightedTile(Tile[] tileOptions)
    {
        float totalWeight = tileOptions.Sum(t => t.weight);
        float randomWeight = UnityEngine.Random.Range(0, totalWeight);
        float cumulativeWeight = 0.0f;

        foreach (var tile in tileOptions)
        {
            cumulativeWeight += tile.weight;
            if (randomWeight <= cumulativeWeight)
            {
                return tile;
            }
        }

        // Fallback, should not happen
        return tileOptions[0];
    }
}