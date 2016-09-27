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
    /// Unit tests for class MathematicsModularFramework.ModuleGraph.
    /// </summary>
    public class ModuleGraphTests
    {
        private ModuleGraph testModuleGraph;
        private Divider divider;
        private Multiplier multiplier;
        private Doubler doubler;
        private FirstModule firstModule;
        private SecondModule secondModule;

        [SetUp]
        protected void SetUp()
        {
            testModuleGraph = new ModuleGraph();
            divider = new Divider();
            multiplier = new Multiplier();
            doubler = new Doubler();
            firstModule = new FirstModule();
            secondModule = new SecondModule();
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddModule() method is called with a module which already exists in the graph.
        /// </summary>
        [Test]
        public void AddModule_ModuleAlreadyExists()
        {
            testModuleGraph.AddModule(divider);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.AddModule(divider);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The specified module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider' already exists in the graph."));
            Assert.AreEqual("module", e.ParamName);
        }

        /// <summary>
        /// Success test for method AddModule().
        /// </summary>
        [Test]
        public void AddModule()
        {
            Assert.AreEqual(0, testModuleGraph.EndPoints.Count<IModule>());

            testModuleGraph.AddModule(divider);

            Assert.IsTrue(testModuleGraph.EndPoints.Contains<IModule>(divider));
        }

        /// <summary>
        /// Tests that an exception is thrown when the RemoveModule() method is called with a module that doesn't exist in the graph.
        /// </summary>
        [Test]
        public void RemoveModule_ModuleDoesntExist()
        {
            testModuleGraph.AddModule(multiplier);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveModule(divider);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The specified module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider' does not exist in the graph."));
            Assert.AreEqual("module", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the RemoveModule() method is called with a module that has slot links referencing its input slot.
        /// </summary>
        [Test]
        public void RemoveModule_ModulesInputSlotIsLinked()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveModule(multiplier);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The graph contains slot links referencing the input slot(s) of module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier'."));
            Assert.AreEqual("module", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the RemoveModule() method is called with a module that has slot links referencing its output slot.
        /// </summary>
        [Test]
        public void RemoveModule_ModulesOutputSlotIsLinked()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveModule(divider);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The graph contains slot links referencing the output slot(s) of module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider'."));
            Assert.AreEqual("module", e.ParamName);
        }

        /// <summary>
        /// Success test for method RemoveModule().
        /// </summary>
        [Test]
        public void RemoveModule()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);

            testModuleGraph.RemoveModule(divider);

            Assert.AreEqual(1, testModuleGraph.EndPoints.Count<IModule>());
            Assert.IsFalse(testModuleGraph.EndPoints.Contains<IModule>(divider));
        }

        /// <summary>
        /// Tests that an exception is thrown if the CreateSlotLink() method is called with an 'outputSlot' parameter whose parent module has not been added to the graph.
        /// </summary>
        [Test]
        public void CreateSlotLink_OutputSlotModuleNotAddedToGraph()
        {
            testModuleGraph.AddModule(divider);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.CreateSlotLink(multiplier.GetOutputSlot("Result"), divider.GetInputSlot("Dividend"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Output slot 'Result's parent module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier' does not exist in the graph."));
            Assert.AreEqual("outputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CreateSlotLink() method is called with an 'inputSlot' parameter whose parent module has not been added to the graph.
        /// </summary>
        [Test]
        public void CreateSlotLink_InputSlotModuleNotAddedToGraph()
        {
            testModuleGraph.AddModule(multiplier);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.CreateSlotLink(multiplier.GetOutputSlot("Result"), divider.GetInputSlot("Dividend"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Input slot 'Dividend's parent module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider' does not exist in the graph."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CreateSlotLink() method is called with an input and output slot on the same module.
        /// </summary>
        [Test]
        public void CreateSlotLink_AttemptToLinkModuleToItself()
        {
            testModuleGraph.AddModule(doubler);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.CreateSlotLink(doubler.GetOutputSlot("Result"), doubler.GetInputSlot("Input"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempt to create a slot link between an output on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Doubler' and an input on the same module would create a circular reference."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CreateSlotLink() method is called with an 'inputSlot' parameter which is already assigned to by a slot link.
        /// </summary>
        [Test]
        public void CreateSlotLink_InputSlotAlreadyLinked()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Remainder"), multiplier.GetInputSlot("Multiplicand"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Input slot 'Multiplicand' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier' is already referenced by a slot link."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CreateSlotLink() method is called where the data type of the input slot is derived from (i.e. subclass of) the data type of the output slot.
        /// <remarks>Actual exception is thrown in the constructor of the SlotLink class.</remarks>
        /// </summary>
        [Test]
        public void CreateSlotLink_InputSlotDataTypeNotAssignableFromOutputSlotDataType()
        {
            testModuleGraph.AddModule(firstModule);
            testModuleGraph.AddModule(secondModule);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.CreateSlotLink(firstModule.GetOutputSlot("BaseClassResult"), secondModule.GetInputSlot("DerivedClassInput"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Data type 'MathematicsModularFramework.UnitTests.ModuleGraphTests+TestDerivedClass' of input slot 'DerivedClassInput' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+SecondModule' is not assignable from data type 'MathematicsModularFramework.UnitTests.ModuleGraphTests+TestBaseClass' of output slot 'BaseClassResult' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+FirstModule'."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CreateSlotLink() method is called where the data type of the input slot is completely different from the data type of the output slot.
        /// <remarks>Actual exception is thrown in the constructor of the SlotLink class.</remarks>
        /// </summary>
        [Test]
        public void CreateSlotLink_DifferentInputSlotAndOutputSlotDataTypes()
        {
            testModuleGraph.AddModule(firstModule);
            testModuleGraph.AddModule(secondModule);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.CreateSlotLink(firstModule.GetOutputSlot("BaseClassResult"), secondModule.GetInputSlot("StringInput"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Data type 'System.String' of input slot 'StringInput' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+SecondModule' is not assignable from data type 'MathematicsModularFramework.UnitTests.ModuleGraphTests+TestBaseClass' of output slot 'BaseClassResult' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+FirstModule'."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Success test for method CreateSlotLink() where the data type of the input slot is a base class of (i.e. superclass of) the data type of the output slot.
        /// <remarks>Actual exception checking code under tests is in the constructor of the SlotLink class.</remarks>
        /// </summary>
        [Test]
        public void CreateSlotLink_InputSlotDataTypeIsAssignableFromOutputSlotDataType()
        {
            testModuleGraph.AddModule(firstModule);
            testModuleGraph.AddModule(secondModule);

            testModuleGraph.CreateSlotLink(firstModule.GetOutputSlot("DerivedClassResult"), secondModule.GetInputSlot("BaseClassInput"));

            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(firstModule.GetOutputSlot("DerivedClassResult")).Contains<InputSlot>(secondModule.GetInputSlot("BaseClassInput")));
            Assert.AreEqual(1, testModuleGraph.GetInputSlotsLinkedToOutputSlot(firstModule.GetOutputSlot("DerivedClassResult")).Count<InputSlot>());
        }

        /// <summary>
        /// Success test for method CreateSlotLink() which ensures multiple links can be created for the same output slot.
        /// </summary>
        [Test]
        public void CreateSlotLink_MultipleLinksFromSameOutputSlot()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);

            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            Assert.IsTrue(testModuleGraph.EndPoints.Contains<IModule>(multiplier));
            Assert.IsFalse(testModuleGraph.EndPoints.Contains<IModule>(divider));
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplicand")));
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplier")));
            Assert.AreEqual(2, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());
        }

        /// <summary>
        /// Success test for method CreateSlotLink().
        /// </summary>
        [Test]
        public void CreateSlotLink()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);

            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));

            Assert.IsTrue(testModuleGraph.EndPoints.Contains<IModule>(multiplier));
            Assert.IsFalse(testModuleGraph.EndPoints.Contains<IModule>(divider));
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplicand")));
            Assert.AreEqual(1, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());
        }

        /// <summary>
        /// Tests that an exception is thrown if the RemoveSlotLink() method is called with an 'outputSlot' parameter whose parent module has not been added to the graph.
        /// </summary>
        [Test]
        public void RemoveSlotLink_OutputSlotModuleNotAddedToGraph()
        {
            testModuleGraph.AddModule(divider);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveSlotLink(multiplier.GetOutputSlot("Result"), divider.GetInputSlot("Dividend"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Output slot 'Result's parent module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier' does not exist in the graph."));
            Assert.AreEqual("outputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the RemoveSlotLink() method is called with an 'inputSlot' parameter whose parent module has not been added to the graph.
        /// </summary>
        [Test]
        public void RemoveSlotLink_InputSlotModuleNotAddedToGraph()
        {
            testModuleGraph.AddModule(multiplier);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveSlotLink(multiplier.GetOutputSlot("Result"), divider.GetInputSlot("Dividend"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Input slot 'Dividend's parent module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider' does not exist in the graph."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the RemoveSlotLink() method is called with an output slot whose parent module has no slot links defined.
        /// </summary>
        [Test]
        public void RemoveSlotLink_NoLinksDefinedOnModule()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveSlotLink(multiplier.GetOutputSlot("Result"), divider.GetInputSlot("Dividend"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("A slot link does not exist between output slot 'Result' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier',  and input slot 'Dividend' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider'.'"));
            Assert.AreEqual("outputSlot", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the RemoveSlotLink() method is called with an output slot and input slot for which no link exists.
        /// </summary>
        [Test]
        public void RemoveSlotLink_LinkDoesNotExistForSpecifiedSlots()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.RemoveSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("A slot link does not exist between output slot 'Quotient' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Divider',  and input slot 'Multiplicand' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier'.'"));
            Assert.AreEqual("outputSlot", e.ParamName);
        }

        /// <summary>
        /// Success test for method RemoveSlotLink().
        /// </summary>
        [Test]
        public void RemoveSlotLink()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            testModuleGraph.RemoveSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            Assert.IsTrue(testModuleGraph.EndPoints.Contains<IModule>(divider));
            Assert.AreEqual(0, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());
        }

        /// <summary>
        /// Success test for method OutputSlotIsLinkedToInputSlot().
        /// </summary>
        [Test]
        public void OutputSlotIsLinkedToInputSlot()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);

            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(divider.GetInputSlot("Dividend")));
            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(divider.GetInputSlot("Divisor")));
            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(multiplier.GetInputSlot("Multiplicand")));
            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(multiplier.GetInputSlot("Multiplier")));

            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(divider.GetInputSlot("Dividend")));
            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(divider.GetInputSlot("Divisor")));
            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(multiplier.GetInputSlot("Multiplicand")));
            Assert.IsTrue(testModuleGraph.OutputSlotIsLinkedToInputSlot(multiplier.GetInputSlot("Multiplier")));

            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));

            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(divider.GetInputSlot("Dividend")));
            Assert.IsFalse(testModuleGraph.OutputSlotIsLinkedToInputSlot(divider.GetInputSlot("Divisor")));
            Assert.IsTrue(testModuleGraph.OutputSlotIsLinkedToInputSlot(multiplier.GetInputSlot("Multiplicand")));
            Assert.IsTrue(testModuleGraph.OutputSlotIsLinkedToInputSlot(multiplier.GetInputSlot("Multiplier")));
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetOutputSlotLinkedToInputSlot() method is called for an input slot which has not been linked to an output slot.
        /// </summary>
        [Test]
        public void GetOutputSlotLinkedToInputSlot_SlotIsNotLinked()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testModuleGraph.GetOutputSlotLinkedToInputSlot(multiplier.GetInputSlot("Multiplicand"));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Input slot 'Multiplicand' on module 'MathematicsModularFramework.UnitTests.ModuleGraphTests+Multiplier' is not referenced by a slot link."));
            Assert.AreEqual("inputSlot", e.ParamName);
        }

        /// <summary>
        /// Success test for method GetOutputSlotLinkedToInputSlot().
        /// </summary>
        [Test]
        public void GetOutputSlotLinkedToInputSlot()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));

            OutputSlot testOutputSlot = testModuleGraph.GetOutputSlotLinkedToInputSlot(multiplier.GetInputSlot("Multiplier"));
            Assert.AreEqual(divider.GetOutputSlot("Quotient"), testOutputSlot);
            testOutputSlot = testModuleGraph.GetOutputSlotLinkedToInputSlot(multiplier.GetInputSlot("Multiplicand"));
            Assert.AreEqual(divider.GetOutputSlot("Quotient"), testOutputSlot);
        }

        /// <summary>
        /// Success test for method GetInputSlotsLinkedToOutputSlot().
        /// </summary>
        [Test]
        public void GetInputSlotsLinkedToOutputSlot()
        {
            testModuleGraph.AddModule(divider);
            testModuleGraph.AddModule(multiplier);

            Assert.AreEqual(0, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());

            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Remainder"), multiplier.GetInputSlot("Multiplicand"));
            Assert.AreEqual(1, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplier")));

            testModuleGraph.RemoveSlotLink(divider.GetOutputSlot("Remainder"), multiplier.GetInputSlot("Multiplicand"));
            testModuleGraph.CreateSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplicand"));
            Assert.AreEqual(2, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplier")));
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplicand")));

            testModuleGraph.RemoveSlotLink(divider.GetOutputSlot("Quotient"), multiplier.GetInputSlot("Multiplier"));
            Assert.AreEqual(1, testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Count<InputSlot>());
            Assert.IsTrue(testModuleGraph.GetInputSlotsLinkedToOutputSlot(divider.GetOutputSlot("Quotient")).Contains<InputSlot>(multiplier.GetInputSlot("Multiplicand")));
        }

        #region Test Classes

        private class Divider : ModuleBase
        {
            public Divider()
                : base()
            {
                Description = "Divides two integers producing quotient and remainder integer results.";
                AddInputSlot("Dividend", "The number to divide.", typeof(Int32));
                AddInputSlot("Divisor", "The number to divide by.", typeof(Int32));
                AddOutputSlot("Quotient", "The whole number part of the result.", typeof(Int32));
                AddOutputSlot("Remainder", "The remaining part of the result.", typeof(Int32));
            }

            protected override void ImplementProcess()
            {
            }
        }

        private class Multiplier : ModuleBase
        {
            public Multiplier()
                : base()
            {
                Description = "Multiplies two integers.";
                AddInputSlot("Multiplicand", "The number to multiply.", typeof(Int32));
                AddInputSlot("Multiplier", "The number to multiply by.", typeof(Int32));
                AddOutputSlot("Result", "The result of the multiplication.", typeof(Int32));
            }

            protected override void ImplementProcess()
            {
            }
        }

        private class Doubler : ModuleBase
        {
            public Doubler()
                : base()
            {
                Description = "Doubles an integer.";
                AddInputSlot("Input", "The number to double.", typeof(Int32));
                AddOutputSlot("Result", "The doubled number.", typeof(Int32));
            }

            protected override void ImplementProcess()
            {
            }
        }

        private class TestBaseClass
        {
        }

        private class TestDerivedClass : TestBaseClass
        {
        }

        private class FirstModule : ModuleBase
        {
            public FirstModule()
                : base()
            {
                description = "First (source) module.";
                AddOutputSlot("BaseClassResult", "Result of type 'BaseClass'", typeof(TestBaseClass));
                AddOutputSlot("DerivedClassResult", "Result of type 'TestDerivedClass'", typeof(TestDerivedClass));
            }

            protected override void ImplementProcess()
            {
            }
        }

        private class SecondModule : ModuleBase
        {
            public SecondModule()
                : base()
            {
                description = "Second (target) module.";
                AddInputSlot("DerivedClassInput", "Input of type 'DerivedClass'", typeof(TestDerivedClass));
                AddInputSlot("BaseClassInput", "Input of type 'TestBaseClass'", typeof(TestBaseClass));
                AddInputSlot("StringInput", "Input of type 'String'", typeof(String));
            }

            protected override void ImplementProcess()
            {
            }
        }

        #endregion
    }
}
