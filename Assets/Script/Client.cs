﻿using UnityEngine;
using System.Collections;
using System.Net;
public class Client : MonoBehaviour
{
    public static Client Instance;

    //要连接的服务器地址  
    public string IP = "192.168.9.230";
    //要连接的端口  
    public int Port = 10100;
    //聊天信息  
    string Message = "";
    //声明一个二维向量   
    Vector2 Sc;
    bool IsMatching;
    bool Matched;
    public string opponentIp;
    public bool SearchLock;

    public GameObject onlinePanel;
    public GameObject startPanel;
    UILabel findButton;
    bool failConnect = false;
    public void Awake()
    {
        findButton = onlinePanel.transform.FindChild("FindButton/label").GetComponent<UILabel>();
        onlinePanel.SetActive(false);
        Client.Instance = this;
    }
    public string GetLocalIp()
    {
        string hostname = Dns.GetHostName();//得到本机名   
        IPHostEntry localhost = Dns.GetHostEntry(hostname);
        IPAddress localaddr = localhost.AddressList[0];
        return localaddr.ToString();
    }
    public void FindOpponent()
    {
        if (!SearchLock && Network.peerType == NetworkPeerType.Client)
        {
            //发送给接收的函数, 模式为全部, 参数为信息  
            networkView.RPC("Searching", RPCMode.Others, GetLocalIp());
            IsMatching = true;
            SearchLock = true;
        }
    }
    //void OnGUI()
    //{
    //    //端类型的状态  
    //    switch (Network.peerType)
    //    {
    //        //运行于服务器端  
    //        case NetworkPeerType.Server:
    //            break;
    //        //运行于客户端  
    //        case NetworkPeerType.Client:
    //            OnClient();
    //            break;
    //        //正在尝试连接到服务器  
    //        case NetworkPeerType.Connecting:
    //            break;
    //    }
    //}
    public void OffLine()
    {
        onlinePanel.SetActive(false);
        startPanel.SetActive(true);
        failConnect = false;
        Matched = false;
        SearchLock = false;
        LevelManager.Instance.IsOnline = false;
        opponentIp = "";
    }
    public void StartConnect() //禁止客户端连接运行, 服务器未初始化  
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            //Debug.Log("StartConnect");
            opponentIp = "";
            IsMatching = false;
            Matched = false;
            SearchLock = false;
            NetworkConnectionError error = Network.Connect(IP, Port);
            //连接状态  
            switch (error)
            {
                case NetworkConnectionError.NoError:
                    break;
                default:
                    Debug.Log("客户端错误" + error);
                    break;
            }

        }
        startPanel.SetActive(false);

        onlinePanel.SetActive(true);
       // Debug.Log("a");
    }

    public void GiveUp()//认输
    {
        Client.Instance.OnWin();
        GuideText.Instance.ReturnText(104);
        GuideText.Instance.ReturnMenu = true;
    }
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Matched = false;
        SearchLock = false;
        LevelManager.Instance.IsOnline = false;
        CameraMoving.Instance.Move(-1);
        LevelManager.Instance.EndGame();
    }

    public void OnEndRound()
    {
        networkView.RPC("EndRound", RPCMode.Others, Client.Instance.GetLocalIp());
    }
    public void OnTrajectoryChange(int attackID, TrajectoryType trajectory)
    {
        networkView.RPC("TrajectoryChange", RPCMode.Others, Client.Instance.GetLocalIp(), attackID, trajectory);
    }

    public void OnCast(string ID)
    {
        networkView.RPC("Cast", RPCMode.Others, GetLocalIp(), ID);
    }
    public void OnSheildUse(string ID, int trajectory)
    {
        networkView.RPC("Shield", RPCMode.Others, Client.Instance.GetLocalIp(), ID, trajectory);
    }
    public void OnCohesion(int attackID)
    {
        networkView.RPC("Cohesion", RPCMode.Others, GetLocalIp(), attackID);
    }
    public void OnEntrench(int attackID)
    {
        networkView.RPC("Entrench", RPCMode.Others, GetLocalIp(), attackID);
    }
    public void OnDraw()
    {
        networkView.RPC("Draw", RPCMode.Others, GetLocalIp());
    }

    public void OnLose()
    {
        networkView.RPC("Lose", RPCMode.Others, GetLocalIp());
    }

    public void OnWin()
    {
        networkView.RPC("Win", RPCMode.Others, GetLocalIp());
    }
    [RPC]
    void EndRound(string opIp)
    {
        if (opponentIp == opIp)
        {
            EnergyManager.Instance.enemyTurnsEnd = true;
            if (EnergyManager.Instance.myTurnsEnd)
            {
                StartCoroutine(EnergyManager.Instance.End());
            }
        }
    }
    //接收请求的方法. 注意要在上面添加[RPC]  
    [RPC]
    void Searching(string matchIp)
    {
        //刚从网络接收的数据的相关信息,会被保存到NetworkMessageInfo这个结构中      
        if (IsMatching)
        {
            opponentIp = matchIp;
            networkView.RPC("Match", RPCMode.Others, matchIp, GetLocalIp());
            IsMatching = false;
            Matched = true;
            LevelManager.Instance.level = 10;
            LevelManager.Instance.IsOnline = true;
            LevelManager.Instance.StartGame();
            onlinePanel.SetActive(false);
        }
    }

    [RPC]
    void Match(string ip1, string ip2)
    {
        if (ip1 == GetLocalIp())
        {
            opponentIp = ip2;
            IsMatching = false;
            Matched = true;
            onlinePanel.SetActive(false);
            LevelManager.Instance.IsOnline = true;
            LevelManager.Instance.StartGame();
        }
    }


    [RPC]
    void Win(string opIp)
    {
        if (opponentIp == opIp)
        {
            opponentIp = "";
            Matched = false;
            SearchLock = false;
            Debug.Log("Win detected");
            CameraMoving.Instance.Move(-1);
            LevelManager.Instance.EndGame();
            LevelManager.Instance.IsOnline = false;
        }
    }
    [RPC]
    void Lose(string opIp)
    {
        if (opponentIp == opIp)
        {
            opponentIp = "";
            Matched = false;
            SearchLock = false;
            Debug.Log("Lost detected");
            CameraMoving.Instance.Move(-1);
            LevelManager.Instance.EndGame();
            LevelManager.Instance.IsOnline = false;
        }
    }
    [RPC]
    void Draw(string opIp)
    {
        if (opponentIp == opIp)
        {
            opponentIp = "";
            Matched = false;
            SearchLock = false;
            Debug.Log("Draw detected");
            CameraMoving.Instance.Move(-1);
            LevelManager.Instance.EndGame();
            LevelManager.Instance.IsOnline = false;
        }
    }

    [RPC]
    void Cast(string opIp, string ID)
    {
        if (opponentIp == opIp)
        {
            Debug.Log(ID + " detected");
            EnergyManager.Instance.enemyOperationID.Add(ID);
            EnergyManager.Instance.operation.Add(Operation.Cast);
            EnergyManager.Instance.enemyTrajectoryID.Add(0);
            //create magic
        }
    }
    [RPC]
    void TrajectoryChange(string opIp, int ID, int trajectory)
    {
        if (opponentIp == opIp)
        {
            Debug.Log(ID + "detected,trajectory:" + trajectory);
            EnergyManager.Instance.enemyOperationID.Add(ID.ToString());
            EnergyManager.Instance.operation.Add(Operation.ChangeTrajectory);
            EnergyManager.Instance.enemyTrajectoryID.Add(trajectory);
        }
    }
    [RPC]
    void Cohesion(string opIp, int attackID)
    {
        if (opponentIp == opIp)
        {
            EnergyManager.Instance.enemyOperationID.Add(attackID.ToString());
            EnergyManager.Instance.operation.Add(Operation.Cohere);
        }
    }
    [RPC]
    void Entrench(string opIp, int defenseID)
    {
        if (opponentIp == opIp)
        {
            EnergyManager.Instance.enemyOperationID.Add(defenseID.ToString());
            EnergyManager.Instance.operation.Add(Operation.Entrench);
        }
    }
    [RPC]
    void Shield(string opIp, string ID, int position)
    {
        if (opponentIp == opIp)
        {
            EnergyManager.Instance.enemyOperationID.Add(ID);
            EnergyManager.Instance.operation.Add(Operation.Cast);
            EnergyManager.Instance.enemyTrajectoryID.Add(position);
        }
    }
    // Use this for initialization  
    void Start()
    {

    }
    void OnFailedToConnect(NetworkConnectionError error)
    {
        //Debug.Log("Could not connect to server: " + error);
        if (onlinePanel.activeSelf)
        {
            findButton.text = "连接失败";
            failConnect = true;
        }
    }

    // Update is called once per frame  
    void Update()
    {
        if (onlinePanel.activeSelf && !failConnect)
        {
            switch (Network.peerType)
            {
                case NetworkPeerType.Disconnected:
                    findButton.text = "登陆中...";
                    break;
                case NetworkPeerType.Client:
                    findButton.text = "寻找对手";
                    break;

            }

        }
    }
}  
