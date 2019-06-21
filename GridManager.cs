//using System;
//using System;
//using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    //public Sprite[] spriteList;

    const int nAnimalTiles = 8;

    GameObject[] tile_Frames = new GameObject[3];
    Vector2[] tilePositions = new Vector2[nAnimalTiles];
    Vector3[] tileScale = new Vector3[nAnimalTiles];
    GameObject[] tiles = new GameObject[nAnimalTiles];

    List<string> animalNames = new List<string>();
    //private string[] animalNames = {"cat_", "cow_", "dog_", "elephant_", "goat_", "horse_",
    //                                "lion_", "monkey_", "rhino_", "tiger_"};
    protected string currentAnimalType;
    protected string selectedAnimal;
    protected string[] animal_til = new string[nAnimalTiles];

    Collider2D m_Collider;
    Vector3 m_Center;
    Vector3 m_Size, m_Min, m_Max;
    Vector3 selected_position;
    Vector3 target_position;
    Vector3 mid_position;
    Vector3 final_position;

    GameObject selected = null;
    static float waitSeconds = 1f;
    float secondsToWait = 1f;
    bool rightSelection = false;
    bool reloadTiles = false;
    bool reachMidPoint = false;
    // Start is called before the first frame update
    void Start()
    {
        ProcessSpritesFolder();
        PrepareSprites();
        Debug.Log("Grid position: " + transform.position);
        GenerateGrid();
    }
    private void ProcessSpritesFolder()
    {
        var sprites = Resources.LoadAll("Sprites", typeof(Sprite));
        List<string> imagesNameList = new List<string>();
        List<string> animalTypeList = new List<string>();
        foreach (var t in sprites)
        {
            imagesNameList.Add(t.name);
        }
        bool insertType = false;
        foreach (var str in imagesNameList)
        {
            insertType = true;
            string[] image_name= str.Split('_');
            foreach (var type in animalTypeList)
            {
                if (image_name[0].Equals(type))
                {
                    insertType = false;
                    break;
                }
            }
            if (insertType)
                animalTypeList.Add(image_name[0]);
        }
        for (int i=0; i< animalTypeList.Count; i++)
        {
            animalNames.Add(string.Concat(animalTypeList[i], "_"));
        }
        foreach (var type in animalNames)
            Debug.Log(type);
    }
    private void PrepareSprites()
    {

    }
    private void GenerateGrid()
    {
        setupFrames();
        fixedTilesPosition();
        fixedTilesScale();
        setupTilesToSprites();
        setTilesTheirPositionAndScale();
        LoadSpritesToTiles();
        final_position = tiles[2].transform.position;
    }
    private void fixedTilesPosition()
    {
        // Row 1
        tilePositions[0] = new Vector2(-3f, 1.5f);
        tilePositions[1] = new Vector2(0, 1.5f);
        tilePositions[2] = new Vector2(3f, 1.5f);

        // Row 2
        tilePositions[3] = new Vector2(-3.6f, -1.5f);
        tilePositions[4] = new Vector2(-1.8f, -1.5f);
        tilePositions[5] = new Vector2(0, -1.5f);
        tilePositions[6] = new Vector2(1.8f, -1.5f);
        tilePositions[7] = new Vector2(3.6f, -1.5f);
    }
    private void fixedTilesScale()
    {
        // Row 1
        tileScale[0] = new Vector3(2.2F, 2.2F, 0);
        tileScale[1] = new Vector3(2.2F, 2.2F, 0);
        tileScale[2] = new Vector3(2.2F, 2.2F, 0);

        // Row 2
        tileScale[3] = new Vector3(1.1F, 1.1F, 0f);
        tileScale[4] = new Vector3(1.1F, 1.1F, 0f);
        tileScale[5] = new Vector3(1.1F, 1.1F, 0f);
        tileScale[6] = new Vector3(1.1F, 1.1F, 0f);
        tileScale[7] = new Vector3(1.1F, 1.1F, 0f);
    }
    private void setupTilesToSprites()
    {
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("ImageTile"));
        for (int i = 0; i < nAnimalTiles; i++)
        {
            tiles[i] = (GameObject)Instantiate(referenceTile, transform);
            tiles[i].GetComponent<SpriteRenderer>().sortingOrder = 20;
        }
        Destroy(referenceTile);
    }
    private void setTilesTheirPositionAndScale()
    {
        for(int i = 0; i<nAnimalTiles; i++)
        {
            tiles[i].transform.position = tilePositions[i];
            tiles[i].transform.localScale = tileScale[i];
            Debug.Log("localScale : " + tiles[i].transform.localScale);
        }
    }
    private void setupFrames()
    {
        GameObject referenceTileFrame = (GameObject)Instantiate(Resources.Load("frame3"));
        referenceTileFrame.transform.localScale += new Vector3(1.5F, 1.5F, 0);
        // Row 1
        tile_Frames[0] = (GameObject)Instantiate(referenceTileFrame, transform);
        tile_Frames[0].transform.position = new Vector2(-3f, 1.5f);

        tile_Frames[1] = (GameObject)Instantiate(referenceTileFrame, transform);
        tile_Frames[1].transform.position = new Vector2(0, 1.5f);

        tile_Frames[2] = (GameObject)Instantiate(referenceTileFrame, transform);
        tile_Frames[2].transform.position = new Vector2(3f, 1.5f);

        Destroy(referenceTileFrame);
    }
    // Update is called once per frame
    void Update()
    {
        UserInputHandler();
        TileMovement();
        PlannedReloadOfTiles();
    }
    private void LoadSpritesToTiles()
    {
        ChooseAnimals();
        for (int i = 0; i < nAnimalTiles; i++)
        {
            tiles[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(animal_til[i]);
        }
    }
    private void ChooseAnimals()
    {
        currentAnimalType = animalNames[Random.Range(0, 10)];
        animal_til[0] = string.Concat("Sprites/", currentAnimalType, Random.Range(1, 6));
        do
        {
            animal_til[1] = string.Concat("Sprites/", currentAnimalType, Random.Range(1, 6));
        } while (animal_til[0].Equals(animal_til[1]) == true);

        animal_til[2] = "Question";

        string row2_AnimalType;
        for (int i = 3; i < nAnimalTiles; i++)
        {
            do
            {
                row2_AnimalType = animalNames[Random.Range(0, 10)];
            } while (row2_AnimalType.Equals(currentAnimalType));
            animal_til[i] = string.Concat("Sprites/", row2_AnimalType, Random.Range(1, 6));
        }

        int index = Random.Range(3, nAnimalTiles);
        do
        {
            animal_til[index] = string.Concat("Sprites/", currentAnimalType, Random.Range(1, 6));
        } while (animal_til[index].Equals(animal_til[0]) == true ||
                 animal_til[index].Equals(animal_til[1]) == true);

        //for (int i = 0; i < 6; i++)
        //    Debug.Log(animal_til[i]);
        //Debug.Log("animal arr:""animal arr:"+ animal_til);
    }
    private void loadUpperRow()
    {
        //currentAnimalType = animalNames[Random.Range(0, 10)];
        //Debug.Log("curr animal :"+ currentAnimalType);
    }
    GameObject ClickSelect()
    {
        if (selected != null) return selected;

        //Converting Mouse Pos to 2D (vector2) World Pos
        //Debug.Log("mousePos :" + Input.mousePosition + "world Pos :" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up);

        if (hit.collider != null && (hit.transform.gameObject.GetComponent<SpriteRenderer>()) != null)
        {
//            Debug.Log(hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite.name);
            //Fetch the Collider from the GameObject
            m_Collider = hit.collider;
            //           colliderInfo();
            //hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite
            return hit.transform.gameObject;
        }
        else return null;
    }
    private void colliderInfo()
    {
        //Fetch the center of the Collider volume
        m_Center = m_Collider.bounds.center;
        //Fetch the size of the Collider volume
        m_Size = m_Collider.bounds.size;
        //Fetch the minimum and maximum bounds of the Collider volume
        m_Min = m_Collider.bounds.min;
        m_Max = m_Collider.bounds.max;

        //Output to the console the center and size of the Collider volume
        Debug.Log("Collider Center : " + m_Center);
        Debug.Log("Collider Size : " + m_Size);
        Debug.Log("Collider bound Minimum : " + m_Min);
        Debug.Log("Collider bound Maximum : " + m_Max);
    }
    void PlannedReloadOfTiles()
    {
        if (reloadTiles )
        {
            secondsToWait -= Time.deltaTime;  // T.dt is secs since last update
            if (secondsToWait <= 0)
            {
                reloadTiles = false;
                secondsToWait = waitSeconds;
                // do thing here:
                setTilesTheirPositionAndScale();
                LoadSpritesToTiles();
            }
        }
        return;
    }
    bool CheckSelectedTile(GameObject Selected = null)
    {
        bool output = false;
        if (Selected == null) return output;

        string tile_name = Selected.GetComponent<SpriteRenderer>().sprite.name;
        string[] tile_name_list = tile_name.Split('_');
        string[] currentAnimal_name_list = currentAnimalType.Split('_');
//            Debug.Log("tile_name_list : " + tile_name_list[0] + "   currentAnimalType : " + currentAnimal_name_list[0]);
        if (tile_name_list[0].Equals(currentAnimal_name_list[0]))
            output = true;
        return output;
    }
    void UserInputHandler()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            selected = ClickSelect();
            TileSelection();
        }
    }
    void TileSelection()
    {
        if (selected)
        {
            rightSelection = CheckSelectedTile(selected);
            selected_position = selected.transform.position;

            if (rightSelection)
                target_position = final_position;
            else
            {
                mid_position = (selected_position + final_position) / 2f;
                Debug.Log("selected_position: " + selected_position +
                    " final_position : " + final_position +
                    " mid_position : " + mid_position);
                target_position = mid_position;
                reachMidPoint = false;
            }
        }
    }
    void TileMovement()
    {
        if (selected)
        {
            selected.GetComponent<SpriteRenderer>().sortingOrder = 98;

            selected.transform.position = Vector3.LerpUnclamped(selected.transform.position, target_position, Time.deltaTime);
   //         selected.transform.localScale = Vector3.Slerp(selected.transform.localScale, tiles[2].transform.localScale, Time.deltaTime);

            if (Vector3.Distance(selected.transform.position, target_position) < 0.2f)
            {
                if (rightSelection)
                {
                    SpriteRenderer sr = tiles[2].GetComponent<SpriteRenderer>();
                    if (!(sr.sprite.name).Equals("Question")) return;
                    sr.sprite = selected.GetComponent<SpriteRenderer>().sprite;
                    selected.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Transparent");
                    selected.GetComponent<SpriteRenderer>().sortingOrder = 20;
                    selected = null;
                    reloadTiles = true;
                }
                else if (reachMidPoint == true)
                {
                    selected.transform.position = selected_position;
                    selected.GetComponent<SpriteRenderer>().sortingOrder = 20;
                    selected = null;
                }
                else
                {
                    reachMidPoint = true;
                    target_position = selected_position;
                }
            }
            //Debug.Log("Time.deltaTime : " + (Time.deltaTime + 0.05f));
        }
    }
}