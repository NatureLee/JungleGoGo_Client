using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public enum CODE
{
    PT_LOGIN = 0,
    PT_LOGIN_SUCC,
    PT_LOGIN_ERROR,

    PT_MAKING = 10,
    PT_MAKING_SUCC,
    PT_MAKING_ERROR
};


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PacketHeader
{
    public int PackSize;
    public int PackNum;
    public CODE code;
    public PacketHeader() { }

    //헤더
    public PacketHeader(int size,int num,CODE type)
    {
        PackSize = size;
        PackNum = num;
        code = type;
    }
}

//login 할 때
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_Login : PacketHeader
{
    public PT_Login(string ID_value,string PW_value)
        : base(Marshal.SizeOf(typeof(PT_Login)), sizeof(int), CODE.PT_LOGIN)
    {
        LoginID = ID_value;
        PassWord = PW_value;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string LoginID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string PassWord;
}

//login 성공
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_Login_Succ : PacketHeader
{
    public PT_Login_Succ() { }
    public PT_Login_Succ(string NAME_value)
        : base(Marshal.SizeOf(typeof(PT_Login_Succ)), sizeof(int), CODE.PT_LOGIN_SUCC)
    {
        LoginName = NAME_value;
      
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string LoginName;
}

//login 실패
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_Login_Error : PacketHeader
{
     public PT_Login_Error() { }
     public PT_Login_Error(int error)
        : base(Marshal.SizeOf(typeof(PT_Login_Error)), sizeof(int), CODE.PT_LOGIN_ERROR)
    {
        errorCode = error;
    }
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int errorCode;
}

//Sign up 할 때
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_MAKING : PacketHeader
{
    public PT_MAKING(string ID_value, string PW_value, string Name_value)
        : base(Marshal.SizeOf(typeof(PT_MAKING)), sizeof(int), CODE.PT_MAKING)
    {
        LoginID = ID_value;
        PassWord = PW_value;
        NickName = Name_value;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string LoginID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string PassWord;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string NickName;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_MAKING_SUCC : PacketHeader
{
    public PT_MAKING_SUCC()
        : base(Marshal.SizeOf(typeof(PT_MAKING_SUCC)), sizeof(int), CODE.PT_MAKING_SUCC)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_MAKING_ERROR : PacketHeader
{
    public PT_MAKING_ERROR()
        : base(Marshal.SizeOf(typeof(PT_MAKING_SUCC)), sizeof(int), CODE.PT_MAKING_SUCC)
    {

    }
}

public class Protocol : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
