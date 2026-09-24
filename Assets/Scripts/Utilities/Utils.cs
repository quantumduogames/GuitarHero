using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Utils : MonoBehaviour
{
    public static void ShuffleArray<T>(T[] array) //not needed return array is a referenced type
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            // Scambia gli elementi
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public static string AddPlusToStringNumber(int number)
    {
        return string.Format("+{0}", number);
    }

    public static string AddMultiplyToStringNumber(int number)
    {
        return string.Format("x{0}", number);
    }

    public static void FlashImage(
     Image image,
     Color glowColor,
     float duration = 0.3f,
     int flashes = 2,
     Action onComplete = null)
    {
        if (image == null)
            return;

        image.DOKill();
        image.gameObject.SetActive(true);

        Color originalColor = image.color;

        image.DOColor(glowColor, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(flashes * 2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                image.color = originalColor;
                image.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    public static void PuffDisappear(
        Image image,
        GameObject puffPrefab,
        float duration = 0.2f,
        Action onComplete = null)
    {
        if (image == null)
        {
            onComplete?.Invoke();
            return;
        }

        RectTransform rectTransform = image.rectTransform;
        Color originalColor = image.color;
        Vector3 originalScale = rectTransform.localScale;

        image.DOKill();
        rectTransform.DOKill();

        SpawnPuff(image, puffPrefab);

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            rectTransform.DOScale(originalScale * 1.2f, duration * 0.4f)
                .SetEase(Ease.OutQuad));

        sequence.Append(
            rectTransform.DOScale(Vector3.zero, duration * 0.6f)
                .SetEase(Ease.InBack));

        sequence.Join(
            image.DOFade(0f, duration * 0.6f));

        sequence.OnComplete(() =>
        {
            image.gameObject.SetActive(false);

            // Prepariamo l'immagine per un eventuale riutilizzo.
            rectTransform.localScale = originalScale;
            image.color = originalColor;

            onComplete?.Invoke();
        });
    }

    public static void PuffAppear(
        Image image,
        GameObject puffPrefab,
        float duration = 0.25f,
        Action onComplete = null)
    {
        if (image == null)
        {
            onComplete?.Invoke();
            return;
        }

        RectTransform rectTransform = image.rectTransform;
        Vector2 finalPosition = rectTransform.anchoredPosition;

        image.DOKill();
        rectTransform.DOKill();

        Color color = image.color;
        color.a = 0f;
        image.color = color;

        rectTransform.localScale = Vector3.zero;
        image.transform.parent.gameObject.SetActive(true);
        image.gameObject.SetActive(true);

        SpawnPuff(image, puffPrefab);

        float downwardOffset = 25f;

        Sequence sequence = DOTween.Sequence();

        // Scale continuo da 0 a 1
        sequence.Insert(
            0f,
            rectTransform
                .DOScale(new Vector3Int(1, 1, 1), duration)
                .SetEase(Ease.OutCubic));

        // Fade rapido
        sequence.Insert(
            0f,
            image.DOFade(1f, duration * 0.5f)
                .SetEase(Ease.OutQuad));

        // Prima scende leggermente
        sequence.Insert(
            0f,
            rectTransform
                .DOAnchorPosY(
                    finalPosition.y - downwardOffset,
                    duration * 0.55f)
                .SetEase(Ease.OutSine));

        // Poi torna e si assesta al centro
        sequence.Insert(
            duration * 0.55f,
            rectTransform
                .DOAnchorPosY(
                    finalPosition.y,
                    duration * 0.45f)
                .SetEase(Ease.OutCubic));

        sequence.OnComplete(() =>
        {
            rectTransform.localScale = new Vector3Int(1, 1, 1);
            rectTransform.anchoredPosition = finalPosition;

            Color finalColor = image.color;
            finalColor.a = 1f;
            image.color = finalColor;

            onComplete?.Invoke();
        });
    }

    public static void ShowWonReward(
      List<Image> imagesToGlow,
      Image sourceImage,
      Image rewardImage,
      GameObject puffPrefab,
      Color illuminatedColor,
      int flashCount = 3,
      Action onComplete = null)
    {
        if (sourceImage == null || rewardImage == null)
        {
            onComplete?.Invoke();
            return;
        }

        Sprite wonSprite = sourceImage.sprite;

        Action continueRewardAnimation = () =>
        {
            PuffDisappear(
                sourceImage,
                puffPrefab,
                0.2f,
                () =>
                {
                    rewardImage.sprite = wonSprite;
                    rewardImage.preserveAspect = true;

                    PuffAppear(
                        rewardImage,
                        puffPrefab,
                        0.4f,
                        onComplete);
                });
        };

        // Il flash è facoltativo
        if (imagesToGlow != null && imagesToGlow.Count > 0 && flashCount > 0)
        {
            foreach (var image in imagesToGlow)
            {
                FlashImage(
               image,
               illuminatedColor,
               0.3f,
               flashCount,
               continueRewardAnimation);
            }
        }
        else
        {
            continueRewardAnimation.Invoke();
        }
    }

    private static void SpawnPuff(Image target, GameObject puffPrefab)
    {
        if (target == null || puffPrefab == null)
            return;

        Canvas rootCanvas = target.GetComponentInParent<Canvas>()?.rootCanvas;

        Transform parent = rootCanvas != null
            ? rootCanvas.transform
            : target.transform.parent;

        GameObject puff = UnityEngine.Object.Instantiate(
            puffPrefab,
            target.transform.position,
            Quaternion.identity,
            parent);

        puff.transform.SetAsLastSibling();

        ParticleSystem particleSystem = puff.GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
        {
            particleSystem.Play();

            float lifetime =
                particleSystem.main.duration +
                particleSystem.main.startLifetime.constantMax;

            UnityEngine.Object.Destroy(puff, lifetime + 0.2f);
        }
        else
        {
            // Se il prefab ha un Animator, imposta qui una durata adeguata.
            UnityEngine.Object.Destroy(puff, 2f);
        }
    }
}
