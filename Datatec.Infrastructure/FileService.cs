
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Infrastructure
{
    public class FileService : IFileService
    {
        private readonly ILogService logService;
        private string _filePath;
        private FileMode _fileMode;

        public FileService(ILogService logService)
        {
            this.logService = logService;
        }
        public void ConfigurePath(string path,FileMode fileMode)
        {
            _filePath = path;
            _fileMode = fileMode;
        }

        public string ReadFirstLine()
        {
            try
            {
               return File.ReadLines(_filePath).First();
            }
            catch (Exception ex)
            {
                 logService.Log(LogLevel.Error, ex.ToString());
            }
            return null;
        }

        public void WriteLine(string line)
        {

            try
            {
               if (!File.Exists(_filePath))
                    _fileMode = FileMode.Create;

                using (FileStream stream = new FileStream(_filePath, _fileMode))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(line);
                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                logService.Log(LogLevel.Error, ex.ToString());
            }
        }
    }
}
