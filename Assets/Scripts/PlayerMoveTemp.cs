using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTemp : MonoBehaviour
{
    private Transform playerT;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        playerT = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        playerT.position += new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed);
    }
}
