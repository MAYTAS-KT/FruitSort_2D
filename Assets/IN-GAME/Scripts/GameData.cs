using System.Collections.Generic;
using UnityEngine;

namespace FruitSort
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "GameData")]
    public class GameData : ScriptableObject
    {
        [System.Serializable]
        public class BasketData
        {
            public string basketName;
            public ColorBasketType colorBasketType;
            public FruitBasketType fruitBasketTypes;
            public Size basketSize;
            public Sprite basketSprite;
            public Color basketColor;
        }

        [System.Serializable]
        public class FruitData
        {
            public string fruitName;
            public Sprite fruitSprite;
            public ColorBasketType fruitColor;
            public FruitBasketType fruitType;
            public Size fruitSize;
        }

        public GameObject basketPrefab;
        public GameObject fruitPrefab;
        public List<BasketData> Baskets;
        public List<FruitData> Fruits;

    }
}