using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyDeath : MonoBehaviour
{
    private EnemyController enemyController;
    [SerializeField] GameObject[] coinPiles;
    bool isDead = false;
    int chanceForCopper = 50;
    int chanceForSilver = 30;
    // Chance for gold = 100 - chanceForCopper - chanceForSilver

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckEnemyDead())
        {
            SpawnCurrency();
        }
    }

    private int GetIndex()
    {
        int index = Random.Range(0, 101);
        if (index < chanceForCopper) { index = 0; }
        else if (index < (chanceForCopper + chanceForSilver)) { index = 1; }
        else { index = 2; }
        return index;
    }

    private bool CheckEnemyDead()
    {
        if (enemyController.getHP() <= 0 && !isDead)
        {
            return true;
        }
        return false;
    }

    private void SpawnCurrency()
    {
        int index = GetIndex();
        GameObject pile = coinPiles[index];
        pile.transform.position = enemyController.GetComponentInParent<Transform>().position;
        Ray ray = new Ray(pile.transform.position, -Vector3.up);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            pile.transform.position = hit.point;
        }
        Instantiate(pile);
        Debug.Log("Spawning pile of coins");
        isDead = true;
        Destroy(gameObject);
    }
}
