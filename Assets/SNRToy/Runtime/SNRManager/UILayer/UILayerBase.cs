using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UILayerManager;
using UnityEngine;

public class UILayerBase : MonoBehaviour
{

    public virtual void Show(bool ani = false)
    {
        this.gameObject.SetActive(true);
        Vector3 v3 = ani ? Vector3.zero : Vector3.one;
        this.transform.localScale = v3;
        if (ani)
        {
            transform.DOScale(1, 0.3f).SetEase(Ease.OutBounce).OnComplete(ShowAnimationComplete);
        }
        else
        {
            ShowAnimationComplete();
        }
    }

    void ShowAnimationComplete()
    {
        GameObject coverLayer = LayerManager.Instance.mCoverLayer;
        coverLayer.transform.SetParent(transform);
        coverLayer.transform.SetAsFirstSibling();
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
        LayerManager.Instance.mCoverLayer.transform.SetParent(LayerManager.Instance.transform);
    }



    public virtual void InitLayer()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
