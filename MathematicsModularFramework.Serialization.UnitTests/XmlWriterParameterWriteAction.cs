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
    /// An NMock action which takes a System.Xml.XmlWriter from a specified method parameter, and writes a specified string to it.
    /// </summary>
    class XmlWriterParameterWriteAction : IAction
    {
        // TODO: Should make this class more generic / flexible... i.e. able to write a series of XML elements using XmlWriter methods, rather than just writing strings.
        //   Would make it more flexible for testing more complex serialization

        private Int32 parameterIndex;
        private String stringToWrite;

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.UnitTests.XmlWriterParameterWriteAction class.
        /// </summary>
        /// <param name="parameterIndex">The 0 based index of the parameter to capture.</param>
        /// <param name="stringToWrite">The string to write to the XML writer parameter.</param>
        public XmlWriterParameterWriteAction(Int32 parameterIndex, String stringToWrite)
        {
            this.parameterIndex = parameterIndex;
            this.stringToWrite = stringToWrite;
        }

        public void Invoke(NMock2.Monitoring.Invocation invocation)
        {
            if ((typeof(XmlWriter).IsAssignableFrom(invocation.Parameters[parameterIndex].GetType())) == false)
            {
                throw new ArgumentException("Parameter at index " + parameterIndex + " is not assignable to type " + typeof(XmlWriter).FullName + ".");
            }
            XmlWriter xmlWriter = (XmlWriter)invocation.Parameters[parameterIndex];
            xmlWriter.WriteString(stringToWrite);
        }

        public void DescribeTo(System.IO.TextWriter writer)
        {
            writer.Write("wrote string \"" + stringToWrite + "\" to XmlWriter at parameter " + parameterIndex);
        }
    }
}
