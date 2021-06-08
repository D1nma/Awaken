using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCode : MonoBehaviour
{
    public Transform[] SpotSpawn;
    public GameObject player;
    public static bool cheat;
    CharacterController cc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            if (!cc)
            {
                cc = player.GetComponent<CharacterController>();
            }
            else if (cc)
            {
                if (Input.GetKeyDown("[0]"))
                {
                    if (SpotSpawn[0])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[0].position)
                        {
                            player.transform.position = SpotSpawn[0].position;
                        }
                        else if (player.transform.position == SpotSpawn[0].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 0");
                    }
                }
                else if (Input.GetKeyDown("[1]"))
                {
                    if (SpotSpawn[1])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[1].position)
                        {
                            player.transform.position = SpotSpawn[1].position;
                        }
                        else if (player.transform.position == SpotSpawn[1].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 1");
                    }

                }
                else if (Input.GetKeyDown("[2]"))
                {
                    if (SpotSpawn[2])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[2].position)
                        {
                            player.transform.position = SpotSpawn[2].position;
                        }
                        else if (player.transform.position == SpotSpawn[2].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 2");
                    }

                }
                else if (Input.GetKeyDown("[3]"))
                {
                    if (SpotSpawn[3])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[3].position)
                        {
                            player.transform.position = SpotSpawn[3].position;
                        }
                        else if (player.transform.position == SpotSpawn[3].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 3");
                    }

                }
                else if (Input.GetKeyDown("[4]"))
                {
                    if (SpotSpawn[4])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[4].position)
                        {
                            player.transform.position = SpotSpawn[4].position;
                        }
                        else if (player.transform.position == SpotSpawn[4].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 4");
                    }

                }
                else if (Input.GetKeyDown("[5]"))
                {
                    if (SpotSpawn[5])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[5].position)
                        {
                            player.transform.position = SpotSpawn[5].position;
                        }
                        else if (player.transform.position == SpotSpawn[5].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 5");
                    }

                }
                else if (Input.GetKeyDown("[6]"))
                {
                    if (SpotSpawn[6])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[6].position)
                        {
                            player.transform.position = SpotSpawn[6].position;
                        }
                        else if (player.transform.position == SpotSpawn[6].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 6");
                    }

                }
                else if (Input.GetKeyDown("[7]"))
                {
                    if (SpotSpawn[7])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[7].position)
                        {
                            player.transform.position = SpotSpawn[7].position;
                        }
                        else if (player.transform.position == SpotSpawn[7].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 7");
                    }

                }
                else if (Input.GetKeyDown("[8]"))
                {
                    if (SpotSpawn[8])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[8].position)
                        {
                            player.transform.position = SpotSpawn[8].position;
                        }
                        else if (player.transform.position == SpotSpawn[8].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 8");
                    }

                }
                else if (Input.GetKeyDown("[9]"))
                {
                    if (SpotSpawn[9])
                    {
                        PlayersController.canControl = false;
                        cc.enabled = false;

                        if (player.transform.position != SpotSpawn[9].position)
                        {
                            player.transform.position = SpotSpawn[9].position;
                        }
                        else if (player.transform.position == SpotSpawn[9].position)
                        {
                            PlayersController.canControl = true;
                            cc.enabled = true;
                        }
                        Debug.Log("TP 9");
                    }

                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    if (cheat)
                    {
                        cheat = false;
                    }
                    else
                    {
                        cheat = true;
                    }
                }
            }
            
        }
        else if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
    }
}
