using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointTextAnimation : MonoBehaviour
{
    public float animationDuration = 0.5f;
    public float scaleMultiplier = 1.5f;

    private TMP_Text checkpointText;
    private Vector3 originalScale;

    private void Start()
    {
        checkpointText = gameObject.GetComponent<TMP_Text>();
        originalScale = checkpointText.transform.localScale;
    }

    public void AnimateCheckpointText()
    {
        StartCoroutine(AnimateTextCoroutine());
    }

    private IEnumerator AnimateTextCoroutine()
    {
        float timer = 0f;
        Vector3 targetScale = originalScale * scaleMultiplier;

        while (timer < animationDuration)
        {
            float t = timer / animationDuration;
            checkpointText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        checkpointText.transform.localScale = originalScale;
    }
}