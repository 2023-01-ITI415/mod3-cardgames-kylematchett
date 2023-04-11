using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(Deck))]
// a
[RequireComponent(typeof(JsonParseLayout))]

public class Poker : MonoBehaviour
{
    private static Poker S;
    Card[,] cards = new Card[5, 5];
    public List<Card> drawPile;
    private JsonLayout jsonLayout;
    private Deck deck;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public GameObject blankPrefab;
    private int cardsPlayed = 0;
    public int score = 0;
    public int highScore=0;
    public int[] rows = new int[5];
    public int[] cols = new int[5];
    

    void Start()
    {


        if (S != null) Debug.LogError("Attempted to set S more than once!"); // b
        S = this;
        highScore = PlayerPrefs.GetInt("highscore",highScore);
        highScoreText.text = $"High Score: {highScore:#,###}";

        jsonLayout = GetComponent<JsonParseLayout>().layout;
        deck = GetComponent<Deck>();
        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);

        //Deck.Shuffle(ref deck.cards);
        drawPile = deck.cards;
        while (drawPile.Count > 25)
        {
            drawPile[25].gameObject.SetActive(false);
            drawPile.RemoveAt(25);
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject bc = Instantiate(blankPrefab, new Vector3(i * 4, j * 4, 0), Quaternion.identity);
                bc.GetComponent<BlankCard>().x = i;
                bc.GetComponent<BlankCard>().y = j;

            }
        }
        int count = 0;
        foreach (Card card in drawPile)
        {
            card.transform.position = new Vector3(-4, 7 + count * 0.25f, count);
            card.faceUp = false;
            card.SetSortingOrder(-1 * count);
            count++;
        }
        //drawPile[0].transform.position = new Vector3(-4,7,0);

        drawPile[0].faceUp = true;
        drawPile[0].SetSortingOrder(3);


    }

    Card Draw()
    {
        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        for (int i = 0; i < drawPile.Count; i++)
        {
            drawPile[i].transform.position = new Vector3(-4, drawPile[i].transform.position.y-0.25f, drawPile[i].transform.position.z - 1);

        }
        if (drawPile.Count > 0)
        {
            drawPile[0].SetSortingOrder(3);
            drawPile[0].faceUp = true;
        }
        return card;
    }


    static public void CARD_CLICKED(BlankCard card)
    {
        //Debug.Log(card.x + " " + card.y);
        Card c = S.Draw();
        c.transform.position = new Vector3(card.x * 4, card.y * 4, -1);
        S.cards[card.x, card.y] = c;
        S.cardsPlayed++;
        S.cols[card.x]++;
        S.rows[card.y]++;

        for(int i =0;i<5;i++){
            if(S.cols[i]==5){
                S.cols[i]++;
                List<Card> temp = new List<Card>();
                temp.Add(S.cards[i,0]);
                temp.Add(S.cards[i,1]);
                temp.Add(S.cards[i,2]);
                temp.Add(S.cards[i,3]);
                temp.Add(S.cards[i,4]);
                S.score+=evaluate(temp);
                    
            }
            if(S.rows[i]==5){
                S.rows[i]++;
                List<Card> temp = new List<Card>();
                temp.Add(S.cards[0,i]);
                temp.Add(S.cards[1,i]);
                temp.Add(S.cards[2,i]);
                temp.Add(S.cards[3,i]);
                temp.Add(S.cards[4,i]);
                S.score+=evaluate(temp);
            }
        }
        S.scoreText.text = $"Score: {S.score:#,###}";
        if(S.cardsPlayed==25)
            S.Game_End();
    }

    void Game_End(){
        
        if(score>highScore){
            highScore=score;
        }
        Debug.Log("here");
        PlayerPrefs.SetInt("highscore",highScore);
        highScoreText.text = $"High Score: {highScore:#,###}";
        
    }

    //royal flush: 100
    //straight flush: 75
    //four: 50
    //full house: 25
    //flush: 20
    //straight: 15
    //three: 10
    //two pair: 5
    //one pair: 2
    static int evaluate(List<Card> hand)
    {
        //sort hand
        Card low = hand[0];
        List<Card> sorted = new List<Card>();
        while (hand.Count != 0)
        {
            low = hand[0];
            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].rank < low.rank)
                {
                    low = hand[i];
                }
            }
            sorted.Add(low);
            hand.Remove(low);

        }

        //foreach(Card x in sorted)
        //Debug.Log(x.rank + " " + x.suit);



        bool flush = false;
        char suit = sorted[0].suit;
        int count = 0;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].suit == suit)
                count++;
            else
            {
                //Debug.Log("here");
                break;
            }
        }
        //Debug.Log("count: " + count);
        if (count == 5)
            flush = true;

        if (sorted[0].rank == 1 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11 && sorted[1].rank == 10)
        {
            //royal straight flush
            if (flush)
                return 100;

        }
        //normal
        if ((sorted[0].rank == 1) && (
        (sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[4].rank == 13 && sorted[3].rank == 12) ||
        (sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[3].rank == 4 && sorted[4].rank == 13)))
        {

            //straight flush
            if (flush)
                return 75;

        }



        //four
        if ((sorted[0].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank && sorted[2].rank == sorted[3].rank) ||
        (sorted[4].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank && sorted[2].rank == sorted[3].rank))
        {
            return 50;
        }

        //full house
        if ((sorted[0].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank && sorted[3].rank == sorted[4].rank) ||
        (sorted[0].rank == sorted[1].rank && sorted[2].rank == sorted[3].rank && sorted[3].rank == sorted[4].rank))
        {
            return 25;
        }

        //flush
        if (flush)
            return 20;

        //
        //straight
        int rank = sorted[0].rank;
        bool straight = false;

        for (int i = 0; i < 5; i++)
        {
            if (rank + i != sorted[i].rank)
            {
                straight = false;
                break;
            }
            straight = true;
        }
        if (straight)
            return 15;

        //straight over king ace
        if (sorted[0].rank == 1 && (
        (sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11 && sorted[1].rank == 10) ||
        (sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[1].rank == 2 && sorted[4].rank == 13 && sorted[3].rank == 12 && sorted[2].rank == 11) ||
        (sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[4].rank == 13 && sorted[3].rank == 12) ||
        (sorted[1].rank == 2 && sorted[2].rank == 3 && sorted[3].rank == 4 && sorted[4].rank == 13)))
        {

            return 15;
        }

        //three
        if ((sorted[0].rank == sorted[1].rank && sorted[1].rank == sorted[2].rank) ||
        (sorted[1].rank == sorted[2].rank && sorted[2].rank == sorted[3].rank) ||
        (sorted[2].rank == sorted[3].rank && sorted[3].rank == sorted[2].rank))
        {
            return 10;
        }

        //two pair
        if (sorted[0].rank == sorted[1].rank && ((sorted[2].rank == sorted[3].rank) || (sorted[3].rank == sorted[4].rank)) ||
        (sorted[1].rank == sorted[2].rank && sorted[3].rank == sorted[4].rank))
        {
            return 5;
        }

        //one pair
        if (sorted[0].rank == sorted[1].rank || sorted[1].rank == sorted[2].rank || sorted[2].rank == sorted[3].rank || sorted[3].rank == sorted[4].rank)
            return 2;

        //nothing
        return 0;
    }

}
