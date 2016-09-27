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
using System.IO;
using System.Xml;
using NUnit.Framework;
using MathematicsModularFramework.UnitTests.TestModules;

namespace MathematicsModularFramework.Serialization.UnitTests
{
    /// <summary>
    /// Unit tests for class MathematicsModularFramework.Serialization.XmlDataSerializer.
    /// </summary>
    public class XmlDataSerializerTests
    {
        private const String xmlWrappingTag = "SerializedData";

        private MemoryStream memoryStream;
        private XmlWriter xmlWriter;
        private XmlDataSerializer testXmlDataSerializer;

        [SetUp]
        protected void SetUp()
        {
            memoryStream = new MemoryStream();
            xmlWriter = XmlWriter.Create(memoryStream);

            testXmlDataSerializer = new XmlDataSerializer();

            // Write XML document headers and tag to the XML writer, so that test methods can write partial XML which is still valid
            xmlWriter.WriteStartElement(xmlWrappingTag);
        }

        [TearDown]
        protected void TearDown()
        {
            ((IDisposable)xmlWriter).Dispose();
            memoryStream.Dispose();
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddDataTypeSupport_DataTypeParameterNull() method is called with a null 'dataType' parameter.
        /// </summary>
        [Test]
        public void AddDataTypeSupport_DataTypeParameterNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testXmlDataSerializer.AddDataTypeSupport(null, new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), new Func<XmlReader, Object>((xmlReader) => { return 0; }));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'dataType' cannot be null."));
            Assert.AreEqual("dataType", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddDataTypeSupport() method is called with a null 'serializationOperation' parameter.
        /// </summary>
        [Test]
        public void AddDataTypeSupport_SerializationOperationParameterNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testXmlDataSerializer.AddDataTypeSupport(typeof(String), null, new Func<XmlReader, Object>((xmlReader) => { return 0; }));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'serializationOperation' cannot be null."));
            Assert.AreEqual("serializationOperation", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddDataTypeSupport() method is called with a null 'deserializationOperation' parameter.
        /// </summary>
        [Test]
        public void AddDataTypeSupport_DeserializationOperationParameterNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testXmlDataSerializer.AddDataTypeSupport(typeof(String), new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), null);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'deserializationOperation' cannot be null."));
            Assert.AreEqual("deserializationOperation", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddDataTypeSupport() method is called where support for the specified data type already exists.
        /// </summary>
        [Test]
        public void AddDataTypeSupport_SupportForDataTypeAlreadyExists()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testXmlDataSerializer.AddDataTypeSupport(typeof(String), new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), new Func<XmlReader, Object>((xmlReader) => { return 0; }));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Serialization support already exists for data type " + typeof(String).FullName + "."));
            Assert.AreEqual("dataType", e.ParamName);
        }

        /// <summary>
        /// Success tests for the AddDataTypeSupport() method.
        /// </summary>
        [Test]
        public void AddDataTypeSupport()
        {
            Assert.AreEqual(false, testXmlDataSerializer.DataTypeIsSupported(typeof(XmlWriter)));

            testXmlDataSerializer.AddDataTypeSupport(typeof(XmlWriter), new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), new Func<XmlReader, Object>((xmlReader) => { return 0; }));

            Assert.AreEqual(true, testXmlDataSerializer.DataTypeIsSupported(typeof(XmlWriter)));
        }

