using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnOpen : MonoBehaviour
{
    public LeanTweenType EasingType = LeanTweenType.easeOutCubic;
    public float DelaySeconds = 1f;
    public float Duration = 0.5f;

    private Vector3 originalScale;

    public void Start()
    {
        //originalScale = this.transform.localScale; 
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.transform.localScale = new Vector3(0, 0, 0);
            Scale();
        }
        */
    }

    IEnumerator waitABit()
    {
        this.transform.localScale = new Vector3(originalScale.x / 2, originalScale.y / 2, originalScale.z / 2);
        yield return new WaitForSeconds(DelaySeconds);
        Scale();

    }

    private void OnEnable()
    {
        originalScale = this.transform.localScale;
        StartCoroutine(waitABit());
    }

    void Scale()
    {
        LeanTween.scale(this.gameObject, originalScale, Duration).setOnComplete(DoExtraStuff).setEase(EasingType);
    }

    void DoExtraStuff()
    {

    }
}
