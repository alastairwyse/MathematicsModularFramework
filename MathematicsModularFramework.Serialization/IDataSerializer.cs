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

namespace MathematicsModularFramework.Serialization
{
    /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="T:MathematicsModularFramework.Serialization.IDataSerializer"]/*'/>
    public interface IDataSerializer
    {
        // TODO: DataTypeIsSupported() method is not actually used anywhere.  Consider whether it is actually necessary or not, and possibly remove.

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IDataSerializer.DataTypeIsSupported(System.Type)"]/*'/>
        bool DataTypeIsSupported(Type dataType);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IDataSerializer.SerializeDataValue(System.Type,System.Object,System.IO.StreamWriter)"]/*'/>
        void SerializeDataValue(Type dataType, Object dataValue, StreamWriter streamWriter);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IDataSerializer.DeserializeDataValue(System.Type,System.IO.StreamReader"]/*'/>
        Object DeserializeDataValue(Type dataType, StreamReader streamReader);
    }
}
