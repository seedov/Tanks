using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public GameObject projectile;
    public static GameMode mode;
//    public Camera SecondPersonCamera, IsometricCamera;
    public static Camera CurrentCamera;
    private Camera currentCamera;
    public CameraController SpyArcade, SpySP;
    Joystick joystick;

    public TankController _tank;
    public TankController tank
    {
        get { return _tank; }
        set
        {
            _tank = value;
            SpyArcade.Target = _tank.gameObject;
            SpySP.Target = _tank.gameObject;
        }
    }
	// Use this for initialization
	void Start () {
        joystick = GetComponentInChildren<Joystick>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (mode == GameMode.Arcade)
        {
            tank.MoveAhead(joystick.Direction);
        }
        else if (mode == GameMode.SP)
        {
            var dotMove = Vector3.Dot(joystick.Direction, Vector3.forward);
            var dotRotate = Vector3.Dot(joystick.Direction, Vector3.right);
            if (Mathf.Abs(dotMove) > 0.7f)
                if (dotMove > 0) tank.MoveForward();
                else tank.MoveBackward();
            if (Mathf.Abs(dotRotate) > 0.7f)
            {
                tank.Rotate(dotRotate > 0);
            }

            //tank.Rotate(joystick.Direction
        }
        else
        {
            if (isDragging)
            {
                Swiping();
            }
            
            var a = Mathf.Atan2(joystick.Direction.x, joystick.Direction.z)*Mathf.Rad2Deg;
            
            if (joystick.Direction != Vector3.zero)
            {
                var dotJoy = Vector3.Dot(joystick.Direction, Vector3.forward);
                
                var dir = Quaternion.AngleAxis(a, Vector3.up) * tank.TowerDirection;// - Quaternion..AngleAxis(tank.Tower.transform.rotation.y, Vector3.up) ;
                //print(string.Format("y = {0}, dir = {1}", tank.Tower.transform.localEulerAngles.y, joystick.Direction));
                tank.MoveBothDirections(dir);
            }
        }

	}

    public void ButtonShotClicked()
    {
        tank.Shot();
    }

    public void RbSecondPersonCheckedChanged(bool isChecked)
    {
        mode = GameMode.SP;
        RbSwitchCameraChanged();
    }
    public void RbArcadeCheckedChanged(bool isChecked)
    {
        mode = GameMode.Arcade;
        RbSwitchCameraChanged();
    }
    public void RbWWRCheckedChanged(bool isChecked)
    {
        mode = GameMode.WWR; 
        RbSwitchCameraChanged();
    }
    public void RbSwitchCameraChanged()
    {
        CurrentCamera = currentCamera = mode == GameMode.Arcade ? SpyArcade.Camera : SpySP.Camera;
        SpySP.Camera.gameObject.SetActive(mode == GameMode.SP || mode == GameMode.WWR);
        SpyArcade.Camera.gameObject.SetActive(mode == GameMode.Arcade);
    }
    public int z;
    public void GroundClicked()
    {
        return;
        var ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            tank.MoveToPoint(hit.point);
            //Instantiate(projectile, hit.point, transform.rotation);
        }
    }

    bool isDragging;
    Vector3 beginDragPosition;
    public void GroundPress()
    {
        isDragging = true;
        beginDragPosition = Input.mousePosition;
    }
    public void GroundReleased(){
        isDragging = false;
    }
    public void Swiping()
    {
        if (!isDragging) return;
        tank.RotateTower (Input.mousePosition.x > beginDragPosition.x);
  //      tank.Rotate(Input.mousePosition.x < beginDragPosition.x);

    }
    //public void JoystickPressed()
    //{
    //    print("joystick pressed");
    //}
    //public void JoystickDragged()
    //{
    //    print("joystick dragged");
    //}
    //public void JoystickReleased()
    //{
    //    print("joystick released");
    //}

}
public enum GameMode
{
    SP,
    Arcade,
    WWR
}