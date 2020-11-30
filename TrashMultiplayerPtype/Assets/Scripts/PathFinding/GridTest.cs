using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest
{
    private int width;
    private int height;
    private int depth;
    //private Vector3 cellSize;

    private int[,,] gridArray;

    public GridTest(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        //this.cellSize = cellSize;

        gridArray = new int[width, height, depth];

        for (int x=0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    
                }
            }
        }
    }
}
