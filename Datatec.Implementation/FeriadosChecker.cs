using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatec.Interfaces;

namespace Datatec.Implementation
{
    public class FeriadosChecker : IFeriadosChecker
    {
        public bool esFeriado(DateTime fecha)
        {
            return false;
        }
    }
}
