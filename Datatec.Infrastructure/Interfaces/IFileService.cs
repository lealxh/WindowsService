using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Infrastructure
{
    public interface IFileService
    {
        void ConfigurePath(string path, FileMode fileMod);
        void WriteLine(string message);
        string ReadFirstLine();

    }
}
