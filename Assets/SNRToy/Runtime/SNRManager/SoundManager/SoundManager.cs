using System.Collections;
using System.Collections.Generic;
using UniFramework.Singleton;
using UnityEngine;

public class SoundManager : MonoBehaviour, ISingleton
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




    public static SoundManager Instance
    {
        get
        {
            if (!UniSingleton.Contains<SoundManager>())
            {
                UniSingleton.CreateSingleton<SoundManager>();
            }

            return UniSingleton.GetSingleton<SoundManager>();
        }
    }


    public void OnCreate(System.Object createParam)
    {

    }


    public void OnUpdate()
    {

    }


    public void OnDestroy()
    {

    }
}
