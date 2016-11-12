using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ScoutingBox : MonoBehaviour
{
    public Transform scoutingBox;
    public Animator boxLid;
    public Transform boxLight;
    public GachaAnimation gachaAnimation;

    public bool loadedIdolData;

    void Start()
    {
        gachaAnimation = this.GetComponent<GachaAnimation>();
        boxLight.localPosition = new Vector3(boxLight.localPosition.x, boxLight.localPosition.y, 3.0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            loadedIdolData = gachaAnimation.IdolHandler();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (loadedIdolData)
            {
                scoutingBox.GetComponent<AudioSource>().Play();
                StartBoxAnimation();
            }
        }

        if (boxLid.GetCurrentAnimatorStateInfo(0).IsName("Exit"))
        {
            //
        }
    }

    void StartBoxAnimation()
    {
        Hashtable boxParams = new Hashtable();
        Hashtable boxPosition = new Hashtable();

        boxParams.Add("canMoveUp", false);
        boxParams.Add("canShake", false);

        boxPosition.Add("position", new Vector3(0, 0, 0));
        boxPosition.Add("islocal", true);
        boxPosition.Add("time", 0.4f);
        boxPosition.Add("easeType", iTween.EaseType.easeOutQuad);
        boxPosition.Add("oncomplete", "AnimateBox");
        boxPosition.Add("oncompletetarget", this.gameObject);
        boxPosition.Add("oncompleteparams", boxParams);
        iTween.MoveTo(scoutingBox.gameObject, boxPosition);
    }

    void AnimateBox(object vals)
    {
        Hashtable v = (Hashtable)vals;
        bool canMoveUp = (bool)v["canMoveUp"];
        bool canShake = (bool)v["canShake"];

        Hashtable boxParams = new Hashtable();
        Hashtable boxPosition = new Hashtable();
        Hashtable boxRotation = new Hashtable();

        if (!canMoveUp && !canShake)
        {
            boxParams.Add("canMoveUp", true);
            boxParams.Add("canShake", true);
            boxParams.Add("isDone", false);

            boxPosition.Add("position", new Vector3(0, -2.0f, 0));
            boxPosition.Add("oncomplete", "AnimateBox");
            boxPosition.Add("oncompletetarget", this.gameObject);
            boxPosition.Add("oncompleteparams", boxParams);
        }
        else if (canMoveUp && canShake)
        {
            boxParams.Add("canMoveUp", false);
            boxParams.Add("canShake", true);
            boxParams.Add("isDone", false);

            boxPosition.Add("position", new Vector3(0, 0, 0));
            boxPosition.Add("oncomplete", "AnimateBox");
            boxPosition.Add("oncompletetarget", this.gameObject);
            boxPosition.Add("oncompleteparams", boxParams);

            boxRotation.Add("amount", new Vector3(0, 0, 20));
            boxRotation.Add("time", 0.8f);
            iTween.ShakeRotation(scoutingBox.gameObject, boxRotation);
        }
        else if (!canMoveUp && canShake)
        {
            boxParams.Add("canMoveUp", false);
            boxParams.Add("canShake", false);
            boxParams.Add("isDone", true);

            boxPosition.Add("position", new Vector3(0, -2.0f, 0));
            boxPosition.Add("oncomplete", "EndBoxAnimation");
            boxPosition.Add("oncompletetarget", this.gameObject);
        }

        boxPosition.Add("islocal", true);
        boxPosition.Add("time", 0.4f);
        boxPosition.Add("easeType", iTween.EaseType.easeOutQuad);

        iTween.MoveTo(scoutingBox.gameObject, boxPosition);
    }

    void EndBoxAnimation()
    {
        boxLid.SetBool("canOpenBox", true);

        boxLight.localPosition = new Vector3(boxLight.localPosition.x, boxLight.localPosition.y, 0);
        Hashtable light = new Hashtable();
        light.Add("alpha", 0.0f);
        light.Add("time", 0.15f);
        light.Add("easeType", iTween.EaseType.linear);
        iTween.FadeFrom(boxLight.gameObject, light);

        gachaAnimation.RevealGacha();
    }
}
