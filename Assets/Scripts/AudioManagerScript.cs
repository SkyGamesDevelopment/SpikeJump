using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerScript : MonoBehaviour
{
	#region Variables
	public AudioMixer mixer;

	public AudioSource BubbleDestroy;
	public AudioSource ButtonClick;
	public AudioSource CollectCoin;
	public AudioSource CollectCoin2;
	public AudioSource Death;
	public AudioSource IAPbuy;
	public AudioSource Jump1;
	public AudioSource Jump2;
	public AudioSource WallHit;
	public AudioSource RecordLane;
	public AudioSource DeathLane;
	#endregion

	//Changing volume function that gets the music/sound bool and mute/unmute bool
	public void ChangeVolume (bool isMusic, bool mute)
	{
		//Checking is that music or sound
		if (isMusic)
		{
			//Checking what to do, mute or unmute
			if (mute)
			{
				mixer.SetFloat("MusicVolume", -80f);
				PlayerSaveData psd = SaveManager.LoadData();
				psd.isMusicMuted = true;
				SaveManager.SaveData(psd);
			}
			else
			{
				mixer.SetFloat("MusicVolume", 0f);
				PlayerSaveData psd = SaveManager.LoadData();
				psd.isMusicMuted = false;
				SaveManager.SaveData(psd);
			}
		}
		else
		{
			//Checking what to do, mute or unmute
			if (mute)
			{
				mixer.SetFloat("SoundVolume", -80f);
				PlayerSaveData psd = SaveManager.LoadData();
				psd.isSoundMuted = true;
				SaveManager.SaveData(psd);
			}
			else
			{
				mixer.SetFloat("SoundVolume", 0f);
				PlayerSaveData psd = SaveManager.LoadData();
				psd.isSoundMuted = false;
				SaveManager.SaveData(psd);
			}
		}
	}
}
