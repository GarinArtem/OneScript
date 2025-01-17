﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Collections.Generic;
using System.Xml.Schema;
using OneScript.Contexts.Enums;
using OneScript.Types;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;

namespace OneScript.StandardLibrary.XMLSchema.Enumerations
{
    public class XSDisallowedSubstitutions : ClrEnumValueWrapper<XmlSchemaDerivationMethod>
    {
        internal XSDisallowedSubstitutions(EnumerationXSDisallowedSubstitutions instance, XmlSchemaDerivationMethod realValue) 
            : base(instance, realValue)
        {
        }

        public static XSDisallowedSubstitutions FromNativeValue(XmlSchemaDerivationMethod native) 
            => EnumerationXSDisallowedSubstitutions.FromNativeValue(native);

        public static XmlSchemaDerivationMethod ToNativeValue(XSDisallowedSubstitutions wrapper) 
            => wrapper.UnderlyingValue;
    }

    [SystemEnum("НедопустимыеПодстановкиXS", "XSDisallowedSubstitutions")]
    public class EnumerationXSDisallowedSubstitutions : EnumerationContext
    {

        private readonly Dictionary<XmlSchemaDerivationMethod, XSDisallowedSubstitutions> _valuesCache;

        private EnumerationXSDisallowedSubstitutions(TypeDescriptor typeRepresentation, TypeDescriptor valuesType)
            : base(typeRepresentation, valuesType)
        {
            _valuesCache = new Dictionary<XmlSchemaDerivationMethod, XSDisallowedSubstitutions>
            {
                { XmlSchemaDerivationMethod.All, new XSDisallowedSubstitutions(this, XmlSchemaDerivationMethod.All) },
                { XmlSchemaDerivationMethod.Restriction, new XSDisallowedSubstitutions(this, XmlSchemaDerivationMethod.Restriction) },
                { XmlSchemaDerivationMethod.Substitution, new XSDisallowedSubstitutions(this, XmlSchemaDerivationMethod.Substitution) },
                { XmlSchemaDerivationMethod.Extension, new XSDisallowedSubstitutions(this, XmlSchemaDerivationMethod.Extension) }
            };
        }

        internal static XSDisallowedSubstitutions FromNativeValue(XmlSchemaDerivationMethod native)
        {
            
            switch (native)
            {
                case XmlSchemaDerivationMethod.All:
                case XmlSchemaDerivationMethod.Restriction:
                case XmlSchemaDerivationMethod.Substitution:
                case XmlSchemaDerivationMethod.Extension:

                    EnumerationXSDisallowedSubstitutions enumeration = GlobalsHelper.GetEnum<EnumerationXSDisallowedSubstitutions>();
                    return enumeration._valuesCache[native];

                default:
                    return null;
            }
        }

        public static EnumerationXSDisallowedSubstitutions CreateInstance(ITypeManager typeManager)
        {

            var type = typeManager.RegisterType(
                "ПеречислениеНедопустимыеПодстановкиXS",
                "EnumerationXSDisallowedSubstitutions",
                typeof(EnumerationXSDisallowedSubstitutions));
            
            var enumValueType = typeManager.RegisterType(
                "НедопустимыеПодстановкиXS",
                "XSDisallowedSubstitutions", 
                typeof(XSDisallowedSubstitutions));

            EnumerationXSDisallowedSubstitutions instance = new EnumerationXSDisallowedSubstitutions(type, enumValueType);

            instance.AddValue("Все", "All", instance._valuesCache[XmlSchemaDerivationMethod.All]);
            instance.AddValue("Ограничение", "Restriction", instance._valuesCache[XmlSchemaDerivationMethod.Restriction]);
            instance.AddValue("Подстановка", "Substitution", instance._valuesCache[XmlSchemaDerivationMethod.Substitution]);
            instance.AddValue("Расширение", "Extension", instance._valuesCache[XmlSchemaDerivationMethod.Extension]);

            return instance;
        }
    }
}
