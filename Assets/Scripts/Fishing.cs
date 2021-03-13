using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{ // Set public variables in Inspector
    public GameObject pole;                    // Assign a fishing pole prefab in Inspector; tilt & angle to your liking
    public Transform lineStart;                // Create/assign empty GameObject at the tip of the pole

    Camera cam;
    Vector3 pos = new Vector3(200, 200, 0);
    public int minFishingDistance;            // Min distance (7ish)
    public int maxFishingDistance;            // Max distance (28ish) 
    public bool wasInterrupted;                // Interrupted flag (moved, mob attacked, etc); public for outside access
    public float fishJumpingSpeed;            // Adjust jumpming speed (3ish)

    private RaycastHit hit;
    private GameObject scriptTarget;        // Create primitive sphere for scriptTarget, replace with fish if hit
    private GameObject fish;                // Create primitive "fish" (replace with model if you have one)
    private GameObject setHook;                // Sphere appears to click
    private float fishJumpHeight;            // Height of fish jump
    private Vector3 startPos;                // Hold player starting position on cast
    private bool fishjump, fishOn, noNibble,EnMain;     // Simple states


    private void Awake()
    {
        wasInterrupted = false;

        // Set some defaults if unset
        if (maxFishingDistance <= 0)
        {
            maxFishingDistance = 28;
        }

        if (minFishingDistance < 0)
        {
            minFishingDistance = 1;
        }

        if (fishJumpingSpeed <= 0)
        {
            fishJumpingSpeed = 3f;
        }
    }

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }


    void Update()
    {
        Ray ray = cam.ScreenPointToRay(pos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        if (pole == null || lineStart == null)
        {
            return;
        }

        // Start Fishing via 'F'; modify for your game and don't do this at all
        // instead have your game's action dispatcher do GetComponent<QM_Fishing>().ActionDispatcher();
        // on hotkey press, icon click, whatever
        if (Input.GetKeyUp(KeyCode.E) && EnMain)
        {
            ActionListener();
        }
    }


    public void ActionListener()
    {

        /* Check your game's action dispatcher, make sure player isn't in combat, incapacitated, etc
        if(!playerHasControl)
            return;
        */

        /* If the fishing pole isn't already in your player's hand, swap it in
        if(playerDoesNotHavePole)...
        */
        Ray _ray = Camera.main.ScreenPointToRay(pos);

        // Assumes target Water has MeshCollider isTrigger=Yes and 
        // player can interact with layer Water is on (Water, by default)
        if (Physics.Raycast(_ray, out hit, Mathf.Infinity))
        {
            ActionChecks();
        }

        else
        {
            return;
        }
        ActionInit();
    }


    private void ActionInit()
    {
        // [Re]set states
        wasInterrupted = false;
        fishOn = false;
        fishjump = false;

        StopAllCoroutines();

        if (scriptTarget.gameObject != null)
        {
            DestroyImmediate(scriptTarget.gameObject);
        }



        if (pole.gameObject.GetComponent<LineRenderer>() != null)
        {
            DestroyImmediate(pole.gameObject.GetComponent<LineRenderer>());
        }

        if (setHook.gameObject != null)
        {
            DestroyImmediate(setHook.gameObject);
        }


    }

    private void ActionChecks()
    {
        // Did we hit fishable Water with Raycast? This is determined by simple bool on supplemental 
        // script QM_WaterProps; you could do something different of course. The idea is that our player should not
        // be able to fish everywhere there's water (i.e perhaps an indoor scene, sacred aqueduct, etc)

        if (hit.transform.GetComponent<FishingWater>() == null || hit.transform.GetComponent<FishingWater>().canBeFished == false)
        {
            return;
        }

        // Inventory check? Up to you if you want to check inventory now/return if it's full
        // or check at the very end; either way actual Inventory is out of scope for all of this

        // Check Distance
        float _dist = Vector3.Distance(transform.position, hit.point);

        if (_dist < minFishingDistance)
        {
            Debug.Log("You scare away the fish; try casting further away.");
            return;
        }

        if (_dist > maxFishingDistance)
        {
            Debug.Log("Out of range.");
            return;
        }

        // Demo create Primitive object instead of using a prefab
        scriptTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;

        // Set "bobber" properties
        scriptTarget.transform.position = hit.point;
        scriptTarget.GetComponent<Renderer>().material.color = Color.red;
        scriptTarget.transform.localScale = new Vector3(.25f, .5f, .25f);


        // Demo creating guiText
        GameObject fishingUIText = new GameObject();

        fishingUIText.AddComponent(typeof(Text));

        fishingUIText.transform.position = new Vector3(.43f, .8f);

        fishingUIText.GetComponent<Text>().fontSize = 18;
        fishingUIText.GetComponent<Text>().text = "You cast your line...";

        Destroy(fishingUIText, 2.5f);

        StartCoroutine(ActionMain());
    }


    private IEnumerator ActionMain()
    {

        // Scale according to your taste and WaitForSeconds value
        int fishingTimer = 1000;
        int thisRun = fishingTimer;
        bool runningMiniGame = false;

        // Randoms
        // How far out of water does fish breach? Shorter = more difficult
        fishJumpHeight = Random.Range(4, 7);
        Vector3 fishJumpTarget = new Vector3(hit.point.x, hit.point.y + fishJumpHeight, hit.point.z);

        // Random 0...1; noNibble means dead cast (but still cycles). You could adjust hard-coded ".2"
        // based on playerSkill, bait/gear quality, buff, etc
        float noBites = Random.value;
        if (noBites <= .10)
        {
            noNibble = true;
        }
        else
        {
            noNibble = false;
        }

        // Range (within fishingTimer) that fish will bite
        // Leave enough room for cycle to complete on low
        int biteAt = Random.Range(350, 500);

        // If LineRenderer components are somehow still around, get rid of them
        // to avoid errors, duplicate lines
        if (pole.gameObject.GetComponent<LineRenderer>() != null)
        {
            DestroyImmediate(pole.gameObject.GetComponent<LineRenderer>());
        }

        // Demo create LineRenderer via runtime script
        LineRenderer fishLine = pole.gameObject.AddComponent<LineRenderer>();
        Vector3 v1 = lineStart.position;
        Vector3 v2 = scriptTarget.transform.position;
        fishLine.startColor = Color.green;
        fishLine.endColor = Color.green;

        fishLine.startWidth = (0.005f);
        fishLine.endWidth = (0.007f);

        fishLine.positionCount = (2);
        fishLine.SetPosition(0, v1);
        fishLine.SetPosition(1, v2);
        fishLine.material = new Material(Shader.Find("Diffuse"));

        // Player movement cancels action
        Vector3 startPos = transform.position;

        // ------------- MAIN ----------------

        while (!wasInterrupted && thisRun > 0)
        {

            // Adjust LineRenderer component for animation sway, player rotate, fish jump, etc
            v1 = lineStart.position;
            fishLine.SetPosition(0, v1);

            v2 = scriptTarget.transform.position;
            fishLine.SetPosition(1, v2);


            if (!fishOn)
            {
                // Demo using Mathf.PingPong to achieve a little bounce to "bobber"
                float scrTgtY = Mathf.PingPong(Time.time, .1f) - .05f;
                scriptTarget.transform.position = new Vector3(scriptTarget.transform.position.x, scriptTarget.transform.position.y + scrTgtY, scriptTarget.transform.position.z);
            }

            // When thisRun equals Random biteAt, "Fish On!" (unless it's a dead cycle)
            if (!noNibble && thisRun == biteAt)
            {
                FishOn();
            }

            // Breach/jump fish; adjust "7" to your speed taste. Player has the interval between 
            // fish breach to apex of jump to react, otherwise fail

            if (fishjump)
            {
                scriptTarget.transform.Translate(Vector3.up * Time.deltaTime * fishJumpingSpeed);

                // Demo mini-game
                if (!runningMiniGame)
                {
                    StartCoroutine(MiniGame());
                    runningMiniGame = true;
                }

                // Has fish reached apex?
                if (scriptTarget.transform.position.y > fishJumpTarget.y)
                {
                    fishjump = false;
                    StopCoroutine("MiniGame()");
                    scriptTarget.AddComponent<Rigidbody>();
                    scriptTarget.GetComponent<Renderer>().material.color = Color.red;
                }
            }

            // If the fish falls back to water, cycle is over/fail; dual-purposing wasInterrupted
            // to end Coroutine
            if (fishOn && scriptTarget.transform.position.y < hit.point.y)
            {
                wasInterrupted = true;
            }

            // Movement cancels action
            if (startPos != transform.position)
            {
                Debug.Log("You stop fishing.");
                wasInterrupted = true;
            }

            // Decrement counter
            thisRun--;

            // If you change Wait len, remember to scale other values
            yield return new WaitForSeconds(0.01f);
        }

        // ------------- end WHILE ----------------

        if (noNibble)
        {

            GameObject fishingUIText = new GameObject();

            fishingUIText.AddComponent(typeof(Text));
            fishingUIText.transform.position = new Vector3(.43f, .85f);
            fishingUIText.GetComponent<Text>().fontSize = 18;
            fishingUIText.GetComponent<Text>().text = "The fish aren't biting";

            Destroy(fishingUIText, 2.5f);
        }

        // Reset states for next run
        ActionInit();

        // The End.

    }


    private IEnumerator MiniGame()
    {

        GameObject fishingUIText = new GameObject();

        fishingUIText.AddComponent(typeof(Text));

        fishingUIText.transform.position = new Vector3(.35f, .8f);

        fishingUIText.GetComponent<Text>().fontSize = 18;
        fishingUIText.GetComponent<Text>().color = Color.green;
        fishingUIText.GetComponent<Text>().text = "Set the hook! [Press G with Mouse in Circle]";

        Destroy(fishingUIText, 2);

        setHook = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;
        // Set "setHook" properties
        float setHookX = Random.Range(-3, 3);
        float setHookY = Random.Range(-1, 1);

        Vector3 setHookTarget = new Vector3(hit.point.x + setHookX, hit.point.y + fishJumpHeight + setHookY, hit.point.z);

        setHook.transform.position = setHookTarget;

        setHook.GetComponent<Renderer>().material.color = Color.green;

        setHook.transform.localScale = new Vector3(.7f, .7f, .7f);
        setHook.name = "setHook";

        bool miniGameDone = false;

        while (fishjump && !miniGameDone)
        {

            // Press G to set hook
            if (Input.GetKeyUp(KeyCode.G))
            {

                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Raycast(_ray, out hitInfo, Mathf.Infinity))
                {
                    if (hitInfo.transform.name == setHook.name)
                    {
                        DestroyImmediate(setHook);
                        DestroyImmediate(fishingUIText);
                        miniGameDone = ActionSuccess();
                    }
                }
            }

            yield return null;
        }

        DestroyImmediate(setHook);
        DestroyImmediate(fishingUIText);
    }

    private void FishOn()
    {

        // This bit uses a Primitive Sphere. If you have a fish model you could
        // use that instead, swapping out this code for that

        // Get rid of "bobber"
        DestroyImmediate(scriptTarget.gameObject);

        // Create "fish"
        scriptTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;

        // Set fish properties
        scriptTarget.transform.position = hit.point;
        scriptTarget.GetComponent<Renderer>().material.color = Color.yellow;
        scriptTarget.transform.localScale = new Vector3(.7f, 1.2f, .2f);

        fishOn = true;
        fishjump = true;
    }


    private bool ActionSuccess()
    {
        // This is success code; there's too many things that you could be doing in your game
        // for me to guess at: add fish to inventory, bump fishing skill, determine how fast they clicked vs
        // how far away the hit.point was and give treasure map for great click, spawn mermaid/man, 
        // bump Fishing GUI score, etc.

        // To close, we'll clean-up and put success message up

        GameObject successUIText = new GameObject();

        successUIText.AddComponent(typeof(Text));

        successUIText.transform.position = new Vector3(.35f, .85f);

        successUIText.GetComponent<Text>().fontSize = 18;
        successUIText.GetComponent<Text>().color = Color.white;
        successUIText.GetComponent<Text>().text = "You caught a nice fish";

        Destroy(successUIText, 2.5f);

        return true;
    }



}
