using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemSelectedAnimation : MonoBehaviour
{
    public LeanTweenType EasingType = LeanTweenType.easeOutCubic;
    public float Duration = 0.2f;
    public float SelectedScaleValue = 1.05f;

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

    public void Select()
    {
        LeanTween.scale(this.gameObject, new Vector3(SelectedScaleValue, SelectedScaleValue, SelectedScaleValue), Duration).setOnComplete(DoExtraStuff).setEase(EasingType);
    }

    public void Unselect()
    {
        LeanTween.scale(this.gameObject, new Vector3(1, 1, 1), Duration).setOnComplete(DoExtraStuff).setEase(EasingType);
    }


    void DoExtraStuff()
    {

    }
}
