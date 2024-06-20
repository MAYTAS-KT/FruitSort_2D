using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static FruitSort.GameData;


namespace FruitSort
{
    public class GameManager : MonoBehaviour
    {

        [Header("Checks")]
        [SerializeField] bool randomizeSortingCriteria;
        [Header("Sorting Type")]
        public SortingCriteria sortingCriteria=SortingCriteria.Color;
       
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

        private List<FruitData> shuffledFruits;
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


            shuffledFruits = new List<FruitData>(gameData.Fruits);
            shuffledBaskets = new List<BasketData>(gameData.Baskets);

        }

        public void LoadGame()
        {
            if (randomizeSortingCriteria)
            {
                sortingCriteria = (SortingCriteria)UnityEngine.Random.Range(0, Enum.GetValues(typeof(SortingCriteria)).Length);
            }

            ShuffleList(shuffledFruits);
            ShuffleList(shuffledBaskets);

            SpawnFruits();
            SpawnBaskets();


            gameTimer.ResetAndStartTimer();
        }

        public void SpawnFruits()
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
                temp.fruitSize = fruitData.fruitSize;
                temp.fruitImage.sprite = fruitData.fruitSprite;
            }
            
        }

        public void SpawnBaskets()
        {
            Basket temp;

            if (habitatGroupLayout.childCount > 0)//USED FOR RESETTING 
            {
                foreach (Transform child in habitatGroupLayout)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var basketData in shuffledBaskets)
            {
                if (sortingCriteria==SortingCriteria.Size && basketData.basketSize==Size.None)
                {
                    continue;
                }

                // Instantiate the animal prefab and get its DragAndDrop component
                GameObject newBasket = Instantiate(gameData.basketPrefab, habitatGroupLayout);
                temp = newBasket.GetComponent<Basket>();

                // Assign properties to the DragAndDrop component
                temp.basketColor = basketData.colorBasketType;
                temp.fruitBasketType= basketData.fruitBasketTypes;
                temp.basketSize = basketData.basketSize;
                temp.BasketImage.sprite = basketData.basketSprite;

                switch (sortingCriteria)
                {
                    case SortingCriteria.Size:
                        temp.BasketName.text =basketData.basketSize.ToString();
                        break;
                    case SortingCriteria.Color:
                        temp.BasketName.text =basketData.colorBasketType.ToString();
                        temp.BasketImage.color = basketData.basketColor;
                        break;
                    case SortingCriteria.Type:
                        temp.BasketName.text =basketData.fruitBasketTypes.ToString();
                        break;
                    default:
                        temp.BasketName.text = basketData.basketName;
                        break;
                }
            }
        }

        public void CheckGuess(DragAndDrop fruit, Basket basket)
        {
            bool isCorrect = false;

            switch (sortingCriteria)
            {
                case SortingCriteria.Size:
                    isCorrect = fruit.fruitSize == basket.basketSize;
                    break;
                case SortingCriteria.Color:
                    isCorrect = fruit.fruitColor == basket.basketColor;
                    break;
                case SortingCriteria.Type:
                    isCorrect = fruit.fruitType==basket.fruitBasketType;
                    break;
            }

            if (isCorrect)
            {
                CorrectGuess();
            }
            else
            {
                WrongGuess();
            }

            if (animalLayoutGroup.transform.childCount == 0)
            {
                AllAnimalSorted();
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
