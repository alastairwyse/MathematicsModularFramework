/*
 * Copyright 2016 Alastair Wyse (http://www.oraclepermissiongenerator.net/mathematicsmodularframework/)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MathematicsModularFramework.Serialization
{
    /// <summary>
    /// Serializes and deserializes objects to and from an XML document.
    /// </summary>
    public class XmlDataSerializer : IXmlDataSerializer
    {
        /// <summary>The System.Globalization.CultureInfo to use when serializing and deserializing.</summary>
        protected CultureInfo defaultCulture = CultureInfo.InvariantCulture;
        /// <summary>String used to indicate blank data values when serializing/deserializing (e.g. for blank strings).</summary>
        protected String emptyIndicatorElementName = "Empty";
        /// <summary>The number of digits to write to the right of the decimal point when serializing System.Double objects.</summary>
        protected Int32 doubleFloatingPointDigits = 16;
        /// <summary>Stores mappings between data types and methods which serialize and deserialize those data types.</summary>
        protected Dictionary<Type, Tuple<Action<Object, System.Xml.XmlWriter>, Func<System.Xml.XmlReader, Object>>> operationMap;

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.XmlDataSerializer class.
        /// </summary>
        public XmlDataSerializer()
        {
            operationMap = new Dictionary<Type, Tuple<Action<Object, XmlWriter>, Func<XmlReader, Object>>>();
            AddBasicDataTypeSerializationMethodsToMap();
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IDataSerializer.DataTypeIsSupported(System.Type)"]/*'/>
        public bool DataTypeIsSupported(Type dataType)
        {
            return operationMap.ContainsKey(dataType);
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IXmlDataSerializer.SerializeDataValue(System.Type,System.Object,System.Xml.XmlWriter)"]/*'/>
        public void SerializeDataValue(Type dataType, Object dataValue, XmlWriter xmlWriter)
        {
            if (DataTypeIsSupported(dataType) == false)
            {
                throw new CannotSerializeDataTypeException("Cannot serialize data of type '" + dataType.FullName + "'.", dataType);
            }
            Action<Object, XmlWriter> serializationMethod = operationMap[dataType].Item1;
            serializationMethod.Invoke(dataValue, xmlWriter);
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IXmlDataSerializer.DeserializeDataValue(System.Type,System.Xml.XmlReader)"]/*'/>
        public Object DeserializeDataValue(Type dataType, XmlReader xmlReader)
        {
            if (DataTypeIsSupported(dataType) == false)
            {
                throw new CannotSerializeDataTypeException("Cannot deserialize data of type '" + dataType.FullName + "'.", dataType);
            }
            Func<XmlReader, Object> deserializationMethod = operationMap[dataType].Item2;
            Object returnValue = deserializationMethod.Invoke(xmlReader);
            return returnValue;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IDataSerializer.SerializeDataValue(System.Type,System.Object,System.IO.StreamWriter)"]/*'/>
        public void SerializeDataValue(Type dataType, Object dataValue, StreamWriter streamWriter)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter))
            {
                SerializeDataValue(dataType, dataValue, xmlWriter);
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IDataSerializer.DeserializeDataValue(System.Type,System.IO.StreamReader"]/*'/>
        public Object DeserializeDataValue(Type dataType, StreamReader streamReader)
        {
            using (XmlReader xmlReader = XmlReader.Create(streamReader))
            {
                 return DeserializeDataValue(dataType, xmlReader);
            }
        }

        /// <summary>
        /// Adds support for serializing and deserializing the specified data type, using the specified methods.
        /// </summary>
        /// <param name="dataType">The data type to add serialization/deserialization support for.</param>
        /// <param name="serializationOperation">The method which serializes objects of the specified type.</param>
        /// <param name="deserializationOperation">The method which deserializes objects of the specified type.</param>
        public void AddDataTypeSupport(Type dataType, Action<Object, System.Xml.XmlWriter> serializationOperation, Func<System.Xml.XmlReader, Object> deserializationOperation)
        {
            CheckAddUpdateParameters(dataType, serializationOperation, deserializationOperation);
            if (operationMap.ContainsKey(dataType) == true)
            {
                throw new ArgumentException("Serialization support already exists for data type " + dataType.FullName + ".", "dataType");
            }
            operationMap.Add(dataType, new Tuple<Action<object, System.Xml.XmlWriter>, Func<System.Xml.XmlReader, object>>(serializationOperation, deserializationOperation));
        }

        /// <summary>
        /// Updates the serializing and deserializing methods for the specified data type.
        /// </summary>
        /// <param name="dataType">The data type to update serialization/deserialization support for.</param>
        /// <param name="serializationOperation">The method which serializes objects of the specified type.</param>
        /// <param name="deserializationOperation">The method which deserializes objects of the specified type.</param>
        public void UpdateDataTypeSupport(Type dataType, Action<Object, System.Xml.XmlWriter> serializationOperation, Func<System.Xml.XmlReader, Object> deserializationOperation)
        {
            CheckAddUpdateParameters(dataType, serializationOperation, deserializationOperation);
            if (operationMap.ContainsKey(dataType) == false)
            {
                throw new ArgumentException("Serialization support does not exist for data type " + dataType.FullName + ".", "dataType");
            }
            operationMap.Remove(dataType);
            operationMap.Add(dataType, new Tuple<Action<object, System.Xml.XmlWriter>, Func<System.Xml.XmlReader, object>>(serializationOperation, deserializationOperation));
        }

        #region Private Methods

        /// <summary>
        /// Throws an exception if any of the parameters are null.
        /// </summary>
        /// <param name="dataType">The data type in the mapping.</param>
        /// <param name="serializationOperation">The method which serializes objects of the specified type.</param>
        /// <param name="deserializationOperation">The method which deserializes objects of the specified type.</param>
        private void CheckAddUpdateParameters(Type dataType, Action<Object, System.Xml.XmlWriter> serializationOperation, Func<System.Xml.XmlReader, Object> deserializationOperation)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException("dataType", "Parameter 'dataType' cannot be null.");
            }
            if (serializationOperation == null)
            {
                throw new ArgumentNullException("serializationOperation", "Parameter 'serializationOperation' cannot be null.");
            }
            if (deserializationOperation == null)
            {
                throw new ArgumentNullException("deserializationOperation", "Parameter 'deserializationOperation' cannot be null.");
            }
        }

        /// <summary>
        /// Adds serialization and deserialization methods for basic data types to the operation map.
        /// </summary>
        private void AddBasicDataTypeSerializationMethodsToMap()
        {
            // Add methods for System.String
            AddDataTypeSupport
            (
                typeof(System.String),
                // Serialization
                (Object inputObject, XmlWriter xmlWriter) =>
                {
                    if ((String)inputObject == "")
                    {
                        xmlWriter.WriteElementString(emptyIndicatorElementName, "");
                    }
                    else
                    {
                        xmlWriter.WriteString(inputObject.ToString());
                    }
                },
                // Deserialization
                (XmlReader xmlReader) =>
                {
                    string returnValue;

                    if (xmlReader.IsEmptyElement == true)
                    {
                        // This indicates the value is an empty qstring, so consume the self closing tag (e.g. <Empty />)
                        xmlReader.ReadElementString(emptyIndicatorElementName);
                        returnValue = "";
                    }
                    else
                    {
                        returnValue = xmlReader.ReadContentAsString();
                    }

                    return returnValue;
                }
            );

            // Add methods for System.Int32
            AddDataTypeSupport
            (
                typeof(System.Int32),
                // Serialization
                (Object inputObject, XmlWriter xmlWriter) =>
                {
                    Int32 inputInt32 = (Int32)inputObject;
                    xmlWriter.WriteString(inputInt32.ToString(defaultCulture));
                },
                // Deserialization
                (XmlReader xmlReader) =>
                {
                    return Convert.ToInt32(xmlReader.ReadString(), defaultCulture);
                }
            );

            // Add methods for System.Double
            AddDataTypeSupport
            (
                typeof(System.Double),
                // Serialization
                (Object inputObject, XmlWriter xmlWriter) =>
                {
                    Double inputDouble = (Double)inputObject;
                    xmlWriter.WriteString(inputDouble.ToString("e" + doubleFloatingPointDigits, defaultCulture));
                },
                // Deserialization
                (XmlReader xmlReader) =>
                {
                    return Convert.ToDouble(xmlReader.ReadString(), defaultCulture);
                }
            );

            // Add methods for System.Boolean
            AddDataTypeSupport
            (
                typeof(System.Boolean),
                // Serialization
                (Object inputObject, XmlWriter xmlWriter) =>
                {
                    Boolean inputBoolean = (Boolean)inputObject;
                    xmlWriter.WriteString(inputBoolean.ToString());
                },
                // Deserialization
                (XmlReader xmlReader) =>
                {
                    return Convert.ToBoolean(xmlReader.ReadString());
                }
            );

            // TODO: Add methods for other basic data types.
        }

        #endregion
    }
}
