using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class TileEditorLogic : MonoBehaviour
{
    public static GameObject tileEditorLogic;

    #region Visual Representation of Map & Buttons
    GameObject[,] mapTiles;
    float tileVisualSize;

    GameObject mapTilesContainerParent;
    GameObject mapSpritesContainerParent;
    GameObject tileButtonsContainerParent;
    GameObject spriteButtonsContainerParent;

    #endregion

    #region CanvasUI Elements

    GameObject canvasResizeButton;
    GameObject canvasResizeXInputField;
    GameObject canvasResizeYInputField;

    GameObject canvasSaveButton;
    GameObject canvasLoadButton;
    GameObject canvasFileNameInputField;

    #endregion

    void Start()
    {
        if (TileEditorLogic.tileEditorLogic != null)
        {
            Debug.Log("Singleton violation in TileEditorLogic!");
            return;
        }

        TileEditorLogic.tileEditorLogic = this.gameObject;

        TextureSpriteID.Init();
        MapData.Init();
        ContentLoader.Init();
        ConnectToSceneCanvasUI();

        tileButtonsContainerParent = new GameObject("TileButtonsContainerParent");
        spriteButtonsContainerParent = new GameObject("SpriteButtonsContainerParent");

        float yOffset = 0.25f;
        int c = 0;

        foreach (int tileID in TextureSpriteID.AllTiles)
        {
            c++;

            GameObject tileButton = ContentLoader.GetNewMapTileGameObject(tileID);
            tileButton.GetComponent<SpriteRenderer>().sprite = ContentLoader.GetTexturedSprite(tileID);
            tileButton.gameObject.AddComponent<EditorButton>();
            tileButton.gameObject.GetComponent<EditorButton>().SetMapObjectTypeAndTextureID(MapObjectTypeID.Tile, tileID);

            tileButton.transform.parent = tileButtonsContainerParent.transform;

            tileButton.name = "TileButton" + c;

            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            newPos.z = 0;
            newPos.x = newPos.x - tileButton.GetComponent<SpriteRenderer>().bounds.extents.x;
            newPos.y = newPos.y - tileButton.GetComponent<SpriteRenderer>().bounds.extents.y;

            newPos.y -= yOffset;
            yOffset += tileButton.GetComponent<SpriteRenderer>().bounds.size.y;

            tileButton.transform.position = newPos;
        }

        yOffset = 0;
        c = 0;
        foreach (int tileID in TextureSpriteID.AllSprites)
        {
            c++;

            GameObject tileButton = ContentLoader.GetNewMapSpriteGameObject(tileID);
            tileButton.GetComponent<SpriteRenderer>().sprite = ContentLoader.GetTexturedSprite(tileID);
            tileButton.gameObject.AddComponent<BoxCollider2D>();
            tileButton.gameObject.AddComponent<EditorButton>();
            tileButton.gameObject.GetComponent<EditorButton>().SetMapObjectTypeAndTextureID(MapObjectTypeID.Sprite, tileID);
            
            if(tileID != TextureSpriteID.SpriteEraser)
                tileButton.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            tileButton.transform.parent = spriteButtonsContainerParent.transform;
            tileButton.name = "SpriteButton" + c;

            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            newPos.z = 0;
            newPos.x = -newPos.x + tileButton.GetComponent<SpriteRenderer>().bounds.extents.x;
            newPos.y = newPos.y - tileButton.GetComponent<SpriteRenderer>().bounds.extents.y;

            newPos.y -= yOffset;
            yOffset += tileButton.GetComponent<SpriteRenderer>().bounds.size.y;

            tileButton.transform.position = newPos;
        }

        GameObject temp = ContentLoader.GetNewMapTileGameObject(TextureSpriteID.Grass);
        tileVisualSize = temp.GetComponent<SpriteRenderer>().bounds.size.x;
        Destroy(temp);

        CreateMapVisuals();
    }
    void Update()
    {

    }
    void CreateMapVisuals()
    {
        mapTilesContainerParent = new GameObject("MapTilesContainerParent");
        mapSpritesContainerParent = new GameObject("MapSpritesContainerParent");

        mapTiles = new GameObject[MapData.numTilesX, MapData.numTilesY];

        Vector3 TopLeftOfLayout = new Vector3(-(float)MapData.numTilesX / 2f * tileVisualSize + (tileVisualSize / 2f), -(float)MapData.numTilesY / 2f * tileVisualSize + (tileVisualSize / 2), 0);
        Vector3 currentPos = TopLeftOfLayout;

        for (int i = 0; i < MapData.numTilesX; i++)
        {
            for (int j = 0; j < MapData.numTilesY; j++)
            {
                mapTiles[i, j] = ContentLoader.GetNewMapTileGameObject(MapData.mapTiles[i, j]);
                mapTiles[i, j].transform.parent = mapTilesContainerParent.transform;
                mapTiles[i, j].transform.position = currentPos;
                currentPos.y += tileVisualSize;
            }
            currentPos.y = TopLeftOfLayout.y;
            currentPos.x += tileVisualSize;
        }

        foreach (MapSpriteDataRepresentation s in MapData.mapSprites)
        {
            GameObject go = ContentLoader.GetNewMapSpriteGameObject(s.id);
            go.transform.parent = mapSpritesContainerParent.transform;
            Vector3 tilePos = mapTiles[s.x, s.y].transform.position;
            go.transform.position = tilePos;
            go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
    public void DestoryMapVisuals()
    {
        Destroy(mapTilesContainerParent);
        Destroy(mapSpritesContainerParent);
    }

    public void ResizeButtonPressed()
    {
        int x, y;

        try
        {
            x = int.Parse(canvasResizeXInputField.GetComponent<InputField>().text);
            y = int.Parse(canvasResizeYInputField.GetComponent<InputField>().text);
        }
        catch
        {
            Debug.Log("Unable to convert to int, aborting resize.");
            return;
        }

        MapData.ProcessResize(x, y);
        DestoryMapVisuals();
       CreateMapVisuals();
    }
    public void SaveButtonPressed()
    {
        string fileName = canvasFileNameInputField.GetComponent<InputField>().text;
        MapData.ProcessSaveMap(fileName);
    }
    public void LoadButtonPressed()
    {

        DestoryMapVisuals();
        string fileName = canvasFileNameInputField.GetComponent<InputField>().text;
        MapData.ProcessLoadMap(fileName);
        //i add here
        
       
        
        CreateMapVisuals();



    }
    public void MapTilePressed(GameObject sender)
    {
        for (int i = 0; i < MapData.numTilesX; i++)
        {
            for (int j = 0; j < MapData.numTilesY; j++)
            {
                if (mapTiles[i, j] == sender)
                {
                    MapData.ProcessMapTilePressed(i, j);
                }
            }
        }

        DestoryMapVisuals();
        CreateMapVisuals();
    }
    public void EditorButtonPressed(int mapObjectType, int textureID)
    {
        MapData.ProcessEditorButtonPressed(mapObjectType, textureID);
    }

}

