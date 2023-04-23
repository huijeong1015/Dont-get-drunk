using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeBGM : MonoBehaviour
{
    TMP_Dropdown bgmList;
    AudioSource[] bgm;
    GameObject bird_text;
    GameObject hail_text;
    GameObject rain_text; 

    // Start is called before the first frame update
    void Start()
    {
        bgmList = this.GetComponent<TMP_Dropdown>();
        bgm = GameObject.Find("bg").GetComponents<AudioSource>();
        bird_text = GameObject.Find("bird_text");
        rain_text = GameObject.Find("rain_text");
        hail_text = GameObject.Find("hail_text");
    }

    // Update is called once per frame
    public void DropdownValueChanged(TMP_Dropdown change)
    {
        int bgmIndex = change.value;

        if (bgmIndex == 0) {
            Debug.Log("Playing Bird by nojisuma");

            // Play bird audio source
            bgm[0].Play();
            bgm[1].Stop();
            bgm[2].Stop();
        } else if (bgmIndex == 1) {
            Debug.Log("Playing Hail by nojisuma");

            // Play hail audio source
            bgm[0].Stop();
            bgm[1].Play();
            bgm[2].Stop();
        } else if (bgmIndex == 2) {
            Debug.Log("Playing Rain by nojisuma");

            // Play rain audio source
            bgm[0].Stop();
            bgm[1].Stop();
            bgm[2].Play();
        }
    }
}
