using System;
using System.Configuration;

namespace Datatec.Infrastructure
{
    [ConfigurationCollection(typeof(PeriodoSetting))]
    public class PeriodosCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Periodo";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName,
              StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PeriodoSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PeriodoSetting)(element)).Nombre;
        }

        public PeriodoSetting this[int idx]
        {
            get { return (PeriodoSetting)BaseGet(idx); }
        }
    }
}
