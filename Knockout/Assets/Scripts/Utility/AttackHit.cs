using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    [SerializeField] enum AttacksWhat {EnemyBase, Player};
    [SerializeField] AttacksWhat attacksWhat;
    int launchDirection = 1;
    [SerializeField] GameObject parent;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent(attacksWhat.ToString()) != null)
        {
            if (parent.transform.position.x < other.transform.position.x)
            {
                launchDirection = 1;
            }
            else
            {
                launchDirection = -1;
            }
            other.gameObject.GetComponent(attacksWhat.ToString()).SendMessage("Hit", launchDirection);
        }
    }
}
