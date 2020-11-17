using System;
using System.IO;

namespace WebApplication1
{
    public class CvManager
    {
        public string Folder { get; }
        
        private const string FILE_TYPE = "pdf";

        public CvManager(string folder)
        {
            if (folder.EndsWith("/") && folder.EndsWith("\\"))
                folder.Substring(0, folder.Length - 1);

            Folder = folder;
        }

        public Guid GenerateNewId()
        {
            Guid id;
            do id = Guid.NewGuid();
            while (File.Exists(GetFilePath(id)));
            return id;
        }

        public void DeleteCv(Guid id)
        {
            string path = GetFilePath(id);
            if (!File.Exists(path))
                throw new ApplicationException("The provided id does not exist!");

            File.Delete(path);
        }

        public string GetFilePath(Guid id) => $"{Folder}\\{id}.{FILE_TYPE}";
    }
}
