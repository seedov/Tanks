using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class TankController : MonoBehaviour {
    public GameObject Tower;
    MeshRenderer[] renderers;

    public float RotationSpeed=1;
    public float Speed=1;
    private int rechargeTime = 4;
    bool recharging, isMovingToPoint;
    public GameObject Projectile;
    public GameObject ProjectileSP;
    public Vector3 Direction, TowerDirection=Vector3.forward;
    public GUIText text;
    public int Healh = 5;
    private SpriteRenderer sprite;

    private UIManager uiManager;
    private CameraController[] spies;
 //   public GUIText TextForTank;

    public Color Color
    {
        set
        {
            if(renderers == null)
                renderers = GetComponentsInChildren<MeshRenderer>();
            foreach(var renderer in renderers)
                renderer.material.color = value;
        }
        get
        {
            return renderers[0].material.color;
        }
    }

	// Use this for initialization
	void Start () {
        TowerDirection = Vector3.forward;
  //      sprite = GetComponentInChildren<SpriteRenderer>();
  //      sprite.gameObject.SetActive(networkView.isMine);

   //     Color = rnd.Next(0, 2) == 0 ? Color.green : Color.red;
        text = GetComponentInChildren<GUIText>();
        if(Network.isClient)
            text.transform.parent = null;
        recharging = false;
        RotateTowards(0);
        if(text!=null)
            text.text = "";

        if (!networkView.isMine)
        {
            rigidbody.isKinematic = true;
        }
	}

    //void OnNetworkInstantiate(NetworkMessageInfo info)
    //{
    //    print("inst");
    //    if (Network.isServer) return;
    //    uiManager = FindObjectOfType<UIManager>();
    //    spies = FindObjectsOfType<CameraController>();

    //    var t = FindObjectsOfType<TankController>().Single(ta => ta.networkView.owner == info.sender);
    //    if (t == null) Debug.LogError("Tank isnull");
    //    t.text = TextForTank;
    //    if (uiManager != null)
    //    {

    //        uiManager.tank = t;
    //    }
    //    foreach (var spy in spies)
    //        spy.Target = t.gameObject;

    //}

    [RPC]
    private void ShotRPC()
    {
        UpdateTowerDirection();
        var p = Instantiate(Projectile) as GameObject;
  //      p.collider.enabled = networkView.isMine;
        p.transform.position = ProjectileSP.transform.position;
        p.rigidbody.AddForce(TowerDirection * 6000);
    }

    public void Shot()
    {
        if (recharging) return;
        StartCoroutine(Recharge());
        recharging = true;
        networkView.RPC("ShotRPC", RPCMode.All);
    }

    private IEnumerator Recharge()
    {
        text.text = "4";
        yield return new WaitForSeconds(1);
        text.text = "3";
        yield return new WaitForSeconds(1);
        text.text = "2";
        yield return new WaitForSeconds(1);
        text.text = "1";
        yield return new WaitForSeconds(1);
        text.text = "Ready";
        recharging = false;
        yield return new WaitForSeconds(1);
        text.text = "";
        
    }

    public void RotateTower(bool CW, float speed=0.5f)
    {
        var sign = CW ? 1 : -1;
        Tower.transform.Rotate(Vector3.up, sign*speed);
        UpdateTowerDirection();
    }
    private void UpdateTowerDirection()
    {
        var a = Tower.transform.eulerAngles.y;
        if (a > 180) a -= 360;
        // a = Mathf.Round(a);
        TowerDirection = Quaternion.AngleAxis(a, Vector3.up) * Vector3.forward;

    }


    public void Rotate(bool CW)
    {
        var sign = CW?1:-1;
        transform.Rotate(Vector3.up, sign * RotationSpeed);
        var a = transform.eulerAngles.y;
        if (a > 180) a -= 360;
        // a = Mathf.Round(a);
        Direction = Quaternion.AngleAxis(a, Vector3.up) * Vector3.forward;
        Direction.Normalize();
        UpdateTowerDirection();
    }

    public bool RotateTowards(float angle)
    {
        var a = transform.eulerAngles.y;
        if (a > 180) a -= 360;
       // a = Mathf.Round(a);
        Direction = Quaternion.AngleAxis(a, Vector3.up) * Vector3.forward;
        Direction.Normalize();

        var deltaAngle = Mathf.DeltaAngle(a, angle);


        if (Mathf.Abs(deltaAngle)<1) return false;

        var rot = Mathf.Sign(deltaAngle) * RotationSpeed;
        transform.Rotate(Vector3.up, rot);

        if(UIManager.mode == GameMode.WWR)
            RotateTower(deltaAngle < 0, RotationSpeed);

        return true;
    }

    

    public void MoveForward()
    {

        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }
    public void MoveBackward()
    {

        transform.Translate(-Vector3.forward * Time.deltaTime * Speed);
    }

    /// <summary>
    /// Двигаться в указанном направлении. Если направление больше чем на 90 градусов отличается от текущего - двигаться задним ходом
    /// </summary>
    /// <param name="direction"></param>
    public void MoveBothDirections(Vector3 direction)
    {
        var dotDir = Vector3.Dot(direction, Direction);
        if (dotDir > 0)
            MoveAhead(direction);
        else
            MoveBackward(direction);
    }

    /// <summary>
    /// Двигаться в указанном направлении всегда разворачиваясь передом по направлению движения
    /// </summary>
    /// <param name="direction"></param>
    public void MoveAhead(Vector3 direction)
    {
//        if (isMoving) return;
//        isMoving = true;
        if (direction == Vector3.zero) return;
        if(RotateTowards(Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg)) return;

        MoveForward();
    }

    /// <summary>
    /// Двигаться в указанном направлении всегда разворачиваясь задом по направлению движения
    /// </summary>
    /// <param name="direction"></param>
    public void MoveBackward(Vector3 direction)
    {
        var dir = -direction;
        if (RotateTowards(Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg)) return;

        MoveBackward();
    }

    private Vector3 destinationPoint;
    public void MoveToPoint(Vector3 destinationPoint)
    {
        this.destinationPoint = destinationPoint;
        isMovingToPoint = true;
    }
	
	void FixedUpdate () {
        if (!networkView.isMine)
        {
            text.text = Healh.ToString();
        }
        var p =  UIManager.CurrentCamera.WorldToViewportPoint(transform.position);// new Vector3(transform.position.x, transform.position.z, 0));
//        print(transform.position +"    "+p);
        text.transform.position = new Vector3(p.x, p.y+0.1f, 0);
        if (isMovingToPoint)
        {
            var direction = destinationPoint - transform.position;
            
            if (Mathf.Round(transform.position.x) == Mathf.Round(destinationPoint.x) && Mathf.Round(transform.position.z) == Mathf.Round(destinationPoint.z))
            {
                destinationPoint = Vector3.zero;
                isMovingToPoint = false;
            }
            else
                MoveAhead(direction);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveAhead(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveAhead(-Vector3.right);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveAhead(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveAhead(-Vector3.forward);
        }

        if (Input.GetKeyUp("enter"))
            Shot();
    }

    System.Random rnd = new System.Random();

    [RPC]
    void ApplyDamageRPC(int damage)
    {
        if(damage == 0)
        {
            text.text = "Not Hit";
            StartCoroutine(WaitAndClearText());
            return;
        }

        Healh -= damage;
        if (Healh == 0)
            OnDied();
        else
        {
            text.text = "Hit";
            StartCoroutine(WaitAndClearText());
        }
    }

    public void ApplyDamage(int damage)
    {
        if (!networkView.isMine) return;
        if (rnd.Next(0, 2) == 0)
        {
            networkView.RPC("ApplyDamageRPC", RPCMode.All, 0);
        }
        else
        {
            networkView.RPC("ApplyDamageRPC", RPCMode.All, damage);
        }
    }

    IEnumerator WaitAndClearText()
    {
        yield return new WaitForSeconds(2);
        text.text = "";
    }

    public event Action<TankController> Died;
    public void OnDied()
    {
        //привязать гуи текст обратно, чтобы он дестроился с танком
        text.transform.parent = transform;
        var h = Died;

        if (h != null)
        {
            
            h(this);
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        var towerRot = Quaternion.identity;
        var dir = Vector3.zero;
        var towerDir = Vector3.zero;
        var isRed = true;
        if (stream.isWriting)
        {
            pos = transform.position;
            rot = transform.rotation;
            towerRot = Tower.transform.rotation;
            dir = Direction;
            towerDir = TowerDirection;
            isRed = Color == Color.red;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref towerRot);
            stream.Serialize(ref dir);
            stream.Serialize(ref towerDir);
            stream.Serialize(ref isRed);
        }
        else
        {
            stream.Serialize(ref pos);
            transform.position = pos;
            stream.Serialize(ref rot);
            transform.rotation = rot;
            stream.Serialize(ref towerRot);
            Tower.transform.rotation = towerRot;
            stream.Serialize(ref dir);
            Direction = dir;
            stream.Serialize(ref towerDir);
            TowerDirection = towerDir;
            stream.Serialize(ref isRed);
            Color = isRed ? Color.red : Color.green;


        }
    }

    
}
