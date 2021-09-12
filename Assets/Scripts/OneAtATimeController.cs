using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneAtATimeController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float playerSpeed = 1;
    private float petSpeed = 10;
    private Transform playerTransform;
    private Vector2 movement;
    [SerializeField]
    private GameObject pet;
    private Transform petTransform;
    private Vector2 petMovement;
    private PlayerControls controls;
    private PetFollowBehaviour petFollow;

    private bool controllingPet;

    [SerializeField]
    private CameraFollowMultiple mainCamera;

    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls();
        playerTransform = player.GetComponent<Transform>();
        petTransform = pet.GetComponent<Transform>();
        petFollow = pet.GetComponent<PetFollowBehaviour>();

        controllingPet = false;

        controls.Player.Attack.performed += _ => { petFollow.enabled = false; controllingPet = true; mainCamera.targets.Add(petTransform); };
        controls.Player.Attack.canceled += _ => { petFollow.enabled = true; controllingPet = false; };

        controls.Player.Move.performed += (ctx) => movement = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += (_) => movement = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        var movement = this.movement * Time.deltaTime;
        var movementV = new Vector3(movement.x, 0, movement.y);
        if (controllingPet) {
            petTransform.Translate(movementV * petSpeed);
        } else {
            playerTransform.Translate(movementV * playerSpeed);
        }
        if (!controllingPet && Vector3.Distance(petTransform.position, playerTransform.position) < 5) {
            mainCamera.targets.Remove(petTransform);
        }
    }

    void OnEnable(){
        controls.Enable();
    } 
    void OnDisable(){
        controls.Disable();
    }
}
