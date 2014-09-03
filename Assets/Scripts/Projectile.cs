using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    ParticleSystem effect;
	// Use this for initialization
	void Start () {
        effect = GetComponentInChildren<ParticleSystem>();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        effect.Play();
        rigidbody.isKinematic = true;
        collider.enabled = false;
        var tank = collision.gameObject.GetComponent<TankController>();
        if(tank!=null)
            tank.ApplyDamage(1);
        //Destroy(gameObject);
    }
}
