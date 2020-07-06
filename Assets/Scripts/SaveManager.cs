using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager {

	private static string path = Application.persistentDataPath + "/SpikeJumpData.enigma";

	//Function saving game data from PlayerSaveData
	public static void SaveData (PlayerSaveData playerSaveData)
	{
		FileStream fs = new FileStream(path, FileMode.Create);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(fs, playerSaveData);
		fs.Close();
	}
	//Function loading game data
	public static PlayerSaveData LoadData ()
	{
		if (File.Exists(path))
		{
			FileStream fs = new FileStream(path, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			PlayerSaveData loadedData = bf.Deserialize(fs) as PlayerSaveData;
			fs.Close();
			return loadedData;
		}
		else
		{
			GameObject go = GameObject.Find("GameManager");
			return go.GetComponent<GameManagerScript>().FirstSave(); ;
		}
	}
}
