using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChibiLogic : MonoBehaviour
{
    public List<Sprite> muse;
    public List<Sprite> aqours;

    public List<AudioClip> muse_audio;
    public List<AudioClip> aqours_audio;
    public List<Sprite> bgSprites;

    public SpriteRenderer[] tempSprites;

    public AudioSource background;

    public bool isComplete;

    public Text chibiInfo;
    public Text gameInfo;

    [Header("Rankings Info")]
    public Text leaderInfo;
    public Image icon_rank1;
    public Image icon_rank2;
    public Image icon_rank3;
    public Text top3;
    public Text top8;
    public Text top13;
    public int rankingPageNum = 1;
    public int rankInterval;
    public int rankSubInterval;
    public bool showRankings;

    [Header("Countdown Timer")]
    public float currTime = 0;
    public float targetTime = 60;
    public Slider progressTimer;

    [Header("Transition")]
    public Image transition;

    void Start()
    {
        GameManager.Instance.chibiLogic = this.gameObject.GetComponent<ChibiLogic>();

        gameInfo.canvasRenderer.SetAlpha(1.0f);
        leaderInfo.canvasRenderer.SetAlpha(0.0f);

        icon_rank1.canvasRenderer.SetAlpha(0.0f);
        icon_rank2.canvasRenderer.SetAlpha(0.0f);
        icon_rank3.canvasRenderer.SetAlpha(0.0f);

        top3.canvasRenderer.SetAlpha(0.0f);
        top8.canvasRenderer.SetAlpha(0.0f);
        top13.canvasRenderer.SetAlpha(0.0f);

        transition.canvasRenderer.SetAlpha(0.0f);

        GetSprites();
        LoadSprites();
        GetLeaderboards();

        InvokeRepeating("Countdown", 1.0F, 1.0F);
    }

    void GetLeaderboards()
    {
        if (GameManager.Instance.leaderboard.Count > 0)
        {
            int counter = 1;
            string pageOne = "";
            string pageTwo = "";
            string pageThree = "";

            foreach (Leaderboards player in GameManager.Instance.leaderboard)
            {
                if (counter > 0 && counter <= 3)
                {
                    if (counter < 3)
                    {
                        pageOne += player.name + " - " + player.score + "\n";
                    }
                    else
                    {
                        pageOne += player.name + " - " + player.score;
                    }
                }
                if (counter > 3 && counter <= 8)
                {
                    if (counter < 8)
                    {
                        pageTwo += player.name + " - " + player.score + "\n";
                    }
                    else
                    {
                        pageTwo += player.name + " - " + player.score;
                    }
                }
                if (counter > 8 && counter <= 13)
                {
                    if (counter < 13)
                    {
                        pageThree += player.name + " - " + player.score + "\n";
                    }
                    else
                    {
                        pageThree += player.name + " - " + player.score;
                    }
                }
                if (counter > 13)
                {
                    break;
                }
                counter++;
            }

            if (counter < 2)
            {
                pageOne += "\n\n";
            }

            if (counter >= 2 && counter < 3)
            {
                pageOne += "\n";
            }

            top3.text = pageOne;
            top8.text = pageTwo;
            top13.text = pageThree;
        }
    }

    void Countdown()
    {
        currTime += 1;

        if (currTime >= targetTime)
        {
            CancelInvoke();
            GameManager.Instance.LoadNewScene("pokerface");
        }
        else if (currTime == targetTime - 1)
        {
            transition.canvasRenderer.SetAlpha(0.1f);
            transition.CrossFadeAlpha(1.0f, 1.0f, false);
        }
        else
        {
            FadeInHandler();
            progressTimer.value = currTime / targetTime;
        }
    }

    void GetSprites()
    {
        Sprite[] tempChibis = Resources.LoadAll<Sprite>("Loading/Chibis");
        AudioClip[] tempAudio = Resources.LoadAll<AudioClip>("Loading/Clips");
        bgSprites.AddRange(Resources.LoadAll<Sprite>("Loading/Backgrounds"));

        for (int i = 0; i < tempChibis.Length; i++)
        {
            if (i < 9)
            {
                muse.Add(tempChibis[i]);
                muse_audio.Add(tempAudio[i]);
            }
            else if (i >= 9 && i < 18)
            {
                aqours.Add(tempChibis[i]);
                aqours_audio.Add(tempAudio[i]);
            }
        }
    }

    void LoadSprites()
    {
        tempSprites = this.GetComponentsInChildren<SpriteRenderer>();

        if (GameManager.Instance.chibiType == 0)
        {
            GameManager.Instance.chibiType = Random.Range(1, 3);
        }

        if (GameManager.Instance.chibiType == 1)
        {
            LoadBGM(GameManager.Instance.chibiType);
            background.GetComponent<SpriteRenderer>().sprite = bgSprites[1];
            chibiInfo.text = "TYPE 'Team<Idol>' TO SUPPORT YOUR FAVORITE μ's IDOL";

            for (int i = 0; i < muse.Count; i++)
            {
                tempSprites[i].sprite = muse[i];
                tempSprites[i].name = muse[i].name.Substring(4, muse[i].name.Length - 4);
                tempSprites[i].gameObject.GetComponent<AudioSource>().clip = muse_audio[i];

                foreach (ChibiList chibi in GameManager.Instance.chibiList)
                {
                    if (chibi.name == tempSprites[i].name)
                    {
                        tempSprites[i].gameObject.GetComponentInChildren<TextMesh>().text = chibi.score.ToString();
                        break;
                    }
                }
            }
        }

        if (GameManager.Instance.chibiType == 2)
        {
            LoadBGM(GameManager.Instance.chibiType);
            background.GetComponent<SpriteRenderer>().sprite = bgSprites[0];
            chibiInfo.text = "TYPE 'Team<Idol>' TO SUPPORT YOUR FAVORITE Aqours IDOL";

            for (int i = 0; i < aqours.Count; i++)
            {
                tempSprites[i].sprite = aqours[i];
                tempSprites[i].name = aqours[i].name.Substring(4, aqours[i].name.Length - 4);
                tempSprites[i].gameObject.GetComponent<AudioSource>().clip = aqours_audio[i];

                foreach (ChibiList chibi in GameManager.Instance.chibiList)
                {
                    if (chibi.name == tempSprites[i].name)
                    {
                        tempSprites[i].gameObject.GetComponentInChildren<TextMesh>().text = chibi.score.ToString();
                        break;
                    }
                }
            }
        }

        if (GameManager.Instance.chibiType == 1)
        {
            GameManager.Instance.chibiType = 2;
        }
        else
        {
            GameManager.Instance.chibiType = 1;
        }
    }

    void LoadBGM(int bgmType)
    {
        AudioClip[] tempBGM = Resources.LoadAll<AudioClip>("Loading/Ambient");

        foreach (AudioClip bgm in tempBGM)
        {
            if (bgm.name == "mode_muse" && bgmType == 1)
            {
                background.clip = bgm;
                PlayBGM();
            }

            if (bgm.name == "mode_aqours" && bgmType == 2)
            {
                background.clip = bgm;
                PlayBGM();
            }
        }
    }

    void PlayBGM()
    {
        background.Play();
    }
    
    void FadeInHandler()
    {
        if (currTime > 0 && currTime % rankInterval == 0 && rankingPageNum == 1)
        {
            if (gameInfo.canvasRenderer.GetAlpha() == 1)
            {
                if (top3.text != "")
                {
                    FadeInRankPage1();
                }
            }

            if (gameInfo.canvasRenderer.GetAlpha() == 0)
            {
                FadeInGameInfo();
            }
        }

        if (top3.canvasRenderer.GetAlpha() == 1.0f && rankingPageNum == 1)
        {
            if (top8.text != "")
            {
                rankingPageNum = 2;
            }
        }
        if (top8.canvasRenderer.GetAlpha() == 1.0f && rankingPageNum == 2)
        {
            if (top13.text != "")
            {
                rankingPageNum = 3;
            }
            else
            {
                rankingPageNum = 1;
            }
        }
        if (top13.canvasRenderer.GetAlpha() == 1.0f && rankingPageNum == 3)
        {
            rankingPageNum = 4;
        }

        if (showRankings && rankingPageNum > 1 && currTime % rankSubInterval == 0)
        {
            if (rankingPageNum == 2)
            {
                FadeInRankPage2();
            }

            if (rankingPageNum == 3)
            {
                FadeInRankPage3();
            }

            if (rankingPageNum > 3)
            {
                top13.canvasRenderer.SetAlpha(1.0f);
                top13.CrossFadeAlpha(0.0f, 1.0f, false);

                rankingPageNum = 1;
            }
        }
    }

    void FadeInRankPage1()
    {
        gameInfo.canvasRenderer.SetAlpha(1.0f);
        gameInfo.CrossFadeAlpha(0.0f, 1.0f, false);

        leaderInfo.canvasRenderer.SetAlpha(0.1f);
        leaderInfo.CrossFadeAlpha(1.0f, 1.0f, false);

        icon_rank1.canvasRenderer.SetAlpha(0.1f);
        icon_rank1.CrossFadeAlpha(1.0f, 1.0f, false);
        icon_rank2.canvasRenderer.SetAlpha(0.1f);
        icon_rank2.CrossFadeAlpha(1.0f, 1.0f, false);
        icon_rank3.canvasRenderer.SetAlpha(0.1f);
        icon_rank3.CrossFadeAlpha(1.0f, 1.0f, false);

        top3.canvasRenderer.SetAlpha(0.1f);
        top3.CrossFadeAlpha(1.0f, 1.0f, false);

        showRankings = true;
    }

    void FadeInRankPage2()
    {
        icon_rank1.canvasRenderer.SetAlpha(1.0f);
        icon_rank1.CrossFadeAlpha(0.0f, 0.5f, false);
        icon_rank2.canvasRenderer.SetAlpha(1.0f);
        icon_rank2.CrossFadeAlpha(0.0f, 0.5f, false);
        icon_rank3.canvasRenderer.SetAlpha(1.0f);
        icon_rank3.CrossFadeAlpha(0.0f, 0.5f, false);

        top3.canvasRenderer.SetAlpha(1.0f);
        top3.CrossFadeAlpha(0.0f, 0.5f, false);

        top8.canvasRenderer.SetAlpha(0.1f);
        top8.CrossFadeAlpha(1.0f, 0.5f, false);
    }

    void FadeInRankPage3()
    {
        top8.canvasRenderer.SetAlpha(1.0f);
        top8.CrossFadeAlpha(0.0f, 1.0f, false);

        top13.canvasRenderer.SetAlpha(0.1f);
        top13.CrossFadeAlpha(1.0f, 1.0f, false);
    }

    void FadeInGameInfo()
    {
        leaderInfo.canvasRenderer.SetAlpha(1.0f);
        leaderInfo.CrossFadeAlpha(0.0f, 1.0f, false);

        if (top8.text == "")
        {
            icon_rank1.canvasRenderer.SetAlpha(1.0f);
            icon_rank1.CrossFadeAlpha(0.0f, 0.5f, false);
            icon_rank2.canvasRenderer.SetAlpha(1.0f);
            icon_rank2.CrossFadeAlpha(0.0f, 0.5f, false);
            icon_rank3.canvasRenderer.SetAlpha(1.0f);
            icon_rank3.CrossFadeAlpha(0.0f, 0.5f, false);

            top3.canvasRenderer.SetAlpha(1.0f);
            top3.CrossFadeAlpha(0.0f, 0.5f, false);
        }

        if (top13.text == "")
        {
            top8.canvasRenderer.SetAlpha(1.0f);
            top8.CrossFadeAlpha(0.0f, 1.0f, false);
        }

        gameInfo.canvasRenderer.SetAlpha(0.1f);
        gameInfo.CrossFadeAlpha(1.0f, 1.0f, false);

        showRankings = false;
    }

    void ChibiJump(object vals)
    {
        Hashtable ht = (Hashtable)vals;
        bool isRunning = (bool)ht["isRunning"];
        bool isComplete = (bool)ht["isComplete"];

        if (!isRunning && !isComplete)
        {
            isRunning = true;
            StartCoroutine(MoveUp(ht["chibiObject"] as Transform));
        }
    }

    public void CheckMessage(string name, string message)
    {
        for (int i = 0; i < tempSprites.Length; i++)
        {
            if (message.Length > 4)
            {
                if (message.Substring(0, 4).ToLower() == "team")
                {
                    if (message.Substring(4, message.Length - 4).ToLower() == tempSprites[i].name.ToLower())
                    {
                        int score = AddChibiVal(tempSprites[i].name.ToLower());
                        tempSprites[i].gameObject.GetComponentInChildren<TextMesh>().text = score.ToString();

                        Hashtable hash = new Hashtable();
                        hash.Add("isRunning", false);
                        hash.Add("isComplete", false);
                        hash.Add("chibiObject", tempSprites[i].gameObject.transform);
                        tempSprites[i].GetComponent<AudioSource>().Play();
                        ChibiJump(hash);
                    }
                }
            }
        }
    }

    int AddChibiVal(string name)
    {
        bool hasValue = false;

        foreach (ChibiList chibi in GameManager.Instance.chibiList)
        {
            if (chibi.name == name)
            {
                hasValue = true;
                int currScore = chibi.score;
                currScore += 1;
                chibi.score = currScore;
                GameManager.Instance.chibiList.Sort();
                return chibi.score;
            }
        }

        if (!hasValue)
        {
            GameManager.Instance.chibiList.Add(new ChibiList(name, 1));
            GameManager.Instance.chibiList.Sort();
            return 1;
        }

        return 0;
    }

    private IEnumerator MoveUp(Transform chibi)
    {
        Hashtable hash = new Hashtable();
        hash.Add("position", new Vector3(chibi.localPosition.x, -2.35F, 0));
        hash.Add("islocal", true);
        hash.Add("time", 0.2F);
        hash.Add("easeType", iTween.EaseType.linear);
        hash.Add("oncomplete", "MoveDown");
        hash.Add("oncompletetarget", this.gameObject);
        hash.Add("oncompleteparams", chibi);
        iTween.MoveTo(chibi.gameObject, hash);

        yield return null;
    }

    void MoveDown(Transform chibi)
    {
        Hashtable hash = new Hashtable();
        hash.Add("position", new Vector3(chibi.localPosition.x, -3.35F, 0));
        hash.Add("islocal", true);
        hash.Add("time", 0.2F);
        hash.Add("easeType", iTween.EaseType.linear);
        iTween.MoveTo(chibi.gameObject, hash);
        isComplete = true;
    }
}
