using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersController : MonoBehaviour
{
    Animator animator;
    private GameManager gm;
    CharacterController cc;
    public float moveSpeed = 5,oldMoveSpeed,oldColliderHeight;
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
    float turnSmoothVelocity;
    public Transform cam;
    Camera m_MainCamera;
    private bool accroupir = false;
    public static bool canControl = true;
    public bool courrir = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        m_MainCamera = Camera.main;
        cam = m_MainCamera.transform;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        transform.position = gm.lastCheckPointPos;
        oldMoveSpeed = moveSpeed;
        oldColliderHeight = cc.height;
    }

    void Update()
    {
        if (canControl)
        {
            if (!courrir)
            {
                moveSpeed = 5;
            }
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

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
                if (Input.GetButtonDown("Sprint") && !accroupir)
                {
                    animator.SetBool("IsRunning", true);

                    if (StaminaBar.instance.currentStamina > 0 && courrir && isGrounded)
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
                if (Input.GetButtonUp("Sprint") && !accroupir)
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
            if (direction.magnitude <= 0.1)
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
            }
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                animator.SetTrigger("jump");
                velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
                
                if (!accroupir)
                {
                    cc.height = oldColliderHeight/2;
                    moveSpeed = oldMoveSpeed/2;
                    accroupir = true;
                }else
                {
                    cc.height = oldColliderHeight;
                    moveSpeed = oldMoveSpeed;
                    accroupir = false;
                }

            }
            velocity.y += gravity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);
        }
    }
}
