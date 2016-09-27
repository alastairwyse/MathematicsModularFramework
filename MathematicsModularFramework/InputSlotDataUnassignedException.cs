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

namespace MathematicsModularFramework
{
    /// <summary>
    /// The exception that is thrown when an attempt is made to process a module with an input slot with an unassigned data value.
    /// </summary>
    public class InputSlotDataUnassignedException : Exception
    {
        /// <summary>The input slot with the unassigned data value.</summary>
        protected InputSlot inputSlot;

        /// <summary>
        /// The input slot with the unassigned data value.
        /// </summary>
        public InputSlot InputSlot
        {
            get
            {
                return inputSlot;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.InputSlotDataUnassignedException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inputSlot">The input slot with the unassigned data value.</param>
        public InputSlotDataUnassignedException(String message, InputSlot inputSlot)
            : base (message)
        {
            this.inputSlot = inputSlot;
        }
    }
}
