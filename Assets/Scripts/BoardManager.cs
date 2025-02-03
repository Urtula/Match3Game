using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Linq;
using DG.Tweening;


public enum NeighbourDirection
{
    Top =0,
    Right,
    NoNeighbour
}
public class BoardManager : MonoBehaviour
{
    public static int boardWidth = 8;
    public static int boardHeight =8;
    public GameObject tilePrefab; 
    public GameObject[] objectPrefabs;
    private GameObject[,] board;
    public int movesLeft = 0;
    public int currentScore = 0;
    public int targetScore = 0;

    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        //The board is initialized with default values which can later be changed from a menu within the game.
        InitializeBoard(boardWidth, boardHeight);
        CalculateScoreAndMoves(boardWidth, boardHeight);
        //BOARD RESHUFFLÝNG ÝF NO MATCHES EXÝST
        int matchCount = GetCurrentBoardPotentialMatchCount();
        if(matchCount == 0)
        {
            ShuffleBoard();
        }
    }

    void InitializeBoard(int width, int height)
    {
        board = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject baseTile = Instantiate(tilePrefab, new Vector3((Screen.width/260f) * x / (float)boardWidth, (Screen.height/350f) * y / (float)boardHeight, 0), Quaternion.identity);
                baseTile.name = "BaseTile_" + x + "_" + y;
                baseTile.transform.parent = this.transform;
                AdjustTileScale(baseTile);
                // Place the objects at random but also check for matches and select another object if a match occurs when placing.
                int randomIndex = UnityEngine.Random.Range(0, objectPrefabs.Length);
                //Check Vertical match if y is => than 2 due to our order of addind tiles to the board.
                if (y >= 2 && objectPrefabs[randomIndex].tag == board[x, y - 1].tag && objectPrefabs[randomIndex].tag == board[x, y - 2].tag)
                {
                    randomIndex = (randomIndex + 1) % objectPrefabs.Length;
                }
                if (x >= 2 && objectPrefabs[randomIndex].tag == board[x - 1, y].tag && objectPrefabs[randomIndex].tag == board[x - 2, y].tag)
                {
                    randomIndex = (randomIndex + 1) % objectPrefabs.Length;
                }
                GameObject tile = Instantiate(objectPrefabs[randomIndex], baseTile.transform.position, Quaternion.identity);
                AdjustTileScale(tile);
                tile.name = "Tile_" + x + "_" + y;
                tile.transform.parent = baseTile.transform; 
                board[x, y] = tile;
            }
        }
        CenterBoard();
    }
    public void ShuffleBoard()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, objectPrefabs.Length);
                //Check Vertical match if y is => than 2 due to our order of addind tiles to the board.
                if (j >= 2 && objectPrefabs[randomIndex].tag == board[i, j - 1].tag && objectPrefabs[randomIndex].tag == board[i, j - 2].tag)
                {
                    randomIndex = (randomIndex + 1) % objectPrefabs.Length;
                }
                if (i >= 2 && objectPrefabs[randomIndex].tag == board[i - 1, j].tag && objectPrefabs[randomIndex].tag == board[i - 2, j].tag)
                {
                    randomIndex = (randomIndex + 1) % objectPrefabs.Length;
                }
                board[i,j].tag = objectPrefabs[randomIndex].tag;
                board[i, j].GetComponent<SpriteRenderer>().sprite = objectPrefabs[randomIndex].GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
    void CenterBoard()
    {
        float offsetDistanceX = 0f;
        float offsetDistanceY = 0f;
        if (boardWidth % 2 == 0)
        {
            offsetDistanceX = (board[(boardWidth / 2)-1,0].transform.position.x + board[(boardWidth / 2), 0].transform.position.x)/ 2;
        }
        else
        {
            offsetDistanceX = board[boardWidth / 2, 0].transform.position.x;
        }

         offsetDistanceY = (board[0, boardHeight-1].transform.position.y + board[0, 0].transform.position.y) / 2;

        this.transform.position = new Vector3(-offsetDistanceX, -offsetDistanceY,0);
    }
    void AdjustTileScale(GameObject tile)
    {
        tile.transform.localScale = new Vector3(Screen.width / (boardWidth * 700.0f), Screen.height/(boardHeight * 1000.0f), 1);

    }

    int GetCurrentBoardPotentialMatchCount()
    {
        int boardMatchCount = 0;
        for(int i = 0; i < boardWidth - 1; i++)
        {
            for (int j = 0; j < boardHeight - 1; j++)
            {
                // We check the direction of the neighbour to optimize our potential matches checking by eliminating unnecessary checks
                NeighbourDirection neighbourDirection = CheckMatchingNeighbour(i, j);
                if (NeighbourDirection.NoNeighbour != neighbourDirection)
                {
                    //if object has a matching neighbour we can check if any of the neighbours of the 2 tiles are also matching to determine a potential 3 match.
                    int potentialMatchesOfTile = DetermineNumPotentialMatches(i, j, neighbourDirection);
                    if(potentialMatchesOfTile > 0)
                    {
                        boardMatchCount++;
                    }
                }
            }
        }
        return boardMatchCount;
    }

    NeighbourDirection CheckMatchingNeighbour(int x, int y) 
    {
        if (board[x,y].tag == board[x+1,y].tag)
        {
            return NeighbourDirection.Right;
        }
        else if(board[x, y].tag == board[x, y + 1].tag)
        {
            return NeighbourDirection.Top;
        }
        return NeighbourDirection.NoNeighbour;
    }

    int DetermineNumPotentialMatches(int x, int y, NeighbourDirection neighbourDirection)
    {
        int numPotentialMatches = 0;
        if(NeighbourDirection.Right == neighbourDirection)
        {
            // Only Matches can exist in the right side of this tile since it is too close to the left corner
            if(x <= 1)
            {
                if (y == 0)
                {
                    if (board[x,y].tag == board[x+3,y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 2, y+1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x + 3, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 2, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 2, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
            // Only Matches can exist in the left side of this tile since it is too close to the right corner

            else if (x >= boardWidth - 3)
            {
                if (y == 0)
                {
                    if (board[x, y].tag == board[x - 2, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x - 2, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
            //Matches can exist in all directions so we have to check all of them
            else
            {
                if (y == 0)
                {
                    if (board[x, y].tag == board[x + 3, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 2, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }

                    if (board[x, y].tag == board[x - 2, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x + 3, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 2, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 2, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }

                    if (board[x, y].tag == board[x - 2, y].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
        }
        else if(NeighbourDirection.Top == neighbourDirection)
        {
            if (y <= 1)
            {
                if (x == 0)
                {
                    if (board[x, y].tag == board[x, y + 3].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y + 2].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x, y + 3].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y + 2].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y + 2].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
            else if (y >= boardHeight - 3)
            {
                if (x == 0)
                {
                    if (board[x, y].tag == board[x, y - 2].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x, y - 2].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
            //Matches can exist in all directions so we have to check all of them
            else
            {
                if (x == 0)
                {
                    if (board[x, y].tag == board[x, y + 3].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y + 2].tag)
                    {
                        numPotentialMatches++;
                    }

                    if (board[x, y].tag == board[x, y - 2].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x, y + 3].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y + 2].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x -1, y + 2].tag)
                    {
                        numPotentialMatches++;
                    }

                    if (board[x, y].tag == board[x, y - 2].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x + 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                    if (board[x, y].tag == board[x - 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
        }
        else
        {
            //If tile has no matching neighbours a match can occur if its top right and bottom right, top right and top left, top left and bottom left or bottom left and bottom right ttiles are of matching color.
            if(x == 0)
            {
                //Only match occurs if Top right and bottom right tiles are matching
                if(y != 0)
                {
                    if (board[x,y].tag == board[x+1,y+1].tag && board[x, y].tag == board[x + 1, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
            else
            {
                if (y == 0)
                {
                    //Only match occurs if Top right and top left tiles are matching
                    if (board[x, y].tag == board[x + 1, y + 1].tag && board[x, y].tag == board[x - 1, y + 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
                else
                {
                    if (board[x, y].tag == board[x + 1, y + 1].tag && board[x, y].tag == board[x - 1, y + 1].tag ||
                        board[x, y].tag == board[x + 1, y + 1].tag && board[x, y].tag == board[x + 1, y + 1].tag ||
                        board[x, y].tag == board[x - 1, y + 1].tag && board[x, y].tag == board[x - 1, y - 1].tag ||
                        board[x, y].tag == board[x - 1, y - 1].tag && board[x, y].tag == board[x + 1, y - 1].tag)
                    {
                        numPotentialMatches++;
                    }
                }
            }
        }
        return numPotentialMatches;
    }
   
    void CalculateScoreAndMoves(int boardWidth, int boardHeight)
    {
        //Calculate the target score and moves allowed based on the given board dimensions.
        movesLeft = Mathf.Max(boardWidth, boardHeight);
        GameObject.Find("MovesLeftTextBox").GetComponent<TMP_Text>().text = movesLeft.ToString();
        targetScore = boardWidth * boardHeight / 2;
        GameObject.Find("ScoreTextBox").GetComponent<TMP_Text>().text = "Score: " + currentScore.ToString() + "/" + targetScore.ToString();
    }
    public void CheckSwapValidity(GameObject startTile, GameObject endTile)
    {
        //With the help of our naming conventions for the tiles we can split the name with _ to get the x and y values of our tile positions.
        var startTilePos = ( int.Parse(startTile.name.Split("_")[1]), int.Parse(startTile.name.Split("_")[2]));
        var endTilePos = (int.Parse(endTile.name.Split("_")[1]), int.Parse(endTile.name.Split("_")[2]));
        // check if swap is in a valid direction(top right bottom left) we can do this by checking the sum of differences in both the x and y directions. the sum must be 1 for a valid move.
        int sumOfDirectionDiff = Math.Abs(endTilePos.Item1 - startTilePos.Item1) + Math.Abs(endTilePos.Item2 - startTilePos.Item2);
        if (sumOfDirectionDiff == 1)
        {
            //if the move is valid we must then check if it results in a match and if it doesnt have a little return animation to put the tiles in their original place

                Vector3 posTile1 = board[startTilePos.Item1, startTilePos.Item2].transform.position;
                Vector3 posTile2 = board[endTilePos.Item1, endTilePos.Item2].transform.position;

                DOTween.Sequence()
                    .Append(board[startTilePos.Item1, startTilePos.Item2].transform.DOMove(posTile2, 0.5f))
                    .Join(board[endTilePos.Item1, endTilePos.Item2].transform.DOMove(posTile1, 0.5f))
                    .OnComplete(() => HandleSwapEvent(startTilePos, endTilePos));

        }
    }


    void HandleSwapEvent((int, int) startTilePos, (int, int) endTilePos)
    {
        Vector3 posTile1 = board[startTilePos.Item1, startTilePos.Item2].transform.position;
        Vector3 posTile2 = board[endTilePos.Item1, endTilePos.Item2].transform.position;

        // TODO this doesnt need to be DOTween. After Visual effects we actually have to swap the tag between the tiles for checking matches.
        DOTween.Sequence().Append(board[startTilePos.Item1, startTilePos.Item2].transform.DOMove(posTile2, 0.001f))
                          .Join(board[endTilePos.Item1, endTilePos.Item2].transform.DOMove(posTile1, 0.001f));

        string tmpTag = board[startTilePos.Item1, startTilePos.Item2].tag;
        Sprite tmpSprite = board[startTilePos.Item1, startTilePos.Item2].GetComponent<SpriteRenderer>().sprite;
        board[startTilePos.Item1, startTilePos.Item2].tag = board[endTilePos.Item1, endTilePos.Item2].tag;
        board[startTilePos.Item1, startTilePos.Item2].GetComponent<SpriteRenderer>().sprite = board[endTilePos.Item1, endTilePos.Item2].GetComponent<SpriteRenderer>().sprite;
        board[endTilePos.Item1, endTilePos.Item2].GetComponent<SpriteRenderer>().sprite = tmpSprite;
        board[endTilePos.Item1, endTilePos.Item2].tag = tmpTag;
        List<GameObject> matchingTiles = FindCurrentBoardMatches();
        if(matchingTiles != null && matchingTiles.Count > 0 )
        {
            StartCoroutine(HandleTileMatches(matchingTiles));
        }
        else
        {
            //Remove the tiles if there is no match along with their tags and other info we swapped
            tmpTag = board[startTilePos.Item1, startTilePos.Item2].tag;
            tmpSprite = board[startTilePos.Item1, startTilePos.Item2].GetComponent<SpriteRenderer>().sprite;
            board[startTilePos.Item1, startTilePos.Item2].tag = board[endTilePos.Item1, endTilePos.Item2].tag;
            board[startTilePos.Item1, startTilePos.Item2].GetComponent<SpriteRenderer>().sprite = board[endTilePos.Item1, endTilePos.Item2].GetComponent<SpriteRenderer>().sprite;
            board[endTilePos.Item1, endTilePos.Item2].GetComponent<SpriteRenderer>().sprite = tmpSprite;
            board[endTilePos.Item1, endTilePos.Item2].tag = tmpTag;

            posTile1 = board[startTilePos.Item1, startTilePos.Item2].transform.position;
            posTile2 = board[endTilePos.Item1, endTilePos.Item2].transform.position;

            // TODO this doesnt need to be DOTween. After Visual effects we actually have to swap the tag between the tiles for checking matches.
            DOTween.Sequence().Append(board[startTilePos.Item1, startTilePos.Item2].transform.DOMove(posTile2, 0.001f))
                              .Join(board[endTilePos.Item1, endTilePos.Item2].transform.DOMove(posTile1, 0.001f));
            DOTween.Sequence()
                    .Append(board[startTilePos.Item1, startTilePos.Item2].transform.DOMove(posTile2, 0.5f))
                    .Join(board[endTilePos.Item1, endTilePos.Item2].transform.DOMove(posTile1, 0.5f));
        }
        UpdateMovesLeft();
        if (currentScore >= targetScore && SceneManager.GetActiveScene().name == "GameScene") 
        {
            //Game is over player wins
            SceneManager.LoadScene("VictoryScreen");
        }
        else
        {
            int potentialMatchCount = GetCurrentBoardPotentialMatchCount();
            if (potentialMatchCount == 0) 
            {
                ShuffleBoard();
            }
        }
    }
    void UpdateMovesLeft()
    {
        movesLeft--;
        GameObject.Find("MovesLeftTextBox").GetComponent<TMP_Text>().text = movesLeft.ToString();
        if (movesLeft == 0)
        {
            if (currentScore < targetScore && SceneManager.GetActiveScene().name == "GameScene")
            {
                //Game is over player wins
                SceneManager.LoadScene("DefeatScene");
            }
        }
    }
    List<GameObject> FindCurrentBoardMatches()
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        //Horizontal matches
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight - 2; j++)   
            {
                if(board[i, j].tag == board[i,j+1].tag && board[i, j].tag == board[i, j + 2].tag)
                {
                    matchingTiles.Add(board[i, j]); 
                    matchingTiles.Add(board[i, j+1]); 
                    matchingTiles.Add(board[i, j+2]); 
                }
            }
        }
        //Vertical matches
        for (int j = 0; j < boardHeight; j++)
        {
            for (int i = 0; i < boardWidth - 2; i++)
            {
                if (board[i, j].tag == board[i+1, j].tag && board[i, j].tag == board[i+2, j].tag)
                {
                    matchingTiles.Add(board[i, j]);
                    matchingTiles.Add(board[i+1, j]);
                    matchingTiles.Add(board[i+2, j]);
                }
            }
        }

        return matchingTiles.ToHashSet().ToList();
    }
    GameObject AddSingleTile(GameObject tile)
    {
        int randomIndex = UnityEngine.Random.Range(0, objectPrefabs.Length);

        GameObject newTile = Instantiate(objectPrefabs[randomIndex], tile.transform.parent.transform.position, Quaternion.identity);
        AdjustTileScale(newTile);
        newTile.name = tile.name;
        newTile.transform.parent = tile.transform.parent.transform;
        board[int.Parse(tile.name.Split("_")[1]), int.Parse(tile.name.Split("_")[2])] = newTile;
        return newTile;
    }

    private IEnumerator HandleTileMatches(List<GameObject> matchingTiles)
    {
        List<GameObject> newTiles = new List<GameObject>();
        while (matchingTiles.Count > 0)
        {
            // Destroy tiles
            foreach (GameObject go in matchingTiles)
            {
                GameObject newTile = AddSingleTile(go);
                newTile.SetActive(false);
                newTiles.Add(newTile);
                go.GetComponent<TileVFXManager>().PlayExplosionEffect();
                Destroy(go);
            }
            // Wait before refilling
            audioSource.Play();

            yield return new WaitForSeconds(0.5f);
            foreach (GameObject tiles in newTiles)
            {
                tiles.SetActive(true); 
            }
            // Update score and display it
            currentScore += matchingTiles.Count;
            //wait after refilling so player can see new tiles
            yield return new WaitForSeconds(1f);

            matchingTiles.Clear();
            newTiles.Clear();
            matchingTiles = FindCurrentBoardMatches();
            GameObject.Find("ScoreTextBox").GetComponent<TMP_Text>().text = "Score: " + currentScore.ToString() + "/" + targetScore.ToString();
            yield return new WaitForSeconds(0.3f);

            if (currentScore >= targetScore && SceneManager.GetActiveScene().name == "GameScene")
            {
                //Game is over player wins
                SceneManager.LoadScene("VictoryScreen");
            }
        }
    }
}

