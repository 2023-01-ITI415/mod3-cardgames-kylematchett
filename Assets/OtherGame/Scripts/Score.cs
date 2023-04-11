using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class Score : MonoBehaviour
{
    private static Score S;
    public static TMP_Text scoreText;
    public static TMP_Text highScoreText;

    void Start(){
        if (S != null) Debug.LogWarning("Attempt to set Singleton S again!");
        S = this;
    }

    public static void updateScore(){
        //int score = Poker.score;
        scoreText.text = "asdf";//$"Score: {score:#,###}";
    }
}
