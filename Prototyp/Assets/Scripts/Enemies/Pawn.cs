using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    public int hp = 10;

    // Method for receiving damage
    public virtual void Damage()
    {
        hp -= 1;
    }
}
