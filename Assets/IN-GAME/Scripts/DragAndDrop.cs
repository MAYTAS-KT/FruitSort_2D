using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FruitSort.GameData;

namespace FruitSort
{
    public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("FRUIT Data")]
        public ColorBasketType fruitColor;
        public FruitBasketType fruitType;
        public Size fruitSize;
        public Image fruitImage;
        public TextMeshProUGUI fruitText;

        private int startChildIndex;
        private Transform parentToReturnTo = null;
        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetUpPrefab(FruitData animalData)
        {
            fruitColor = animalData.fruitColor;
            fruitType = animalData.fruitType;
            fruitImage.sprite = animalData.fruitSprite;
            fruitText.text=animalData.fruitName;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayClicKSound();
            }
            GameManager.instance.AnimalLayoutGroup(false);//Stop Arranging Animal Layout group
            fruitText.enabled=false;
            transform.localScale *= 1.25f;
            startChildIndex = transform.GetSiblingIndex();
            parentToReturnTo = transform.parent;
            transform.SetParent(transform.root);
        }

        public void OnDrag(PointerEventData eventData)
        {

            transform.position = eventData.position;
            canvasGroup.alpha = 0.75f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out Basket habitat))
            {

                // Notify the GameManager about the drop and pass the data needed for the check
                GameManager.instance.CheckGuess(this, habitat.GetComponent<Basket>());
                Destroy(gameObject);
            }
            else//Return back to original pos
            {
                if (AudioManager.instance!=null)
                {
                    AudioManager.instance.PlayErrorSound();
                }
                fruitText.enabled = true;
                canvasGroup.blocksRaycasts = true;
                transform.SetParent(parentToReturnTo, false);
                transform.SetSiblingIndex(startChildIndex);
                transform.localScale=Vector3.one;
            }

            GameManager.instance.AnimalLayoutGroup(true);//Arrange Animal Layout group

            if (parentToReturnTo.transform.childCount == 0)
            {
                GameManager.instance.AllFruitsSorted();
            }
        }

    }
}
