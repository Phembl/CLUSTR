using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEditor;
using VInspector.Libs;

public class Player : MonoBehaviour
{
    private Controls inputActions;
    private Vector2 moveInput = new Vector2(0, 0);

    private bool isEnabled;
    private bool isMoving;

    private const float SpeedOneStep = 0.5f;
    private const float SpeedHalfStep = 0.25f;
    
    private const int LevelLayerMask = (1 << 7);
    private const int InteractableLayerMask = (1 << 6);

    private int gravityDirection = -1;
    private int currentLevelRotation;
    private Vector3Int moveDirection = new Vector3Int(1, 0, 0);

    private SpriteRenderer playerRenderer;

    [Header("Sprites")] 
    public Sprite normalSprite;
    public Sprite surprisedSprite;
    public Sprite deadSprite;
    public Sprite happySprite;


    void Awake()
    {
        // Instantiate the Input Actions object
        inputActions = new Controls();

    }

    private void OnEnable()
    {
        EventHandler.OnPlayerEnable += EnablePlayer;
        EventHandler.OnPlayerDisable += DisablePlayer;
        EventHandler.OnPlayerSetPositionAndShow += SetPlayerPositionAndShow;
        EventHandler.OnPlayerHideAndReset += HideAndResetPlayer;
        EventHandler.OnRotate += RotatePlayer;
    }

    private void OnDisable()
    {
        EventHandler.OnPlayerEnable -= EnablePlayer;
        EventHandler.OnPlayerDisable -= DisablePlayer;
        EventHandler.OnPlayerSetPositionAndShow -= SetPlayerPositionAndShow;
        EventHandler.OnPlayerHideAndReset -= HideAndResetPlayer;
        EventHandler.OnRotate -= RotatePlayer;
    }

    void Start()
    {
        // Enable the input actions
        inputActions.Gameplay.Enable();

        // Bind actions to methods
        inputActions.Gameplay.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Movement.canceled += ctx => moveInput = Vector2.zero;

        //Set Renderer
        playerRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (moveInput != Vector2.zero && isEnabled)
        {
            DisablePlayer();
            if (moveInput.x > 0) CheckNextStep(1);
            else CheckNextStep(-1);
        }
    }




