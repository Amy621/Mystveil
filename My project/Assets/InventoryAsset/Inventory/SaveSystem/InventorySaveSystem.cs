using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace InventorySystem
{
    
    /// <summary>
    /// Static class, takes in inventory data converts it into binary and places into a save location
    /// </summary>
    
    public static class InventorySaveSystem
    {
        public static void SaveInventory(Dictionary<string, Inventory> inventoryManager, string saveLocation)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/" + saveLocation.ToString();
            FileStream fileStream = new FileStream(path, FileMode.Create);
            InventoryData InventoryData = new InventoryData(inventoryManager);
            formatter.Serialize(fileStream, InventoryData);
            fileStream.Close();
        }
        public static InventoryData LoadItem(string saveLocation)
        {
            string path = Application.persistentDataPath + "/" + saveLocation.ToString();
            if (File.Exists(path))
            {
                FileStream fileStream = new FileStream(path, FileMode.Open);
                if (fileStream.Length == 0)
                {
                    fileStream.Close();
                    return null;
                }
                BinaryFormatter formatter = new BinaryFormatter();
                InventoryData InventoryData = formatter.Deserialize(fileStream) as InventoryData;
                fileStream.Close();
                return InventoryData;
            }
            else
            {
                Debug.LogError("Save File " + path + " does not exist");
                return null;
            }
        }
        public static void Create(string saveLocation)
        {
            string path = Application.persistentDataPath + "/" + saveLocation.ToString();
            if (!File.Exists(path))
            {
                FileStream fileStream = new FileStream(path, FileMode.Create);
                fileStream.Close();
            }
        }

        public static void Reset(string saveLocation)
        {
            string path = Application.persistentDataPath + "/" + saveLocation.ToString();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

