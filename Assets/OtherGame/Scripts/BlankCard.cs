using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankCard : MonoBehaviour
{
    public int x = 0;
    public int y = 0;

    public void OnMouseUpAsButton(){
        Poker.CARD_CLICKED(this);
        //Debug.Log(x + " " + y);
    }
}
