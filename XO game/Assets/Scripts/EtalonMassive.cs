using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EtalonMassive
{
    //all winner combination with this size map
    private int[,] winCombination;
    private int numberArray = 0;
    private int sizeArray = 0;
    public EtalonMassive(int QuantityToWin)
    {
        numberArray = QuantityToWin * 2 + 2;
        sizeArray = QuantityToWin;
    }
    public int [,] WinCombinatio { get => winCombination; }
    public void FillingWinCombination()
    {
        winCombination = new int[numberArray, sizeArray * sizeArray];
        int rows = winCombination.GetUpperBound(0) + 1;
        //Заполнение построчно
        for (int i = 0; i < sizeArray; i++)
            for (int j = 0; j < sizeArray; j++)
                winCombination[i, j + i * sizeArray] = 1;
        //Заполнение сталбцов
        for (int i = sizeArray; i < sizeArray * 2; i++)
            for (int j = 0; j < sizeArray; j++)
                winCombination[i, j * sizeArray + i - sizeArray] = 1;
        for (int j = 0; j < sizeArray; j++)
        {
            //главная диагональ
            winCombination[sizeArray * 2, j + j * sizeArray] = 1;
            //побочная диагональ
            winCombination[sizeArray * 2 + 1, sizeArray * sizeArray - sizeArray - j * (sizeArray - 1)] = 1;
        }
    }
}
