using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    }

    void Update()
    {
        
    }

    public void LoadAllGames()
    {
        Saves = new List<GameSave>();

        BinaryFormatter formatter = new BinaryFormatter();

        DirectoryInfo directory_info = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] file_info = directory_info.GetFiles("*.sdt").OrderByDescending(x => x.LastWriteTime).ToArray();

        foreach(FileInfo info in file_info)
        {
            FileStream stream = File.Open(info.FullName, FileMode.Open);
            GameSave new_save = (GameSave)formatter.Deserialize(stream);
            stream.Close();
            Saves.Add(new_save);
        }
    }

    public IEnumerator LoadActiveSave()
    {
        SceneManager.LoadScene("in_game");
        
        while (ContractManager.Instance == null)
            yield return null;
        while (TimeManager.Instance == null)
            yield return null;
        while (JobManager.Instance == null)
            yield return null;

        ActiveSave.PopulateGameInfo();
    }

    public void SaveGame(string savename)
    {
        savename = Path.GetInvalidFileNameChars()
            .Aggregate(savename, (current, c) => current.Replace(c, '-'));

        GameSave new_save = new GameSave(savename, DateTime.Now);
        new_save.SaveGame();

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = File.Create(Application.persistentDataPath + "/" + new_save.Name + ".sdt");
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