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
using NUnit.Framework;
using MathematicsModularFramework;

namespace MathematicsModularFramework.UnitTests
{
    /// <summary>
    /// Unit tests for class MathematicsModularFramework.ModuleBase.
    /// </summary>
    public class ModuleBaseTests
    {
        private TestModule testModule;

        [SetUp]
        protected void SetUp()
        {
            testModule = new TestModule();
        }

        /// <summary>
        /// Tests if the GetInputSlot() method is called with an invalid 'name' parameter.
        /// </summary>
        [Test]
        public void GetInputSlot_InvalidName()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModule.GetInputSlot("invalid");
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("An input slot with name 'invalid' does not exist on module 'MathematicsModularFramework.UnitTests.ModuleBaseTests+TestModule'."));
            Assert.AreEqual("name", e.ParamName);
        }

        /// <summary>
        /// Success test for method GetInputSlot().
        /// </summary>
        [Test]
        public void GetInputSlot()
        {
            InputSlot testInputSlot = testModule.GetInputSlot("Slot1");

            Assert.AreEqual("Slot1 (type = String)", testInputSlot.Description);
            Assert.AreEqual(typeof(String), testInputSlot.DataType);
        }

        /// <summary>
        /// Tests if the GetOutputSlot() method is called with an invalid 'name' parameter.
        /// </summary>
        [Test]
        public void GetOutputSlot_InvalidName()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModule.GetOutputSlot("invalid2");
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("An output slot with name 'invalid2' does not exist on module 'MathematicsModularFramework.UnitTests.ModuleBaseTests+TestModule'."));
            Assert.AreEqual("name", e.ParamName);
        }

        /// <summary>
        /// Success test for method GetOutputSlot().
        /// </summary>
        [Test]
        public void GetOutputSlot()
        {
            OutputSlot testOutputSlot = testModule.GetOutputSlot("Result");

            Assert.AreEqual("Result of the calculation", testOutputSlot.Description);
            Assert.AreEqual(typeof(Double), testOutputSlot.DataType);
        }

        /// <summary>
        /// Tests that an exception is thrown if a module which has already been processed is processed again.
        /// </summary>
        [Test]
        public void Process_ModuleWhichHasAlreadyBeenProcessed()
        {
            testModule.GetInputSlot("Slot1").DataValue = "Input Data";
            testModule.Process();

            ModuleAlreadyProcessedException e = Assert.Throws<ModuleAlreadyProcessedException>(delegate
            {
                testModule.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Module 'MathematicsModularFramework.UnitTests.ModuleBaseTests+TestModule' has already been processed."));
            Assert.AreSame(testModule, e.Module);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Process() method is called on the module when data of the wrong type has been assigned to its input slot.
        /// </summary>
        [Test]
        public void PreProcess_IncorrectDataTypeSetOnInputSlot()
        {
            testModule.GetInputSlot("Slot1").DataValue = 123;
            
            Exception e = Assert.Throws<Exception>(delegate
            {
                testModule.Process();
            });
            
            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Input slot 'Slot1' on module 'MathematicsModularFramework.UnitTests.ModuleBaseTests+TestModule' specifies data type 'System.String', but contains data of type 'System.Int32'."));
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddInputSlot() method is called with a 'name' parameter that is already used by another input slot.
        /// </summary>
        [Test]
        public void AddInputSlot_NameAlreadyExists()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                DuplicateInputSlotModule testDuplicateInputSlotModule = new DuplicateInputSlotModule();
            });
            
            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Failed to add input slot to module 'MathematicsModularFramework.UnitTests.ModuleBaseTests+DuplicateInputSlotModule'.  The module already contains an input slot with name 'Slot1'."));
            Assert.AreEqual("name", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddOutputSlot() method is called with a 'name' parameter that is already used by another output slot.
        /// </summary>
        [Test]
        public void AddOutputSlot_NameAlreadyExists()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                DuplicateOutputSlotModule testDuplicateOutputSlotModule = new DuplicateOutputSlotModule();
            });
            
            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Failed to add output slot to module 'MathematicsModularFramework.UnitTests.ModuleBaseTests+DuplicateOutputSlotModule'.  The module already contains an output slot with name 'Result'."));
            Assert.AreEqual("name", e.ParamName);
        }

        #region Test Classes

        private class TestModule : ModuleBase
        {
            public TestModule()
                : base()
            {
                description = "Module created for unit testing.";
                AddInputSlot("Slot1", "Slot1 (type = String)", typeof(String));
                AddOutputSlot("Result", "Result of the calculation", typeof(Double));
            }

            protected override void ImplementProcess()
            {
            }
        }

        private class DuplicateInputSlotModule : ModuleBase
        {
            public DuplicateInputSlotModule()
                : base()
            {
                description = "Module created for unit testing.";
                AddInputSlot("Slot1", "Slot1 (type = String)", typeof(String)); 
                AddInputSlot("Slot1", "Slot1 (type = String)", typeof(String));
            }

            protected override void ImplementProcess()
            {
            }
        }

        private class DuplicateOutputSlotModule : ModuleBase
        {
            public DuplicateOutputSlotModule()
                : base()
            {
                description = "Module created for unit testing.";
                AddOutputSlot("Result", "Result of the calculation", typeof(Double));
                AddOutputSlot("Result", "Result of the calculation", typeof(Double));
            }

            protected override void ImplementProcess()
            {
            }
        }

        #endregion
    }
}
