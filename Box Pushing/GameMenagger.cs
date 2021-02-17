using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenagger : MonoBehaviour
{

    private AudioSource[] audio = null;
    private static GameMenagger instance = null;
    public LevelCouter LC = null;
    public static GameMenagger Instance
    {
        get
        {
            if (!instance) instance = new GameMenagger();
            return instance;
        }
    }
    public Crate[] Crates = null;
    private bool bLevelCompleted = false;
    private bool bAcceptInput = true;
    private bool LevelCompleted
    {
        get { return bLevelCompleted; }
        set
        {
            bLevelCompleted = value;
            if (value)
            {
                bAcceptInput = false;
                StartCoroutine(MoveToNextLevel());
                //Play Audio
                if (!audio[0].isPlaying)
                {
                    audio[0].Play();
                    audio[1].Stop();
                }
            }
        }
    }
    private float WinWaitInterval = 2.0f;
    public int NextLevel = 0;
    private Rect WinDisplayPos = new Rect();
    private Rect WinTexCoords = new Rect();
    public Sprite LevelCompleteGraphic = null;
    //Data manager
    private DataManager DM = null;
    private PlayerController PC = null;
    
    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
        WinTexCoords.x = LevelCompleteGraphic.rect.x / LevelCompleteGraphic.texture.width;
        WinTexCoords.y = LevelCompleteGraphic.rect.y / LevelCompleteGraphic.texture.height;
        WinTexCoords.width = (LevelCompleteGraphic.rect.x + LevelCompleteGraphic.rect.width) / LevelCompleteGraphic.texture.width;
        WinTexCoords.height = (LevelCompleteGraphic.rect.y + LevelCompleteGraphic.rect.height) / LevelCompleteGraphic.texture.height;
        Crates = GameObject.FindObjectsOfType<Crate>() ;

        DM = new DataManager();
        PC = GameObject.FindObjectOfType<PlayerController>();
        audio = Object.FindObjectsOfType<AudioSource>();
        audio[1].Play();
    }

    public bool CheckForWin()
    {
        if (bLevelCompleted) return true;
        foreach(Crate c in Crates)
        {
            if (!c.bIsOnDestination)
                return false;
        }
        LevelCompleted = true;
        return true;
    }
    private void OnGUI()
    {

        if (!bLevelCompleted)
        {
            return;
        }
        GUI.DrawTextureWithTexCoords(WinDisplayPos, LevelCompleteGraphic.texture, WinTexCoords);
    }
    public IEnumerator MoveToNextLevel()
    {
        yield return new WaitForSeconds(WinWaitInterval);
        Application.LoadLevel(NextLevel);
    }
    // Update is called once per frame
    void Update()
    {
        WinDisplayPos.x = Screen.width / 2 - LevelCompleteGraphic.rect.width / 2;
        WinDisplayPos.y = Screen.height / 2 - LevelCompleteGraphic.rect.height / 2;
        WinDisplayPos.width = LevelCompleteGraphic.rect.width;
        WinDisplayPos.height = LevelCompleteGraphic.rect.height;
        CheckForWin();
    }
    private void Start()
    {
        LC = GameObject.FindObjectOfType<LevelCouter>();
        if (PlayerPrefs.HasKey("LastLevel") && (LC.LevelLoadTimes<=0))
        {
            int LastLevel = PlayerPrefs.GetInt("LastLevel");
            if (LastLevel > Application.loadedLevel)
            {
                Application.LoadLevel(LastLevel);
                return;
            }
            PlayerPrefs.SetInt("LastLevel", Application.loadedLevel);
            
            RestoreGame();
        }
        LC.LevelLoadTimes++;
    }


    public void SaveGame()
    {
        DM.GD.BD.Clear();

        foreach (Crate C in Crates)
        {
            DataManager.BoxData BD = new DataManager.BoxData();
            BD.BoxTransform.X = C.transform.position.x;
            BD.BoxTransform.Y = C.transform.position.y;
            BD.BoxTransform.Z = C.transform.position.z;
            BD.BoxTransform.RotX = C.transform.eulerAngles.x;
            BD.BoxTransform.RotY = C.transform.eulerAngles.y;
            BD.BoxTransform.RotZ = C.transform.eulerAngles.z;
            BD.BoxTransform.ScaleX = C.transform.localScale.x;
            BD.BoxTransform.ScaleY = C.transform.localScale.y;
            BD.BoxTransform.ScaleZ = C.transform.localScale.z;
            BD.onDestination = C.bIsOnDestination;

            DM.GD.BD.Add(BD);
        }
        DataManager.PlayerData PD = new DataManager.PlayerData();
        DM.GD.PD.PlayerTransform.X = PC.transform.position.x;
        DM.GD.PD.PlayerTransform.Y = PC.transform.position.y;
        DM.GD.PD.PlayerTransform.Z = PC.transform.position.z;
        DM.GD.PD.PlayerTransform.RotX = PC.transform.eulerAngles.x;
        DM.GD.PD.PlayerTransform.RotY = PC.transform.eulerAngles.y;
        DM.GD.PD.PlayerTransform.RotZ = PC.transform.eulerAngles.z;
        DM.GD.PD.PlayerTransform.ScaleX = PC.transform.localScale.x;
        DM.GD.PD.PlayerTransform.ScaleY = PC.transform.localScale.y;
        DM.GD.PD.PlayerTransform.ScaleZ = PC.transform.localScale.z;

        DM.Save(Application.persistentDataPath + "/GameSave.xml");
    }
    public void RestoreGame()
    {
        if(System.IO.File.Exists(Application.persistentDataPath + "/GameSave.xml"))
        {
            DM.Load(Application.persistentDataPath + "/GameSave.xml");
            for(int i=0; i < DM.GD.BD.Count; i++)
            {
                Crates[i].transform.position = new Vector3(DM.GD.BD[i].BoxTransform.X, DM.GD.BD[i].BoxTransform.Y, DM.GD.BD[i].BoxTransform.Z);
                Crates[i].transform.rotation = Quaternion.Euler(DM.GD.BD[i].BoxTransform.RotX, DM.GD.BD[i].BoxTransform.RotY, DM.GD.BD[i].BoxTransform.RotZ);
                Crates[i].transform.localScale = new Vector3(DM.GD.BD[i].BoxTransform.ScaleX, DM.GD.BD[i].BoxTransform.ScaleY, DM.GD.BD[i].BoxTransform.ScaleZ);
                Crates[i].bIsOnDestination = DM.GD.BD[i].onDestination;
            }

            PC.transform.position = new Vector3(DM.GD.PD.PlayerTransform.X, DM.GD.PD.PlayerTransform.Y, DM.GD.PD.PlayerTransform.Z);
            PC.transform.rotation = Quaternion.Euler(DM.GD.PD.PlayerTransform.RotX, DM.GD.PD.PlayerTransform.RotY, DM.GD.PD.PlayerTransform.RotZ);
            PC.transform.localScale = new Vector3(DM.GD.PD.PlayerTransform.ScaleX, DM.GD.PD.PlayerTransform.ScaleY, DM.GD.PD.PlayerTransform.ScaleZ);
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