    private void CheckNextStep(int direction)
    {
        EventHandler.TriggerDisableRotation();
        playerRenderer.sortingOrder = SetSortingOrder();

        Vector3Int forwardVector = new Vector3Int((moveDirection.x * direction), 0, (moveDirection.z * direction));
        Vector3 forwardRotation = transform.rotation.eulerAngles + new Vector3(0f,0f,(-90f * direction));

        //First Raycast Setup
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = forwardVector;

        //Check Wall in front
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit rayHitInfo_1, 1f, LevelLayerMask))
        {
            //Did hit
            rayOrigin = new Vector3(transform.position.x, transform.position.y + (-gravityDirection), transform.position.z);
            //Check Wall on up
            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit rayHitInfo_2, 1f, LevelLayerMask))
            {
                GameObject hitObject = rayHitInfo_1.transform.gameObject;
                MoveWall(forwardVector, hitObject);
            }
            else
            {
                //Check if blocked from above
                if (Physics.Raycast(transform.position, new Vector3(0, (gravityDirection * -1), 0), out RaycastHit rayHitInfo_3, 1f, LevelLayerMask))
                {
                    GameObject hitObject = rayHitInfo_1.transform.gameObject;
                    MoveWall(forwardVector, hitObject);
                }
                else
                {
                    MoveStepUp(forwardVector, forwardRotation);
                }
                
            }

        }

        else //No Wall in front
        {
            MoveStepForward(forwardVector, forwardRotation);
        }
    }

    private void CheckGround() //Called by MoveStepForward
    {
        //Groundcheck Raycast Setup
        Vector3 rayDirection = new Vector3(0,gravityDirection,0);

        //Check for ground beneath
        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit rayHitInfo_2, 10f, LevelLayerMask))
        {
            //Hit Ground
            //Evalute distance to Ground
            if (rayHitInfo_2.distance <= 1.1f)
            {
                //Direct Ground, nothing more to check.
                CheckForInteractable();
            }
            
            else 
            {
                //Drop
                int dropDistance = (int)rayHitInfo_2.distance;
                GameObject hitObject = rayHitInfo_2.transform.gameObject;
                MoveDrop(false, dropDistance, hitObject); 
            }


        }
        else //Don't Hit Ground
        {
            GameObject nothing = null;
            MoveDrop(true, 6, nothing); //Calls true for isDead (Object is unneccessary)
        }
    }

    private void MoveWall(Vector3 forwardVector, GameObject tile)
    {
       //Pushes the wall a little bit
       Vector3 pushVector = new Vector3(forwardVector.x * 0.2f, 0, forwardVector.z * 0.2f);
       tile.transform.DOMove((tile.transform.position + pushVector), 0.2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
       transform.DOMove((transform.position + pushVector), 0.2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo)
           .OnComplete(MoveFinished);

    }

    private void MoveStepForward(Vector3Int forwardVector, Vector3 forwardRotation)
    {
        transform.DORotate(forwardRotation, SpeedOneStep).SetEase(Ease.Linear);
        transform.DOMove((transform.position + forwardVector), SpeedOneStep).SetEase(Ease.Linear)
            .OnComplete(CheckGround);
    }

    private void MoveStepUp(Vector3Int forwardVector, Vector3 forwardRotation)
    {
        transform.DOMoveY((transform.position.y + (gravityDirection * -1)), SpeedHalfStep).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DORotate(forwardRotation, SpeedOneStep);
                transform.DOMove((transform.position + forwardVector), SpeedOneStep).SetEase(Ease.OutQuad)
                    .OnComplete(CheckForInteractable);
            });
    }

    private void MoveDrop(bool isDead, int dropDistance, GameObject tile)
    {
        if (isDead) playerRenderer.sprite = deadSprite;
        Vector3 pushVector = new Vector3(0, 0.1f * gravityDirection, 0);
        
        int dropDirection = gravityDirection * dropDistance;
        transform.DOMoveY((transform.position.y + dropDirection), (SpeedHalfStep * (dropDistance / 2f))).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                if (isDead)
                {
                    KillPlayer();
                }
                else
                {
                    
                    if (dropDistance > 1.1f)
                    {
                        //Pushes the floor a bit down is drop Distance is high enough
                        tile.transform.DOMove((tile.transform.position + pushVector), 0.1f).SetLoops(2, LoopType.Yoyo);
                        transform.DOMove((transform.position + pushVector), 0.1f).SetLoops(2, LoopType.Yoyo)
                            .OnComplete(CheckForInteractable);
                    }
                    else
                    {
                        CheckForInteractable();
                    }
                   
                }

            });
                
    }
    

    private void CheckForInteractable()
    {   
        //Check for interactable
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + (-gravityDirection), transform.position.z);
        Vector3 rayDirection = new Vector3(0, gravityDirection, 0);
        
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit rayHitInfo_1, 1f, InteractableLayerMask))
        {
            //Hits interactable
            //Gets the interactable component from the Interactable Game Object
            GameObject interactableObject = rayHitInfo_1.transform.gameObject;
            Interactable interactable = rayHitInfo_1.collider.gameObject.GetComponent<Interactable>();
            StartCoroutine(ResolveInteraction(interactableObject, interactable));
        }
       
        else
        {   //No Interactable
            MoveFinished();
        }
    }
    
    private IEnumerator ResolveInteraction(GameObject interactionObject, Interactable interactable)
    {
        string interactableType = interactionObject.tag;
        switch (interactableType)
        {
            case "Key":
                playerRenderer.sprite = surprisedSprite;
                //keyFeedback?.PlayFeedbacks();
                EventHandler.TriggerPlayerInteracted(interactable, this.gameObject);
                yield return new WaitForSeconds(0.2f);
                MoveFinished();
                break;
            
            case "Goal":
                playerRenderer.sprite = happySprite;
                EventHandler.TriggerPlayerInteracted(interactable, this.gameObject);
                yield return new WaitForSeconds(0.3f);
        
                playerRenderer.DOFade(0, 0.3f)
                    .OnComplete(() =>
                    {
                        HideAndResetPlayer();
                        //Send Event to Finish Level
                        EventHandler.TriggerLevelFinished(true);
                    });
                
                break;
           
            case "Teleporter":
                playerRenderer.sprite = surprisedSprite;
                yield return new WaitForSeconds(0.1f);
                EventHandler.TriggerPlayerInteracted(interactable, this.gameObject);
                yield return new WaitForSeconds(0.8f);
                CheckGround();
                break;
            
            case "GravitySwitch":
                playerRenderer.sprite = surprisedSprite;
                yield return new WaitForSeconds(0.1f);
                Vector3 targetRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180f);
                EventHandler.TriggerPlayerInteracted(interactable, this.gameObject); //This only triggers on Interactables
                transform.DORotate(targetRotation, 1f)
                    .OnComplete(() =>
                    {
                        gravityDirection *= -1;
                        CheckGround();
                    });
                break;
                
            
            default:
                EventHandler.TriggerPlayerInteracted(interactable, this.gameObject);
                MoveFinished();
                break;
        }
        
    }
    
    public void MoveFinished()
    {
        transform.localScale = new Vector3(1, 1, 1);
        playerRenderer.sprite = normalSprite;
        
        Vector3Int exactRotation = Vector3Int.RoundToInt(transform.rotation.eulerAngles);
        transform.rotation = Quaternion.Euler(exactRotation);
        
        Vector3Int exactPosition = Vector3Int.RoundToInt(transform.position);
        transform.position = exactPosition;
        
        EventHandler.TriggerEnableRotation();
        
        playerRenderer.sortingOrder = SetSortingOrder();
        EnablePlayer();
    }

    private void RotatePlayer(Vector3 rotationVector, float rotationTime) //Send by CameraController
    {
        if (rotationVector.y > 0)
        {
            if (currentLevelRotation == 3) currentLevelRotation = 0;
            else currentLevelRotation++;
        }
        else
        {
            if (currentLevelRotation == 0) currentLevelRotation = 3;
            else currentLevelRotation--;
        }
        
        playerRenderer.sortingOrder = SetSortingOrder(); //Also sets MoveDirection
        
        Vector3 targetRotation = (transform.eulerAngles + rotationVector);
        transform.DORotate(targetRotation, rotationTime);

    }
    
    private void SetPlayerPositionAndShow(bool fadeIn, Vector3 newPosition)
    {
        
        //Set new Position
        transform.position = newPosition;
        playerRenderer.sortingOrder = SetSortingOrder();

        if (fadeIn)
        {
            //Fade-in player
            playerRenderer.DOFade(1, 0.5f)
                .OnComplete(() =>
                {
                    //Start Movement
                    MoveFinished();
                });
        }
        else
        {
            playerRenderer.color = Color.white;
            MoveFinished();
        }
            
        
    }
    
    private void HideAndResetPlayer()
    {
        DisablePlayer();
        playerRenderer.color = new Color(playerRenderer.color.r, playerRenderer.color.g, playerRenderer.color.b, 0);
        playerRenderer.sprite = normalSprite;
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        
    }
    
    private void EnablePlayer()
    {
        isEnabled = true;
    }
    
    private void DisablePlayer()
    {
        isEnabled = false;
    }
    
    private void KillPlayer()
    {
        HideAndResetPlayer();
        currentLevelRotation = 0;
        isEnabled = false;
        EventHandler.TriggerLevelFinished(false);
    }
    
    private int SetSortingOrder()
    {
        int distance = 0;
        int pos = 0;

        switch (currentLevelRotation)
        {
            case 0:
                pos = Mathf.RoundToInt(transform.position.z);
                distance = (pos * 10);
                moveDirection = new Vector3Int(1, 0, 0);
                break;
            
            case 1:
                pos = Mathf.RoundToInt(transform.position.x);
                distance = (pos * 10);
                moveDirection = new Vector3Int(0, 0, -1);
                break;
            
            case 2:
                pos = Mathf.RoundToInt(-(transform.position.z));
                distance = (pos * 10);
                moveDirection = new Vector3Int(-1, 0, 0);
                break;
            
            case 3:
                pos = Mathf.RoundToInt(-(transform.position.x));
                distance = (pos * 10);
                moveDirection = new Vector3Int(0, 0, 1);
                break;
        }
        return -(distance - 1);
    }

    
}
