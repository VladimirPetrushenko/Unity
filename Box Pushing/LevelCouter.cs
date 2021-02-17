using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCouter : MonoBehaviour
{
    private static LevelCouter instance = null;
    public int LevelLoadTimes = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (instance) { DestroyImmediate(this); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
