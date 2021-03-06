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
    public StaminaBar stb;
    public GameObject SpotGrimper, pivot;
    CharacterController cc;
    public float moveSpeed = 5, oldMoveSpeed, oldColliderHeight;
    //public float rotateSpeed = 180f;
    public float jumpSpeed = 2; //Pas de saut pour l'instant
    public float gravity = -19.62f;
    Vector3 trait;
    float h, v;
    public Transform groundCheck; //crée un empty et le mettre au bas du personnage pour que la sphere crée détecte le sol correctement
    public float groundDistance = 0.4f; //Check le ground pour reset la velocity
    public LayerMask groundMask;
    Vector3 velocity;
    private Quaternion rotCur;
    private Vector3 posCur;
    public float offsetRay = 1;
    bool isGrounded;
    public float turnSmoothTime = 0.1f;
    public float stamina = 10f;
    public static bool moving = false, cacher;
    float turnSmoothVelocity;
    public Transform cam;
    Camera m_MainCamera;
    private bool accroupir = false, Rotation, positionBras, doOnce = false, essoufflement = false;
    public static bool canControl = true, wakeUp = false;
    public float animationLenghtWakeUp = 13.2f;
    public bool courrir = true, grimper, grimpant;
    private bool SUPERUSER = false;

    private static PlayersController instance;
    void Awake()
    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        m_MainCamera = Camera.main;
        courrir = true;
        cam = m_MainCamera.transform;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        pivot = GameObject.Find("pivot");
        SUPERUSER = gm.testeur;
        animator.SetBool("Dead", false);
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
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        }
        else if (!stb)
        {
            stb = gm.Staminabar.GetComponent<StaminaBar>();
        }
        /*else if (!pivot)
        {
            pivot = GameObject.Find("pivot");
        }*/
        else if (!lampeHuile)
        {
            //lampeHuile = GameObject.Find("Lampe à huile").GetComponent<LampeHuile>();
            lampeHuile = gm.LH;
        }
        else if (lampeHuile != null)
        {
            if (lampeHuile.EnMain && !accroupir)
            {
                if (!positionBras)
                {
                    rig.GetComponentInChildren<TwoBoneIKConstraint>().weight = 0.8f; //Le bras se met en place
                    rigHand.weight = 1f;
                    positionBras = true;
                }

                if (!Rotation)
                {
                    //lampeHuile.gameObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f); //le nouveau perso
                    lampeHuile.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    Rotation = true;
                }

            }
        }
        if (GameManager.gameOver == true)
        {
            animator.SetBool("Dead", true);
        }
        else
        {
            animator.SetBool("Dead", false);
        }
        if (SUPERUSER)
        {
            if (lampeHuile)
            {
                lampeHuile.currentHuile = 99999999f;
            }
            canControl = true;
        }

        else if (!canControl && !moving)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", false);


        }
        else if (moving)
        {
            animator.SetBool("IsWalking", true);
        }
        else if (!courrir && !accroupir && !cacher)
        {
            animator.SetBool("IsRunning", false);
            StaminaBar.instance.StopStamina();
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
        if (stb.currentStamina < 25)
        {
            if (!essoufflement)
            {
                AkSoundEngine.PostEvent("Essoufflement_Start", gameObject);
                essoufflement = true;
            }
        }
        else
        {
            essoufflement = false;
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
            if (Input.GetButtonDown("Sprint") && !accroupir && canControl && !cacher)
            {

                animator.SetBool("IsRunning", true);
                if (SUPERUSER)
                {
                    moveSpeed = moveSpeed * 1.4f;
                }
                else
                {
                    if (stb.currentStamina > 0 && courrir && isGrounded && !SUPERUSER)
                    {
                        stb.UseStamina(true);
                        if (stb.use)
                        {
                            moveSpeed = oldMoveSpeed * 1.4f;
                        }

                    }
                    else
                    {
                        animator.SetBool("IsRunning", false);
                        stb.StopStamina();
                        moveSpeed = oldMoveSpeed;

                    }
                }


            }
            else if (Input.GetButtonUp("Sprint") && !accroupir && canControl)
            {
                animator.SetBool("IsRunning", false);
                stb.StopStamina();
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
            if (stb)
            {
                stb.StopStamina();
            }

        }
        if (accroupir|| cacher)
        {
            if (h == 0 && v == 0 || direction.magnitude < 0.1f)
            {
                animator.SetFloat("SpeedMultiplier", 0f);
            }
            else if (direction.magnitude >= 0.1f)
            {
                animator.SetFloat("SpeedMultiplier", 1f);
            }
        }
        if (Input.GetButtonDown("Jump"))
        {

            /*trait = this.transform.position;
            trait.y = this.transform.position.y + offsetRay;
            RaycastHit hit;
            Debug.DrawRay(trait, this.transform.forward * 1, Color.white);
            if (Physics.Raycast(trait, this.transform.forward, out hit, 2f))
            {
                if (hit.transform.gameObject.tag == "Grimpe")
                {
                    grimper = true;
                    SpotGrimper = GameObject.Find("LookAtMe");//hit.transform.gameObject;
                    rotCur = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                    posCur = new Vector3(transform.position.x, hit.point.y, transform.position.z);

                }
                else
                {
                    //grimper = false;
                    SpotGrimper = null;
                }

            }*/

            if (isGrounded && canControl && !grimper && !wakeUp && !accroupir && !cacher)
            {
                animator.SetTrigger("jump");
                AkSoundEngine.PostEvent("Jump", gameObject);
                velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }
            /*else if (isGrounded && canControl && grimper && !wakeUp && !accroupir && !cacher)
            {
                //animation intégrer plus empecher de bouger

                //cc.enabled = false;
                if (!doOnce)
                {
                    canControl = false;
                    grimpant = true;
                    //transform.LookAt(new Vector3(SpotGrimper.transform.position.x, transform.position.y, SpotGrimper.transform.position.z));
                    transform.position = Vector3.Lerp(transform.position, posCur, Time.deltaTime * 5);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotCur, Time.deltaTime * 5);
                    doOnce = true;
                }
                animator.SetBool("Grimper", true);
                StartCoroutine(OnCompleteAnimation(5.3f));



            }*/
        }

        /*if (grimper)
        {
            float distance = Vector3.Distance(transform.position, pivot.transform.position);
            if (distance < 0.1f && doOnce == false)
            {
                grimpant = false; canControl = true;
            }
        }*/
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && canControl && !wakeUp)
        {

            if (!accroupir)
            {
                if (lampeHuile.EnMain)
                {
                    lampeHuile.gameObject.SetActive(false);
                    rig.GetComponentInChildren<TwoBoneIKConstraint>().weight = 0f;
                    rigHand.weight = 0f;
                }
                cc.height = oldColliderHeight / 4;
                moveSpeed = oldMoveSpeed / 4;
                cc.center = new Vector3(0, 0.38f, 0);
                accroupir = true;
                animator.SetBool("Accroupi", true);
                animator.SetBool("IsRunning", false);
                courrir = false;
            }
            else if (accroupir)
            {
                if (lampeHuile.EnMain)
                {
                    StartCoroutine(OnCompleteAccroupirAnimation(1f));
                }
                cc.height = oldColliderHeight;
                cc.center = new Vector3(0, 0.76f, 0);
                moveSpeed = oldMoveSpeed;
                accroupir = false;
                courrir = true;
                animator.SetBool("Accroupi", false);
            }

        }
        if (Input.GetKeyDown(KeyCode.A) && isGrounded && canControl && !accroupir && !wakeUp)
        {

            if (!cacher)
            {
                cc.height = oldColliderHeight / 2;
                cc.center = new Vector3(0, 0.38f, 0);
                moveSpeed = oldMoveSpeed / 2;
                cacher = true;
                animator.SetBool("Cacher", true);
                animator.SetBool("IsRunning", false);
                courrir = false;
            }
            else if (cacher)
            {
                cc.height = oldColliderHeight;
                cc.center = new Vector3(0, 0.76f, 0);
                moveSpeed = oldMoveSpeed;
                cacher = false;
                courrir = true;
                animator.SetBool("Cacher", false);
            }

        }
        cc.Move(velocity * Time.deltaTime);



        velocity.y += gravity * Time.deltaTime;



    }

    private IEnumerator OnCompleteAnimation(float animationLength)
    {

        yield return new WaitForSeconds(animationLength);
        transform.position = pivot.transform.position;
        animator.SetBool("Grimper", false);
        doOnce = false; grimper = false;
    }
    IEnumerator OnCompleteAccroupirAnimation(float animationLength)
    {
            yield return new WaitForSeconds(animationLength);
            lampeHuile.gameObject.SetActive(true); positionBras = false;
    }
    private IEnumerator AnimatorSetWakeUp(float animationLength)
    {
        animator.SetBool("WakeUp", true);
        AkSoundEngine.PostEvent("Lever", gameObject);
        cc.Move(velocity * Time.deltaTime);
        yield return new WaitForSeconds(animationLenghtWakeUp);
        animator.SetBool("WakeUp", false);
        wakeUp = false;
        canControl = true;
    }
}
