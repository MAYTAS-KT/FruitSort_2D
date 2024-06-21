using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FruitSort.GameData;


namespace FruitSort
{
    public class GameManager : MonoBehaviour
    {

        [Header("Checks")]
        [SerializeField] bool randomizeSortingCriteria;
        [Header("Sorting Type")]
        [SerializeField] SortingCriteria sortingCriteria=SortingCriteria.Color;
       
        [SerializeField] GameTimer gameTimer;
        [SerializeField] GameData gameData;
        [SerializeField] LayoutGroup animalLayoutGroup;
        [SerializeField] Transform habitatGroupLayout;

        public static GameManager instance;
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
            shuffledFruits = new List<FruitData>(gameData.Fruits);
            shuffledBaskets = new List<BasketData>(gameData.Baskets);
            UIManager.loadGame += OnLoadGame; 

        }

        public void OnDestroy()
        {
            UIManager.loadGame -= OnLoadGame;
        }

        void OnLoadGame()
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

        public SortingCriteria GetSortingCriteria()
        {
            return sortingCriteria;
        }

        public void SpawnFruits()
        {
            if (animalLayoutGroup.transform.childCount >0)//USED FOR RESETTING 
            {
                foreach (Transform child in animalLayoutGroup.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var fruitData in shuffledFruits)
            {
                if (sortingCriteria==SortingCriteria.Type && fruitData.fruitType==FruitBasketType.Other)
                {
                    continue;
                }

                // Instantiate the animal prefab and get its DragAndDrop component
                GameObject newFruit = Instantiate(gameData.fruitPrefab, animalLayoutGroup.transform);
                newFruit.GetComponent<DragAndDrop>().SetUpPrefab(fruitData);
            }
            
        }

        public void SpawnBaskets()
        {
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

                // Instantiate the animal prefab and get its Basket component
                GameObject newBasket = Instantiate(gameData.basketPrefab, habitatGroupLayout);
                newBasket.GetComponent<Basket>().SetupBasket(basketData);
            }
        }

        public void AllFruitsSorted()
        {
            //ALL ANIMAL SORTED
            gameTimer.StopTimer();
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

    }

}
