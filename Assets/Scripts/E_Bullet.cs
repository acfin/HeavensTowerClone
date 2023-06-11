using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    private Vector3 shootDir;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir= shootDir;
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        float moveSpeed = 10f;
        transform.position += moveSpeed * Time.deltaTime * shootDir;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Walls") || other.gameObject.CompareTag("Floor"))
            Die();
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerManager>().health -= 20;
            Die();
        }
    }
}
