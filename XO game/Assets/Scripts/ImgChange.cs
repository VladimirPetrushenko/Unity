using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgChange : MonoBehaviour
{
    public MainGame.Figure ImageStatus { get; set; } = MainGame.Figure.Blank;

    public Sprite[] sprites = new Sprite[4];
    void ChangeImg()
    {
        SpriteRenderer sprite = this.GetComponent("SpriteRenderer") as SpriteRenderer;
        switch (ImageStatus)
        {
            case MainGame.Figure.Blank: sprite.sprite = sprites[0]; break;
            case MainGame.Figure.Cross: sprite.sprite = sprites[1]; break;
            case MainGame.Figure.Zero: sprite.sprite = sprites[2]; break;
            case MainGame.Figure.Frame: sprite.sprite = sprites[3]; break;
        }
    }

    private void OnMouseDown()
    {
        GameObject game = GameObject.FindGameObjectWithTag("Player");
        MainGame main = game.GetComponent<MainGame>();
        if (main.GameMode == MainGame.GameModes.ChoicePlayer && ImageStatus!=MainGame.Figure.Frame)
        {
            main.choicePlayer = ImageStatus;
            if (ImageStatus == MainGame.Figure.Cross) main.CPU = MainGame.Figure.Zero;//0
            if (ImageStatus == MainGame.Figure.Zero) main.CPU = MainGame.Figure.Cross;//X
            main.Highlighting();
        }
        if(main.GameMode == MainGame.GameModes.GameVSCPU)
        {
            //the player's turn and the field is empty
            if (main.canStep && ImageStatus == 0) 
            {
                ImageStatus = main.choicePlayer;
                main.canStep = false;
            }
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
