using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    private UIManager ui;
    private PlayersController pc;

    public Slider staminaBar;

    private int maxStamina = 200;
    public int currentStamina;
    private int energie = 1;

    public bool use;
    private float time = 0;
    bool doOnce;
    public static StaminaBar instance;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
        
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        ui.stamina.SetActive(false);
    }

    public void UseStamina(bool running)
    {
        if (running)
        {
            ui.stamina.SetActive(true);
            if (currentStamina - energie >= 0)
            {
                use = true;
                //currentStamina -= amount;
                //staminaBar.value = currentStamina;

                if (regen != null)
                    StopCoroutine(regen);
                doOnce = true;
            }
            else
            {
                Debug.Log("Pas assez d'endurance");
                running = false;
                use = false;
            }
        }

    }
    public void StopStamina()
    {
        use = false;
        if (doOnce)
        {
            regen = StartCoroutine(RegenStamina());
            doOnce = false;
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        if (!pc)
        {
            pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayersController>();
        }
        if (!ui)
        {
            ui = GameObject.Find("Canvas").GetComponent<UIManager>();
        }
        if(currentStamina>maxStamina)
        {
            currentStamina = maxStamina;
        }
        if (currentStamina == maxStamina)
        {
            //Debug.Log(time);
            time += Time.deltaTime;
            if (time >= 2)
            {
                ui.stamina.SetActive(false);
                time = 0;
            }
        }
        if (use)
        {
            if (currentStamina > 0 && pc.courrir)
            {
                currentStamina -= energie;
                staminaBar.value = currentStamina;
            }
            else
            {
                pc.courrir = false;
            }

        }
        if (currentStamina > 5)
        {
            pc.courrir = true;
        }
        if (currentStamina <= 0)
        {
            use = false;
            pc.courrir = false;
        }
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(3);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;
            yield return regenTick;
        }
        regen = null;
    }
}
