using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character {

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[4, 6];
        Character c;

        //Left
        if (CurrentX != 0 )//&& CurrentY != 0 && CurrentY != 5)
        {
            c = BoardManager.Instance.Characters[CurrentX - 1, CurrentY];
            if (c == null)
                r[CurrentX - 1, CurrentY] = true;
            else if (isChicken != c.isChicken)
                r[CurrentX - 1, CurrentY] = true;
        }


        //Right
        if (CurrentX != 3 )//&& CurrentY != 0 && CurrentY != 5)
        {
            c = BoardManager.Instance.Characters[CurrentX + 1, CurrentY];
            if (c == null )
                r[CurrentX + 1, CurrentY] = true;
            else if (isChicken != c.isChicken)
                r[CurrentX + 1, CurrentY] = true;
        }


         //Up
         if (isChicken)
         {
             
             if (CurrentY != 5)
             {
                 c = BoardManager.Instance.Characters[CurrentX, CurrentY + 1];
                 if (c == null)
                 {
                     r[CurrentX, CurrentY + 1] = true;
                 }
             }

        }
         else
         {
     
             if (CurrentY != 0)
             {
                 c = BoardManager.Instance.Characters[CurrentX, CurrentY - 1];
                 if (c == null)
                 {
                     r[CurrentX, CurrentY - 1] = true;
                 }
             }

        }

        return r;
    }
}
