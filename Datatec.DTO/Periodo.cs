using System;
using System.Configuration;

namespace Datatec.DTO
{
    public class Periodo 
    {
        public string Nombre { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public TimeSpan IntervaloRevision { get; set; }
        public TimeSpan SilencioPermitido { get; set; }



    }
}
