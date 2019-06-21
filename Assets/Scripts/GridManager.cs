
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    const int nTT = 3; // HARD CODED VALUE
    const int nBT = 5;//  HARD CODED VALUE
    // Tiles
    public GameObject[] TT;                     //Top Tiles
    public GameObject[] BT;                     //Bottom Tiles
    // Tile positions
    Vector2[] TTPositions = new Vector2[nTT];
    Vector2[] BTPositions = new Vector2[nBT];
    // Sprite names and thier counts
    List<string> animalNames = new List<string>();
    List<int> animalCount = new List<int>();
    // Current animal and the selected animal
    protected int currentAnimalType;
    protected int selectedAnimal;
    //
    protected string[] TTAnimalName = new string[nTT];
    protected string[] BTAnimalName = new string[nBT];
    // Start is called before the first frame update
    void Start()
    {
        if (TT.Length != nTT || BT.Length != nBT)
            Debug.LogError("Mismatching tiles!");

        getInitialTilePositions();
        ProcessSpritesFolder();
        LoadChoosenAnimalsToTiles();
    }
    // Update is called once per frame
    void Update()
    {
  
    }
    //Store all the tiles initial poistions 
    void getInitialTilePositions()
    {
        for (int i = 0; i < nTT; i++)
            TTPositions[i] = TT[i].transform.position;

        for (int i = 0; i < nBT; i++)
            BTPositions[i] = BT[i].transform.position;
    }
    //Fetch all the animal types and thier counts from the sprites folder
    private void ProcessSpritesFolder()
    {
        var sprites = Resources.LoadAll("Sprites", typeof(Sprite));
        List<string> imagesNameList = new List<string>();
        List<string> animalTypeList = new List<string>();
        // fetch sprite name
        foreach (var t in sprites)
        {
            imagesNameList.Add(t.name);
        }
        bool insertType = false;
        //Extract the animal name from sprite name
        foreach (var str in imagesNameList)
        {
            insertType = true;
            //Split the string into multiple strings by '_'
            string[] image_name = str.Split('_');
            foreach (var type in animalTypeList)
            {
                if (image_name[0].Equals(type))
                {
                    insertType = false;
                    break;
                }
            }
            if (insertType)
            {
                animalTypeList.Add(image_name[0]);
            }
        }
        //Count number of sprites for each animal type
        for (int i = 0; i < animalTypeList.Count; i++)
        {
            int count = 0;
            foreach (var str in imagesNameList)
            {
                //Split the string into multiple strings by '_'
                string[] image_name = str.Split('_');
                if (animalTypeList[i].Equals(image_name[0]))
                    count++;
            }
            animalCount.Add(count);
        }
        //Add '_' to each animal name, it helps makeup the name later
        for (int i = 0; i < animalTypeList.Count; i++)
        {
            animalNames.Add(string.Concat(animalTypeList[i], "_"));
        }
        //print the animals and their count
        for (int i = 0; i < animalTypeList.Count; i++)
            Debug.Log(animalNames[i] + " " + animalCount[i]);
    }
    //Choose animals for top row tiles
    private void ChooseAnimalsForTopRowTiles()
    {
        // number of animals
        int nAnimal = animalNames.Count;
        // Choose the current animal
        do
        {
            currentAnimalType = Random.Range(0, nAnimal);
        } while (animalCount[currentAnimalType] < 3);
        // number of animals of that kind
        int nCount = animalCount[currentAnimalType];
        // Fill the first tile with the current animal
        TTAnimalName[0] = string.Concat("Sprites/", animalNames[currentAnimalType], Random.Range(1, nCount));
        // Fill the second tile with the current animal and avoid repetition
        do
        {
            TTAnimalName[1] = string.Concat("Sprites/", animalNames[currentAnimalType], Random.Range(1, nCount));
        } while (TTAnimalName[0].Equals(TTAnimalName[1]) == true);

        TTAnimalName[2] = "Question";
        //print the animals and their count
            Debug.Log(TTAnimalName[0] + ", " + TTAnimalName[1] + ", " + TTAnimalName[2]);
    }
    //Choose animals for bottom row tiles
    private void ChooseAnimalsForBottomRowTiles()
    {
        // number of animals
        int nAnimal = animalNames.Count;
        //Bottom tile animal types
        int BT_AnimalType = currentAnimalType;
        // Fill the bottom tiles with the radom animals
        for (int i = 0; i < nBT; i++)
        {
            do
            {
                BT_AnimalType = Random.Range(0, nAnimal);
            } while (BT_AnimalType ==currentAnimalType);
            // number of animals of that kind
            int count1 = animalCount[BT_AnimalType];
            BTAnimalName[i] = string.Concat("Sprites/", animalNames[BT_AnimalType], Random.Range(1, count1));
        }

        int index = Random.Range(0, nBT);
        int count2 = animalCount[currentAnimalType];
        // Fill one of the bottom tiles with the top animals
        do
        {
            BTAnimalName[index] = string.Concat("Sprites/", animalNames[currentAnimalType], Random.Range(1, count2));
        } while (BTAnimalName[index].Equals(TTAnimalName[0]) == true ||
                 BTAnimalName[index].Equals(TTAnimalName[1]) == true);

        for (int i = 0; i < nBT; i++)
            Debug.Log(BTAnimalName[i]);
        //Debug.Log("animal arr:""animal arr:"+ animal_til);
    }
    //Choose animals for bottom row tiles
    private void LoadChoosenAnimalsToTiles()
    {
        ChooseAnimalsForTopRowTiles();
        ChooseAnimalsForBottomRowTiles();
        for (int i = 0; i < nTT; i++)
            TT[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(TTAnimalName[i]);
        for (int i = 0; i < nBT; i++)
            BT[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(BTAnimalName[i]);
    }
}
