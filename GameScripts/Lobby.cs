using System;
using System.IO;                     //입출력 stream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//using System.Text;

//Marshaling
using System.Runtime.InteropServices;

//tcp를 위해 사용해야하는 namespace
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class Lobby : MonoBehaviour {

    byte[] sendByte;

    public GameObject StartSuccText;
    public GameObject StartBtnUI;
    public GameObject LeaveBtn;

    public GameObject StartText;
    public GameObject pushText;

    int QueueCount;

    public static float MyWinningRate;


    // Use this for initialization
    void Start () {

        DoubleCheck();
    }
	
	// Update is called once per frame
	void Update () {

        QueueCount = AsyncClient.AsyncObject.Instance().AsyncQueue.Count;

        if (QueueCount > 0)
        {
            AsyncClient.AsyncObject.Instance().AsyncQueue.Dequeue();
            ReadSocket();
        }
	}

    public void StartBtn()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_GAMESTART GameStart_Pack = new PT_C_GAMESTART();

        //직렬화시켜 보내기
        sendByte = Serialize(GameStart_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);

        StartText.SetActive(false);
        pushText.SetActive(false);
    }

    public void ExitBtn()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_ROOMOUT RoomLeave_Pack = new PT_C_ROOMOUT();

        //직렬화시켜 보내기
        sendByte = Serialize(RoomLeave_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);
    }


    public void ReadSocket()
    {
        try
        {
            AsyncClient.AsyncObject.Instance().ReceiveToServer();
        }
        catch
        {
            Debug.Log("AsyncRecvSocket FAIL");
          
        }

        GPacketHeader recvpackHead = new GPacketHeader();
        
        recvpackHead = (GPacketHeader)Deserialize(AsyncClient.AsyncObject.Instance().recvBuffer, typeof(GPacketHeader));

        Debug.Log("recv length :" + AsyncClient.AsyncObject.Instance().recvBuffer.Length);

        ParsingLobbyPacket(recvpackHead);

    }
    
    private void ParsingLobbyPacket(GPacketHeader mPacket)
    {

        switch (mPacket.gcode)
        {
            case GCODE.PT_S_GAMESTART_SUCC:
                GameStartSucc();
                break;

            case GCODE.PT_S_GAMESTARTALL:
                GameStartAll();
                break;

            case GCODE.PT_S_ROOMOUT_SUCC:
                LobbyExitSucc();
                break;

            case GCODE.PT_S_RATE:
                GetRate();
                break;

            default:
                StartBtn();
                break;
       
        }

    }

    //구조체 내용이 소켓을 통해서 전송 될 준비가 된 바이트 배열이 반환
    //바이트 배열로 직렬화 시켜주는거
    public byte[] Serialize(object data)
    {
        // 메모리 크기 구하기
        int RawSize = Marshal.SizeOf(data);
        IntPtr buffer = Marshal.AllocHGlobal(RawSize); //메모리 할당(임시공간)

        //마샬링 하는 부분(구조체를 바꿈)
        //(마샬링할 데이터,메모리 블럭 포인터,데이터 넣을때 false)
        Marshal.StructureToPtr(data, buffer, false);

        //최종 바이트형 배열 정의
        byte[] RawData = new byte[RawSize];

        //최종 배열에 데이터 넣기
        Marshal.Copy(buffer, RawData, 0, RawSize);
        Marshal.FreeHGlobal(buffer); //해제

        return RawData;
    }


    //배열을 구조체에 비직렬화시킴
    public object Deserialize(byte[] data, Type dataType)//,int sPtr)
    {
        // sPtr = 0;

        int RawSize = Marshal.SizeOf(dataType);
        if (RawSize > data.Length)
        {
            return null;
        }
        IntPtr buffer = Marshal.AllocHGlobal(RawSize);
        Marshal.Copy(data, 0, buffer, RawSize);

        object objData = Marshal.PtrToStructure(buffer, dataType);
        Marshal.FreeHGlobal(buffer);

        // Debug.Log("sPtr : " + sPtr);

        return objData;
    }


    void GetRate()
    {
        PT_S_RATE recvpack = new PT_S_RATE();

        recvpack = (PT_S_RATE)Deserialize(AsyncClient.AsyncObject.Instance().recvBuffer, typeof(PT_S_RATE));

        int win = recvpack.winRate;
        int lose = recvpack.loseRate;


		if (win == 0)
			MyWinningRate = 0;
		else
		{
			//승률 계산
			MyWinningRate = (float)(win / (float)(win + lose)) * 100;
		}
        Debug.Log("win : " + win + "    lose: " + lose + "     winningRate : "+ MyWinningRate);
    }

    public void GameStartSucc()
    {
        StartSuccText.SetActive(true);
        LeaveBtn.SetActive(true);

        StartBtnUI.SetActive(false);

    }

    public void GameStartAll()
    {
        PT_S_GAMESTARTALL startall_pack = new PT_S_GAMESTARTALL();

        startall_pack = (PT_S_GAMESTARTALL)Deserialize(AsyncClient.AsyncObject.Instance().recvBuffer, typeof(PT_S_GAMESTARTALL));
    
        BoardManager.isturn = startall_pack.isTurn;

        SceneManager.LoadScene("SampleScene");
        
    }

    public void LobbyExitSucc()
    {
        Debug.Log("LobbyExitSucc");

        StartBtnUI.SetActive(true);
        pushText.SetActive(true);
        LeaveBtn.SetActive(false);
        StartSuccText.SetActive(false);
    }

    void DoubleCheck()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_INLOBBY Lobby_Pack = new PT_C_INLOBBY(TCPLogin.MyNick);

        //직렬화시켜 보내기
        sendByte = Serialize(Lobby_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);
    }
}
