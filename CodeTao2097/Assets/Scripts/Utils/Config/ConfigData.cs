using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Represents a data table file in csv format
    /// </summary>
    public class ConfigData
    {
        /// <summary>
        /// Dictionary that contains every data table, key is the id of the inner dictionary, inner value is the data of the row
        /// </summary>
        protected Dictionary<string, List<Dictionary<string, string>>> datas;

        /// <summary>
        /// name of the config file
        /// </summary>
        public string fileName;
        
        public ConfigData(string fileName)
        {
            this.fileName = fileName;
            datas = new Dictionary<string, List<Dictionary<string, string>>>();
        }

        /// <summary>
        /// Load the text file
        /// </summary>
        /// <returns></returns>
        public TextAsset LoadFile()
        {
            return Resources.Load<TextAsset>($"{fileName}");
        }

        /// <summary>
        /// Read and save data from the text
        /// </summary>
        /// <param name="txt"></param>
        public void Load(string txt)
        {
            string[] dataArr = txt.Split('\n');
            string[] titleArr = dataArr[0].Trim().Split(','); // retrieve the first row being the inner keys
            
            // start reading the content from the third row
            for (int i = 2; i < dataArr.Length; i++)
            {
                string[] contentArr = dataArr[i].Trim().Split(',');
                if (contentArr.Length != titleArr.Length)
                {
                    continue;
                }
                
                string id = contentArr[0];
                if (!datas.ContainsKey(id))
                {
                    datas.Add(id, new List<Dictionary<string, string>>());
                }
                
                Dictionary<string, string> data = new Dictionary<string, string>();
                for (int j = 0; j < titleArr.Length; j++)
                {
                    data.Add(titleArr[j], contentArr[j]);
                }
                datas[id].Add(data);
            }
        }
        
        public Dictionary<string, string> GetDataById(string id)
        {
            if (datas.ContainsKey(id))
            {
                return datas[id].FirstOrDefault();
            }
            else
            {
                Debug.LogError($"Data {fileName} does not contain id {id}");
                return null;
            }
        }

        public List<Dictionary<string, string>> GetDatasById(string id)
        {
            if (datas.ContainsKey(id))
            {
                return datas[id];
            }
            else
            {
                Debug.LogError($"Data {fileName} does not contain id {id}");
                return null;
            }
        }
    }
}