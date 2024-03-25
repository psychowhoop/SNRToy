using System.Collections.Generic;
using SNRLogHelper;
using UnityEngine;


namespace UILayerManager
{
    /// <summary>
    /// not singleton
    /// </summary>
    public class LayerManager : MonoBehaviour
    {

        public GameObject mCoverLayer;//the default black background
        public List<GameObject> mLayerList;

        public GameObject GetLayerObj(string layerName)
        {
            GameObject sLayer = null;
            foreach (GameObject ly in mLayerList)
            {
                if (ly.name == layerName)
                {
                    sLayer = ly;
                    break;
                }
            }

            return sLayer;
        }


        public UILayerBase GetLayerSc(string layerName)
        {
            GameObject sLayer = GetLayerObj(layerName);
            if (sLayer == null)
            {
                SLog.Err($"{layerName} layer not exist");
                return null;
            }

            return sLayer.GetComponent<UILayerBase>();
        }

        public static void Show(string layerName)
        {
            LayerManager lm = FindObjectOfType<LayerManager>();
            if (lm != null)
            {
                lm.ShowLayer(layerName);
            }
            else
            {
                SLog.Err("not find layerManager");
            }
        }

        public void ShowLayer(string layerName)
        {
            UILayerBase sc = GetLayerSc(layerName);
            if (sc != null)
            {
                sc.Show();
            }
        }

        public static void Hide(string layerName)
        {
            LayerManager lm = FindObjectOfType<LayerManager>();
            if (lm != null)
            {
                lm.HideLayer(layerName);
            }
            else
            {
                SLog.Err("not find layerManager");
            }
        }

        public void HideLayer(string layerName)
        {
            UILayerBase sc = GetLayerSc(layerName);
            if (sc != null)
            {
                sc.Hide();
            }
        }


        void Start()
        {
            SLog.Log("uimanager start now");

        }

        void Awake()
        {
            SLog.Log("uimanger awake now");
        }


    }
}


