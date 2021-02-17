using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
public class DataManager
{
    [XmlRoot("GameData")]
    public struct DataTransform
    {
        public float X;
        public float Y;
        public float Z; 
        public float RotX;
        public float RotY;
        public float RotZ;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
    }

    public class BoxData
    {
        public DataTransform BoxTransform;
        public bool onDestination = false;
    }
    public class PlayerData
    {
        public DataTransform PlayerTransform;
    }

    public class GameData
    {
        public List<BoxData> BD = new List<BoxData>();
        public PlayerData PD = new PlayerData();
    }

    public GameData GD = new GameData();

    public void Save(string FileName = "GameData.xml")
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        FileStream file = new FileStream(FileName, FileMode.Create);
        serializer.Serialize(file, GD);
        file.Close();
    }
    public void Load(string FileName = "GameData.xml")
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        FileStream file = new FileStream(FileName, FileMode.Open);
        GD = serializer.Deserialize(file) as GameData;
        file.Close();
    }
}
