using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum DifficultyLevel {Easy = 0, Medium = 1, Hard = 2 }
class ComputerPlayer : Player
{
    DifficultyLevel level = DifficultyLevel.Hard;
    public void Step(int SizeMap, GameObject[] selectMap,int[,] winCombination)
    {
        if(SizeMap>3)
                randomStep(SizeMap, selectMap);
        else {
            int index = Move(CreateIntMapArray(selectMap), SizeMap, winCombination);
            ImgChange change = selectMap[index].GetComponent<ImgChange>();
            if (change.ImageStatus == 0)
            {
                change.ImageStatus = PlayerFigure;
                CanStep = false;
            }
        }
    }
    private void randomStep(int SizeMap, GameObject[] selectMap)
    {
        //random computer step
        System.Random random = new System.Random();
        int index = random.Next(0, SizeMap * SizeMap);
        ImgChange change = selectMap[index].GetComponent<ImgChange>();
        if (change.ImageStatus == 0)
        {
            change.ImageStatus = PlayerFigure;
            CanStep = false;
        }
    }
    
    public int Move(int[] mapArray, int sizeMap, int[,] winCombination)
    {
        int bestScore = int.MinValue;
        int bestMove = 0;
        for (int i = 0; i < sizeMap; i++)
        {
            for (int j = 0; j < sizeMap; j++)
            {
                if (mapArray[i * sizeMap + j] == 0)
                {
                    mapArray[i * sizeMap + j] = (int)PlayerFigure;
                    int score = MiniMax(mapArray, PlayerFigure, false, winCombination, sizeMap);
                    mapArray[i * sizeMap + j] = 0;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        // new position for II with a minimax score more that bestscore
                        bestMove = i * sizeMap + j;
                    }
                }
            }
        }
        return bestMove;
    }
    public int MiniMax(int[] mapArray, Figure player, bool isMaximating, int[,] winCombination, int sizeMap)
    {
        if (ProcedureTestWin(mapArray, winCombination, sizeMap, sizeMap, player))
            if (player == PlayerFigure)
                return +1;
            else
                return -1;
        if (mapArray.Where(x => x == 0).Count() == 0)
            return 0;
        if (isMaximating)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < sizeMap; i++)
            {
                for (int j = 0; j < sizeMap; j++)
                {
                    if (mapArray[i * sizeMap + j] == 0)
                    {
                        mapArray[i * sizeMap + j] = (int)PlayerFigure;
                        int score = MiniMax(mapArray, PlayerFigure, false, winCombination, sizeMap);
                        mapArray[i * sizeMap + j] = 0;
                        bestScore = Math.Max(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < sizeMap; i++)
            {
                for (int j = 0; j < sizeMap; j++)
                {
                    if (mapArray[i * sizeMap + j] == 0)
                    {
                        int score = 0;
                        if (PlayerFigure == Figure.Zero)
                        {
                            mapArray[i * sizeMap + j] = (int)Figure.Cross;
                            score = MiniMax(mapArray, Figure.Cross, true, winCombination, sizeMap);
                        }
                        if (PlayerFigure == Figure.Cross)
                        {
                            mapArray[i * sizeMap + j] = (int)Figure.Zero;
                            score = MiniMax(mapArray, Figure.Zero, true, winCombination, sizeMap);
                        }
                        mapArray[i * sizeMap + j] = 0;
                        bestScore = Math.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }
}
