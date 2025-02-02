using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSwapManager : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private GameObject startTile;
    private GameObject endTile;
    private Vector2 startInputPosition;
    public float swipeThreshold = 0.5f;
    private bool isTileClicked = false;
    private void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    startInputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    startTile = DetectTileUnderPosition(startInputPosition);
        //}
        //else if (Input.GetMouseButtonUp(0) && true == isTileClicked)
        //{
        //    Vector2 endInputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    endTile = DetectTileUnderPosition(endInputPosition);
        //    GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>().CheckSwapValidity(startTile, endTile);
        //    isTileClicked = false;
        //}
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startInputPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    startTile = DetectTileUnderPosition(startInputPosition);
                    
                break;

                case TouchPhase.Ended:
                    Vector2 endInputPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    endTile = DetectTileUnderPosition(endInputPosition);
                    GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>().CheckSwapValidity(startTile, endTile);
                    isTileClicked = false;
                break;
            }
        }
    }
    GameObject DetectTileUnderPosition(Vector2 worldPosition)
    {
        GameObject tile;
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null && (hit.collider.tag == "GreenTile" || hit.collider.tag == "YellowTile" || hit.collider.tag == "RedTile" || hit.collider.tag == "BlueTile"))
        {
            tile = hit.collider.gameObject;
            isTileClicked = true;
            return tile;
        }
        return null;
    }

    void DetectTileUnderTouch(Vector2 screenPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            //selectedTile = hit.collider.gameObject;
        }
    }

    //void DetectSwipeAndSwap(Vector2 endTouchPosition)
    //{
    //    Vector2 swipeDelta = (Vector2)Camera.main.ScreenToWorldPoint(endTouchPosition) - (Vector2)transform.position;

    //    if (swipeDelta.magnitude >= swipeThreshold)
    //    {
    //        Vector2Int direction;

    //        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
    //        {
    //            direction = swipeDelta.x > 0 ? Vector2Int.right : Vector2Int.left;
    //        }
    //        else
    //        {
    //            direction = swipeDelta.y > 0 ? Vector2Int.up : Vector2Int.down;
    //        }

    //        TrySwap(direction);
    //    }

    //    star = null;
    //}

    //void TrySwap(Vector2Int direction)
    //{
    //    Vector2 targetPosition = (Vector2)transform.position + (Vector2)direction;
    //    RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.zero);

    //    if (hit.collider != null)
    //    {
    //        GameObject targetTile = hit.collider.gameObject;

    //        if (targetTile != null)
    //        {
    //            // Swap logic using grid manager
    //            Vector2Int currentPos = Vector2Int.RoundToInt(transform.position);
    //            Vector2Int targetPos = Vector2Int.RoundToInt(targetTile.transform.position);
    //        }
    //    }
    //}
}
