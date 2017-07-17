using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yLibrary.Voronoi.Formatter
{
    public class AlyshevReader
    {
        StreamReader reader;
        
        /// <summary>
        /// Indicates, if the diagram was read.
        /// </summary>
        public bool DiagramRead => diagramRead;
        bool diagramRead = false;

        /// <summary>
        /// Contains the last message about the successibility of reading the diagram.
        /// </summary>
        public string LastReadMessage => message;
        string message = Messages.READ_NOT_CALLED, path;

        public AlyshevReader(string path)
        {
            this.path = path;
            
        }

        public Site[] Read()
        {
            reader = new StreamReader(path);

            if (!File.Exists(path))
            {
                message = Messages.NO_FILE;
                return null;
            }

            string[] file = reader.ReadToEnd().Split(new string[] { " ", "\n", "\t" }, 
                                                     StringSplitOptions.RemoveEmptyEntries);
            if (file.Length % 2 == 1)
            {
                message = Messages.INVALID_ENTRY_COUNT;
                return null;
            }

            List<Site> sites = new List<Site>();
            for(int i = 0; i < file.Length; i += 2)
            {
                double a, b;
                if (!double.TryParse(file[i], out a))
                {
                    message = string.Format(Messages.INVALID_ENTRY, i);
                    return null;
                }
                if (!double.TryParse(file[i+1], out b))
                {
                    message = string.Format(Messages.INVALID_ENTRY, i + 1);
                    return null;
                }
                sites.Add(new Site(a, b));
            }

            reader.Close();
            reader.Dispose();
            return sites.ToArray();
        }

        #region Message constants.
        private static class Messages
        {
            internal const string NO_FILE = "File in the given path does not exist.",
                READ_NOT_CALLED = "Read() method has not been called yet.",
                INVALID_ENTRY_COUNT = "Input file contains odd number of entries.",
                INVALID_ENTRY = "Invalid number entry in input file at {0} entry.";
        }
        #endregion
    }
}
