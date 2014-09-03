using UnityEngine;
using System.Collections;
using System;

public class Joystick : MonoBehaviour {
    public Camera cam;
    UICursor cursor;
    public Vector3 Direction;
    Touch touch;

	// Use this for initialization
	void Start () {
        cursor = GetComponentInChildren<UICursor>();
	}
	
	// Update is called once per frame
	void Update () {
	    UpdateDirection();
	}

    private void UpdateDirection()
    {
        var dir = new Vector3(cursor.transform.localPosition.x, cursor.transform.localPosition.z, cursor.transform.localPosition.y).normalized;
        if(dir != Direction) OnDirectionChanged();
        Direction = dir;

    }

    public void DraggingOver()
    {
        cursor.enabled = true;
        var pos = Input.mousePosition;// touch.position;
        pos.x = Mathf.Clamp01(pos.x / Screen.width);
        pos.y = Mathf.Clamp01(pos.y / Screen.height);
        cursor.transform.position = cam.ViewportToWorldPoint(pos);
    }

    public void DraggingOut()
    {
        cursor.enabled = false;
    }
    public void JoystickPressed()
    {
//        print(Input.touchCount);
 //       touch = Input.touches[Input.touchCount - 1];
    }
    public void JoystickReleased()
    {
    //    cursor.enabled = true;
        cursor.transform.localPosition = Vector3.zero;
    }

    public event Action DirectionChanged;
    public void OnDirectionChanged()
    {
        var h = DirectionChanged;
        if (h != null) h();
    }
}
