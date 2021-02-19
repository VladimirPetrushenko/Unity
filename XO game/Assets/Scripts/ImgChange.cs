using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgChange : MonoBehaviour
{
    public Figure ImageStatus { get; set; } = Figure.Blank;

    public Sprite[] sprites = new Sprite[4];
    void ChangeImg()
    {
        SpriteRenderer sprite = this.GetComponent("SpriteRenderer") as SpriteRenderer;
        switch (ImageStatus)
        {
            case Figure.Blank: sprite.sprite = sprites[0]; break;
            case Figure.Cross: sprite.sprite = sprites[1]; break;
            case Figure.Zero: sprite.sprite = sprites[2]; break;
            case Figure.Frame: sprite.sprite = sprites[3]; break;
            case Figure.Cube: sprite.sprite = sprites[4]; break;
        }
    }

    private void OnMouseDown()
    {
        GameObject game = GameObject.FindGameObjectWithTag("Player");
        MainGame main = game.GetComponent<MainGame>();
        if (main.GameMode == GameModes.ChoicePlayer && ImageStatus != Figure.Frame) 
        {
            main.players[0] = new Player { PlayerFigure = ImageStatus };
            if (ImageStatus == Figure.Cross)
            {
                main.players[1] = new ComputerPlayer { PlayerFigure = Figure.Zero };
            }
            if (ImageStatus == Figure.Zero)
            {
                main.players[1] = new ComputerPlayer { PlayerFigure = Figure.Cross };
            }
            main.Highlighting();
        }
        if(main.GameMode == GameModes.GameVSCPU)
        {
            for (int i = 0; i < main.players.Length; i++)
            {
                if (main.players[i].CanStep && ImageStatus == 0)
                {
                    ImageStatus = main.players[i].PlayerFigure;
                    main.players[i].CanStep = false;
                    main.players[(i+1) % main.countPlayers].CanStep = true;
                    if (main.MultuPlayer)
                        main.ChangeBackground(main.players[(i + 1) % main.countPlayers].PlayerFigure);
                    break;
                }
            }
            ////the player's turn and the field is empty
            //if (main.canStep && ImageStatus == 0)
            //{
            //    ImageStatus = main.players[0].PlayerFigure;
            //    main.canStep = false;
            //    if (main.MultuPlayer)
            //        main.ChangeBackground(false);
            //}
            //else if (!main.canStep && ImageStatus == 0)
            //{
            //    ImageStatus = main.players[1].PlayerFigure;
            //    main.canStep = true;
            //    if (main.MultuPlayer)
            //        main.ChangeBackground(true);
            //}
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ChangeImg();
    }
}
