using UnityEngine;

public class SoundManager : MonoBehaviour
{

    bool _isMute = false;
    public bool IsMute
    {
        get { return _isMute; }
        set
        {
            _isMute = value;
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









}
