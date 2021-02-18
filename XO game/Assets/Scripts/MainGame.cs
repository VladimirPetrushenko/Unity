using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{
    int WhoWin = 0;
    //save player choice
    public int choicePlayer = 0, CPU = 0;
    // player can step or not
    public bool canStep = true;
    // need Draw or not
    bool DrawSelect = true;
    // link into prefab BlankImage
    public GameObject Img;
    //Game map
    public GameObject[] selectMap;
    //Game mode to switch modes
    public int GameMode { get; private set; } = 0;
    //Drawed Start Menu
    private void mainMenu()
    {
        Rect rect = DrawLabelToDisplay("Меню игры", 100, 40, new GUIStyle());

        //shitft the center to the width of the button frame
        rect.x = rect.x - 15;

        bool gameChange;

        rect = DrawButtons(rect, "Начать игру", out gameChange);
        //if button1 was pressed change gameMode
        if (gameChange)
            GameMode = 1;

        rect = DrawButtons(rect, "Выход", out gameChange);
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
        if (GameMode == 0)
        {
            mainMenu();
            //reset player selection from previous game
            choicePlayer = 0;
            CPU = 0;
        }
        if (GameMode == 1)
        {
            
            if (DrawSelect)
                DrawChoiseToDisplay();
            if (choicePlayer != 0)
            {
                GameMode = 2;
                DestroyGameObject();
            }
        }
        if (GameMode == 2)
        {
            // Draw new game
            if (DrawSelect)
                DrawBlanksToDisplay(new Vector3(-3.2f, -3.3f, 0), 9);
            if (!canStep)
                CPUStep();
            //if game map is end, next stage
            if (!GameContinue()) 
                GameMode = 3;
            // check if there is a winner
            if (TestWin(choicePlayer)) { WhoWin = choicePlayer; GameMode = 3; }
            if (TestWin(CPU)) { WhoWin = CPU; GameMode = 3; }
        }
        if (GameMode == 3)
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        //Destroy all game object
        DestroyGameObject();
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
        rect.width -= 50;
        rect.height -= 25;
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
        int index = Random.Range(0, 9);
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
        DrawSelect = true;
        if (selectMap != null)
            foreach (var s in selectMap)
                Destroy(s);
    }

    //Draw new map in the game
    private void DrawBlanksToDisplay(Vector3 vector, int count)
    {
        selectMap = new GameObject[count];
        for(int i = 0; i < Mathf.Sqrt(count); i++)
        {
            for(int j = 0; j < Mathf.Sqrt(count); j++)
            {
                DrawOneImg(vector, new Vector3(3.2f * i, 3.3f * j, 0), 3 * i + j);
            }
        }
        DrawSelect = false;
    }

    private void DrawChoiseToDisplay()
    {
        //start position
        Vector3 vector = new Vector3(-3, 0, 0);
        //created two gameObjects (X and 0)
        selectMap = new GameObject[2];
        //drawed a cross to Display
        DrawOneImg(vector, new Vector3(0, 0, 0), 0, 1);
        //draw a zero to Display
        DrawOneImg(vector,new Vector3(6,0,0), 1, 2);
        DrawSelect = false;
    }
    
    //Draw image on display
    private void DrawOneImg(Vector3 vector,Vector3 changeVector, int i, int status = 0)
    {
        vector = vector + changeVector;
        selectMap[i] = Instantiate(Img, vector, Quaternion.identity);
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
    private bool TestWin(int Who)
    {
        int[,] winCombination = new int[,]{   { 1,1,1, // X X X
                                                0,0,0,
                                                0,0,0},

                                              { 0,0,0,
                                                1,1,1,   // X X X
                                                0,0,0 },

                                              { 0,0,0,
                                                0,0,0,
                                                1,1,1 }, // X X X

                                              { 1,0,0,  // X
                                                0,1,0,  //   X
                                                0,0,1 },//     X

                                              { 0,0,1,  //     X
                                                0,1,0,  //   X
                                                1,0,0 },// X

                                              { 1,0,0,  // X
                                                1,0,0,  // X
                                                1,0,0 },// X

                                              { 0,0,1,  //      X
                                                0,0,1,  //      X
                                                0,0,1 },//      X

                                               {0,1,0,  //   X
                                                0,1,0,  //   X
                                                0,1,0 } }; //X     
        int[] testMap = new int[9];
        if (selectMap != null)
        {
            //transfer all Who points to an array
            for (int i = 0; i < selectMap.Length; i++)
            {
                GameObject s = selectMap[i];
                ImgChange change = s.GetComponent<ImgChange>();
                if (change.ImageStatus == Who)
                {
                    testMap[i] = 1;
                }
            }
            //check if matches with the winning combination
            for (int variableWin = 0; variableWin < 8; variableWin++)
            {
                int win = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (winCombination[variableWin, i] == 1)
                        if (testMap[i] == 1)
                            win++;
                }
                if (win == 3)
                    return true;
            }
        }
        return false;
    }
}
