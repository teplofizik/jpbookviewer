using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLFile;

namespace JefViewer
{
    [Serializable]
    public class Config
    {
        public bool Changed = false;

        /// <summary>
        /// Идентификатор книги
        /// </summary>
        public string BookID = "";

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber = 0;

        /// <summary>
        /// Режим отображения кандзи
        /// </summary>
        public int ViewMode = 0;

        /// <summary>
        /// Путь к выходным файлам
        /// </summary>
        public string LastPath = "";
        
        public void Save()
        {
            Changed = false;
            this.SaveXML(GetConfigPath());
        }

        private static string GetConfigPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CodhRoisReader\\config.xml";
        }
        
        public static Config Load()
        {
            string ConfFile = GetConfigPath();
            if (File.Exists(ConfFile))
            {
                try
                {
                    var C = XMLFile.XMLFile.LoadXML<Config>(ConfFile);
                    
                    return C;
                }
                catch (Exception E)
                {
                    return new Config();
                }
            }
            else
            {
                return new Config();
            }
        }
    }
}
