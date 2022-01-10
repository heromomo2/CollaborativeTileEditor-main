using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


static public class MapData
{
    public static int[,] mapTiles;
    public static LinkedList<MapSpriteDataRepresentation> mapSprites;
   // public static LinkedList<MapSpriteDataRepresentation> mapSpriteTiles;

    public static int numTilesX = 10;
    public static int numTilesY = 5;

    static int selectedEditorButtonTextureID;
    static int selectedEditorButtonMapObjectType;

    static public void Init()
    {
        CreateMapTiles();
        mapSprites = new LinkedList<MapSpriteDataRepresentation>();
        mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.BlackMage1, 2, 2));
        mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.BlackMage1, 3, 2));
        mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.BlackMage1, 2, 3));
    }
    static private void CreateMapTiles()
    {
        mapTiles = new int[numTilesX, numTilesY];
        for (int i = 0; i < numTilesX; i++)
        {
            for (int j = 0; j < numTilesY; j++)
            {
                mapTiles[i, j] = TextureSpriteID.Grass;


            }
        }
    }
    static public void ProcessEditorButtonPressed(int mapObjectType, int textureID)
    {
        selectedEditorButtonMapObjectType = mapObjectType;
        selectedEditorButtonTextureID = textureID;
    }
    static public void ProcessMapTilePressed(int x, int y)
    {
        if (selectedEditorButtonMapObjectType == MapObjectTypeID.Tile)
            mapTiles[x, y] = selectedEditorButtonTextureID;
        else if (selectedEditorButtonMapObjectType == MapObjectTypeID.Sprite)
        {
            if (selectedEditorButtonTextureID != TextureSpriteID.SpriteEraser)
            {
                MapSpriteDataRepresentation removeMe = null;
                foreach (MapSpriteDataRepresentation s in mapSprites)
                {
                    if (s.x == x && s.y == y)
                    {
                        removeMe = s;
                        break;
                    }
                }
                if (removeMe != null)
                    mapSprites.Remove(removeMe);

                mapSprites.AddLast(new MapSpriteDataRepresentation(selectedEditorButtonTextureID, x, y));

            }
            else if (selectedEditorButtonTextureID == TextureSpriteID.SpriteEraser)
            {
                MapSpriteDataRepresentation removeMe = null;
                foreach (MapSpriteDataRepresentation s in mapSprites)
                {
                    if (s.x == x && s.y == y)
                    {
                        removeMe = s;
                        break;
                    }
                }
                if (removeMe != null)
                    mapSprites.Remove(removeMe);
            }

        }
    }
    static public void ProcessResize(int x, int y)
    {
        //Debug.Log("Processing Resize: " + x + "," + y);

        numTilesX = x;
        numTilesY = y;

        CreateMapTiles();


    }
    static public void ProcessSaveMap(string name)
    {
        Debug.Log("Process SaveMap: " + name);

        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + name + ".txt");

        

        sw.WriteLine(GridSaveSignifiers.MapSignifier + "," + numTilesX + "," + numTilesY + ","  );

        foreach (MapSpriteDataRepresentation msdr in mapSprites)
        {
            if (msdr.id == 0)
            {
                sw.WriteLine(GridSaveSignifiers.TileSignifier + "," + msdr.id + "," + msdr.x + "," + msdr.y);
            }
            else 
            {
                sw.WriteLine(GridSaveSignifiers.SpriteSignifier + "," + msdr.id + "," + msdr.x + "," + msdr.y);
            }
        }

        
        
        sw.Close();
        AssetDatabase.Refresh();
    }
    static public void ProcessLoadMap(string name)
    {
        
        Debug.Log("Process LoadMap: " + name);

        if (File.Exists("C:/Users/Owner/Desktop/CollaborativeTileEditor-main/Assets/"+ name + ".txt"))
        {
            mapSprites.Clear();
           

            StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + name+ ".txt");

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');
                
                int signifier = int.Parse(csv[0]);

                switch (signifier)
                {
                    case GridSaveSignifiers.MapSignifier:
                  MapData.numTilesX = int.Parse(csv[1]);
                  MapData.numTilesY = int.Parse(csv[2]);
                        break;

                    case GridSaveSignifiers.SpriteSignifier:
                        mapSprites.AddLast(new MapSpriteDataRepresentation(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3])));
                        break;
                    case GridSaveSignifiers.TileSignifier:
                        mapSprites.AddLast(new MapSpriteDataRepresentation(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3])));
                        break;
                }



            }
                sr.Close();
            
        }
        else 
        {
            Debug.Log("file doesn't exist");
        }
    }

}

public class MapSpriteDataRepresentation
{
    public int id;
    public int x, y;

    public MapSpriteDataRepresentation(int ID, int X, int Y)
    {
        id = ID;
        x = X;
        y = Y;
    }
}


public static class MapObjectTypeID
{
    public const int Tile = 1;
    public const int Sprite = 2;
}



public static class GridSaveSignifiers
{
    public const int MapSignifier = 1;


    public const int SpriteSignifier = 2;


    public const int TileSignifier = 3;


}