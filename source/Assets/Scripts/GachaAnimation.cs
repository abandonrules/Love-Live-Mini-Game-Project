using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GachaAnimation : MonoBehaviour
{
    [Header("Box Mods")]
    public int numOfCards;
    public enum BoxProperties
    {
        None,
        guaranteedSR,
        guaranteedSSR,
        guaranteedUR
        //SR_plus,
        //SSR_plus
    }
    public BoxProperties boxProperties;
    public enum IdolGroup
    {
        muse,
        aqours
    }
    public IdolGroup idolGroup;
    public enum MuseTypes
    {
        None,
        firstYears,
        secondYears,
        thirdYears,
        bibi,
        printemps,
        lilyWhite
    }
    public MuseTypes museTypes;
    public enum AqoursTypes
    {
        None,
        firstYears,
        secondYears,
        thirdYears,
        cyaron,
        azalea,
        guiltyKiss
    }
    public AqoursTypes aqoursTypes;

    [Header("Static References")]
    public Transform cardParent;
    public Transform idolParent;
    public Transform envelopePrefab;
    public Transform idolPrefab;
    public Transform transparency;
    public Material matTransparency;
    public List<Transform> envelopeObjects;
    public List<Transform> idolObjects;
    public List<Sprite> envelopeSprites;
    public List<AudioClip> envelopeClips;
    public List<Sprite> idolSprites;
    public List<float> envelopePosition;
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
        envelopeClips.AddRange(Resources.LoadAll<AudioClip>("ScoutingBox/Clips/Cards"));
        envelopePosition.AddRange(new List<float> { -4.5f, 4.5f });

        transparency.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        transparency.localPosition = new Vector3(0, 0, 10);
    }

    public bool IdolHandler()
    {
        foreach (Transform envelope in envelopeObjects)
        {
            GameObject.Destroy(envelope.gameObject);
        }
        envelopeObjects.Clear();

        StartCoroutine("GetIdolData");

        foreach (ScoutingList card in GameManager.Instance.scoutingList)
        {
            string[] idolData = card.name.Split(":"[0]);
            string rarity = idolData[0];
            string name = idolData[1];
            //Debug.Log(" #" + card.ID + " " + rarity + " " + name);
            CreateEnvelope(rarity, name, card.ID);
        }

        return true;
    }

    public void RevealGacha()
    {
        foreach (Transform envelope in envelopeObjects)
        {
            StartCoroutine(AnimateEnvelopes(envelope));
        }

        StartCoroutine(ShowEnvelope(currentIdol));
    }

    IEnumerator GetIdolData()
    {
        for (int i = 0; i < numOfCards; i++)
        {
            string rarity = GetRarity(i);
            string name = GetIdolNames();
            string idol = rarity + ":" + name;
            int id = GetIdolId(name, rarity);
            GameManager.Instance.scoutingList.Add(new ScoutingList(idol, id));
        }

        yield return null;
    }

    string GetRarity(int currentCard)
    {
        if (currentCard == numOfCards - 1)
        {
            if (boxProperties.ToString() == "None")
            {
                string rarity = NormalSearchRarity();
                return rarity;
            }
            else
            {
                if (boxProperties.ToString() == "guaranteedSR")
                {
                    return rarityType[1];
                }
                else if (boxProperties.ToString() == "guaranteedSSR")
                {
                    return rarityType[2];
                }
                else if (boxProperties.ToString() == "guaranteedUR")
                {
                    return rarityType[3];
                }
                /*  IterateRarity returning NULL
                else if (boxProperties.ToString() == "SR_plus")
                {
                    string rarity = IterateRarity("SR");
                    return rarity;
                }
                else if (boxProperties.ToString() == "SSR_plus")
                {
                    Debug.Log("start");
                    string rarity = IterateRarity("SSR");
                    Debug.Log(IterateRarity("SSR"));
                    return null;
                }
                */
            }
        }
        else
        {
            string rarity = NormalSearchRarity();
            return rarity;
        }
        
        return null;
    }

    string NormalSearchRarity()
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

    string IterateRarity(string rarity)
    {
        int percent = Random.Range(1, 101);
        Debug.Log(percent);
        if (percent == 1)
        {
            return rarityType[3];
        }
        else if (percent > 1 && percent <= 5)
        {
            Debug.Log("Hit " + rarityType[2]);
            return rarityType[2];
        }
        else if (percent > 5 && percent <= 15)
        {
            if (rarity == "SR")
            {
                return rarityType[1];
            }
            else
            {
                IterateRarity(rarity);
            }
        }
        else if (percent > 15 && percent <= 100)
        {
            IterateRarity(rarity);
        }

        return null;
    }

    string GetIdolNames()
    {
        List<string> tempList = new List<string>() { };

        if (idolGroup.ToString() == "muse")
        {
            if (museTypes.ToString() == "None")
            {
                return museNames[Random.Range(0, 9)];
            }
            else if (museTypes.ToString() == "firstYears")
            {
                tempList = new List<string> { museNames[4], museNames[5], museNames[7] };
            }
            else if (museTypes.ToString() == "secondYears")
            {
                tempList = new List<string> { museNames[0], museNames[2], museNames[3] };
            }
            else if (museTypes.ToString() == "thirdYears")
            {
                tempList = new List<string> { museNames[1], museNames[6], museNames[8] };
            }
            else if (museTypes.ToString() == "bibi")
            {
                tempList = new List<string> { museNames[1], museNames[5], museNames[8] };
            }
            else if (museTypes.ToString() == "printemps")
            {
                tempList = new List<string> { museNames[0], museNames[2], museNames[7] };
            }
            else if (museTypes.ToString() == "lilyWhite")
            {
                tempList = new List<string> { museNames[3], museNames[4], museNames[6] };
            }
        }
        else
        {
            //  Fill in Aqours types
        }

        return tempList[Random.Range(0, 3)];
    }

    int GetIdolId(string name, string rarity)
    {
        string[] museData = museIds.text.Split("\n"[0]);
        bool foundIdol = false;

        foreach (string line in museData)
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

    void CreateEnvelope(string rarity, string name, int id)
    {
        Transform envelope = Instantiate(envelopePrefab, new Vector3(-20, -2, 0), Quaternion.identity) as Transform;
        envelope.GetComponent<SpriteRenderer>().sprite = GetEnvelopeSprite(rarity);
        envelope.name = rarity;
        envelopeObjects.Add(envelope);

        Transform idol = Instantiate(idolPrefab, new Vector3(10f, -6f, envelope.localPosition.z - 0.15f), Quaternion.identity) as Transform;
        idol.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GetIdolSprite(idol, id);
        idol.name = " #" + id + " " + name;
        idolObjects.Add(idol);

        envelope.parent = cardParent;
        idol.parent = idolParent;
    }

    void GetIdolSprite(Transform idol, int id)
    {
        string spriteURL = "";
        if (id < 1019)
        {
            spriteURL = "http://i.schoolido.lu/cards/transparent/" + id + "Transparent.png";
        }
        else
        {
            spriteURL = "http://i.schoolido.lu/cards/transparent/navi_" + id + ".png";
        }
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

        switch (rarity)
        {
            case "R":
                type = envelopeSprites[4];
                break;
            case "SR":
                type = envelopeSprites[5];
                break;
            case "SSR":
                type = envelopeSprites[6];
                break;
            case "UR":
                type = envelopeSprites[7];
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
        envelope.localPosition = new Vector3(0, -2, 0);
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
            hash.Add("position", new Vector3(envelopePosition[1], -1.5f, -0.15f));
        }
        else
        {
            envelope.localPosition = new Vector3(-6.0f, envelope.localPosition.y, envelope.localPosition.z);
            envelope.localEulerAngles = new Vector3(0, 0, 25.0f);
            hash.Add("position", new Vector3(envelopePosition[0], -1.5f, -0.15f));
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

        envelope.GetComponent<SpriteRenderer>().sprite = OpenEnvelope(envelope);
        cardParent.GetComponent<AudioSource>().clip = GetEnvelopeAudio(envelope);
        cardParent.GetComponent<AudioSource>().Play();

        if (envelope.localPosition.x == envelopePosition[1])
        {
            idol.localPosition = new Vector3(-2.0f, -6f, -0.2f);
        }
        else if (envelope.localPosition.x == envelopePosition[0])
        {
            idol.localPosition = new Vector3(-10.0f, -6f, -0.2f);
        }

        Hashtable position = new Hashtable();
        if (idol.localPosition.x == -2)
        {
            position.Add("position", new Vector3(-10.0f, -6f, -0.2f));
        }
        else if (idol.localPosition.x == -10)
        {
            position.Add("position", new Vector3(-2.0f, -6f, -0.2f));
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

        Hashtable transparencyAlpha = new Hashtable();
        transparencyAlpha.Add("alpha", 0.0f);
        transparencyAlpha.Add("time", 0.15f);
        transparencyAlpha.Add("easeType", iTween.EaseType.linear);
        iTween.FadeFrom(transparency.gameObject, transparencyAlpha);

        transparency.localPosition = new Vector3(0, 0, -0.14f);
    }

    Sprite OpenEnvelope(Transform envelope)
    {
        string rarity = envelope.GetComponent<SpriteRenderer>().sprite.name;

        switch(rarity)
        {
            case "R":
                return envelopeSprites[0];
            case "SR":
                return envelopeSprites[1];
            case "SSR":
                return envelopeSprites[2];
            case "UR":
                return envelopeSprites[3];
            default:
                return envelope.GetComponent<SpriteRenderer>().sprite;
        }
    }

    AudioClip GetEnvelopeAudio(Transform envelope)
    {
        string rarity = envelope.GetComponent<SpriteRenderer>().sprite.name;

        switch (rarity)
        {
            case "_R":
                return envelopeClips[0];
            case "_SR":
                return envelopeClips[1];
            case "_SSR":
                return envelopeClips[2];
            case "_UR":
                return envelopeClips[3];
            default:
                return envelopeClips[0];
        }
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

        Hashtable transparencyAlpha = new Hashtable();
        transparencyAlpha.Add("alpha", 0.0f);
        transparencyAlpha.Add("time", 0.15f);
        transparencyAlpha.Add("easeType", iTween.EaseType.linear);
        transparencyAlpha.Add("oncomplete", "ResetTransparency");
        transparencyAlpha.Add("oncompletetarget", this.gameObject);
        iTween.FadeTo(transparency.gameObject, transparencyAlpha);
    }

    void ResetTransparency()
    {
        iTween.Stop(transparency.gameObject);
        transparency.GetComponent<SpriteRenderer>().material = matTransparency;
        transparency.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        transparency.localPosition = new Vector3(0, 0, 10);
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
