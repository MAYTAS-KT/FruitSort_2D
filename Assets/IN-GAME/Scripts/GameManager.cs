using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FruitSort.GameData;


namespace FruitSort
{
    public class GameManager : MonoBehaviour
    {

        [Header("CHECKS")]
        [SerializeField] bool doColorCheck;
        [SerializeField] bool doTypeCheck;
        [SerializeField] bool doSizeCheck;

        [SerializeField] GameTimer gameTimer;
        [SerializeField] GameData gameData;
        [SerializeField] LayoutGroup animalLayoutGroup;
        [SerializeField] Transform habitatGroupLayout;
        [SerializeField] Image visualImageRef;

        [Header("COLOR REF")]
        [SerializeField] Color originalColor;
        [SerializeField] Color correctGuessColor;
        [SerializeField] Color WrongGuessColor;

        public static GameManager instance;

        private AudioManager audioManager;
        private UIManager uiManager;

        private List<AnimalData> shuffledFruits;
        private List<BasketData> shuffledBaskets;

        

        private void Awake()
        {
            // Ensure there is only one instance of the GameManager
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            if (AudioManager.instance != null)
            {
                audioManager = AudioManager.instance;
            }
            originalColor=visualImageRef.color;

            int randomInt = UnityEngine.Random.Range(0, 3);

            shuffledFruits = new List<AnimalData>(gameData.Fruits);
            shuffledBaskets = new List<BasketData>(gameData.Baskets);

        }

        public void LoadGame()
        {
            ShuffleList(shuffledFruits);
            ShuffleList(shuffledBaskets);

            SpawnAnimals();
            SpawnHabitats();


            gameTimer.ResetAndStartTimer();
        }

        public void SpawnAnimals()
        {
            DragAndDrop temp;

            if (animalLayoutGroup.transform.childCount >0)//USED FOR RESETTING 
            {
                foreach (Transform child in animalLayoutGroup.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var fruitData in shuffledFruits)
            {
                // Instantiate the animal prefab and get its DragAndDrop component
                GameObject newFruit = Instantiate(gameData.fruitPrefab, animalLayoutGroup.transform);
                temp = newFruit.GetComponent<DragAndDrop>();

                // Assign properties to the DragAndDrop component
                temp.fruitText.text = fruitData.fruitName;
                temp.fruitType = fruitData.fruitType;
                temp.fruitColor = fruitData.fruitColor;
                temp.fruitImage.sprite = fruitData.fruitSprite;
            }
            
        }

        public void SpawnHabitats()
        {
            Habitat temp;

            if (habitatGroupLayout.childCount > 0)//USED FOR RESETTING 
            {
                foreach (Transform child in habitatGroupLayout)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var basketData in shuffledBaskets)
            {
                // Instantiate the animal prefab and get its DragAndDrop component
                GameObject newBasket = Instantiate(gameData.basketPrefab, habitatGroupLayout);
                temp = newBasket.GetComponent<Habitat>();

                // Assign properties to the DragAndDrop component
                temp.BasketName.text = basketData.basketName;
                temp.basketColor = basketData.colorBasketType;
                temp.fruitBasketType= basketData.fruitBasketTypes;
                temp.BasketImage.sprite = basketData.basketSprite;
            }
        }

        public void CorrectGuess()
        {
            Debug.Log("Correct Guess");
            audioManager.PlayCorrectGuessSound();
            ShowCorrectVisual();

        }

        public void WrongGuess()
        {
            Debug.Log("Wrong Guess");
            audioManager.PlayWrongGuessSound();
            ShowWrongVisual();
        }

        public void AllAnimalSorted()
        {
            Debug.Log("ALL ANIMAL SORTED");
            gameTimer.StopTimer();
            audioManager.PlayWinSound();

        }

        public void AnimalLayoutGroup(bool isEnabled)
        {
            animalLayoutGroup.enabled = isEnabled;
        }

        #region Shuffle Logic

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        #endregion

        #region Visual
        public void ShowCorrectVisual()
        {
            visualImageRef.color=correctGuessColor;
            Invoke(nameof(setoriginalColor), 0.5f);
        }
        public void ShowWrongVisual()
        {
            visualImageRef.color = WrongGuessColor;
            Invoke(nameof(setoriginalColor), 0.5f);
        }
        private void setoriginalColor()
        {
           visualImageRef.color=originalColor;
        }
        #endregion

    }

}
