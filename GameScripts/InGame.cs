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

public class InGame : MonoBehaviour {

    byte[] sendByte;

    int QueueCount;


    public Text Player1;
    public Text Player2;
    public static string OtherNick;

    public GameObject Lion;
    public GameObject Tiger;

    public Text myRate;
    public Text otherRate;

    public GameObject outUser;

   // public GameObject outUser;
   // public 
   //
    // Use this for initialization
    void Start () {

        SendNIckName();
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

        ParsingGamePacket(recvpackHead);

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


    private void ParsingGamePacket(GPacketHeader mPacket)
    {

        switch (mPacket.gcode)
        {
            case GCODE.PT_S_MOVETURN:
                Debug.Log("PT_S_MOVETURN");
                ChangeIsChickenTurn();
                break;

            case GCODE.PT_S_MOVE:
                MovePack();
                break;

            case GCODE.PT_S_PLAYERNICK:
                ShowNickName();
                break;

            case GCODE.PT_C_WINNER:
                PT_C_WINNER winnerpack = new PT_C_WINNER();
                winnerpack = (PT_C_WINNER)Deserialize(AsyncClient.AsyncObject.Instance().recvBuffer, typeof(PT_C_WINNER));

                GameObject.Find("Board").GetComponent<BoardManager>().EndGame(winnerpack.winner);
                break;

            case GCODE.PT_S_ROOMOUT_SUCC:
                SceneManager.LoadScene("Lobby");
                break;

            case GCODE.PT_S_ROOMLEAVE_SUCC:
                outUser.SetActive(true);
                GameObject.Find("GameManager").GetComponent<InGame>().SendWinner(TCPLogin.MyNick, InGame.OtherNick);
                break;
        }
    }

    private void ChangeIsChickenTurn()
   {
       BoardManager.isChickenTurn = !BoardManager.isChickenTurn;
   }
  
    public void InputData()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_MOVETURN MoveTurn_Pack = new PT_C_MOVETURN();

        //직렬화시켜 보내기
        sendByte = Serialize(MoveTurn_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);
    }

    public void MovePack()
    {
        PT_S_MOVE recvpackHead = new PT_S_MOVE();

        recvpackHead = (PT_S_MOVE)Deserialize(AsyncClient.AsyncObject.Instance().recvBuffer, typeof(PT_S_MOVE));
        GameObject.Find("Board").GetComponent<BoardManager>().SelectCharacter(recvpackHead.moveX1, recvpackHead.moveY1);
        GameObject.Find("Board").GetComponent<BoardManager>().MoveCharacter(recvpackHead.moveX2, recvpackHead.moveY2);
    }

    public void SendNIckName()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_PLAYERNICK PNICK_Pack = new PT_C_PLAYERNICK(TCPLogin.MyNick, Lobby.MyWinningRate);

        //직렬화시켜 보내기
        sendByte = Serialize(PNICK_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("Send My Nick :"+ TCPLogin.MyNick +"  My WinningRate :" + Lobby.MyWinningRate);
    }

    void ShowNickName()
    {
        Debug.Log("ShowNICKNAME");

        PT_S_PLAYERNICK recvpackHead = new PT_S_PLAYERNICK();

        recvpackHead = (PT_S_PLAYERNICK)Deserialize(AsyncClient.AsyncObject.Instance().recvBuffer, typeof(PT_S_PLAYERNICK));

        //닉&승률 표현
        OtherNick = recvpackHead.othernick;
  
        if (BoardManager.isturn)
        {
            Lion.GetComponent<Text>().color = Color.red;

            Player1.text = TCPLogin.MyNick;
            Player2.text = recvpackHead.othernick;

            myRate.text = "승률 : "+Lobby.MyWinningRate.ToString("N1") + "%";
            otherRate.text = "승률 : " + recvpackHead.otherwinRate.ToString("N1") + "%";



        }
        if (!BoardManager.isturn)
        {
            Tiger.GetComponent<Text>().color = Color.red;

            Player1.text = recvpackHead.othernick;
            Player2.text = TCPLogin.MyNick;

            myRate.text = "승률 : " + recvpackHead.otherwinRate.ToString("N1")+"%";
            otherRate.text = "승률 : " + Lobby.MyWinningRate.ToString("N1") + "%";
        }


    }


    public void SendWinner(string winner, string loser)
    {
        PT_C_WINNER win_Pack = new PT_C_WINNER(winner,loser);
        sendByte = Serialize(win_Pack);

        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);
    }

    //게임 중간
    public void ExitButton()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_ROOMLEAVE RoomOut_Pack = new PT_C_ROOMLEAVE();

        //직렬화시켜 보내기
        sendByte = Serialize(RoomOut_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);

        SceneManager.LoadScene("Lobby");

    }

    //게임 끝나고
    public void OutAllButton()
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_ROOMOUT RoomLeave_Pack = new PT_C_ROOMOUT();

        outUser.SetActive(false);

        //직렬화시켜 보내기
        sendByte = Serialize(RoomLeave_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);
    }

    public float Rate(int win , int lose)
    {
        float result;

        result = win / (win + lose) * 100;

        return result;

    }

}
