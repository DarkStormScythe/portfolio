using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public GameObject selectionTilePrefab;
    public LineRenderer selectionBox;

    TileType buildTile = TileType.Floor;
    bool objectConstructionOn = false;
    string buildObjectType;

    Vector3 startFramePosition;
    Vector3 endFramePosition;
    Vector3 dragStartPosition;
    Tile selectedTile;
    List<GameObject> tileHighlightList;
    List<Tile> selectedTileList;

    // Start is called before the first frame update
    void Start () {
        tileHighlightList = new List<GameObject> ();
        selectedTileList = new List<Tile> ();
    }

    // Update is called once per frame
    void Update () {

        /// <summary>
        /// Note about ScreenToWorldPoint: returns the position of the mouse on worldspace relative to the near-clipping plane
        /// On a perspective (3D) view, the screen point and the actual world point may differ from expectation, on account that the "screen" is smaller than the actual world viewed
        /// In this case, using raycasting is a better option
        /// </summary>
        startFramePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        startFramePosition.z = 0;

        DragSelectUpdate ();
        CameraUpdate ();

        /// <summary>
        /// Reminder: When updating the frame end position of the mouse, remember that the position of the mouse at the end of the frame
        /// may not be the same as the position of the mouse at the start of the frame.
        /// </summary>
        endFramePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        endFramePosition.z = 0;
    }

    void DragSelectUpdate () {
        // If over UI element, skip everything else
        if ( EventSystem.current.IsPointerOverGameObject() ) {
            return;
        }

        // Start drag
        if ( Input.GetMouseButtonDown ( 0 ) ) {
            SelectionListCleanup ();

            dragStartPosition = startFramePosition;
            //selectionBox.gameObject.SetActive ( true );
            //selectionBox.startWidth = 0.02f;
            //selectionBox.endWidth = 0.02f;
        }

        //if ( selectionBox.gameObject.activeSelf ) {
        //    Vector3[] boxCoord = new Vector3[4] { new Vector3 (dragStartPosition.x, dragStartPosition.y, -1),
        //                                          new Vector3 (startFramePosition.x, dragStartPosition.y, -1),
        //                                          new Vector3 (startFramePosition.x, startFramePosition.y, -1),
        //                                          new Vector3 (dragStartPosition.x, startFramePosition.y, -1) };
        //    selectionBox.SetPositions ( boxCoord );
        //}

        if ( Input.GetMouseButton ( 0 ) ) {
            float startSelection_X = dragStartPosition.x;
            float startSelection_Y = dragStartPosition.y;
            float endSelection_X = endFramePosition.x;
            float endSelection_Y = endFramePosition.y;

            // Flip start and end positions if selection is dragged from a positive direction to a negative direction
            if ( endSelection_X < startSelection_X ) {
                float tmp = endSelection_X;
                endSelection_X = startSelection_X;
                startSelection_X = tmp;
            }
            if ( endSelection_Y < startSelection_Y ) {
                float tmp = endSelection_Y;
                endSelection_Y = startSelection_Y;
                startSelection_Y = tmp;
            }

            // Loop through the coordinates from the selection box
            // Skip coordinates by the size of the tile, so that tiles won't be called more than once, if tiles are larger than 1 unit
            for ( float x_coord = startSelection_X; x_coord <= endSelection_X; x_coord++ ) {
                for ( float y_coord = startSelection_Y; y_coord <= endSelection_Y; y_coord++ ) {
                    Tile t = WorldController.Instance.GetTileAtWorldCoord( new Vector3 (x_coord, y_coord));
                    if ( t != null ) {
                        // We'll initialize all the highlight tiles we need and store them in a list for easy cleanup later
                        // At the same time, all physical tiles that have been selected goes into a second list for processing, when
                        // the player pushes the buttons to build various objects
                        GameObject _selectedTile = (GameObject) Instantiate ( selectionTilePrefab, new Vector3 (t.X, t.Y, 0), Quaternion.identity );
                        _selectedTile.transform.localScale = new Vector3 ( t.TileSize, t.TileSize );
                        tileHighlightList.Add ( _selectedTile );

                        selectedTileList.Add ( t );

                        if ( objectConstructionOn ) {
                            //WorldController.Instance.World.ConstructFixedObject ( buildObjectType, t );
                        }
                    }
                }
            }
        }

        // End drag
        if ( Input.GetMouseButtonUp ( 0 ) ) {
            //float startSelection_X = selectionBox.GetPosition(0).x;
            //float startSelection_Y = selectionBox.GetPosition(0).y;
            //float endSelection_X = selectionBox.GetPosition(2).x;
            //float endSelection_Y = selectionBox.GetPosition(2).y;

            float startSelection_X = dragStartPosition.x;
            float startSelection_Y = dragStartPosition.y;
            float endSelection_X = endFramePosition.x;
            float endSelection_Y = endFramePosition.y;

            // Flip start and end positions if selection is dragged from a positive direction to a negative direction
            if ( endSelection_X < startSelection_X ) {
            float tmp = endSelection_X;
            endSelection_X = startSelection_X;
            startSelection_X = tmp;
            }
            if ( endSelection_Y < startSelection_Y ) {
                float tmp = endSelection_Y;
                endSelection_Y = startSelection_Y;
                startSelection_Y = tmp;
            }

            // Loop through the coordinates from the selection box
            // Skip coordinates by the size of the tile, so that tiles won't be called more than once, if tiles are larger than 1 unit
            for ( float x_coord = startSelection_X; x_coord <= endSelection_X; x_coord++ ) {
                for ( float y_coord = startSelection_Y; y_coord <= endSelection_Y; y_coord++ ) {
                    Tile t = WorldController.Instance.GetTileAtWorldCoord( new Vector3 (x_coord, y_coord));
                    if ( t != null ) {
                        // We'll initialize all the highlight tiles we need and store them in a list for easy cleanup later
                        // At the same time, all physical tiles that have been selected goes into a second list for processing, when
                        // the player pushes the buttons to build various objects
                        GameObject _selectedTile = (GameObject) Instantiate ( selectionTilePrefab, new Vector3 (t.X, t.Y, 0), Quaternion.identity );
                        _selectedTile.transform.localScale = new Vector3 ( t.TileSize, t.TileSize );
                        tileHighlightList.Add ( _selectedTile );

                        selectedTileList.Add ( t );

                        if ( objectConstructionOn ) {
                            //WorldController.Instance.World.ConstructFixedObject ( buildObjectType, t );
                        }
                    }
                }
            }

            selectionBox.gameObject.SetActive ( false );
        }

        // Deselect everything
        if ( Input.GetMouseButtonDown ( 1 ) ) {
            SelectionListCleanup ();
        }
    }

    void SelectionListCleanup () {
        // Clean up old highlights
        while ( tileHighlightList.Count > 0 ) {
            GameObject _highlight = tileHighlightList[0];
            tileHighlightList.RemoveAt ( 0 );
            Destroy ( _highlight );
        }

        // Clean up old selected tiles
        while ( selectedTileList.Count > 0 ) {
            selectedTileList.RemoveAt ( 0 );
        }
    }

    void CameraUpdate () {
        // Handle screen dragging
        if ( Input.GetMouseButton ( 2 ) ) {
            Vector3 diff = endFramePosition - startFramePosition;
            Camera.main.transform.Translate ( diff );
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis ( "Mouse ScrollWheel" );
        Camera.main.orthographicSize = Mathf.Clamp ( Camera.main.orthographicSize, 3f, 50f );
    }

    public void BuildFloor () {
        if ( selectedTileList.Count > 0 ) {
            foreach ( Tile t in selectedTileList ) {
                t.Type = TileType.Floor;
            }
        }
    }

    public void Bulldoze () {
        if ( selectedTileList.Count > 0 ) {
            foreach ( Tile t in selectedTileList ) {
                t.Type = TileType.Empty;
            }
        }
    }

    public void BuildFixedObject ( string objType ) {
        objectConstructionOn = true;
        buildObjectType = objType;
    }
}
