using System.Collections;
using System.Collections.Generic;
using SNRKWordDefine;
using UniFramework.Singleton;
using UnityEngine;

namespace SNRUserDataManager
{
    public class UserDataManager : MonoBehaviour, UniFramework.Singleton.ISingleton
    {

        public bool IsNewUser
        {
            get
            {
                return PlayerPrefs.GetInt(KWord.NewUser, 1) == 1;
            }
        }

        /// <summary>
        /// user boot the app times
        /// </summary>
        /// <value></value>
        public int BootTimes
        {
            get
            {
                return PlayerPrefs.GetInt(KWord.AppBootTimes, 0);
            }
        }

        public static UserDataManager Instance
        {
            get
            {
                if (!UniSingleton.Contains<UserDataManager>())
                {
                    UniSingleton.CreateSingleton<UserDataManager>();
                }

                return UniSingleton.GetSingleton<UserDataManager>();
            }
        }




        void UpdateBootTimes()
        {
            int curBootTimes = this.BootTimes + 1;
            PlayerPrefs.SetInt(KWord.AppBootTimes, curBootTimes);
        }





        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region ISingleton
        public void OnCreate(System.Object createParam)
        {
            UpdateBootTimes();

        }

        public void OnUpdate()
        {

        }


        public void OnDestroy()
        {

        }

        #endregion ISingleton


    }
}


