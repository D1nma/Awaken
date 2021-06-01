using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
	float deltaTime = 0.0f;
	private bool Key;
	float buttonPressedTime;

    private void Start()
    {
		Key = false;
    }

    void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		if((Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F)) 
			|| (Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.LeftControl)) 
			|| (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.LeftControl)) 
			|| (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.LeftAlt))
			|| (Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftAlt))
			|| (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F)))
		{	
			buttonPressedTime = Time.time;
			if (buttonPressedTime > 2f)
            {
				if (Key)
				{
					Key = false;
					buttonPressedTime = 0;
				}
				else
				{
					Key = true;
					buttonPressedTime = 0;

				}
			}
        }
	}

	void OnGUI()
	{
        if (Key)
        {
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect(0, 0, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = new Color(0f, 11.3137083f, 0.808121622f, 1f);
			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
			GUI.Label(rect, text, style);
		}
		/*else if (!Key)
        {
				Event e = Event.current;
				if (e.isKey)
				{
					Debug.Log("key code: " + e.keyCode);
				}
		}*/
		
	}
}
