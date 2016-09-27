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

namespace MathematicsModularFramework.Serialization
{
    /// <summary>
    /// The exception that is thrown when deserialization of the data held by a slot fails.
    /// </summary>
    public class DataDeserializationException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.DataDeserializationException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataDeserializationException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.DataDeserializationException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public DataDeserializationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
