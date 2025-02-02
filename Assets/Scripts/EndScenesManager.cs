using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScenesManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "VictoryScreen")
        {

        }
        else if(SceneManager.GetActiveScene().name == "DefeatScreen")
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
