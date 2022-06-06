using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Level Texture")]
    [SerializeField] private Texture2D levelTexture;
    [SerializeField] private List<Texture2D> levelTextureList;
    [Header("Tiles- Prefab")]

    [SerializeField] private GameObject prefabWallTile;
    [SerializeField] private GameObject prefabRoadTile;

    [Header("Ball and Road paint color")]
    public Color paintColor;

    [HideInInspector] public List<RoadTile> roadTileList = new List<RoadTile>();
    [HideInInspector] public RoadTile defaultBallRoadTile; //default ball position

    private Color colorWall = Color.white;
    private Color colorRoad = Color.black;

    private float unitPerPixel; //kiremit ölçeði

    public int LevelNumber;

    private void Awake()
    {
        
        
    }
    private void Start()
    {
        LevelNumber = PlayerPrefs.GetInt("Level",0);
        if (LevelNumber == levelTextureList.Count)
        {
            LevelNumber = 0;
        }
        levelTexture = levelTextureList[LevelNumber];
        Generate();
        //toplar için varsayýlan konum olarak ilk yol döþemesini ata
        defaultBallRoadTile = roadTileList[0];
    }

    private void Generate()
    {
        unitPerPixel = prefabWallTile.transform.lossyScale.x; //nesnenin küresel ölçeði. 
        float halfUnitPerPixel = unitPerPixel / 2f;

        float width = levelTexture.width;
        float height = levelTexture.height;

        //ýzgarayý ortalamak için
        Vector3 offset = (new Vector3(width / 2f, 0f, height / 2f) * unitPerPixel) - new Vector3(halfUnitPerPixel, 0f, halfUnitPerPixel);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //get pixel color:
                Color pixelColor = levelTexture.GetPixel(x, y);

                Vector3 spawnPos = ((new Vector3(x, 0f, y) * unitPerPixel) - offset);

                if (pixelColor == colorWall)
                {
                    Spawn(prefabWallTile, spawnPos);
                }
                else if (pixelColor==colorRoad)
                {
                    Spawn(prefabRoadTile, spawnPos);
                }
            }
        }
        
        ///
        /// lossyScale : Objenin genel (global) boyutlarýnýn tutulduðu deðiþkendir.
        /// Deðer atamasý yapýlamaz sadece okunabilir moddadýr. Deðer atamasý yapmak için localScale deðiþkenini kullanabilirsiniz.
        ///

    }

    private void Spawn(GameObject prefabTile, Vector3 position)
    {
        //fix Y position:
        position.y = prefabTile.transform.position.y;

        GameObject obj = Instantiate(prefabTile, position, Quaternion.identity, transform);

        if (prefabTile == prefabRoadTile)
            roadTileList.Add(obj.GetComponent<RoadTile>());
    }

    public void Levelcomplete()
    {
        LevelNumber++;
        PlayerPrefs.SetInt("Level", LevelNumber);
        SceneManager.LoadScene(0);
    }
}
