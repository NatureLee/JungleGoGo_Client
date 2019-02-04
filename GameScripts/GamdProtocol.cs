using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public enum GCODE
{
    //게임
    PT_C_INLOBBY = 0,
    PT_S_RATE,
    PT_C_GAMESTART,
    PT_S_GAMESTART_SUCC,
    PT_S_GAMESTART_FAIL,
    PT_S_GAMESTARTALL,
    PT_C_ROOMLEAVE,
    PT_S_ROOMLEAVE_SUCC,
    PT_C_ROOMOUT,
    PT_S_ROOMOUT_SUCC,
    PT_C_PLAYERNICK,
    PT_S_PLAYERNICK,


    //인게임
    PT_C_MOVETURN = 100,
    PT_S_MOVETURN,
    PT_C_MOVE,
    PT_S_MOVE,
    PT_C_WINNER,
    PT_C_GETMYRATE

};



[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class GPacketHeader
{
    public int PackSize;
    public int PackNum;
    public GCODE gcode;
    public GPacketHeader() { }

    //헤더
    public GPacketHeader(int size, int num, GCODE type)
    {
        PackSize = size;
        PackNum = num;
        gcode = type;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_INLOBBY : GPacketHeader
{
    public PT_C_INLOBBY() { }
    public PT_C_INLOBBY(string nick)
        : base(Marshal.SizeOf(typeof(PT_C_INLOBBY)), sizeof(int), GCODE.PT_C_INLOBBY)
    {
        myNick = nick;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string myNick;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_RATE : GPacketHeader
{   public PT_S_RATE() { }
    public PT_S_RATE(int win,int lose)
        : base(Marshal.SizeOf(typeof(PT_S_RATE)), sizeof(int), GCODE.PT_S_RATE)
    {
        winRate = win;
        loseRate = lose;
    }
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int winRate;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int loseRate;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_GAMESTART : GPacketHeader
{
    public PT_C_GAMESTART()
        : base(Marshal.SizeOf(typeof(PT_C_GAMESTART)), sizeof(int), GCODE.PT_C_GAMESTART)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_GAMESTART_SUCC : GPacketHeader
{
    public PT_S_GAMESTART_SUCC()
        : base(Marshal.SizeOf(typeof(PT_S_GAMESTART_SUCC)), sizeof(int), GCODE.PT_S_GAMESTART_SUCC)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_GAMESTART_FAIL : GPacketHeader
{
    public PT_S_GAMESTART_FAIL()
        : base(Marshal.SizeOf(typeof(PT_S_GAMESTART_FAIL)), sizeof(int), GCODE.PT_S_GAMESTART_FAIL)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_GAMESTARTALL : GPacketHeader
{
    public PT_S_GAMESTARTALL() { }
    public PT_S_GAMESTARTALL(bool turn)
        : base(Marshal.SizeOf(typeof(PT_S_GAMESTARTALL)), sizeof(int), GCODE.PT_S_GAMESTARTALL)
    {
        isTurn = turn;
    }
    [MarshalAs(UnmanagedType.Bool)]
    public bool isTurn;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_ROOMLEAVE : GPacketHeader
{
    public PT_C_ROOMLEAVE()
        : base(Marshal.SizeOf(typeof(PT_C_ROOMLEAVE)), sizeof(int), GCODE.PT_C_ROOMLEAVE)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_ROOMLEAVE_SUCC : GPacketHeader
{
    public PT_S_ROOMLEAVE_SUCC()
        : base(Marshal.SizeOf(typeof(PT_S_ROOMLEAVE_SUCC)), sizeof(int), GCODE.PT_S_ROOMLEAVE_SUCC)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_ROOMOUT : GPacketHeader
{
    public PT_C_ROOMOUT()
        : base(Marshal.SizeOf(typeof(PT_C_ROOMOUT)), sizeof(int), GCODE.PT_C_ROOMOUT)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_ROOMOUT_SUCC : GPacketHeader
{
    public PT_S_ROOMOUT_SUCC()
        : base(Marshal.SizeOf(typeof(PT_S_ROOMOUT_SUCC)), sizeof(int), GCODE.PT_S_ROOMOUT_SUCC)
    {

    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_MOVETURN : GPacketHeader
{
    public PT_C_MOVETURN()
        : base(Marshal.SizeOf(typeof(PT_C_MOVETURN)), sizeof(int), GCODE.PT_C_MOVETURN)
    {

    }

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_MOVETURN : GPacketHeader
{
    public PT_S_MOVETURN()
        : base(Marshal.SizeOf(typeof(PT_S_MOVETURN)), sizeof(int), GCODE.PT_S_MOVETURN)
    {
        
    }
  
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_MOVE : GPacketHeader
{
    public PT_C_MOVE() { }
    public PT_C_MOVE(int x1, int y1, int x2, int y2)
        : base(Marshal.SizeOf(typeof(PT_C_MOVE)), sizeof(int), GCODE.PT_C_MOVE)
    {
        moveX1 = x1;
        moveY1 = y1;
        moveX2 = x2;
        moveY2 = y2;

    }
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    private int moveX1;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    private int moveY1;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    private int moveX2;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    private int moveY2;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_MOVE : GPacketHeader
{
    public PT_S_MOVE() { }
    public PT_S_MOVE(int x1, int y1, int x2, int y2)
        : base(Marshal.SizeOf(typeof(PT_S_MOVE)), sizeof(int), GCODE.PT_S_MOVE)
    {
        moveX1 = x1;
        moveY1 = y1;
        moveX2 = x2;
        moveY2 = y2;
        
    }
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int moveX1;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int moveY1;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int moveX2;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int moveY2;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_PLAYERNICK : GPacketHeader
{
    public PT_C_PLAYERNICK(string name,float rate)
        : base(Marshal.SizeOf(typeof(PT_C_PLAYERNICK)), sizeof(int), GCODE.PT_C_PLAYERNICK)
    {
        nickname = name;
        myrate = rate;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
     string nickname;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
     float myrate;

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_S_PLAYERNICK : GPacketHeader
{
    public PT_S_PLAYERNICK() { }
    public PT_S_PLAYERNICK(string name,float winRate)
        : base(Marshal.SizeOf(typeof(PT_S_PLAYERNICK)), sizeof(int), GCODE.PT_S_PLAYERNICK)
    {
        othernick = name;
        otherwinRate = winRate;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string othernick;
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public float otherwinRate;
 

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_WINNER : GPacketHeader
{
    public PT_C_WINNER() { }
    public PT_C_WINNER(string name1,string name2)
        : base(Marshal.SizeOf(typeof(PT_C_WINNER)), sizeof(int), GCODE.PT_C_WINNER)
    {
        winner = name1;
        loser = name2;
    }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string winner;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string loser;
   
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class PT_C_GETMYRATE : GPacketHeader
{
    public PT_C_GETMYRATE() { }
    public PT_C_GETMYRATE(int winningRate)
        : base(Marshal.SizeOf(typeof(PT_C_GETMYRATE)), sizeof(int), GCODE.PT_C_GETMYRATE)
    {
        mywinRate = winningRate;
    }
    [MarshalAs(UnmanagedType.I4, SizeConst = 32)]
    public int mywinRate;


}


public class GamdProtocol : MonoBehaviour
{

}

