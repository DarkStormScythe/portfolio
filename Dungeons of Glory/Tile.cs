using System;
using UnityEngine;

/// <summary>
/// In this project, the data and visual layers are being kept separate from each other
/// The data layer should not have any link to the visual layer, or at least as much as possible
/// That way, changes in either layer would not break the other due to changes to said layer
/// </summary>

// The tile's current type
public enum TileType { Empty, Floor };

public class Tile {

    TileType type = TileType.Empty;

    World world;
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public int TileSize { get; set; }

    FixedObject fixedObject;
    LooseObject looseObject;

    Action<Tile> cbTileTypeChanged;

    /// <summary>
    /// Alternate Constructor
    /// The constructor above sets the position of the tiles at their creation
    /// However, it does not result in the specific positioning that I prefer
    /// As such, we'll use this empty constructor instead and set the position of the tiles elsewhere
    /// </summary>
 
    public Tile ( World world, int x, int y, int tileSize ) {
        this.world = world;     // A World instance
        this.X = x;
        this.Y = y;
        this.TileSize = tileSize;
    }

    public TileType Type {
        get { return type; }
        set {
            TileType oldType = type;
            type = value; 

            if (cbTileTypeChanged != null && oldType != type ) {
                cbTileTypeChanged (this);
            }
        }
    }

    public void AddTileTypeCallback (Action<Tile> callback ) {
        cbTileTypeChanged += callback;
    }

    public void RemoveTileTypeCallback (Action<Tile> callback ) {
        cbTileTypeChanged -= callback;
    }

    // Returns a bool to identify if the assignment of the object to the tile is successful
    public bool SetFixedObject ( FixedObject objInstance ) {
        if ( objInstance == null ) {
            // Uninstall any object already fixed to tile
            fixedObject = null;
            return true;
        }

        if ( fixedObject != null ) {
            Debug.LogError ( "Assigning a fixed object on a tile that already has one." );
            return false;
        }

        fixedObject = objInstance;
        return true;
    }
}


/// <summary>
/// Extra Note 1: The reason this tile class does not store or link to any gameobject (aside from the world level) is because
/// we want to keep the data layer and the visual layer separate. That is, whatever changes is made to the data layer should not
/// directly affect the visual layer, and vice versa
/// </summary>