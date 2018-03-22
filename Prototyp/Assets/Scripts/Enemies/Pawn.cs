using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    public int hp = 25;

    // Method for receiving damage
    public virtual void Damage()
    {
        hp -= 1;
    }
}
