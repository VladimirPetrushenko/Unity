using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{
    private int QuantityToWin { get; set; } = 3;
    private int SizeMap { get; set; } = 3;
    //all winner combination with this size map
    private int[,] winCombination;
    public enum GameModes { StartGame = 0, ChoicePlayer = 1, GameVSCPU = 2, EndGame = 3}
    Figure WhoWin = Figure.Blank;
    public enum Figure { Blank = 0, Cross = 1, Zero = 2, Frame = 3}
    //save player choice
    public Figure choicePlayer = Figure.Blank, CPU = Figure.Blank;
    // player can step or not
    public bool canStep = true;
    // need Draw or not
    bool DrawSelect = true;
    // link into prefab BlankImage
    public GameObject Img;
    //Game map
    public GameObject[] selectMap;
    //Game mode to switch modes
    public GameModes GameMode { get; private set; } = GameModes.StartGame;
    //Drawed Start Menu
    private void mainMenu()
    {
        //reset player selection from previous game
        choicePlayer = 0; CPU = 0;
        Rect rect = DrawLabelToDisplay("Menu game", 100, 40, new GUIStyle());
        //m = (int)GUILayout.HorizontalSlider(m, 3, 10);
        //shitft the center to the width of the button frame
        rect.x = rect.x - 15;

        bool gameChange;

        rect = DrawButtons(rect, "Press to start", out gameChange);
        //if button1 was pressed change gameMode
        if (gameChange)
            GameMode = GameModes.ChoicePlayer;

        rect = DrawButtons(rect, "Exit", out gameChange);
        //if button2 was pressed change gameMode
        if (gameChange)
            Application.Quit();
    }

    private static Rect DrawLabelToDisplay(string label, float widthLabel, float heightLabel, GUIStyle gUI)
    {
        // middle of the screen
        float width = Screen.width / 2;
        // 2/3 screen on height
        float height = Screen.height / 3;

        //shift the center by half the label
        width = width - widthLabel / 2;

        Rect rect = new Rect(width, //start position for width
                            height, //start position for height
                            widthLabel, //width from starting position
                            heightLabel);//height from starting position

        //text to display
        GUI.Label(rect, label, gUI);
        return rect;
    }

    private void OnGUI()
    {
        switch (GameMode)
        {
            case GameModes.StartGame:
                mainMenu();
                break;
            case GameModes.ChoicePlayer:
                ChoicePlayer();
                break;
            case GameModes.GameVSCPU:
                // Draw new game
                GameVsCPU();
                break;
            case GameModes.EndGame:
                FinishGame();
                break;
        }
    }

    private void ChoicePlayer()
    {
        if (DrawSelect)
            DrawChoiseToDisplay();
        ChoiceOfMapSize();
        if (GameMode == GameModes.GameVSCPU)
        {
            DestroyGameObject();
            DrawSelect = true;
        }
        if (choicePlayer == Figure.Zero)
            canStep = false;
    }

    private void GameVsCPU()
    {
        if (DrawSelect)
        {
            DrawBlanksToDisplay(new Vector3(-4.2f + 1f * 3f / SizeMap,
                                -4.3f + 1f * 3f / SizeMap, 0), SizeMap, SizeMap);
            FillingWinCombination();
        }
        //if game map is end, next stage
        if (!GameContinue())
            GameMode = GameModes.EndGame;
        // check if there is a winner
        if (TestWin(choicePlayer)) { WhoWin = choicePlayer; GameMode = GameModes.EndGame; }
        else if (TestWin(CPU)) { WhoWin = CPU; GameMode = GameModes.EndGame; }
        if (!canStep && GameMode != GameModes.EndGame)
            CPUStep();
    }

    public void Highlighting()
    {
        DestroyGameObject();
        selectMap = new GameObject[3];
        Vector3 vector = new Vector3(-3, 3, 0);
        Vector3 scale = new Vector3(0.8f, 0.8f, 1);
        Vector3 defaultScale = new Vector3(1f, 1f, 1);
        Vector3 playerPosition = new Vector3(0, 0, 0);
        Vector3 CPUPosition = new Vector3(0, 0, 0);
        if (choicePlayer == Figure.Cross)
        {
            CPUPosition = new Vector3(6, 0, 0);
        }
        if (choicePlayer == Figure.Zero)
        {
            playerPosition = new Vector3(6, 0, 0);
        }
        DrawOneImgScale(vector, playerPosition, scale, 0, choicePlayer);
        DrawOneImgScale(vector, CPUPosition, defaultScale, 1, CPU);
        DrawOneImgScale(vector, playerPosition, defaultScale, 2, Figure.Frame);
        SpriteRenderer renderer = selectMap[0].GetComponent<SpriteRenderer>();
        renderer.sortingOrder = 1;
    }
    private void FinishGame()
    {
        //Destroy all game object
        //DestroyGameObject();
        Rect rect;
        GUIStyle gUI = new GUIStyle();
        gUI.fontSize = 50;
        gUI.alignment = TextAnchor.MiddleCenter;
        //Drawing winner on display
        if (WhoWin == choicePlayer)
            rect = DrawLabelToDisplay("Player wins! Congratulate!", 200, 100, gUI);
        else if (WhoWin == CPU)
            rect = DrawLabelToDisplay("Computer wins! Sorry!", 200, 100, gUI);
        else
            rect = DrawLabelToDisplay("Dead heat!", 200, 100, gUI);
        bool clickButton;
        //rect.width -= 50;
        //rect.height -= 25;
        DrawButtons(rect, "Back to Main menu", out clickButton);
        //when the button is pressed, load the launch of the game
        if (clickButton)
            SceneManager.LoadScene("XO_Unity", LoadSceneMode.Single);
    }

    private bool GameContinue()
    {
        //check if there are any unoccupied places
        foreach (var s in selectMap)
        {
            ImgChange change = s.GetComponent<ImgChange>();
            if (change.ImageStatus == 0)
            {
                return true;
            }
        }
        return false;
    }
    private void CPUStep()
    {
        //random computer step
        int index = Random.Range(0, SizeMap * SizeMap);
        ImgChange change = selectMap[index].GetComponent<ImgChange>();
        if (change.ImageStatus == 0)
        {
            change.ImageStatus = CPU;
            canStep = true;
        }
    }

    // destroy all game object on display
    private void DestroyGameObject()
    {
        DrawSelect = false;
        if (selectMap != null)
            foreach (var s in selectMap)
                Destroy(s);
    }

    //Draw new map in the game
    private void DrawBlanksToDisplay(Vector3 vector, int rows, int colums)
    {
        Vector3 scale = new Vector3(1f * 3f / SizeMap, 1f * 3f / SizeMap, 1);
        selectMap = new GameObject[rows*colums];
        for(int j = 0; j < rows; j++)
        {
            for(int i = 0; i < colums; i++)
            {
                DrawOneImg(vector, new Vector3(3.2f * i * scale.x, 3.2f * j * scale.y, 0), i + SizeMap * j);
            }
        }
        DrawSelect = false;
    }

    private void DrawChoiseToDisplay()
    {
        //start position
        Vector3 vector = new Vector3(-3, 3, 0);
        //created two gameObjects (X and 0)
        selectMap = new GameObject[2];
        //drawed a cross to Display
        DrawOneImg(vector, new Vector3(0, 0, 0), 0, Figure.Cross);
        //draw a zero to Display
        DrawOneImg(vector,new Vector3(6,0,0), 1, Figure.Zero);
        DrawSelect = false;
    }

    private void DrawOneImg(Vector3 vector, Vector3 changeVector, int i, Figure status = Figure.Blank)
    {
        DrawOneImgScale(vector, changeVector, new Vector3(3f / SizeMap, 3f / SizeMap, 1), i, status);
    }
    //Draw image on display
    private void DrawOneImgScale(Vector3 vector,Vector3 changeVector, Vector3 scale, int i, Figure status = Figure.Blank)
    {
        vector = vector + changeVector;
        selectMap[i] = Instantiate(Img, vector, Quaternion.identity);
        selectMap[i].transform.localScale = new Vector3(1 * scale.x, 1 * scale.y, 1 * scale.z);
        ImgChange change = selectMap[i].GetComponent<ImgChange>();
        change.ImageStatus = status;
    }

    private static Rect DrawButtons(Rect rect, string label, out bool m)
    {
        // distance between buttons
        float distance = 10;
        rect.y = rect.y + rect.height + distance;
        m = GUI.Button(rect, label);
        return rect;
    }
    //check who winner
    private bool TestWin(Figure Who)
    {
        int[] testMap = new int[SizeMap*SizeMap];
        if (selectMap != null)
        {
            //transfer all Who points to an array
            for (int i = 0; i < selectMap.Length; i++)
            {
                GameObject s = selectMap[i];
                ImgChange change = s.GetComponent<ImgChange>();
                if (change.ImageStatus == Who)
                {
                    testMap[selectMap.Length-i-1] = 1;
                }
            }
            //check if matches with the winning combination
            for (int variableWin = 0; variableWin < winCombination.GetUpperBound(0) + 1; variableWin++)
            {
                byte[] rows = new byte[SizeMap];
                byte indexRow = 0;
                for (int i = 0; i < testMap.Length; i++)
                {
                    if (winCombination[variableWin, i] == 1)
                    {
                        if (testMap[i] == 1)
                            rows[indexRow]++;
                        else
                            indexRow++;
                    }
                }
                foreach (var r in rows)
                    if (r >= QuantityToWin)
                        return true;
            }
        }
        return false;
    }
    private void FillingWinCombination()
    {
        winCombination = new int[SizeMap * 2 + 2, SizeMap * SizeMap];
        int rows = winCombination.GetUpperBound(0) + 1;
        //Заполнение построчно
        for (int i = 0; i < SizeMap; i++)
            for (int j = 0; j < SizeMap; j++)
                winCombination[i, j + i * SizeMap] = 1;
        //Заполнение сталбцов
        for (int i = SizeMap; i < SizeMap * 2; i++)
            for (int j = 0; j < SizeMap; j++)
                winCombination[i, j * SizeMap + i - SizeMap] = 1;
        for (int j = 0; j < SizeMap; j++)
        {
            //главная диагональ
            winCombination[SizeMap * 2, j + j * SizeMap] = 1;
            //побочная диагональ
            winCombination[SizeMap * 2 + 1, SizeMap * SizeMap - SizeMap - j * (SizeMap - 1)] = 1;
        }
    }
    private void ChoiceOfMapSize()
    {
        var rc = new Rect(Screen.width / 3 + 10, Screen.height / 2, 100, 20);
        GUI.Label(rc, "Size map");
        rc = new Rect(rc.x - 25, rc.y + 20, 100, 20);
        SizeMap = (int)GUI.HorizontalSlider(rc, SizeMap, 3, 10);
        rc = new Rect(rc.x + 105, rc.y - 5, 40, 20);
        GUI.TextField(rc, SizeMap.ToString() + "x" + SizeMap.ToString());
        rc = new Rect(rc.x + 80, rc.y - 15, 100, 40);
        GUI.Label(rc, "Number to win");
        rc = new Rect(rc.x - 9, rc.y + 20, 100, 40);
        QuantityToWin = (int)GUI.HorizontalSlider(rc, QuantityToWin, 3, SizeMap);
        rc = new Rect(rc.x + 105, rc.y - 5, 20, 20);
        GUI.TextField(rc, QuantityToWin.ToString());
        rc = new Rect(Screen.width / 2 - 50, rc.y, 100, 40);
        bool flag;
        DrawButtons(rc, "Start Game", out flag);
        if (flag && choicePlayer != 0) 
            GameMode = GameModes.GameVSCPU;
    }
}
