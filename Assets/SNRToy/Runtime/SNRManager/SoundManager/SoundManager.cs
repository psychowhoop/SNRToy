using SNRKWordDefine;
using SNRLogHelper;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public bool IsMute
    {
        get
        {
            bool mute = PlayerPrefs.GetInt(KWord.IsSoundMute, 1) == 1;

            return mute;
        }
        set
        {
            int storeValue = value ? 1 : 0;
            PlayerPrefs.SetInt(KWord.IsSoundMute, storeValue);
        }
    }


    public void PlayMusic(string name)
    {
        if (!IsMute)
        {
            AudioController.PlayMusic(name);
        }
    }

    public void Play(string name)
    {
        if (!IsMute)
        {
            AudioController.Play(name);
        }
    }

    public void ButtonClick(string name = "DefBtnClick")
    {
        Play(name);
    }


    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
            }

            return _instance;

        }
    }

    private SoundManager()
    {

    }

    void Start()
    {
        SLog.Log("soundmanager start now");

    }

    void Awake()
    {
        SLog.Log("soundmanager awake now");
        if (_instance == null)
        {
            SLog.Warn("soundmanager not init from boot?");
            _instance = this;
        }
        else
        {
            SLog.Warn("destory superfluous soundmanager obj");
            GameObject.Destroy(gameObject);
        }
    }

}
