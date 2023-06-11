using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 8;
	float xInput = 0, zInput = 0;
    Rigidbody rb;

	void Start () {
        rb = GetComponent<Rigidbody>();        
	}
    void Update()
    {
        GetInput();
        Movement();
    }
	void GetInput(){
		xInput = Input.GetAxis("Horizontal"); 
		zInput = Input.GetAxis("Vertical");
	}

	void Movement(){
		Vector3 tempPos = transform.position;
		tempPos += new Vector3(xInput,0, zInput) * speed * Time.deltaTime;
        rb.MovePosition(tempPos);
	}    
}
