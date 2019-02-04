using System;
using System.IO;                     //입출력 stream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;


/*
 * 캐릭터가 바뀌는 바람에 함수 헷갈릴까봐 써놓음
 * 노랑=치킨
 * 분홍=용가리
 */


public class BoardManager : MonoBehaviour {

    byte[] sendByte;


    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }

    public Character[,] Characters { set; get; }
    private Character selectedCharacter;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> CharacterPrefabs;
    private List<GameObject> activeCharacter = new List<GameObject>();

    //선택시 머테리얼 바꾸기
    private Material previousMat;
    public  Material selectedMatYellow;
    public  Material selectedMatPink;


    private Quaternion orientaltion = Quaternion.Euler(270, 180, 0);

    //현재 누구 턴인지 
    public static bool isChickenTurn = true;

    public static bool isturn;


    //상대편 못 움직이게
    public static bool ChickenTurn;
    public static bool DragonTurn;
    
    //score
    public Text ChickenScoreText;
    public Text DragonScoreText;

    private int chickenScore = 0;
    private int dragonScore = 0;

   /* //Attack
    public Text ChickenAttackText;
    public Text DragonAttackText;

    public static int chickeAttackCount = 2;
    public static int dragonAttackCount = 2;
    */

    //End
    public Text Winner;
    public Text Loser;
    public GameObject endBoard;
    public GameObject Board;
    public GameObject Texts;

    private int x;
    private int y;

    private void Start()
    {
        Instance = this;
        SaqwnAllCharacters();
    }

    private void Update()
    {
        UpdateSelection();
        Drawboard();

        if (isChickenTurn == isturn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectionX >= 0 && selectionY >= 0)
                {
                    //선택된 체스말 없으면
                    if (selectedCharacter == null)
                    {
                        
                        //체스말 선택
                        SelectCharacter(selectionX, selectionY);
                    }
                    else
                    {
                        x = selectedCharacter.CurrentX;
                        y = selectedCharacter.CurrentY;
                        //체스말 이동
                        MoveCharacter(selectionX, selectionY);
                        Movepac(x, y, selectionX, selectionY);

                       // GameObject.Find("GameManager").GetComponent<InGame>().InputData();

                    }
                }
            }
        }
    }


    //캐릭터 선택
    public void SelectCharacter(int x, int y)
    {
        if (Characters[x, y] == null)
            return;

        if (Characters[x, y].isChicken != isChickenTurn)
            return;

        allowedMoves = Characters[x, y].PossibleMove();

        selectedCharacter = Characters[x, y];

        //선택시 메테리얼 바꾸기
        previousMat = selectedCharacter.GetComponent<MeshRenderer>().material;
        if (Characters[x, y].isChicken)
        {
            selectedMatYellow.mainTexture = previousMat.mainTexture;
            selectedCharacter.GetComponent<MeshRenderer>().material = selectedMatYellow;
        }
        else if (!Characters[x, y].isChicken)
        {
            selectedMatPink.mainTexture = previousMat.mainTexture;
            selectedCharacter.GetComponent<MeshRenderer>().material = selectedMatPink;
        }
        
        BoardColor.Instance.StepAllowedMoves(allowedMoves);

    }

    public void MoveCharacter(int x, int y)
    {
        if (allowedMoves[x,y])
        {
            Character c = Characters[x, y];
            
            //상대 캐릭터 만났을 시 상대캐릭터 잡아먹음
            if (c != null && c.isChicken != isChickenTurn)
            {
                    //Capture a piece
                    activeCharacter.Remove(c.gameObject);
                    Destroy(c.gameObject);
                    AddScore(x, y, 50);
            }


            Characters[selectedCharacter.CurrentX, selectedCharacter.CurrentY] = null;
            selectedCharacter.transform.position = GetTileCenter(x, y);
            selectedCharacter.SetPosition(x, y);
            Characters[x, y] = selectedCharacter;

            //AddScore(x, y, 10);

            isChickenTurn = !isChickenTurn;

        }

        selectedCharacter.GetComponent<MeshRenderer>().material = previousMat;

        //징검다리판 클론 하나씩만 생성
        BoardColor.Instance.HideSteps();
        selectedCharacter = null;

    }


    //포지션 확인
    private void UpdateSelection()
    {
        if (!Camera.main)
            return;
       

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("BoardPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
            //Debug.Log(hit.point);
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }


    //캐릭터 배치
    private void SpawnCharacters(int index, int x, int y)
    {
        GameObject go = Instantiate(CharacterPrefabs[index], GetTileCenter(x,y), orientaltion) as GameObject;
        go.transform.SetParent(transform);
        Characters[x, y] = go.GetComponent<Character>();
        Characters[x, y].SetPosition(x, y);

        activeCharacter.Add(go);

    }

    private void SaqwnAllCharacters()
    {
        activeCharacter = new List<GameObject>();
        Characters = new Character[4, 6];


        //치킨
        for (int i = 0; i < 4; i++)
        {
            SpawnCharacters(0, i,0);
        }

        //용가리
        for (int i = 0; i < 4; i++)
        {
            SpawnCharacters(1, i, 5);
        }

    }


    //캐릭터 가운데 놓기
    private Vector3 GetTileCenter(int x, int z)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * z) + TILE_OFFSET;

        return origin;
    }

    //보드판 그리기
    private void Drawboard()
    {
        Vector3 widthLine = Vector3.right * 4;
        Vector3 heigthLine = Vector3.forward * 6;

        for (int i = 0; i <= 6; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine,Color.black);

            for (int j = 0; j <= 4; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heigthLine, Color.black);
            }
        }

        //셀렉션 그리기
      /*  if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
               Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(
               Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
              Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
        */
    }

    private void WhoWinner()
    {
        Debug.Log(isturn);
        Debug.Log("MyNick " +TCPLogin.MyNick);
        Debug.Log("otherNick" + InGame.OtherNick);

        if (isturn)
        {
            if (chickenScore >= 100)
            {
                Winner.text = TCPLogin.MyNick;
            }
            else
            {
                Winner.text = InGame.OtherNick;

            }

        }
        if (!isturn)
        {
            if (chickenScore >= 100)
            {
                Winner.text = InGame.OtherNick;
            }
            else
            {
                Winner.text = TCPLogin.MyNick;
            }
        }

        if (Winner.text == TCPLogin.MyNick)
        {
            GameObject.Find("GameManager").GetComponent<InGame>().SendWinner(TCPLogin.MyNick,InGame.OtherNick);
        }
        else
        {
            EndGame(InGame.OtherNick);
        }
    }

    public void EndGame(string name)
    {
        Winner.text = name + " \nWIN";

        foreach (GameObject go in activeCharacter)
        Destroy(go);

        isChickenTurn = true;
        BoardColor.Instance.HideSteps();

        endBoard.SetActive(true);
        Board.SetActive(false);
        Texts.SetActive(false);

     
        SaqwnAllCharacters();
    }


    private void AddScore(int x, int y, int num)
    {
        Character c = Characters[x, y];

        if (isChickenTurn)
        { 
            chickenScore += num;
            ChickenScoreText.text = "Score : " + chickenScore;
        }

        else if (!isChickenTurn)
        {
            dragonScore += num;
            DragonScoreText.text = "Score : " + dragonScore;

        }

        if (chickenScore >= 100 || dragonScore >= 100)
        {
            WhoWinner();
        }
    }


    void Movepac(int x1, int y1, int x2, int y2)
    {
        //패킷 구조체에 담고 마샬링해서 바이트 배열에 담기
        PT_C_MOVE Move_Pack = new PT_C_MOVE(x1, y1, x2, y2);

        //직렬화시켜 보내기
        sendByte = Serialize(Move_Pack);
        AsyncClient.AsyncObject.Instance().asyncSocket.Send(sendByte, sendByte.Length, 0);

        Debug.Log("sendByte :" + sendByte.Length);
    }

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


}



