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
        int[] testMap = new int[SizeMap * SizeMap];
        if (selectMap != null)
        {
            //transfer all PlayerFigure points to an array
            for (int i = 0; i < selectMap.Length; i++)
            {
                GameObject s = selectMap[i];
                ImgChange change = s.GetComponent<ImgChange>();
                if (change.ImageStatus == PlayerFigure)
                {
                    testMap[selectMap.Length - i - 1] = 1;
                }
            }
            //cell offset along the x and y axes, if QuantityToWin is different from SizeMap
            for (int m = 0; m < SizeMap - QuantityToWin + 1; m++)
            {
                for (int k = 0; k < SizeMap - QuantityToWin + 1; k++)
                {
                    if (FindMatchesForWin(testMap, winCombination, SizeMap, QuantityToWin, k, m))
                        return true;
                }
            }
        }
        return false;
    }
    //search whether there was a victory on a certain area of ​​the map with an offset 
    //of k along the x-axis and m along the y-axis
    protected bool FindMatchesForWin(int[] testMap, int[,] winCombination, int SizeMap, int QuantityToWin, int k, int m)
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
                        if (testMap[k + m * SizeMap + i * SizeMap + j] == 1)
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
}
