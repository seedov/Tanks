using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NetworkController : MonoBehaviour {
    public GUIText TextForTank;
    public GameObject tankPrefab;
    private UIManager uiManager;
    private CameraController[] spies;

    public Color color;
    public Color Color
    {
        set
        {
            if (TankInstance != null)
            {
                color = value;
                TankInstance.Color = value;
            }
        }
    }
    private TankController TankInstance;

    int tanksCounter;
	// Use this for initialization
	void Start () {
#if SERVER
        Network.InitializeServer(10, 123456, false);
#else
       var err = Network.Connect("192.168.16.198", 123456);
       print(err);
       uiManager = FindObjectOfType<UIManager>();
       spies = FindObjectsOfType<CameraController>();
#endif
	}

    TankController SpawnTank()
    {
        var tank = Network.Instantiate(tankPrefab, Vector3.up, Quaternion.AngleAxis(0, Vector3.up), 0) as GameObject;
        
        var t = tank.GetComponent<TankController>();
        TankInstance = t;
   //     if(networkView.isMine)
        t.Color = color;// tanksCounter % 2 == 0 ? Color.red : Color.green;

        t.Died += TankDestroyed;
        t.text = TextForTank;
        
        uiManager.tank = t;
        return t;
    }

    private void TankDestroyed(TankController sender)
    {
        sender.Died -= TankDestroyed;
        StartCoroutine(WaitAndDestroy(sender));
        StartCoroutine(WaitAndSpawn());
    }

    IEnumerator WaitAndDestroy(TankController sender)
    {
        yield return new WaitForSeconds(1);
        Network.Destroy(sender.gameObject);
    }

    private IEnumerator WaitAndSpawn()
    {
        print("sp");
        yield return new WaitForSeconds(3);
        var tank = SpawnTank();
        tank.text = TextForTank;
  //      tank.Died += TankDestroyed;
    }

    void OnConnectedToServer()
    {
        var t = SpawnTank();
        //    t.Died += TankDestroyed;

        if (uiManager != null)
        {
            if (t == null) Debug.LogError("Tank isnull");
            uiManager.tank = t;
        }
        foreach (var spy in spies)
            spy.Target = t.gameObject;
    }



    [RPC]
    void SetColor(bool isRed)
    {
        Color = isRed ? Color.red : Color.green;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        tanksCounter++;
        networkView.RPC("SetColor", player, tanksCounter % 2 == 0);
//        var t = SpawnTank();
//        t.Died += TankDestroyed;

        //if (uiManager != null)
        //{
        //    if (t == null) Debug.LogError("Tank isnull");
        //    uiManager.tank = t;
        //}
        //foreach (var spy in spies)
        //    spy.Target = t.gameObject;

    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }


	
	// Update is called once per frame
	void Update () {
	
	}
}
