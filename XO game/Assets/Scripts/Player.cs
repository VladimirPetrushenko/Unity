using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    public Figure PlayerFigure { get; set; } = Figure.Blank;
    public bool CanStep { get; set; } = false;
    public bool TestWin(GameObject[] selectMap, int[,] winCombination, int SizeMap, int QuantityToWin)
    {
        if (selectMap == null)
            return false;
        //transfer all PlayerFigure points to an int array
        int[] testMap = CreateIntMapArray(selectMap);
        return ProcedureTestWin(testMap, winCombination, SizeMap, QuantityToWin, PlayerFigure);
    }
    public bool ProcedureTestWin(int[] selectMap, int[,] winCombination, int SizeMap, int QuantityToWin, Figure player )
    {
        //cell offset along the x and y axes, if QuantityToWin is different from SizeMap
        for (int x = 0; x < SizeMap - QuantityToWin + 1; x++)
        {
            for (int y = 0; y < SizeMap - QuantityToWin + 1; y++)
            {
                if (FindMatchesForWin(selectMap, winCombination, SizeMap, QuantityToWin, y, x, player))
                    return true;
            }
        }
        return false;
    }
    //search whether there was a victory on a certain area of ​​the map with an offset 
    //of k along the x-axis and m along the y-axis
    protected bool FindMatchesForWin(int[] testMap, int[,] winCombination, int SizeMap, int QuantityToWin, int k, int m, Figure player)
    {
        for (int variableWin = 0; variableWin < winCombination.GetUpperBound(0) + 1; variableWin++)
        {
            byte[] rows = new byte[SizeMap];
            byte indexRow = 0;
            for (int i = 0; i < QuantityToWin; i++)
            {
                for (int j = 0; j < QuantityToWin; j++)
                {
                    if (winCombination[variableWin, i * QuantityToWin + j] == 1)
                    {
                        if (testMap[k + m * SizeMap + i * SizeMap + j] == (int)player)
                            rows[indexRow]++;
                        else
                            indexRow++;
                    }
                }
            }
            foreach (var r in rows)
                if (r >= QuantityToWin)
                    return true;
        }
        return false;
    }
    protected static int[] CreateIntMapArray(GameObject[] selectMap)
    {
        int[] originArray = new int[selectMap.Length];
        for (int i = 0; i < selectMap.Length; i++)
        {
            ImgChange select = selectMap[i].GetComponent<ImgChange>();
            originArray[i] = (int)select.ImageStatus;
        }
        return originArray;
    }
}
