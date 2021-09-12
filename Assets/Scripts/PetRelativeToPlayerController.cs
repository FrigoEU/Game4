using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetRelativeToPlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float playerSpeed = 10;

    [SerializeField]
    private float petSpeed = 18;

    [SerializeField]
    private float minDistance = 2;

    private Transform playerTransform;
    private Vector2 movement;

    [SerializeField]
    private GameObject pet;
    private Transform petTransform;
    private Vector2 petMovement;
    private PlayerControls controls;
    private PetFollowBehaviour petFollow;

    private bool petMovingAwayFromPlayer;
    private bool petMovingToPlayer;
    private bool petMovingClockwise;
    private bool petMovingCounterClockwise;

    public bool toggledControls = false;

    public bool lockRotationOfBack90Degs = true;

    [SerializeField]
    private CameraFollowMultiple mainCamera;

    void Awake()
    {
        controls = new PlayerControls();
        playerTransform = player.GetComponent<Transform>();
        petTransform = pet.GetComponent<Transform>();
        petFollow = pet.GetComponent<PetFollowBehaviour>();

        // Doesn't really gel well with these kind of controls
        petFollow.enabled = false;

        controls.Player.PetMove1.performed += _ => petMovingCounterClockwise = true;
        controls.Player.PetMove2.performed += _ => petMovingAwayFromPlayer = true;
        controls.Player.PetMove3.performed += _ => petMovingToPlayer = true;
        controls.Player.PetMove4.performed += _ => petMovingClockwise = true;

        controls.Player.PetMove1.canceled += _ => petMovingCounterClockwise = false;
        controls.Player.PetMove2.canceled += _ => petMovingAwayFromPlayer = false;
        controls.Player.PetMove3.canceled += _ => petMovingToPlayer = false;
        controls.Player.PetMove4.canceled += _ => petMovingClockwise = false;

        controls.Player.Move.performed += (ctx) => movement = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += (_) => movement = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        var movement = this.movement * Time.deltaTime;
        var movementV = new Vector3(movement.x, 0, movement.y);

        playerTransform.Translate(movementV * playerSpeed);
        petTransform.Translate(movementV * playerSpeed);

        Vector3 toPlayer = Vector3.Normalize(playerTransform.position - petTransform.position ) * petSpeed * Time.deltaTime;


        if (petMovingToPlayer && (petTransform.position - playerTransform.position).magnitude > minDistance)
        {
            petTransform.Translate(toPlayer);
        }
        if (petMovingAwayFromPlayer)
        {
            petTransform.Translate(-1 * toPlayer);
        }

        var radius2D = new Vector2((petTransform.position - playerTransform.position).x, (petTransform.position - playerTransform.position).z);
        var distance = petSpeed * Time.deltaTime;
        // angle between the vector from pet to player, and the vector of the desired next location ( = along the circle with the player as midpoint)
        var angle = (distance / radius2D.magnitude) * (180 / Mathf.PI);

        if (petMovingClockwise && petMovingCounterClockwise){
        } else {
            var petPlayerDiff = petTransform.position - playerTransform.position;
            var angleBetweenPetAndPlayer =
                Vector3.Angle(petTransform.position - playerTransform.position, new Vector3(1, 0 , 0)) // angle
                * (Vector3.Cross(petTransform.position - playerTransform.position, new Vector3(1, 0, 0)).y < 0 ? -1 : 1) // sign
                ;
            Debug.Log(angleBetweenPetAndPlayer);
            if (petMovingClockwise && (!this.lockRotationOfBack90Degs ||
                                       (this.lockRotationOfBack90Degs && (angleBetweenPetAndPlayer > 0
                                                                          || angleBetweenPetAndPlayer < -90
                                                                          || angleBetweenPetAndPlayer > -45))))
            {
                // movement = vector between pet and player current, minus vector between next location (= along the circle with player as midpoint)
                // next location = current location rotated along the angle
                // kind of a roundabout way, there is probably a more straightforward calculation, but couldn't find it atm
                var rotatedClockwise = Rotate(radius2D, angle);
                var clockwise2D = radius2D - rotatedClockwise;
                var clockwise = new Vector3(clockwise2D.x, 0, clockwise2D.y);

                petTransform.Translate(clockwise);
            }
            if (petMovingCounterClockwise && (!this.lockRotationOfBack90Degs ||
                                              (this.lockRotationOfBack90Degs && ( angleBetweenPetAndPlayer > 0
                                                                                || angleBetweenPetAndPlayer > -90
                                                                                 || angleBetweenPetAndPlayer < -135))))
            {
                var rotatedCounterclockwise = Rotate(radius2D, -1 * angle);
                var counterclockwise2D = radius2D - rotatedCounterclockwise;
                var counterclockwise = new Vector3(counterclockwise2D.x, 0, counterclockwise2D.y);

                petTransform.Translate(counterclockwise);
            }
        }


        if (Vector3.Distance(petTransform.position, playerTransform.position) > 10 && !mainCamera.targets.Contains(petTransform))
        {
            mainCamera.targets.Add(petTransform);
        }

        if (Vector3.Distance(petTransform.position, playerTransform.position) < 5) {
            mainCamera.targets.Remove(petTransform);
        }
    }

    void OnEnable(){
        controls.Enable();
    }
    void OnDisable(){
        controls.Disable();
    }

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
