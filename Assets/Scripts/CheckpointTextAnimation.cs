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
        //checkpointText = gameObject.GetComponent<TMP_Text>();
        
    }

    public void AnimateCheckpointText(TMP_Text checkpointTextAkutell)
    {
        StartCoroutine(AnimateTextCoroutine(checkpointTextAkutell));
    }

    //klene animation wenn checkpoint erreicht wird
    private IEnumerator AnimateTextCoroutine(TMP_Text checkpointTextAkutell)
    {
        checkpointText = checkpointTextAkutell;
        originalScale = checkpointText.transform.localScale;
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