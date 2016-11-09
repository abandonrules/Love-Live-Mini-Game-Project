using UnityEngine;
using TwitchChatter;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    protected GameManager() { }

    public List<Leaderboards> leaderboard = new List<Leaderboards>();
    public List<ChibiList> chibiList = new List<ChibiList>();
    public List<PokerList> pokerList = new List<PokerList>();

    [Header("Twitch Chatter")]
    public bool printChatMessages;
    public string botUsername;
    public string botOAuth;
    public string twitchChannel;
    public string joinMessage;
    public string broadcaster;

    [Header("Scene Manager")]
    public int numOfGames;
    public string nextGame;
    public int chibiType;
    public int roundNum;
    public int targetRoundNum;

    public ChibiLogic chibiLogic;
    public PokerFace pokerFace;

    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;        
            SceneManager.sceneLoaded += CheckScene;
            DontDestroyOnLoad(gameObject);
        }
        Application.runInBackground = true;
    }

    void CheckScene(Scene current, LoadSceneMode mode)
    {
        if (Application.isEditor)
        {
            twitchChannel = "pachipachibot";
            broadcaster = "owlremember";
        }
        else
        {
            StartCoroutine("GetConfigFile");
        }
        TwitchJoin();
        RemoveScripts();

        if (current.name == "loading")
        {
            roundNum = 0;
        }
        if (current.name == "pokerface")
        {
            roundNum++;
        }
    }

    IEnumerator GetConfigFile()
    {
        string directory = Application.dataPath + "/../";

        System.IO.FileInfo[] dirInfo = new System.IO.DirectoryInfo(directory).GetFiles();

        foreach (System.IO.FileInfo fileName in dirInfo)
        {
            string[] tempFileName = fileName.ToString().Split("/"[0]);

            if (tempFileName[tempFileName.Length - 1].Contains("Config.txt"))
            {
                string line;

                try
                {
                    StreamReader sr = new StreamReader(fileName.ToString());

                    line = sr.ReadLine();

                    while (line != null)
                    {
                        if (line.Contains("|"))
                        {
                            string[] configContent = line.Split("|"[0]);

                            if (configContent[0] == "[Bot Username]")
                            {
                                botUsername = configContent[1];
                            }

                            if (configContent[0] == "[Bot OAuth]")
                            {
                                botOAuth = configContent[1];
                            }

                            if (configContent[0] == "[Twitch Channel]")
                            {
                                twitchChannel = configContent[1];
                            }

                            if (configContent[0] == "[Join Message]")
                            {
                                joinMessage = configContent[1];
                            }

                            if (configContent[0] == "[Broadcaster]")
                            {
                                broadcaster = configContent[1];
                            }
                        }

                        line = sr.ReadLine();
                    }

                    sr.Close();
                }
                catch (Exception e)
                {
                    print("Exception: " + e.Message);
                }
            }
        }

        yield return null;
    }

    void TwitchJoin()
    {
        TwitchChatClient.singleton.AddChatListener(OnChatMessage);

        TwitchChatClient.singleton.userName = botUsername;
        TwitchChatClient.singleton.oAuthPassword = botOAuth;

        TwitchChatClient.singleton.JoinChannel(twitchChannel);

        if (botUsername != "" && botOAuth != "" && joinMessage != "" && SceneManager.GetActiveScene().name == "title")
        {
            TwitchChatClient.singleton.SendMessage(twitchChannel, joinMessage);
        }
    }

    void RemoveScripts()
    {
        chibiLogic = null;
        pokerFace = null;
    }

    void OnDestroy()
    {
        if (TwitchChatClient.singleton != null)
        {
            TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
        }
    }

    public void SwitchToLoading()
    {
        SceneManager.LoadScene("loading");
    }

    public void LoadNewGame()
    {
        SceneManager.LoadScene(nextGame);
    }

    public void ChooseGame(int gameNum)
    {
        switch(gameNum)
        {
            case 1:
                nextGame = "pokerface";
                break;
            case 2:
                nextGame = "scoutingbox";
                break;
        }
    }

    void OnChatMessage(ref TwitchChatMessage msg)
    {
        if (printChatMessages)
        {
            Debug.Log("TwitchChatter: " + msg.userName + "(" + msg.channelName + "): " + msg.chatMessagePlainText);
        }

        if (SceneManager.GetActiveScene().name == "title")
        {
            if (msg.userName.ToLower() == broadcaster.ToLower())
            {
                if (msg.chatMessagePlainText.ToLower() == "start")
                {
                    SceneManager.LoadScene("loading");
                }
            }
        }

        if (SceneManager.GetActiveScene().name == "loading")
        {
            chibiLogic.CheckMessage(msg.userName, msg.chatMessagePlainText);
        }

        if (SceneManager.GetActiveScene().name == "pokerface")
        {
            pokerFace.CheckMessage(msg.userName, msg.chatMessagePlainText);
        }
        
    }
}
