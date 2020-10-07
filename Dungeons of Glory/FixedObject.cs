using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedObject {
    
    // Base tile of the object (i.e the tile origin)
    // Objects may be larger than a single tile, but any rotations and such will be around this tile
    Tile tile;

    // This will be queried by the visual system to figure out what sprite to render for the instanced object
    string objectType;

    // Movement cost modifier
    // Increasing movementCost will slow down the movement of pawns
    float movementCost;

    // Dimensions of an object in number of tiles
    // Objects may be larger than a single tile
    int width;
    int height;

    /// <summary>
    /// By declaring an empty protected constructor within the class, it overrides the class' ability to use the default public empty constructor
    /// In the event that we do not wish for any program or script to be able to use an empty constructor (possibly because any necessary variables
    /// wouldn't be initialized properly), we can use this to force the usage of secondary constructors.
    /// </summary>
    protected FixedObject () { }

    static public FixedObject CreatePrototype ( string _objType, float _moveCost = 1f, int w = 1, int h = 1 ) {
        FixedObject obj = new FixedObject();

        obj.objectType = _objType;
        obj.movementCost = _moveCost;
        obj.width = w;
        obj.height = h;

        return obj;
    }
    static protected FixedObject InstallObject ( FixedObject proto, Tile tile ) {
        FixedObject obj = new FixedObject();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;

        obj.tile = tile;

        if ( tile.SetFixedObject(obj) == false ) {
            // In the event that the assignment failed (possibly due to another object already inhabiting that tile)
            // Do NOT return newly instantiated object (it will be garbage collected)
            return null;
        }

        return obj;
    }
}
