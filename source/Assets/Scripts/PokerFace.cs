using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PokerFace : MonoBehaviour {

    public AudioSource bgm;
    public Image transition;
    public Text roundNumber;
    public bool isNewPlayer;

    [Header("Instructions")]
    public Text instrTitle;
    public Text instr1;
    public Text instr2;
    public Text instr3;

    [Header("Countdown Timer")]
    public Text time;
    public Image progressBar;
    public int currTime;
    public int targetTime;
    public string countdown;

    [Header("Umi")]
    public List<AudioClip> umiClips;
    public Animator state;
    public bool fakeReaction;
    public bool fakeSound;

    [Header("Honoka")]
    public AudioSource honoka;
    public List<AudioClip> honokaClips;
    public int clipNumHonoka;

    [Header("Kotori")]
    public AudioSource kotori;
    public List<AudioClip> kotoriClips;
    public float timeToPlayKotori;
    public int clipNumKotori;
    public bool canPlayKotori;

    [Header("Hand")]
    public Transform hand;
    public string currentPos;
    public float timeToMove;
    public enum HandDirection
    {
        left,
        right
    }
    public HandDirection handDirection;
    public bool canMoveHand;

    [Header("Post-Game")]
    public Text cardLeft;
    public Text cardRight;
    public float totalLeft;
    private float leftPercent;
    public float totalRight;
    private float rightPercent;
    public bool canAdd;
    public int correctNum;
    public bool canRestart;
    private Vector3 handPosition;
    private Vector3 handRotation;
    private Vector3 handScale;

    void Start()
    {
        GameManager.Instance.pokerFace = this.gameObject.GetComponent<PokerFace>();
        roundNumber.text = "Round " + GameManager.Instance.roundNum.ToString();

        instrTitle.canvasRenderer.SetAlpha(0f);
        instr1.canvasRenderer.SetAlpha(0f);
        instr2.canvasRenderer.SetAlpha(0f);
        instr3.canvasRenderer.SetAlpha(0f);
        transition.canvasRenderer.SetAlpha(1.0f);

        AudioClip[] tempClips = Resources.LoadAll<AudioClip>("PokerFace/Clips");

        foreach(AudioClip clip in tempClips)
        {
            if (clip.name != "kotori_1" && clip.name != "kotori_2" && clip.name != "honk_1" && clip.name != "honk_2")
            {
                umiClips.Add(clip);
            }
            else if (clip.name == "honk_1" || clip.name == "honk_2")
            {
                honokaClips.Add(clip);
            }
            else
            {
                kotoriClips.Add(clip);
            }
        }

        bgm.Play();
        timeToPlayKotori = Random.Range(5.0f, 8.0f);
        timeToMove = Random.Range(5.0f, 8.0f);
        correctNum = Random.Range(0, 2);
        fakeReaction = (Random.value > 0.5f);
        fakeSound = (Random.value > 0.5f);
        currentPos = "middle";
        cardLeft.text = "0 [0%]";
        cardRight.text = "0 [0%]";

        InvokeRepeating("Countdown", 1.0F, 1.0F);
    }

    void Countdown()
    {
        currTime -= 1;

        int minutes = Mathf.FloorToInt(currTime / 60);
        int seconds = Mathf.FloorToInt(currTime - minutes * 60);
        countdown = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (currTime == 67)
        {
            instrTitle.canvasRenderer.SetAlpha(0.1f);
            instrTitle.CrossFadeAlpha(1.0f, 0.5f, false);
            instr1.canvasRenderer.SetAlpha(0.1f);
            instr1.CrossFadeAlpha(1.0f, 0.5f, false);
        }

        if (currTime == 65)
        {
            instr1.canvasRenderer.SetAlpha(1.0f);
            instr1.CrossFadeAlpha(0f, 0.5f, false);
            instr2.canvasRenderer.SetAlpha(0.1f);
            instr2.CrossFadeAlpha(1.0f, 0.5f, false);
        }

        if (currTime == 63)
        {
            instr2.canvasRenderer.SetAlpha(1.0f);
            instr2.CrossFadeAlpha(0f, 0.5f, false);
            instr3.canvasRenderer.SetAlpha(0.1f);
            instr3.CrossFadeAlpha(1.0f, 0.5f, false);
        }

        if (currTime == 63)
        {
            UmiHandler("intro");
        }

        if (currTime == 60)
        {
            instrTitle.canvasRenderer.SetAlpha(1.0f);
            instrTitle.CrossFadeAlpha(0f, 0.2f, false);
            instr3.canvasRenderer.SetAlpha(1.0f);
            instr3.CrossFadeAlpha(0f, 0.2f, false);
            TransitionHandler(false);
        }

        if (currTime > 0 && currTime <= 60)
        {
            time.text = countdown;
            float progress = (currTime * 1.0f) / 60;
            progressBar.fillAmount = progress;
        }

        if (currTime == 0)
        {
            bgm.Stop();
            timeToPlayKotori = 0;
            timeToMove = 0;
            if (correctNum == 0)
            {
                MoveHand("left");
            }
            else
            {
                MoveHand("right");
            }
            PulseText("end");
            AnimateUmi("end");
            UmiHandler("end");
            StartCoroutine(WaitToEnd(10.0f));
        }
    }

    IEnumerator WaitToEnd(float time)
    {
        yield return new WaitForSeconds(time);
    
        canRestart = RewardPlayers();
        if (canRestart)
        {
            GameManager.Instance.pokerList.Clear();
            if (GameManager.Instance.roundNum == GameManager.Instance.targetRoundNum)
            {
                GameManager.Instance.SwitchToLoading();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void Update()
    {
        CheckTime();
        VoteHandler();
    }

    public void CheckMessage(string name, string message)
    {
        foreach (PokerList player in GameManager.Instance.pokerList)
        {
            if (player.name == name)
            {
                return;
            }
        }

        if (currTime > 0 && currTime <= 60)
        {
            if (message.ToLower() == "left")
            {
                totalLeft++;
                leftPercent = (totalLeft / (totalLeft + totalRight)) * 100;
                rightPercent = (totalRight / (totalLeft + totalRight)) * 100;
                GameManager.Instance.pokerList.Add(new PokerList(name, 0));
                PulseText("left");
            }
            else if (message.ToLower() == "right")
            {
                totalRight++;
                leftPercent = (totalLeft / (totalLeft + totalRight)) * 100;
                rightPercent = (totalRight / (totalLeft + totalRight)) * 100;
                GameManager.Instance.pokerList.Add(new PokerList(name, 1));
                PulseText("right");
            }
        }
    }

    void HandHandler()
    {
        if (!canMoveHand)
        {
            timeToMove -= Time.deltaTime;
            if (timeToMove <= 0)
            {
                timeToMove = Random.Range(1.0f, 5.0f);
                canMoveHand = true;
            }
        }
        else
        {
            HandDirection direction = (HandDirection)Random.Range(0, 2);
            if (currentPos != direction.ToString())
            {
                AnimateUmi(direction.ToString());
                MoveHand(direction.ToString());
                currentPos = direction.ToString();
                canMoveHand = false;
            }
        }
    }

    void MoveHand(string direction)
    {
        Hashtable hash = new Hashtable();
        switch (direction)
        {
            case "left":
                hash.Add("position", new Vector3(0.9f, hand.localPosition.y, 0));
                break;
            case "right":
                hash.Add("position", new Vector3(7.1f, hand.localPosition.y, 0));
                break;
            case "middle":
                hash.Add("position", new Vector3(4.0f, hand.localPosition.y, 0));
                break;
        }
        hash.Add("islocal", true);
        hash.Add("time", 0.4F);
        hash.Add("easeType", iTween.EaseType.easeInOutExpo);
        if (currTime == 0)
        {
            hash.Add("oncomplete", "SwitchHand");
            hash.Add("oncompletetarget", this.gameObject);
        }
        iTween.MoveTo(hand.gameObject, hash);
    }

    void SwitchHand()
    {
        if (correctNum == 0)
        {
            handPosition = new Vector3(4, -0.25f, 0);
            handRotation = new Vector3(0, 0, -0.94f);
            handScale = new Vector3(1.4f, 1.25f, 1);
            hand.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PokerFace/Images/hand_1");
        }
        else
        {
            handPosition = new Vector3(6.73f, -0.43f, 0);
            handRotation = new Vector3(0, 0, -0.23f);
            handScale = new Vector3(1.25f, 1.25f, 1);
            hand.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PokerFace/Images/hand_2");
        }

        hand.transform.localPosition = handPosition;
        hand.transform.localEulerAngles = handRotation;
        hand.transform.localScale = handScale;
    }

    void AnimateUmi(string type)
    {
        if (type == "left")
        {
            if (!fakeReaction && correctNum == 0)
            {
                if (!fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                
                state.SetBool("isScared", true);
                state.SetBool("isHappy", false);
            }
            else if (!fakeReaction && correctNum != 0)
            {
                if (fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isHappy", true);
                state.SetBool("isScared", false);
            }
            else if (fakeReaction && correctNum == 0)
            {
                if (fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isHappy", true);
                state.SetBool("isScared", false);
            }
            else if (fakeReaction && correctNum != 0)
            {
                if (!fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isScared", true);
                state.SetBool("isHappy", false);
            }
        }
        else if (type == "right")
        {
            if (!fakeReaction && correctNum == 1)
            {
                if (!fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isScared", true);
                state.SetBool("isHappy", false);
            }
            else if (!fakeReaction && correctNum != 1)
            {
                if (fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isHappy", true);
                state.SetBool("isScared", false);
            }
            else if (fakeReaction && correctNum == 1)
            {
                if (fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isHappy", true);
                state.SetBool("isScared", false);
            }
            else if (fakeReaction && correctNum != 1)
            {
                if (!fakeSound)
                {
                    UmiHandler("sad");
                }
                else
                {
                    UmiHandler("happy");
                }
                state.SetBool("isScared", true);
                state.SetBool("isHappy", false);
            }
        }
        else if (type == "middle")
        {
            state.SetBool("isHappy", false);
            state.SetBool("isScared", false);
        }
        else if (type == "end")
        {
            state.SetBool("isHappy", false);
            state.SetBool("isScared", false);
            state.SetBool("isDone", true);
        }
    }

    void VoteHandler()
    {
        cardLeft.text = totalLeft + " [" + Mathf.RoundToInt(leftPercent) + "%]";
        cardRight.text = totalRight + " [" + Mathf.RoundToInt(rightPercent) + "%]";
    }

    void PulseText(string type)
    {
        Hashtable hash = new Hashtable();

        if (currTime > 0)
        {
            hash.Add("amount", new Vector3(0.15f, 0.15f, 0.15f));
            hash.Add("time", 0.5F);
            hash.Add("easeType", iTween.EaseType.linear);

            if (type == "left")
            {
                iTween.PunchScale(cardLeft.gameObject, hash);
            }
            else
            {
                iTween.PunchScale(cardRight.gameObject, hash);
            }
        }
        else
        {
            if (type == "end")
            {
                hash.Add("amount", new Vector3(0.30f, 0.30f, 0.30f));
                hash.Add("time", 1.0F);
                hash.Add("easeType", iTween.EaseType.linear);
                hash.Add("oncomplete", "PulseText");
                hash.Add("oncompletetarget", this.gameObject);
                hash.Add("oncompleteparams", "end");

                if (correctNum == 0)
                {
                    iTween.PunchScale(cardLeft.gameObject, hash);
                    cardLeft.color = Color.green;
                }
                else
                {
                    iTween.PunchScale(cardRight.gameObject, hash);
                    cardRight.color = Color.green;
                }
            }
        }
    }

    bool RewardPlayers()
    {
        if (GameManager.Instance.leaderboard.Count <= 0)
        {
            foreach (PokerList player in GameManager.Instance.pokerList)
            {
                if (player.input == correctNum)
                {
                    GameManager.Instance.leaderboard.Add(new Leaderboards(player.name, 2));
                }
                else
                {
                    GameManager.Instance.leaderboard.Add(new Leaderboards(player.name, 1));
                }
                GameManager.Instance.leaderboard.Sort();
            }
            return true;
        }
        else
        {
            foreach (PokerList playerPoker in GameManager.Instance.pokerList)
            {
                canAdd = false;
                foreach (Leaderboards playerLeader in GameManager.Instance.leaderboard)
                {
                    if (playerLeader.name == playerPoker.name)
                    {
                        canAdd = true;
                        int currScore = playerLeader.score;
                        if (playerPoker.input == correctNum)
                        {
                            currScore += 2;
                        }
                        else
                        {
                            currScore++;
                        }
                        playerLeader.score = currScore;
                        GameManager.Instance.leaderboard.Sort();
                        break;
                    }
                }

                if (!canAdd)
                {
                    if (playerPoker.input == correctNum)
                    {
                        GameManager.Instance.leaderboard.Add(new Leaderboards(playerPoker.name, 1));
                    }
                    else
                    {
                        GameManager.Instance.leaderboard.Add(new Leaderboards(playerPoker.name, 2));
                    }
                    GameManager.Instance.leaderboard.Sort();
                }
            }
        }

        return true;
    }

    void CheckTime()
    {
        if (currTime > 60)
        {
            progressBar.fillAmount = 1.0f;
        }

        if (currTime > 2 && currTime < 60)
        {
            CheckHonoka();
            CheckKotori();
            HandHandler();
        }

        if (currTime <= 0)
        {
            time.text = "0:00";
            currTime = 0;
            CancelInvoke();
            progressBar.fillAmount = 0f;
        }
    }

    void TransitionHandler(bool fadeIn)
    {
        if (fadeIn)
        {
            transition.canvasRenderer.SetAlpha(0.1f);
            transition.CrossFadeAlpha(1.0f, 1.0f, false);
        }
        else
        {
            transition.canvasRenderer.SetAlpha(1.0f);
            transition.CrossFadeAlpha(0.0f, 1.0f, false);
        }
    }

    void UmiHandler(string name)
    {
        this.GetComponent<AudioSource>().clip = FindUmiClip(name);
        this.GetComponent<AudioSource>().Play();
    }

    AudioClip FindUmiClip(string name)
    {
        foreach(AudioClip clip in umiClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }

    void CheckHonoka()
    {
        if (currTime > 0 && currTime % 15 == 0 && currTime < 60)
        {
            if (!honoka.isPlaying)
            {
                PlayHonokaClip();
                MoveHonoka(true);
                PulseTimer(true);
            }
        }
    }

    void MoveHonoka(bool canMoveUp)
    {
        Hashtable hash = new Hashtable();
        if (canMoveUp)
        {
            hash.Add("position", new Vector3(honoka.gameObject.transform.localPosition.x, -234, 0));
            hash.Add("oncomplete", "MoveHonoka");
            hash.Add("oncompletetarget", this.gameObject);
            hash.Add("oncompleteparams", false);
        }
        else
        {
            hash.Add("position", new Vector3(honoka.gameObject.transform.localPosition.x, -264, 0));
        }
        hash.Add("islocal", true);
        hash.Add("time", 0.2F);
        hash.Add("easeType", iTween.EaseType.linear);
        iTween.MoveTo(honoka.gameObject, hash);
    }

    void PulseTimer(bool canPulse)
    {
        if (canPulse)
        {
            Hashtable hash = new Hashtable();
            hash.Add("amount", new Vector3(0.25f, 0.25f, 0.25f));
            hash.Add("time", 1.0F);
            hash.Add("easeType", iTween.EaseType.linear);
            hash.Add("oncomplete", "PulseTimer");
            hash.Add("oncompletetarget", this.gameObject);
            hash.Add("oncompleteparams", false);
            iTween.PunchScale(time.gameObject, hash);
            time.color = Color.red;
        }
        else
        {
            time.color = Color.white;
        }
    }

    void PlayHonokaClip()
    {
        if (clipNumHonoka == 0)
        {
            clipNumHonoka = 1;
        }
        else
        {
            clipNumHonoka = 0;
        }

        honoka.clip = honokaClips[clipNumHonoka];
        honoka.Play();
    }

    void CheckKotori()
    {
        if (!kotori.isPlaying)
        {
            if (!canPlayKotori)
            {
                timeToPlayKotori -= Time.deltaTime;
                if (timeToPlayKotori <= 0)
                {
                    timeToPlayKotori = Random.Range(5.0f, 8.0f);
                    canPlayKotori = true;
                }
            }
            else
            {
                PlayKotoriClip();
            }
        }
    }

    void PlayKotoriClip()
    {
        if (clipNumKotori == 0)
        {
            clipNumKotori = 1;
        }
        else
        {
            clipNumKotori = 0;
        }

        kotori.clip = kotoriClips[clipNumKotori];
        kotori.Play();
        canPlayKotori = false;
    }
}
