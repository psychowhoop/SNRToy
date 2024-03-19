using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UILayerManager;
using UnityEngine;

public class UILayerBase : MonoBehaviour
{

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
        this.transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.3f).SetEase(Ease.OutBounce).OnComplete(ShowAnimationComplete);
    }

    void ShowAnimationComplete()
    {
        GameObject coverLayer = LayerManager.Instance.mCoverLayer;
        coverLayer.transform.parent = this.transform;
        coverLayer.transform.SetAsFirstSibling();
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
        LayerManager.Instance.mCoverLayer.transform.parent = LayerManager.Instance.transform;
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