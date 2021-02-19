using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum DifficultyLevel {Easy = 0, Medium = 1, Hard = 2 }
class ComputerPlayer : Player
{
    DifficultyLevel level = DifficultyLevel.Easy;
    public void Step(int SizeMap, GameObject[] selectMap)
    {
        switch (level)
        {
            case DifficultyLevel.Easy:
                randomStep(SizeMap, selectMap);
                break;
            case DifficultyLevel.Hard:
                MiniMax(SizeMap, selectMap);
                break;
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
    private void MiniMax(int SizeMap, GameObject[] selectMap)
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
}
