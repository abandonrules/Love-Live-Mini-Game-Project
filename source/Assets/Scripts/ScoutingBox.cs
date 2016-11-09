using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoutingBox : MonoBehaviour {

    public Transform cardPrefab;
    public int cardCount;
    public Transform[] pathPoints;
    public List<Transform> cards;
    public List<Sprite> cardSprites;

    void Start()
    {
        cardSprites.AddRange(Resources.LoadAll<Sprite>("ScoutingBox/Cards"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach(Transform card in cards)
            {
                GameObject.Destroy(card.gameObject);
            }
            cards.Clear();
            cardCount = 0;
            InvokeRepeating("CreateCard", 0.0f, Random.Range(0.075f, 0.1f));
        }

        if (cardCount >= 10)
        {
            CancelInvoke();
        }
    }

    void CreateCard()
    {
        cardCount++;

        Transform card = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity) as Transform;
        cards.Add(card);
        int cardRarity = Random.Range(0, 4);
        Sprite tempCard = cardSprites[cardRarity];
        card.GetComponent<SpriteRenderer>().sprite = tempCard;
        card.name = tempCard.name;
        AnimateCardUp(card);
        RotateCard(card);
    }

    void AnimateCardUp(Transform card)
    {
        int randX = Random.Range(-8, 8);
        Hashtable hash = new Hashtable();
        hash.Add("position", new Vector3(randX, 8.0f, 0));
        hash.Add("islocal", true);
        hash.Add("time", 0.75f);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("oncomplete", "AnimateStop");
        hash.Add("oncompletetarget", this.gameObject);
        hash.Add("oncompleteparams", card);
        iTween.MoveTo(card.gameObject, hash);
    }

    void AnimateStop(Transform card)
    {
        card.localEulerAngles = Vector3.zero;
        iTween.Stop(card.gameObject);
    }

    void RotateCard(Transform card)
    {
        Hashtable hash = new Hashtable();
        hash.Add("amount", new Vector3(0, 0, 1.0f));
        hash.Add("time", 0.5f);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("looptype", "loop");
        iTween.RotateBy(card.gameObject, hash);
    }
}
