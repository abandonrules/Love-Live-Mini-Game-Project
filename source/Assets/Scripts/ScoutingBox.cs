using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ScoutingBox : MonoBehaviour {

    public Transform cardParent;
    public Transform idolParent;
    public Transform envelopePrefab;
    public Transform idolPrefab;
    public List<Transform> envelopeObjects;
    public List<Transform> idolObjects;
    public List<Sprite> envelopeSprites;
    public List<Sprite> idolSprites;
    public List<int> envelopePosition;

    public int currentIdol;

    [Header("References")]
    public List<string> museNames;
    public List<string> rarityType;
    public TextAsset museIds;
    public TextAsset aqoursIds;

    void Start()
    {
        museNames.AddRange(new List<string> { "Honoka", "Eli", "Kotori", "Umi", "Rin", "Maki", "Nozomi", "Hanayo", "Nico" });
        rarityType.AddRange(new List<string> { "R", "SR", "SSR", "UR" });
        envelopeSprites.AddRange(Resources.LoadAll<Sprite>("ScoutingBox/Cards"));

        envelopePosition.AddRange(new List<int> { -4, 4 });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (Transform card in envelopeObjects)
            {
                GameObject.Destroy(card.gameObject);
            }
            envelopeObjects.Clear();

            StartCoroutine("GetCardData");

            foreach(ScoutingList card in GameManager.Instance.scoutingList)
            {
                string[] idol = card.name.Split(":"[0]);
                string rarity = idol[0];
                string name = idol[1];
                //Debug.Log(" #" + card.ID + " " + rarity + " " + name);
                CreateCard(rarity, name, card.ID);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (Transform envelope in envelopeObjects)
            {
                StartCoroutine(AnimateEnvelopes(envelope));
            }

            StartCoroutine(ShowEnvelope(currentIdol));
        }
    }

    IEnumerator GetCardData()
    {
        for (int i = 0; i < 10; i++)
        {
            string rarity = GetRarity();
            string name = museNames[Random.Range(0, 9)];
            string idol = rarity + ":" + name;
            int id = GetCardId(name, rarity);
            GameManager.Instance.scoutingList.Add(new ScoutingList(idol, id));
        }

        yield return null;
    }

    string GetRarity()
    {
        int percent = Random.Range(1, 101);

        if (percent == 1)
        {
            return rarityType[3];
        }
        else if (percent > 1 && percent <= 5)
        {
            return rarityType[2];
        }
        else if (percent > 5 && percent <= 15)
        {
            return rarityType[1];
        }
        else if (percent > 15 && percent <= 100)
        {
            return rarityType[0];
        }


        return null;
    }

    int GetCardId(string name, string rarity)
    {
        string[] museData = museIds.text.Split("\n"[0]);
        bool foundIdol = false;

        foreach(string line in museData)
        {
            if (line.Contains(name))
            {
                foundIdol = true;
                continue;
            }

            if (foundIdol)
            {
                if (line.Contains(rarity))
                {
                    string[] removeHead = line.Split(":"[0]);
                    string[] ids = removeHead[1].Split(","[0]);
                    return int.Parse(ids[Random.Range(0, ids.Length)]);
                }
            }
        }

        return 0;
    }

    void CreateCard(string rarity, string name, int id)
    {
        float depthOffset = Random.Range(-0.2f, -0.1f);
        Transform envelope = Instantiate(envelopePrefab, new Vector3(0, 0, depthOffset), Quaternion.identity) as Transform;
        envelope.GetComponent<SpriteRenderer>().sprite = GetEnvelopeSprite(rarity);
        envelope.name = rarity;
        envelopeObjects.Add(envelope);

        Transform idol = Instantiate(idolPrefab, new Vector3(10f, -6f, depthOffset + 0.1f), Quaternion.identity) as Transform;
        idol.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GetIdolSprite(idol, id);
        idol.name = " #" + id + " " + name;
        idolObjects.Add(idol);

        envelope.parent = cardParent;
        idol.parent = idolParent;
    }

    void GetIdolSprite(Transform idol, int id)
    {
        string spriteURL = "http://i.schoolido.lu/cards/transparent/" + id + "Transparent.png";
        StartCoroutine(LoadImage(idol, spriteURL));
    }

    IEnumerator LoadImage(Transform idol, string url)
    {
        WWW www = new WWW(url);
        yield return www;
        idol.GetComponent<SpriteRenderer>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    Sprite GetEnvelopeSprite(string rarity)
    {
        Sprite type = null;

        switch(rarity)
        {
            case "R":
                type = envelopeSprites[0];
                break;
            case "SR":
                type = envelopeSprites[1];
                break;
            case "SSR":
                type = envelopeSprites[2];
                break;
            case "UR":
                type = envelopeSprites[3];
                break;
        }

        return type;
    }

    IEnumerator AnimateEnvelopes(Transform envelope)
    {
        yield return new WaitForSeconds(Random.Range(0.075f, 1.0f));
        AnimateEnvelopeUp(envelope);
        RotateEnvelopeUp(envelope);
    }

    void AnimateEnvelopeUp(Transform envelope)
    {
        int randX = Random.Range(-8, 8);
        Hashtable hash = new Hashtable();
        hash.Add("position", new Vector3(randX, 8.0f, 0));
        hash.Add("islocal", true);
        hash.Add("time", 0.75f);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("oncomplete", "AnimateStop");
        hash.Add("oncompletetarget", this.gameObject);
        hash.Add("oncompleteparams", envelope);
        iTween.MoveTo(envelope.gameObject, hash);
    }

    IEnumerator ShowEnvelope(int index)
    {
        if (currentIdol == 0)
        {
            yield return new WaitForSeconds(2.0f);
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
        }
        AnimateEnvelopeDown(envelopeObjects[index], idolObjects[index]);
        RotateEnvelopeDown(envelopeObjects[index]);
        currentIdol++;
    }

    void AnimateEnvelopeDown(Transform envelope, Transform idol)
    {
        Hashtable objectVals = new Hashtable();
        objectVals.Add("envelope", envelope);
        objectVals.Add("idol", idol);

        Hashtable hash = new Hashtable();

        if (currentIdol % 2 == 0)
        {
            envelope.localPosition = new Vector3(6.0f, envelope.localPosition.y, envelope.localPosition.z);
            envelope.localEulerAngles = new Vector3(0, 0, -25.0f);
            hash.Add("position", new Vector3(envelopePosition[1], -1.5f, 0));
        }
        else
        {
            envelope.localPosition = new Vector3(-6.0f, envelope.localPosition.y, envelope.localPosition.z);
            envelope.localEulerAngles = new Vector3(0, 0, 25.0f);
            hash.Add("position", new Vector3(envelopePosition[0], -1.5f, 0));
        }
        hash.Add("islocal", true);
        hash.Add("time", 0.75f);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("oncomplete", "AnimateIdol");
        hash.Add("oncompletetarget", this.gameObject);
        hash.Add("oncompleteparams", objectVals);
        iTween.MoveTo(envelope.gameObject, hash);
    }

    void AnimateIdol(object vals)
    {
        Hashtable v = (Hashtable)vals;
        Transform envelope = (Transform)v["envelope"];
        Transform idol = (Transform)v["idol"];

        if (envelope.localPosition.x == 4)
        {
            idol.localPosition = new Vector3(-2.0f, -6f, 0);
        }
        else if (envelope.localPosition.x == -4)
        {
            idol.localPosition = new Vector3(-10.0f, -6f, 0);
        }

        Hashtable position = new Hashtable();
        if (idol.localPosition.x == -2)
        {
            position.Add("position", new Vector3(-10.0f, -6f, 0));
        }
        else if (idol.localPosition.x == -10)
        {
            position.Add("position", new Vector3(-2.0f, -6f, 0));
        }        
        position.Add("islocal", true);
        position.Add("time", 0.75f);
        position.Add("easeType", iTween.EaseType.easeOutCirc);
        position.Add("oncomplete", "FadeOutIdol");
        position.Add("oncompletetarget", this.gameObject);
        position.Add("oncompleteparams", vals);
        iTween.MoveTo(idol.gameObject, position);

        Hashtable idolAlpha = new Hashtable();
        idolAlpha.Add("alpha", 0.0f);
        idolAlpha.Add("time", 0.15f);
        idolAlpha.Add("easeType", iTween.EaseType.linear);
        iTween.FadeFrom(idol.gameObject, idolAlpha);
    }

    void FadeOutIdol(object vals)
    {
        Hashtable v = (Hashtable)vals;
        Transform envelope = (Transform)v["envelope"];
        Transform idol = (Transform)v["idol"];

        Hashtable envelopeAlpha = new Hashtable();
        envelopeAlpha.Add("alpha", 0.0f);
        envelopeAlpha.Add("time", 0.15f);
        envelopeAlpha.Add("easeType", iTween.EaseType.linear);
        envelopeAlpha.Add("oncomplete", "IterateEnvelope");
        envelopeAlpha.Add("oncompletetarget", this.gameObject);
        envelopeAlpha.Add("oncompleteparams", vals);
        iTween.FadeTo(envelope.gameObject, envelopeAlpha);

        Hashtable idolAlpha = new Hashtable();
        idolAlpha.Add("alpha", 0.0f);
        idolAlpha.Add("time", 0.15f);
        idolAlpha.Add("easeType", iTween.EaseType.linear);
        iTween.FadeTo(idol.gameObject, idolAlpha);
    }

    void IterateEnvelope(object vals)
    {
        Hashtable v = (Hashtable)vals;
        Transform envelope = (Transform)v["envelope"];
        Transform idol = (Transform)v["idol"];

        Destroy(envelope.gameObject);
        Destroy(idol.gameObject);

        if (currentIdol < envelopeObjects.Count)
        {
            StartCoroutine(ShowEnvelope(currentIdol));
        }
    }

    void AnimateStop(Transform currObject)
    {
        currObject.localEulerAngles = Vector3.zero;
        iTween.Stop(currObject.gameObject);
    }

    void RotateEnvelopeUp(Transform envelope)
    {
        Hashtable hash = new Hashtable();
        hash.Add("amount", new Vector3(0, 0, 1.0f));
        hash.Add("time", 0.5f);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("looptype", "loop");
        iTween.RotateBy(envelope.gameObject, hash);
    }

    void RotateEnvelopeDown(Transform envelope)
    {
        Hashtable hash = new Hashtable();

        if (envelope.localPosition.x >= 0)
        {
            hash.Add("amount", new Vector3(0, 0, 1.0f));
        }
        else if (envelope.localPosition.x < 0)
        {
            hash.Add("amount", new Vector3(0, 0, -1.0f));
        }

        hash.Add("time", 0.75f);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("looptype", "none");
        iTween.RotateBy(envelope.gameObject, hash);
    }
}