        /// <summary>
        /// Tests that an exception is thrown if the UpdateDataTypeSupport() method is called with a null 'dataType' parameter.
        /// </summary>
        [Test]
        public void UpdateDataTypeSupport_DataTypeParameterNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testXmlDataSerializer.UpdateDataTypeSupport(null, new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), new Func<XmlReader, Object>((xmlReader) => { return 0; }));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'dataType' cannot be null."));
            Assert.AreEqual("dataType", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the UpdateDataTypeSupport() method is called with a null 'serializationOperation' parameter.
        /// </summary>
        [Test]
        public void UpdateDataTypeSupport_SerializationOperationParameterNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testXmlDataSerializer.UpdateDataTypeSupport(typeof(String), null, new Func<XmlReader, Object>((xmlReader) => { return 0; }));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'serializationOperation' cannot be null."));
            Assert.AreEqual("serializationOperation", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the UpdateDataTypeSupport() method is called with a null 'deserializationOperation' parameter.
        /// </summary>
        [Test]
        public void UpdateDataTypeSupport_DeserializationOperationParameterNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testXmlDataSerializer.UpdateDataTypeSupport(typeof(String), new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), null);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'deserializationOperation' cannot be null."));
            Assert.AreEqual("deserializationOperation", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the UpdateDataTypeSupport() method is called where support for the specified data type does not already exist.
        /// </summary>
        [Test]
        public void UpdateDataTypeSupport_SupportForDataTypeDoesntExist()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testXmlDataSerializer.UpdateDataTypeSupport(typeof(XmlWriter), new Action<Object, XmlWriter>((dataValue, xmlWriter) => { }), new Func<XmlReader, Object>((xmlReader) => { return 0; }));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Serialization support does not exist for data type " + typeof(XmlWriter).FullName + "."));
            Assert.AreEqual("dataType", e.ParamName);
        }

        /// <summary>
        /// Success tests for the UpdateDataTypeSupport() method.
        /// </summary>
        [Test]
        public void UpdateDataTypeSupport()
        {
            const String testString = "Test String";

            Assert.AreEqual(true, testXmlDataSerializer.DataTypeIsSupported(typeof(String)));
            // Update the serializer with a method for strings that returns the above fixed string
            testXmlDataSerializer.UpdateDataTypeSupport(typeof(String), new Action<Object, XmlWriter>((dataValue, xmlWriter2) => { xmlWriter2.WriteString(testString); }), new Func<XmlReader, Object>((xmlReader2) => { return 0; }));

            testXmlDataSerializer.SerializeDataValue(typeof(String), "Other String", xmlWriter);

            Assert.AreEqual(testString, ExtractTestResultsFromXmlStream(memoryStream));
        }

        /// <summary>
        /// Tests that an exception is thrown if the DeserializeDataValue() method is called with a data type which is not supported.
        /// </summary>
        [Test]
        public void DeserializeDataValue_DataTypeNotSupported()
        {
            CannotSerializeDataTypeException e = Assert.Throws<CannotSerializeDataTypeException>(delegate
            {
                using (XmlReader xmlReader = XmlReader.Create(memoryStream))
                {
                    testXmlDataSerializer.DeserializeDataValue(typeof(UnserializableObject), xmlReader);
                }
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Cannot deserialize data of type '" + typeof(UnserializableObject).FullName + "'."));
            Assert.AreEqual(typeof(UnserializableObject), e.DataType);
        }

        /// <summary>
        /// Tests that an exception is thrown if the SerializeDataValue() method is called with a data type which is not supported.
        /// </summary>
        [Test]
        public void SerializeDataValue_DataTypeNotSupported()
        {
            CannotSerializeDataTypeException e = Assert.Throws<CannotSerializeDataTypeException>(delegate
            {
                testXmlDataSerializer.SerializeDataValue(typeof(UnserializableObject), "<Test>Any string</Test>", xmlWriter);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Cannot serialize data of type '" + typeof(UnserializableObject).FullName + "'."));
            Assert.AreEqual(typeof(UnserializableObject), e.DataType);
        }

        /// <summary>
        /// Success tests for the SerializeDataValue() and DeserializeDataValue() methods for System.String data types.
        /// </summary>
        [Test]
        public void Serialize_String()
        {
            const String testString = "<Test></Test> string with <embedded> xml.";

            testXmlDataSerializer.SerializeDataValue(typeof(String), testString, xmlWriter);

            WriteClosingElementAndResetXmlStream(xmlWriter, memoryStream);

            Object resultObject;
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                AdvanceXmlReaderPosition(xmlReader);
                resultObject = testXmlDataSerializer.DeserializeDataValue(typeof(String), xmlReader);
            }

            Assert.AreEqual(typeof(String), resultObject.GetType());
            Assert.AreEqual(testString, (String)resultObject);
        }

        /// <summary>
        /// Success tests for the SerializeDataValue() and DeserializeDataValue() methods for an empty System.String data type.
        /// </summary>
        [Test]
        public void Serialize_EmptyString()
        {
            const String testString = "";

            testXmlDataSerializer.SerializeDataValue(typeof(String), testString, xmlWriter);

            WriteClosingElementAndResetXmlStream(xmlWriter, memoryStream);

            Object resultObject;
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                AdvanceXmlReaderPosition(xmlReader);
                resultObject = testXmlDataSerializer.DeserializeDataValue(typeof(String), xmlReader);
            }

            Assert.AreEqual(typeof(String), resultObject.GetType());
            Assert.AreEqual(testString, (String)resultObject);
        }

        /// <summary>
        /// Success tests for the SerializeDataValue() and DeserializeDataValue() methods for System.Int32 data types.
        /// </summary>
        [Test]
        public void Serialize_Int32()
        {
            const Int32 testInt32 = Int32.MaxValue;

            testXmlDataSerializer.SerializeDataValue(typeof(Int32), testInt32, xmlWriter);

            WriteClosingElementAndResetXmlStream(xmlWriter, memoryStream);

            Object resultObject;
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                AdvanceXmlReaderPosition(xmlReader);
                resultObject = testXmlDataSerializer.DeserializeDataValue(typeof(Int32), xmlReader);
            }

            Assert.AreEqual(typeof(Int32), resultObject.GetType());
            Assert.AreEqual(testInt32, (Int32)resultObject);
        }

        /// <summary>
        /// Success tests for the SerializeDataValue() and DeserializeDataValue() methods for System.Double data types.
        /// </summary>
        [Test]
        public void Serialize_Double()
        {
            const Double testDouble = Double.MaxValue;

            testXmlDataSerializer.SerializeDataValue(typeof(Double), testDouble, xmlWriter);

            WriteClosingElementAndResetXmlStream(xmlWriter, memoryStream);

            Object resultObject;
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                AdvanceXmlReaderPosition(xmlReader);
                resultObject = testXmlDataSerializer.DeserializeDataValue(typeof(Double), xmlReader);
            }

            Assert.AreEqual(typeof(Double), resultObject.GetType());
            Assert.AreEqual(testDouble, (Double)resultObject);
        }

        # region Private Methods

        /// <summary>
        /// Removes the 'wrapping' XML from the inputted memory stream and returns just the data that was written in between the &lt;SerializedData&gt; tags as a string.
        /// </summary>
        /// <param name="xmlReader">The memory stream to extract the data from.</param>
        /// <returns>The data.</returns>
        private String ExtractTestResultsFromXmlStream(MemoryStream memoryStream)
        {
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            memoryStream.Position = 0;
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                xmlReader.ReadStartElement(xmlWrappingTag);
                return xmlReader.ReadString();
            }
        }

        /// <summary>
        /// Writes closing 'wrapping' XML element to the specified XML writer, and resets the position of the specified memory stream to 0.
        /// </summary>
        /// <param name="xmlWriter">The XML writer to write the closing element to.</param>
        /// <param name="memoryStream">The memory stream to reset the position of.</param>
        private void WriteClosingElementAndResetXmlStream(XmlWriter xmlWriter, MemoryStream memoryStream)
        {
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            memoryStream.Position = 0;
        }

        /// <summary>
        /// Advances the position in the specified XML reader, to after the wrapping element tag.
        /// </summary>
        /// <param name="xmlReader">The XML reader to advance the position of.</param>
        private void AdvanceXmlReaderPosition(XmlReader xmlReader)
        {
            xmlReader.ReadStartElement(xmlWrappingTag);
        }

        #endregion
    }
}
