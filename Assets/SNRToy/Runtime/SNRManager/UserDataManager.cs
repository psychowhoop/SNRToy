using SNRKWordDefine;
using SNRLogHelper;
using UnityEngine;

namespace SNRUserDataManager
{
    public class UserDataManager : MonoBehaviour
    {

        public bool IsNewUser
        {
            get
            {
                return PlayerPrefs.GetInt(KWord.NewUser, 1) == 1;
            }

            set
            {
                int storeValue = value ? 1 : 0;
                PlayerPrefs.SetInt(KWord.NewUser, storeValue);
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

        void UpdateBootTimes()
        {
            int curBootTimes = this.BootTimes + 1;
            PlayerPrefs.SetInt(KWord.AppBootTimes, curBootTimes);
        }

        #region Monobehaviour Lifeloop
        // Start is called before the first frame update
        void Start()
        {
            SLog.Log("user data start now");
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Awake()
        {
            UpdateBootTimes();
            if (_instance == null)
            {
                SLog.Warn("usedata manager not init from boot?");
                _instance = this;
            }
            else
            {
                SLog.Warn("destroy superfluous userdata manager");
                Destroy(gameObject);
            }
        }

        #endregion

        #region Singleton

        private static UserDataManager _instance;
        private UserDataManager()
        {

        }

        public static UserDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UserDataManager>();
                }

                return _instance;
            }
        }



        #endregion 


    }
}


