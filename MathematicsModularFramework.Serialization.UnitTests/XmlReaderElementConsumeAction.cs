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
using System.Xml;
using NMock2;
using NMock2.Actions;

namespace MathematicsModularFramework.Serialization.UnitTests
{
    /// <summary>
    /// An NMock action which takes a System.Xml.XmlReader from a specified method parameter, and consumes the contents of the element at the current position in the XMLReader.
    /// </summary>
    /// <remarks>This was created specifically for mocking the IXmlDataSerializer interface, to advance the read position of XMLReader classes which call methods on the mock interface.</remarks>
    class XmlReaderElementConsumeAction : IAction
    {
        private Int32 parameterIndex;

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.UnitTests.XmlReaderElementConsumeAction class.
        /// </summary>
        /// <param name="parameterIndex">The 0 based index of the parameter to capture.</param>
        public XmlReaderElementConsumeAction(Int32 parameterIndex)
        {
            this.parameterIndex = parameterIndex;
        }

        public void Invoke(NMock2.Monitoring.Invocation invocation)
        {
            if ((typeof(XmlReader).IsAssignableFrom(invocation.Parameters[parameterIndex].GetType())) == false)
            {
                throw new ArgumentException("Parameter at index " + parameterIndex + " is not assignable to type " + typeof(XmlReader).FullName + ".");
            }
            XmlReader xmlReader = (XmlReader)invocation.Parameters[parameterIndex];
            xmlReader.ReadContentAsString();
        }

        public void DescribeTo(System.IO.TextWriter writer)
        {
            writer.Write("consumed XML element contents from XmlReader at parameter " + parameterIndex);
        }
    }
}
