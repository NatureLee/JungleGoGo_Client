/*
 * 로그인 패킷 주고받기
 * 바이트변환 & 마샬링 구조체
 */

using System;
using System.IO;                     //입출력 stream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Text;

//Marshaling
using System.Runtime.InteropServices;

//tcp를 위해 사용해야하는 namespace
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCPLogin : MonoBehaviour
{
    [Header("SubjectField")]
    public GameObject Subject;


    [Header("LoginField")]
    public InputField ID;
    public InputField PW;
    public Text LoginResultText;

    [Header("SignUpField")]
    public InputField sID;
    public InputField sPW;
    public InputField NICK;


    string Id;
    string Pw;
    string Nick;

    public static string MyNick;
    public static float MyWinningRate1;

    bool isLogin;

    //회원가입
    [Header("For Sign")]
    public GameObject Login;
    public GameObject signUp;
    public Text signResult;
    public GameObject signupResult;
    public GameObject retryBtn;
    public GameObject UpText;

    byte[] sendByte;
    byte[] recvbuffer;


    bool recvReady = false;
    //public bool ChangeBtn = true;


    // Use this for initialization
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoginBtn()
    {
        Id = ID.text;
        Pw = PW.text;

        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_Login Login_Pack = new PT_Login(Id, Pw);

        //직렬화시켜 보내기
        sendByte = Serialize(Login_Pack);
        GameObject.Find("LoginManager").GetComponent<connect>().socket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);

        recvReady = true;

        if (recvReady == true)
        {
            ReadSocket();
         }
    }

    public void SignUpBtn_InLogin()
    {
        signUp.SetActive(true);
        Login.SetActive(false);
    }

    public void SignUpBtn()
    {
        Id = sID.text;
        Pw = sPW.text;
        Nick = NICK.text;

        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_MAKING Making_Pack = new PT_MAKING(Id, Pw, Nick);

        //직렬화시켜 보내기
        sendByte = Serialize(Making_Pack);
        GameObject.Find("LoginManager").GetComponent<connect>().socket.Send(sendByte, sendByte.Length, 0);

        recvReady = true;

        if (recvReady == true)
        {
            Debug.Log("회원가입하고 데이터받음");
            ReadSocket();
        }

    }


    void ReadSocket()
    {
        recvbuffer = new byte[256];

        try
        {
            GameObject.Find("LoginManager").GetComponent<connect>().socket.Receive(recvbuffer, recvbuffer.Length, 0);
            Debug.Log("recvbuff" + Encoding.UTF8.GetString(recvbuffer));
        }
        catch
        {
            Debug.Log("RecvSocket fail");
            // Debug.Log(e.Message + "Error code" + e.ErrorCode);
            //return (e.ErrorCode);
        }

        PacketHeader recvpackHead = new PacketHeader();

        recvpackHead = (PacketHeader)Deserialize(recvbuffer, typeof(PacketHeader));

        Debug.Log("recv length :" + recvbuffer.Length);

        ParsingPacket(recvpackHead);

        recvReady = false;
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
        Marshal.Copy(data, 0 ,buffer, RawSize);

        object objData = Marshal.PtrToStructure(buffer, dataType);
        Marshal.FreeHGlobal(buffer);

       // Debug.Log("sPtr : " + sPtr);

        return objData;
    }
    

    private void ParsingPacket(PacketHeader mPacket)
    {
        switch (mPacket.code)
        {
            case CODE.PT_LOGIN_SUCC:
                LoginResultText.text = "               ** 성공 **";
                 Login_SUcc();
                AsyncClient.AsyncObject.Instance().ConnectTOServer();
                break;

            case CODE.PT_LOGIN_ERROR:
                Login_Error();
                break;


            case CODE.PT_MAKING_SUCC:
                Making_Succ();
                break;


            case CODE.PT_MAKING_ERROR:
                Making_Error();
                break;

            default:
             if(Login.activeSelf == true)
                {
                    Debug.Log("default login");
                    LoginBtn();
                }
             if(signUp.activeSelf == true)
                {
                    Debug.Log("default signup");
                }
               break; 
        }

    }

    void Login_SUcc()
    {
        PT_Login_Succ recvpack = new PT_Login_Succ();

        recvpack = (PT_Login_Succ)Deserialize(recvbuffer, typeof(PT_Login_Succ));

        MyNick = recvpack.LoginName;

        SceneManager.LoadScene("Lobby");
        
    }

    void Login_Error()
    {

        PT_Login_Error recvpack = new PT_Login_Error();

        recvpack = (PT_Login_Error)Deserialize(recvbuffer, typeof(PT_Login_Error));

        if (recvpack.errorCode == 1)
        {
            LoginResultText.text = "** ID,PW를 다시 확인하세요 **";
        }

        if (recvpack.errorCode == 2)
        {
            LoginResultText.text = "           ** 중복 로그인 **";
        }


    }

    void Making_Succ()
    {
        signupResult.SetActive(true);
        signUp.SetActive(false);
        retryBtn.SetActive(false);
        
        signResult.text = "가입성공!";
    }

    void Making_Error()
    {
        signupResult.SetActive(true);
        retryBtn.SetActive(true);
        signUp.SetActive(false);
        Login.SetActive(false);

        signResult.text = "가입실패!";
    }

    public void ExitBtn()
    {
        Login.SetActive(true);
        
        signupResult.SetActive(false);
    }

    public void RetryBtn()
    {
        signUp.SetActive(true);
        signupResult.SetActive(false);
    }

    public void S_LoginBtn()
    {
        Login.SetActive(true);
        Subject.SetActive(false);
    }

    public void S_SignUpBtn()
    {
        signUp.SetActive(true);
        Subject.SetActive(false);
    }

    //로그인으로 돌아가기
    public void GOLoginBtn()
    {
        Login.SetActive(true);
        signUp.SetActive(false);
    }


    public void RuleBtn()
    {
        SceneManager.LoadScene("GameInfo",LoadSceneMode.Single);
    }

}
