using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JsonParseDeck))]
// a
public class Deck : MonoBehaviour
{
    [Header("Inscribed")]
    public CardSpritesSO cardSprites;
    public GameObject prefabCard;
    public GameObject prefabSprite;
    public bool startFaceUp = true;
    public int GoldCards = 6;
    public int SilverCards = 6;
    public static int silvers = 6;
    public static int golds = 6;

    [Header("Dynamic")]
    public Transform deckAnchor;
    public List<Card> cards;

    private JsonParseDeck jsonDeck;
    static public GameObject SPRITE_PREFAB { get; private set; }


    /*
        void Start()
        {
            InitDeck();
            Shuffle(ref cards);
        }
    */



    /// <summary>
    /// The Prospector class will call InitDeck to set up the deck and build
    /// all 52 card GameObjects from the jsonDeck and cardSprites information.
    /// </summary> > 
    public void InitDeck()
    {
        // Create a static reference to spritePrefab for the Card class to use
        SPRITE_PREFAB = prefabSprite;
        // Call Init method on the CardSpriteSO instance assigned to cardSprites
        cardSprites.Init();

        // Get a reference to the JsonParseDeck component
        jsonDeck = GetComponent<JsonParseDeck>();
        // b

        // Create an anchor for all the Card GameObjects in the Hierarchy
        if (GameObject.Find("_Deck") == null)
        {
            // c
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        MakeCards();
    }



    bool contains(int[] arr, int x)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == x)
                return true;
        }
        return false;
    }


    /// <summary>
    /// Create a GameObject for each card in the deck.
    /// </summary>
    void MakeCards()
    {
        cards = new List<Card>();
        Card c;
        int[] gold = new int[GoldCards];

        int[] silver = new int[SilverCards];
        int[] used = new int[GoldCards + SilverCards];

        for (int i = 0; i < used.Length; i++)
            used[i] = -1;

        for (int i = 0; i < gold.Length; i++)
        {
            int n = Random.Range(0, 52);
            while (contains(used, n))
                n = Random.Range(0, 52);
            gold[i] = n;
            used[i] = n;

        }




        for (int i = 0; i < silver.Length; i++)
        {
            int n = Random.Range(0, 52);
            while (contains(used, n))
                n = Random.Range(0, 52);
            silver[i] = n;
            used[i + gold.Length] = n;
        }
        int count = 0;

        // Generate 13 cards for each suit
        string suits = "CDHS";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {

                Card.type cardType;
                // d
                if (contains(gold, count))
                    cardType = Card.type.gold;
                else if (contains(silver, count))
                    cardType = Card.type.silver;
                else
                    cardType = Card.type.normal;

                c = MakeCard(suits[i], j, cardType);
                // e

                //Debug.Log(c.cardType);
                cards.Add(c);

                // This aligns the cards in nice rows fortesting
                c.transform.position = new Vector3((j - 7) * 3, (i - 1.5f) * 4, 0);

                count++;
            }
        }
    }

    /// <summary>
    /// Creates a card GameObject based on suit and rank.
    /// Note that this method assumes it will be passed avalid suit and rank.
    /// </summary>
    /// <param name="suit">The suit of the card (e.g., ’C’)</param>
    /// <param name="rank">The rank from 1 to 13</param>
    /// <returns></returns>
    Card MakeCard(char suit, int rank, Card.type type)
    {
        GameObject go = Instantiate<GameObject>(prefabCard, deckAnchor); // f
        Card card = go.GetComponent<Card>();

        card.Init(suit, rank, type, startFaceUp);
        // g
        return card;
    }


    /// <summary>
    /// Shuffle a List(Card) and return the result to the original list. // b
    /// </summary>
    /// <param name="refCards">As a ref, this alters on the original list</param>
    static public void Shuffle(ref List<Card> refCards)
    {

        Shuffle2(ref refCards);
        // a
        // Create a temporary List to hold the new shuffle order
        List<Card> cards1 = new List<Card>();


        List<int> gscards = new List<int>();
        
        for (int i = 0; i < refCards.Count; i++)
        {
            if (refCards[i].cardType == Card.type.gold || refCards[i].cardType == Card.type.silver)
            {
                gscards.Add(i);
                
            }

        }
        gscards.Sort();
        gscards.Reverse();

        
        foreach (int i in gscards)
        {
            cards1.Add(refCards[i]);
            refCards.RemoveAt(i);
        }

        while (refCards.Count >= 25)
        {
            cards1.Add(refCards[0]);
            refCards.RemoveAt(0);
        }
        foreach(Card x in refCards){
            if(x.cardType == Card.type.gold || x.cardType == Card.type.silver)
                Debug.Log("here");
        }
        Shuffle2(ref cards1);
        foreach (Card c in refCards)
        {
            cards1.Add(c);
        }
        refCards = cards1;
        // c

    }

    


    static public void Shuffle2(ref List<Card> refCards)
    {
        // a

        // Create a temporary List to hold the new shuffle order
        List<Card> tCards = new List<Card>();

        int ndx; // This will hold the index of the card to be moved
                 // Repeat as long as there are cards in the original List
        while (refCards.Count > 0)
        {
            // Pick the index of a random card
            ndx = Random.Range(0, refCards.Count);
            // Add that card to the temporary List
            tCards.Add(refCards[ndx]);
            // And remove that card from the original List
            refCards.RemoveAt(ndx);
        }
        // Replace the original List with the temporary List
        refCards = tCards;
        return;// refCards;
        // c
    }


}

