using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    public GameSave ActiveSave;

    public List<GameSave> Saves = new List<GameSave>();

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log(string.Format("Save Path: {0}", Application.persistentDataPath));
        LoadAllGames();
    }

    void Update()
    {
        
    }

    public void LoadAllGames()
    {
        Saves = new List<GameSave>();

        var formatter = new BinaryFormatter();

        var directory_info = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] file_info = directory_info.GetFiles("*.sdt").OrderByDescending(x => x.LastWriteTime).ToArray();

        foreach(FileInfo info in file_info)
        {
            FileStream stream = File.Open(info.FullName, FileMode.Open);
            try
            {
                var new_save = (GameSave)formatter.Deserialize(stream);
                Saves.Add(new_save);
            }
            catch(SerializationException ex)
            {
                Debug.Log(string.Format("Failed to load game save {0}: {1}", info.Name, ex.Message));
            }
            finally
            {
                stream.Close();
            }
        }
    }

    public IEnumerator LoadActiveSave()
    {
        SDTUIController.Instance.MainMenuCanvas.gameObject.SetActive(false);
        SDTUIController.Instance.InGameCanvas.gameObject.SetActive(true);
        TimeManager.Unlock();
        TimeManager.Unpause();
        
        while (ContractManager.Instance == null)
            yield return null;
        while (TimeManager.Instance == null)
            yield return null;
        while (JobManager.Instance == null)
            yield return null;

        ActiveSave.PopulateGameInfo();
    }

    public void SaveGame()
    {
        string save_name = string.Format("{0}-{1}",
            Character.MyCharacter.Name.Replace(" ", string.Empty),
            TimeManager.CurrentDate.ToString("ddMMyyyy"));

        var new_save = new GameSave(save_name, DateTime.Now);
        new_save.SaveGame();

        var formatter = new BinaryFormatter();

        string file_path = string.Format("{0}/{1}.sdt",
            Application.persistentDataPath,
            save_name);
        FileStream stream = File.Create(file_path);
        formatter.Serialize(stream, new_save);
        stream.Close();
    }

    public void DeleteGame(GameSave save)
    {
        if (!Saves.Contains(save)) return;

        if(File.Exists(Application.persistentDataPath + "/" + save.Name + ".sdt"))
            File.Delete(Application.persistentDataPath + "/" + save.Name + ".sdt");

        Saves.Remove(save);
    }
}
