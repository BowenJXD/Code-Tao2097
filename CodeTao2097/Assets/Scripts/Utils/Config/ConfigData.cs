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
            for (int i = 1; i < dataArr.Length; i++)
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

        public string Save()
        {
            // inverse of load, save the data in csv form
            // first row being the titles
            string result = "";
            foreach (var pair in datas.First().Value.First())
            {
                result += $"{pair.Key},";
            }
            result = result.TrimEnd(',') + "\n";
            foreach (var data in datas)
            {
                foreach (var row in data.Value)
                {
                    string rowStr = "";
                    foreach (var pair in row)
                    {
                        rowStr += $"{pair.Value},";
                    }
                    result += $"{rowStr.TrimEnd(',')}\n";
                }
            }

            return result;
        }
        
        public Dictionary<string, string> GetDataById(string id)
        {
            if (datas.ContainsKey(id))
            {
                return datas[id].FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        
        public void SetDataById(string id, Dictionary<string, string> data)
        {
            if (datas.ContainsKey(id))
            {
                datas[id].Clear();
                datas[id].Add(data);
            }
            else
            {
                datas.Add(id, new List<Dictionary<string, string>>());
                datas[id].Add(data);
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
                return null;
            }
        }
        
        public void SetDatasById(string id, List<Dictionary<string, string>> datas)
        {
            if (this.datas.ContainsKey(id))
            {
                this.datas[id].Clear();
                this.datas[id].AddRange(datas);
            }
            else
            {
                this.datas.Add(id, datas);
            }
        }
    }
}