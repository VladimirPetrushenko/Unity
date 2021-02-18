using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgChange : MonoBehaviour
{
    public int ImageStatus { get; set; } = 0;

    public Sprite[] sprites = new Sprite[3];
    void ChangeImg()
    {
        SpriteRenderer sprite = this.GetComponent("SpriteRenderer") as SpriteRenderer;
        switch (ImageStatus)
        {
            case 0: sprite.sprite = sprites[0]; break;
            case 1: sprite.sprite = sprites[1]; break;
            case 2: sprite.sprite = sprites[2]; break;
        }
    }

    private void OnMouseDown()
    {
        GameObject game = GameObject.FindGameObjectWithTag("Player");
        MainGame main = game.GetComponent<MainGame>();
        if (main.GameMode == 1)
        {
            main.choicePlayer = ImageStatus;
            if (ImageStatus == 1) main.CPU = 2;//0
            if (ImageStatus == 2) main.CPU = 1;//X
        }
        if(main.GameMode == 2)
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
