using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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
        [SerializeField] Image fruitImage;
        [SerializeField] TextMeshProUGUI fruitText;

        private int startChildIndex;
        private Transform parentToReturnTo = null;
        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetUpPrefab(FruitData fruitData)
        {
            fruitText.text = fruitData.fruitName;
            fruitType = fruitData.fruitType;
            fruitColor = fruitData.fruitColor;
            fruitSize = fruitData.fruitSize;
            fruitImage.sprite = fruitData.fruitSprite;
        }

        #region DragFuncitons

        public void OnBeginDrag(PointerEventData eventData)
        {
          
            AudioManager.instance?.PlayClicKSound();
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
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out Basket basket))
            {

                bool isCorrect = false;

                switch (GameManager.instance.GetSortingCriteria())
                {
                    case SortingCriteria.Size:
                        isCorrect = fruitSize == basket.basketSize;
                        break;
                    case SortingCriteria.Color:
                        isCorrect = fruitColor == basket.basketColor;
                        break;
                    case SortingCriteria.Type:
                        isCorrect = fruitType == basket.fruitBasketType;
                        break;
                }

                if (isCorrect)
                {
                    AudioManager.instance.PlayCorrectGuessSound();
                    PerformScaleIntoImageAnimation();
                    basket.PerformCorrectAnimation();
                }
                else
                {
                    AudioManager.instance.PlayWrongGuessSound();
                    PerformScaleOutImageAnimation();
                    basket.PerformVibrationAnimation();
                }

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

        #endregion

        #region Dotween Functions

        private void PerformScaleIntoImageAnimation()//Correct
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
                Destroy(gameObject)
            );
        }

        private void PerformScaleOutImageAnimation()//Wrong
        {
            fruitImage.DOFade(0, 0.25f);
            transform.DOScale(1.25f, 0.25f).OnComplete(() => Destroy(gameObject));

        }

        #endregion 
    }
}
