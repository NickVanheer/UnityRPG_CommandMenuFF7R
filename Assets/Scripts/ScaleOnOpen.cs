using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnOpen : MonoBehaviour
{
    public LeanTweenType EasingType = LeanTweenType.easeOutCubic;
    public float DelaySeconds = 1f;
    public float Duration = 0.5f;

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
        this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(DelaySeconds);
        Scale();

    }

    private void OnEnable()
    {
        StartCoroutine(waitABit());
    }

    void Scale()
    {
        LeanTween.scale(this.gameObject, new Vector3(1, 1, 1), Duration).setOnComplete(DoExtraStuff).setEase(EasingType);
    }

    void DoExtraStuff()
    {

    }
}
