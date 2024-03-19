using System.Collections;
using System.Collections.Generic;
using SNRLogHelper;
using UniFramework.Singleton;
using UnityEngine;


namespace UILayerManager
{
    public class LayerManager : MonoBehaviour, ISingleton
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

        public void Show(string layerName)
        {
            UILayerBase sc = GetLayerSc(layerName);
            if (sc != null)
            {
                sc.Show();
            }
        }

        public void Hide(string layerName)
        {
            UILayerBase sc = GetLayerSc(layerName);
            if (sc != null)
            {
                sc.Hide();
            }
        }


        public static LayerManager Instance
        {
            get
            {
                if (!UniSingleton.Contains<LayerManager>())
                {
                    UniSingleton.CreateSingleton<LayerManager>();
                }

                return UniSingleton.GetSingleton<LayerManager>();

            }

        }

        void Start()
        {
            SLog.Log("start now");

        }

        void Awake()
        {
            SLog.Log("awake now");
        }

        #region ISingleton
        public void OnCreate(System.Object createParam)
        {

        }

        public void OnUpdate()
        {

        }

        public void OnDestroy()
        {

        }
        #endregion


    }
}


