using System;
using System.Configuration;

namespace Datatec.Infrastructure
{
    public class PeriodoSetting : ConfigurationElement
    {
        [ConfigurationProperty("Nombre", IsRequired = true, IsKey = true)]
        public string Nombre
        {
            get {
                string var= (string)base["Nombre"];
                return var;

            }
        }

        [ConfigurationProperty("HoraInicio", IsRequired = true, IsKey = false)]
        public DateTime HoraInicio
        {
            get {

                return (DateTime)base["HoraInicio"];

            }
        }

        [ConfigurationProperty("HoraFin", IsRequired = true, IsKey = false)]
        public DateTime HoraFin
        {
            get { return (DateTime)base["HoraFin"]; }
        }

        [ConfigurationProperty("IntervaloRevision", IsRequired = true, IsKey = false)]
        public TimeSpan IntervaloRevision
        {
            get { return (TimeSpan)base["IntervaloRevision"]; }
        }


        [ConfigurationProperty("SilencioPermitido", IsRequired = true, IsKey = false)]
        public TimeSpan SilencioPermitido
        {
            get { return (TimeSpan)base["SilencioPermitido"]; }
        }




    }
}
