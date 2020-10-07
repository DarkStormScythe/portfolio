using System.Collections.Generic;
using UnityEngine;

public class World {
    // A grid of tiles with variable height and width
    Tile[,] world;

    Dictionary<string, FixedObject> fixedObjPrototypes;

    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int TileSize { get; protected set; }

    public World ( int width = 100, int height = 100, int tileSize = 1 ) {
        Width = width;
        Height = height;
        TileSize = tileSize;

        // Initialize the grid
        world = new Tile[width, height];

        // I loop through height(y) first because I prefer tiles to run horizontally, rather than vertically
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                world[x, y] = new Tile (this, x, y, TileSize);
            }
        }
    }

    protected void CreateFixedObjectPrototypes () {
        fixedObjPrototypes = new Dictionary<string, FixedObject> ();

        // Temp function
        // Will later be replaced by reading from data file
        FixedObject wallPrototype = FixedObject.CreatePrototype (
            "Wall",     // Object identifier
            0,          // Movement cost - (higher numbers mean slower movement, and 0 means this object is impassable)
            1,          // Width
            1           // Height
        );

        fixedObjPrototypes.Add ( "Wall", wallPrototype );
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if(Random.Range(0,2) == 0)
                {
                    world[x, y].Type = TileType.Empty;
                } else
                {
                    world[x, y].Type = TileType.Floor;
                }
            }
        }
    }

    public Tile GetTileAt (int x, int y) {
        if (x >= 0 && x < Width && y >= 0 && y < Height) {
            return world[x, y];
        } else {
            Debug.LogError("Tile out of range.");
            return null;
        }
    }

}
