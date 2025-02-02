using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResizeBoardClass : MonoBehaviour
{
    public GameObject Board;
    public void ResizeBoard()
    {
        int newBoardWidth = 0;
        int newBoardHeight = 0;
        try
        {
            newBoardWidth = int.Parse(GameObject.Find("WidthTextBox").GetComponent<TMP_InputField>().text);
            newBoardHeight = int.Parse(GameObject.Find("HeightTextBox").GetComponent<TMP_InputField>().text);
            BoardManager.boardWidth = newBoardWidth;
            BoardManager.boardHeight = newBoardHeight;
            //CleanUp for the old board
            Destroy(GameObject.Find("Board"));
            Vector3 vector3 = new Vector3(0, 0, 0);
            GameObject newBoard = Instantiate(Board, vector3, Quaternion.identity);
            newBoard.name = "Board";
        }
        catch (System.Exception ex)
        {
            // Warn user to give valid input.
        }
    }
}
