using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static FruitSort.GameData;

namespace FruitSort
{
    public class Basket : MonoBehaviour
    {
        public ColorBasketType basketColor;
        public FruitBasketType fruitBasketType;
        public Size basketSize;
        [SerializeField] Image BasketImage;
        [SerializeField] Image basketFrame;
        [SerializeField] TextMeshProUGUI BasketName;


        public void SetupBasket(BasketData basketData)
        {

            // Assign properties to the DragAndDrop component
            basketColor = basketData.colorBasketType;
            fruitBasketType = basketData.fruitBasketTypes;
            basketSize = basketData.basketSize;
            BasketImage.sprite = basketData.basketSprite;

            switch (GameManager.instance.GetSortingCriteria())
            {
                case SortingCriteria.Size:
                    BasketName.text = basketData.basketSize.ToString();
                    break;
                case SortingCriteria.Color:
                    BasketName.text = basketData.colorBasketType.ToString();
                    BasketImage.color = basketData.basketColor;
                    break;
                case SortingCriteria.Type:
                    BasketName.text = basketData.fruitBasketTypes.ToString();
                    break;
                default:
                    BasketName.text = basketData.basketName;
                    break;
            }
        }

        #region DoTween Animations

        public void PerformVibrationAnimation()
        {
            Color originalColor = basketFrame.color;
            basketFrame.color = Color.red;
            transform.DOShakePosition(0.5f, 20f, 10, 90f).OnComplete(() => basketFrame.color = originalColor);
        }

        public void PerformCorrectAnimation()
        {
            Color originalColor = basketFrame.color;
            basketFrame.color = Color.green;

            float duration = 0.25f;
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;

            DG.Tweening.Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(targetScale, duration / 2).SetEase(Ease.OutQuad));
            sequence.Append(transform.DOScale(originalScale, duration / 2).SetEase(Ease.InQuad));
            sequence.OnComplete(() => basketFrame.color = originalColor);
            sequence.Play();
        }

        #endregion
    }

}

