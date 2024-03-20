using System.Collections.Generic;
using SNRLogHelper;
using UnityEngine;


namespace UILayerManager
{
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


        private static LayerManager _instance;

        private LayerManager()
        {

        }

        public static LayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LayerManager>();
                }

                return _instance;

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


