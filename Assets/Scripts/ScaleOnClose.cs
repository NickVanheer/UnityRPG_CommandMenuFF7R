using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnClose : MonoBehaviour
{
    public LeanTweenType EasingType = LeanTweenType.easeOutCubic;
    public float DelaySeconds = 0.2f;
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
        this.transform.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(DelaySeconds);
        Scale();

    }

    public void Close()
    {
        StartCoroutine(waitABit());
    }

    void Scale()
    {
        LeanTween.scale(this.gameObject, new Vector3(0, 0, 0), Duration).setOnComplete(DoExtraStuff).setEase(EasingType);
    }

    void DoExtraStuff()
    {
        this.gameObject.SetActive(false);
    }
}
