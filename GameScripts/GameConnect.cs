using System;
using System.IO;                     //입출력 stream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;


//tcp를 위해 사용해야하는 namespace
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class GameConnect : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {

    }

    //소켓삭제
    private void OnApplicationQuit()
    {

        AsyncClient.AsyncObject.Instance().asyncSocket.Disconnect(true);
        AsyncClient.AsyncObject.Instance().asyncSocket.Close();

    }
}

namespace AsyncClient
{
    public class AsyncObject
    {

        public Queue<byte[]> AsyncQueue = new Queue<byte[]>();
       

        //ip주소 & port번호
        string ipAdress = "127.0.0.1";
        public int port = 11550;


        static AsyncObject instace;

        public byte[] recvBuffer = new byte[256];
        public Socket asyncSocket;

        Boolean isconnected = false;

        public static AsyncObject Instance()
        {
            if (instace == null)
            {
                instace = new AsyncObject();
            }

            return instace;
        }

        public void ConnectTOServer()
        {
            //소켓생성
            asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //서버와 연결
            //try-catch문은 예외처리
            try  // 실행될 코드
            {
                IPAddress ipAddr = IPAddress.Parse(ipAdress);
                IPEndPoint ipendPoint = new IPEndPoint(ipAddr, port);
                asyncSocket.Connect(ipendPoint);

                asyncSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), asyncSocket);

                isconnected = true;
            }
            catch (SocketException se)  // 받아낼 예외
            {
                Debug.Log("asyncSocket Connect Error!  : " + se.ToString());
                isconnected = false;
                return;
            }

        }

        public void ReceiveToServer()
        {
            if (isconnected)
            {
                asyncSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), asyncSocket);

            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int recvBytes = asyncSocket.EndReceive(ar);

                if (recvBytes > 0)
                {
                    asyncSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), asyncSocket);

                    AsyncQueue.Enqueue(recvBuffer);
                }
            }
            catch
            {
                return;
            }

        }
    }
}