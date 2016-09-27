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

namespace MathematicsModularFramework.UnitTests.TestModules
{
    /// <summary>
    /// A test module with an input slot that contains a data type which is outside of the System.* namespace.
    /// </summary>
    /// <remarks>Used for testing serialization of module graphs which contain data types which are not part of the .NET core library.</remarks>
    public class InputSlotCustomTypeModule : ModuleBase
    {
        public InputSlotCustomTypeModule()
            : base()
        {
            Description = "Module with an input slot that contains a data type which is outside of the System.* namespace";
            AddInputSlot("CustomDataTypeInput", "Input of type MathematicsModularFramework.UnitTests.TestModules.CustomDataType", typeof(CustomDataType));
        }

        protected override void ImplementProcess()
        {
        }
    }

    public class CustomDataType
    {
    }
}
