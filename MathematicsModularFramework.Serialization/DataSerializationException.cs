﻿/*
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

namespace MathematicsModularFramework.Serialization
{
    /// <summary>
    /// The exception that is thrown when serialization of the data held by a slot fails.
    /// </summary>
    public class DataSerializationException : Exception
    {
        /// <summary>The data that was attempted to be serialized.</summary>
        protected Object dataValue;
        
        /// <summary>
        /// The data that was attempted to be serialized.
        /// </summary>
        public Object DataValue
        {
            get
            {
                return dataValue;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.DataSerializationException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="dataValue">The data that was attempted to be serialized.</param>
        public DataSerializationException(String message, Object dataValue)
            : base(message)
        {
            this.dataValue = dataValue;
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.DataSerializationException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="dataValue">The data that was attempted to be serialized.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public DataSerializationException(String message, Object dataValue, Exception innerException)
            : base(message, innerException)
        {
            this.dataValue = dataValue;
        }
    }
}
