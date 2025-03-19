using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    new LevelHandler levelHandler;
    
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        // Get LevelHandler to pass player
        levelHandler = this.GetComponent<LevelHandler>();
        
        //Instantiate Player
        Debug.Log("Creating player");
        Vector3 playerInitialPos = new Vector3(100,100,0);
        GameObject player = Instantiate(playerPrefab, playerInitialPos, Quaternion.identity);
        levelHandler.player = player;
        
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(0.3f); //Wait for general Init
        levelHandler.StartLevelHandler();
    }
    
}
