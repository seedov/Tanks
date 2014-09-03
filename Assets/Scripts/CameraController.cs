using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public GameObject Target;
    public CameraMode CameraMode;
    public Camera Camera;
	// Use this for initialization
	void Start () {
   //     Camera = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Target == null) return;
        var tank = Target.GetComponent<TankController>();
        transform.position = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
        if (UIManager.mode == GameMode.SP)
            transform.rotation = Quaternion.AngleAxis(Target.transform.rotation.eulerAngles.y, Vector3.up);// Target.transform.rotation;// Quaternion.Euler(transform.rotation.x, Target.transform.rotation.y, transform.rotation.z);
        else if(UIManager.mode == GameMode.WWR)
      //      transform.rotation = tank.Tower.transform.rotation;
            transform.rotation = Quaternion.AngleAxis(tank.Tower.transform.rotation.eulerAngles.y, Vector3.up);//. Quaternion.Euler(tank.Tower.transform.rotation);
	}
}
public enum CameraMode
{
    SecondPerson,
    Arcade
}