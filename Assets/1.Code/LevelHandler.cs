using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using VInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelHandler : MonoBehaviour
{
    //Setup from Editor
    [ReadOnly]
    public SpriteRenderer fadeScreen;
    [ReadOnly]
    public TMP_Text tutorialTextComponent;
    
    public GameObject[] levels;
    public int levelNumber = 1;
    
    //Internal
    [HideInInspector]
    public GameObject player;
    private SpriteRenderer playerRenderer;
    private const float LevelMoveTime = 1.5f;
    private GameObject currentLevel;
    private Camera mainCamera;
    private Player playerScript;
    private const int LastTutLevel = 7;
    
    //Tutorial
    private readonly string[] tutorialText = { 
        "Collect the <color=#F3E36A>key</color> and find the goal.", 
        "<color=#4D7B9B>Brighter</color> parts are in front of you.", 
        "<color=#224B6B>Darker</color> parts are behind you.", 
        "Use <color=#F3E36A>objects</color> in the world to help you.",
        "<color=#F3E36A>Press and move mouse</color> to rotate the camera.",
        "Have fun :)"
    };
    

    private void OnEnable()
    {
        EventHandler.OnLevelFinished += FinishLevel;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
        tutorialTextComponent.text = "";
        tutorialTextComponent.color = new Color(tutorialTextComponent.color.r, tutorialTextComponent.color.g, tutorialTextComponent.color.b, 0f);
    }

    public void StartLevelHandler() //Is called by GameHandler
    {
        playerRenderer = player.GetComponent<SpriteRenderer>();
        playerRenderer.color = new Color(playerRenderer.color.r, playerRenderer.color.g, playerRenderer.color.b, 0f); //Hide Player
        playerScript = player.GetComponent<Player>();
        NewLevel();
    }
    
    private void FinishLevel(bool success)
    {
        if (levelNumber < LastTutLevel) tutorialTextComponent.DOFade(0f, 0.5f);
        //Start Next Level
        if (success)
        {
            if (levelNumber < levels.Length)
            {
                levelNumber++;
                NewLevel();
            }
        }
        else
        {
            StartCoroutine(RestartLevel());
        }
        

    }
    
    private void NewLevel()
    {
        GameObject newLevel = null;
        
        EventHandler.TriggerDisableRotation(); //Disable Cam
        Vector3 rotationVector = GetRotation(); // Gets a vector in accordance to current camera Rotation
        
        if (currentLevel != null)
        {
            //Tweens the old level away 
            GameObject oldLevel = currentLevel;
            Vector3 oldLevelTarget = new Vector3(rotationVector.x * -10, 0, rotationVector.z * -10);
            oldLevel.transform.DOMove(oldLevelTarget, LevelMoveTime).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    Destroy(oldLevel);
                    //Reset Camera rotation so rotation Vector is not needed anymore
                    EventHandler.TriggerResetRotation();
                
                    //Create new level to the right
                    Debug.Log("Creating level");
                    Vector3 nextLevelPos = new Vector3(10,0,0);
                    newLevel = Instantiate(levels[levelNumber], nextLevelPos, Quaternion.identity);
                
                    //Move-in new Level
                    Vector3 newLevelTarget = new Vector3(0, 0, 0);
                    newLevel.transform.DOMove(newLevelTarget, LevelMoveTime).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            FinishLevelCreation(newLevel, true);
                        });
                });

        }
        
        else //This is only needed on first loading when no current level has been designated yet.
        {
            //Create new level to the right
            Debug.Log("Creating level");
            Vector3 nextLevelPos = new Vector3(10,0,0);
            newLevel = Instantiate(levels[levelNumber], nextLevelPos, Quaternion.identity);
                
            //Move-in new Level
            Vector3 newLevelTarget = new Vector3(0, 0, 0);
            newLevel.transform.DOMove(newLevelTarget, LevelMoveTime).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    FinishLevelCreation(newLevel, true);
                });
        }
        
        
    }

    private IEnumerator RestartLevel()
    {
        EventHandler.TriggerDisableRotation(); //Stop Camera control
        
        fadeScreen.DOFade(1f, 0.8f) //Fade-in Blockscreen
            .OnComplete(() =>
        {
            Destroy(currentLevel);
            
            //Reset Camera rotation so rotation Vector is not needed anymore
            EventHandler.TriggerResetRotation();
            
            //Re-create level
            Debug.Log("Creating level");
            Vector3 nextLevelPos = new Vector3(0, 0, 0);
            currentLevel = Instantiate(levels[levelNumber], nextLevelPos, Quaternion.identity);
            Transform levelSpawn = currentLevel.transform.Find("Start");
            
            player.transform.position = levelSpawn.position; //Set Player to Start
            playerRenderer.color = Color.white; //Show Player
        });
        
        yield return new WaitForSeconds(1.5f);
            
        fadeScreen.DOFade(0f, 0.8f) //Fade-out BLockscreen
            .OnComplete(() =>
            { 
                if (levelNumber < LastTutLevel) WriteTutorialText(); 
                
                // Start Player
                playerScript.MoveFinished();
            });
        
  
    }

    private void FinishLevelCreation(GameObject level, bool newLevel)
    {
        currentLevel = level;
        Transform levelSpawn = currentLevel.transform.Find("Start");
        
        if (levelNumber < LastTutLevel) WriteTutorialText();
            
        // Start Player & Camera Control
        EventHandler.TriggerPlayerSetPositionAndShow(newLevel, levelSpawn.position);
    }

    private Vector3 GetRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);
        
        if (Mathf.Abs(mainCamera.transform.eulerAngles.y - 0) < 0.1f)
        {
            rotationVector = new Vector3Int(1, 0, 0);
        }
        else if (Mathf.Abs(mainCamera.transform.eulerAngles.y - 90) < 0.1f)
        {
            rotationVector = new Vector3Int(0, 0, -1);
           
        }
        else if (Mathf.Abs(mainCamera.transform.eulerAngles.y - 180) < 0.1f)
        {
            rotationVector = new Vector3Int(-1, 0, 0);
          
        }
        else if (Mathf.Abs(mainCamera.transform.eulerAngles.y - 270) < 0.1f)
        {
            rotationVector = new Vector3Int(0, 0, 1);
           
        }
        
        return rotationVector;
    }

    private void WriteTutorialText()
    {
        string nextText = tutorialText[levelNumber-1];
        tutorialTextComponent.text = nextText;
        tutorialTextComponent.DOFade(1f, 0.5f);
    }
    
}
