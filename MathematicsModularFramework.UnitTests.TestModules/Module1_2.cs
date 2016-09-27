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
    /// A test module with 1 input and 2 outputs.
    /// </summary>
    public class Module1_2 : ModuleBase
    {
        public Module1_2()
            : base()
        {
            Description = "Module with 1 input and 2 outputs";
            AddInputSlot("Input1", "The first input slot", typeof(Int32));
            AddOutputSlot("Output1", "The first output slot", typeof(Int32));
            AddOutputSlot("Output2", "The second output slot", typeof(Int32));
        }

        protected override void ImplementProcess()
        {
            GetOutputSlot("Output1").DataValue = GetInputSlot("Input1").DataValue;
            GetOutputSlot("Output2").DataValue = GetInputSlot("Input1").DataValue;
        }
    }
}
