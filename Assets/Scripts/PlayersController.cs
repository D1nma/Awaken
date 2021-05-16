using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayersController : MonoBehaviour
{
    [SerializeField]
    public Rig rig = null;
    public Rig rigHand = null;
    LampeHuile lampeHuile;
    Animator animator;
    private GameManager gm;
    CharacterController cc;
    public float moveSpeed = 5, oldMoveSpeed, oldColliderHeight;
    //public float rotateSpeed = 180f;
    public float jumpSpeed = 2; //Pas de saut pour l'instant
    public float gravity = -19.62f;
    float h, v;
    public Transform groundCheck; //crée un empty et le mettre au bas du personnage pour que la sphere crée détecte le sol correctement
    public float groundDistance = 0.4f; //Check le ground pour reset la velocity
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    public float turnSmoothTime = 0.1f;
    public float stamina = 10f;
    public static bool moving = false;
    float turnSmoothVelocity;
    public Transform cam;
    Camera m_MainCamera;
    private bool accroupir = false;
    public static bool canControl = true, wakeUp = false;
    public float animationLenghtWakeUp = 11f;
    public bool courrir = true;
    private bool SUPERUSER = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        m_MainCamera = Camera.main;
        courrir = true;
        cam = m_MainCamera.transform;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        SUPERUSER = gm.testeur;
        if (!SUPERUSER)
        {
            transform.position = gm.lastCheckPointPos;
            canControl = false;
            wakeUp = true;
            StartCoroutine(AnimatorSetWakeUp(animationLenghtWakeUp));
        }
        else
        {
            Debug.Log("TESTEUR ACTIVE");
        }
        oldMoveSpeed = moveSpeed;
        oldColliderHeight = cc.height;
    }

    void Update()
    {
        if (!lampeHuile)
        {
            lampeHuile = GameObject.Find("Lampe à huile").GetComponent<LampeHuile>();
        }
        if (lampeHuile != null)
        {
            if (lampeHuile.EnMain)
            {
                rig.GetComponentInChildren<TwoBoneIKConstraint>().weight = 0.8f; //Le bras se met en place
                rigHand.weight = 1f;

                //lampeHuile.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f); //aucun effet
            }
        }
        if (SUPERUSER)
        {
            lampeHuile.currentHuile = 99999999f;
            canControl = true;
        }

        if (!canControl && !moving)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("jump", false);


        }
        if (moving)
        {
            Debug.Log(moving);
           animator.SetBool("IsWalking", true);
        }
        if (!courrir)
        {
            animator.SetBool("IsRunning", false);
            moveSpeed = 5;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("jump", false);
        }
        if (canControl)
        {
            if (cc.enabled == false)
            {
                cc.enabled = true;
            }
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
        else
        {
            h = 0;
            v = 0;
        }


        /*Vector3 wantedMovement = transform.forward * moveSpeed * v * Time.deltaTime;
        cc.Move(wantedMovement);
        transform.Rotate(Vector3.up * h * rotateSpeed * Time.deltaTime);*/ //Pas top solution différente trouvé

        /*Vector3 move = transform.right * h + transform.forward * v;
        cc.Move(move * moveSpeed * Time.deltaTime
        transform.Rotate(Vector3.up * h * rotateSpeed * Time.deltaTime);*/ //Méthode plutôt pour FirstPerson

        Vector3 direction = new Vector3(h, 0f, v).normalized; //normalized pour appuyer sur 2 touches et pas que ca aille plus vite
        if (direction.magnitude >= 0.1)
        {
            animator.SetBool("IsWalking", true);
            if (Input.GetButtonDown("Sprint") && !accroupir && canControl)
            {
                animator.SetBool("IsRunning", true);
                if (SUPERUSER)
                {
                    moveSpeed = moveSpeed * 1.4f;
                }
                else
                {
                    if (StaminaBar.instance.currentStamina > 0 && courrir && isGrounded && !SUPERUSER)
                    {
                        StaminaBar.instance.UseStamina(true);
                        if (StaminaBar.instance.use)
                        {
                            moveSpeed = moveSpeed * 1.4f;
                        }

                    }
                    else
                    {
                        animator.SetBool("IsRunning", false);
                        StaminaBar.instance.StopStamina();
                        moveSpeed = oldMoveSpeed;
                    }
                }


            }
            if (Input.GetButtonUp("Sprint") && !accroupir && canControl)
            {
                animator.SetBool("IsRunning", false);
                StaminaBar.instance.StopStamina();
                moveSpeed = oldMoveSpeed;
            }
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //Atan2 méthode math retourne l'angle entre 0° et x
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * moveSpeed * Time.deltaTime);


        }
        if (direction.magnitude <= 0.1 && !moving)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
        if (Input.GetButtonDown("Jump") && isGrounded && canControl)
        {
            animator.SetBool("jump", true);
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && canControl)
        {

            if (!accroupir)
            {
                cc.height = oldColliderHeight / 3;
                moveSpeed = oldMoveSpeed / 3;
                accroupir = true;
                animator.SetBool("Accroupi", true);
                animator.SetBool("IsRunning", false);
                courrir = false;
            }
            else
            {
                cc.height = oldColliderHeight;
                moveSpeed = oldMoveSpeed;
                accroupir = false;
                courrir = true;
                animator.SetBool("Accroupi", false);
            }

        }
        if (canControl)
        {
            cc.Move(velocity * Time.deltaTime);
        }


        velocity.y += gravity * Time.deltaTime;



    }
    private IEnumerator AnimatorSetWakeUp(float animationLength)
    {
        animator.SetBool("WakeUp", true);
        yield return new WaitForSeconds(animationLenghtWakeUp);
        animator.SetBool("WakeUp", false);
        wakeUp = false;
        canControl = true;
    }
}
