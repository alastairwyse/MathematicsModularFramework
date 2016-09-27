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
using NMock2;
using MathematicsModularFramework;

namespace MathematicsModularFramework.UnitTests
{
    /// <summary>
    /// Unit tests for class MathematicsModularFramework.ModuleGraphProcessor.
    /// </summary>
    public class ModuleGraphProcessorTests
    {
        private ModuleGraphProcessor testModuleGraphProcessor;
        private Mockery mockery;

        [SetUp]
        protected void SetUp()
        {
            testModuleGraphProcessor = new ModuleGraphProcessor();
            mockery = new Mockery();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ProcessModule() method is called on a module graph that contains a module without a slot link referencing one of its input slots.
        /// </summary>
        [Test]
        public void Process_ModuleHasEmptyInputs()
        {
            IModule mockModule = mockery.NewMock<IModule>();
            ModuleGraph testModuleGraph = new ModuleGraph();
            testModuleGraph.AddModule(mockModule);
            InputSlot testInputSlot = new InputSlot("DataInput", "Test slot for data input", typeof(Int32), mockModule);
            List<InputSlot> testInputSlotList = new List<InputSlot>();
            testInputSlotList.Add(testInputSlot);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockModule).GetProperty("Inputs").Will(Return.Value(testInputSlotList));
            }

            InputSlotDataUnassignedException e = Assert.Throws<InputSlotDataUnassignedException>(delegate
            {
                testModuleGraphProcessor.Process(testModuleGraph, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Input slot 'DataInput' on module 'MockObjectType1' does not have any data assigned to it."));
            Assert.AreSame(testInputSlot, e.InputSlot);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that no exception is thrown if the ProcessModule() method is called on a module graph that contains a module without a slot link referencing one of its input slots, but where the 'allowEmptyModuleInputSlots' parameter is set to true.
        /// </summary>
        [Test]
        public void Process_ModuleHasEmptyInputsButPermitted()
        {
            IModule mockModule = mockery.NewMock<IModule>();
            ModuleGraph testModuleGraph = new ModuleGraph();
            testModuleGraph.AddModule(mockModule);
            InputSlot testInputSlot = new InputSlot("DataInput", "Test slot for data input", typeof(Int32), mockModule);
            List<InputSlot> testInputSlotList = new List<InputSlot>();
            testInputSlotList.Add(testInputSlot);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockModule).GetProperty("Inputs").Will(Return.Value(testInputSlotList));
                Expect.Once.On(mockModule).SetProperty("Logger");
                Expect.Once.On(mockModule).SetProperty("MetricLogger");
                Expect.Once.On(mockModule).Method("Process").WithNoArguments();
                Expect.Once.On(mockModule).GetProperty("Outputs").Will(Return.Value(new List<OutputSlot>()));
            }

            testModuleGraphProcessor.Process(testModuleGraph, true);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that the Process() method correctly recurses from and end point module to a parent module via slot link.
        /// </summary>
        [Test]
        public void Process_CheckRecursionOfModuleLinkedToOutputSlot()
        {
            IModule firstMockModule = mockery.NewMock<IModule>();
            IModule secondMockModule = mockery.NewMock<IModule>();
            ModuleGraph testModuleGraph = new ModuleGraph();
            OutputSlot firstModuleOutputSlot = new OutputSlot("FirstModuleOutput", "FirstModuleOutput description", typeof(Int32), firstMockModule);
            InputSlot secondModuleInputSlot = new InputSlot("SecondModuleInput", "SecondModuleInput description", typeof(Int32), secondMockModule);
            testModuleGraph.AddModule(firstMockModule);
            testModuleGraph.AddModule(secondMockModule);
            List<InputSlot> secondModuleInputSlotList = new List<InputSlot>();
            secondModuleInputSlotList.Add(secondModuleInputSlot);
            List<OutputSlot> firstModuleOutputSlotList = new List<OutputSlot>();
            firstModuleOutputSlotList.Add(firstModuleOutputSlot);
            firstModuleOutputSlot.DataValue = 123;

            using (mockery.Ordered)
            {
                Expect.Once.On(firstMockModule).Method("GetOutputSlot").With("FirstModuleOutput").Will(Return.Value(firstModuleOutputSlot));
                Expect.Once.On(secondMockModule).Method("GetInputSlot").With("SecondModuleInput").Will(Return.Value(secondModuleInputSlot));
                Expect.Once.On(secondMockModule).GetProperty("Inputs").Will(Return.Value(secondModuleInputSlotList));
                Expect.Once.On(firstMockModule).GetProperty("Inputs").Will(Return.Value(new List<InputSlot>()));
                Expect.Once.On(firstMockModule).SetProperty("Logger");
                Expect.Once.On(firstMockModule).SetProperty("MetricLogger");
                Expect.Once.On(firstMockModule).Method("Process").WithNoArguments();
                Expect.Once.On(firstMockModule).GetProperty("Outputs").Will(Return.Value(firstModuleOutputSlotList));
                Expect.Once.On(secondMockModule).SetProperty("Logger");
                Expect.Once.On(secondMockModule).SetProperty("MetricLogger");
                Expect.Once.On(secondMockModule).Method("Process").WithNoArguments();
                Expect.Once.On(secondMockModule).GetProperty("Outputs").Will(Return.Value(new List<OutputSlot>()));
            }

            testModuleGraph.CreateSlotLink(firstMockModule.GetOutputSlot("FirstModuleOutput"), secondMockModule.GetInputSlot("SecondModuleInput"));
            testModuleGraphProcessor.Process(testModuleGraph, false);

            // Checks that the data value was correctly passed from the output slot to the input slot
            Assert.AreEqual(123, secondModuleInputSlot.DataValue);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Process() method is called on a graph which contains a circular reference.
        /// </summary>
        [Test]
        public void Process_GraphContainsCircularReference()
        {
            IModule firstMockModule = mockery.NewMock<IModule>();
            IModule secondMockModule = mockery.NewMock<IModule>();
            IModule thirdMockModule = mockery.NewMock<IModule>();
            ModuleGraph testModuleGraph = new ModuleGraph();
            testModuleGraph.AddModule(firstMockModule);
            testModuleGraph.AddModule(secondMockModule);
            testModuleGraph.AddModule(thirdMockModule);
            InputSlot firstModuleInputSlot = new InputSlot("FirstModuleInput", "FirstModuleOutput description", typeof(Int32), firstMockModule);
            OutputSlot firstModuleOutputSlot1 = new OutputSlot("FirstModuleOutput1", "FirstModuleOutput1 description", typeof(Int32), firstMockModule);
            OutputSlot firstModuleOutputSlot2 = new OutputSlot("FirstModuleOutput2", "FirstModuleOutput2 description", typeof(Int32), firstMockModule);
            InputSlot secondModuleInputSlot = new InputSlot("SecondModuleInput", "SecondModuleInput description", typeof(Int32), secondMockModule);
            OutputSlot secondModuleOutputSlot = new OutputSlot("SecondModuleOutput", "SecondModuleOutput description", typeof(Int32), secondMockModule);
            InputSlot thirdModuleInputSlot = new InputSlot("ThirdModuleInput", "ThirdModuleInput description", typeof(Int32), thirdMockModule);
            List<InputSlot> thirdModuleInputSlotList = new List<InputSlot>();
            thirdModuleInputSlotList.Add(thirdModuleInputSlot);
            List<InputSlot> firstModuleInputSlotList = new List<InputSlot>();
            firstModuleInputSlotList.Add(firstModuleInputSlot);
            List<InputSlot> secondModuleInputSlotList = new List<InputSlot>();
            secondModuleInputSlotList.Add(secondModuleInputSlot);

            using (mockery.Ordered)
            {
                Expect.Once.On(firstMockModule).Method("GetOutputSlot").With("FirstModuleOutput1").Will(Return.Value(firstModuleOutputSlot1));
                Expect.Once.On(secondMockModule).Method("GetInputSlot").With("SecondModuleInput").Will(Return.Value(secondModuleInputSlot));
                Expect.Once.On(firstMockModule).Method("GetOutputSlot").With("FirstModuleOutput2").Will(Return.Value(firstModuleOutputSlot2));
                Expect.Once.On(thirdMockModule).Method("GetInputSlot").With("ThirdModuleInput").Will(Return.Value(thirdModuleInputSlot));
                Expect.Once.On(secondMockModule).Method("GetOutputSlot").With("SecondModuleOutput").Will(Return.Value(secondModuleOutputSlot));
                Expect.Once.On(firstMockModule).Method("GetInputSlot").With("FirstModuleInput").Will(Return.Value(firstModuleInputSlot));

                Expect.Once.On(thirdMockModule).GetProperty("Inputs").Will(Return.Value(thirdModuleInputSlotList));
                Expect.Once.On(firstMockModule).GetProperty("Inputs").Will(Return.Value(firstModuleInputSlotList));
                Expect.Once.On(secondMockModule).GetProperty("Inputs").Will(Return.Value(secondModuleInputSlotList));
            }

            testModuleGraph.CreateSlotLink(firstMockModule.GetOutputSlot("FirstModuleOutput1"), secondMockModule.GetInputSlot("SecondModuleInput"));
            testModuleGraph.CreateSlotLink(firstMockModule.GetOutputSlot("FirstModuleOutput2"), thirdMockModule.GetInputSlot("ThirdModuleInput"));
            testModuleGraph.CreateSlotLink(secondMockModule.GetOutputSlot("SecondModuleOutput"), firstMockModule.GetInputSlot("FirstModuleInput"));
            Exception e = Assert.Throws<Exception>(delegate
            {
                testModuleGraphProcessor.Process(testModuleGraph, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Graph contains a circular reference involving module 'MockObjectType1'."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that an EmptyGraphValidationError is returned if the Validate() method is called on an empty graph.
        /// </summary>
        [Test]
        public void Validate_GraphIsEmpty()
        {
            ModuleGraph testModuleGraph = new ModuleGraph();

            List<ValidationError> errors = testModuleGraphProcessor.Validate(testModuleGraph);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(typeof(EmptyGraphValidationError), errors[0].GetType());
        }

        /// <summary>
        /// Tests that an UnlinkedInputSlotValidationError is returned if the Validate() method is called on a module with an unlinked input slot.
        /// </summary>
        [Test]
        public void Validate_UnlinkedInputSlot()
        {
            IModule mockModule = mockery.NewMock<IModule>();
            ModuleGraph testModuleGraph = new ModuleGraph();
            testModuleGraph.AddModule(mockModule);
            InputSlot moduleInputSlot = new InputSlot("ModuleInput", "ModuleInput description", typeof(Int32), mockModule);
            List<InputSlot> moduleInputSlotList = new List<InputSlot>();
            moduleInputSlotList.Add(moduleInputSlot);
            List<OutputSlot> moduleOutputSlotList = new List<OutputSlot>();

            using (mockery.Ordered)
            {
                Expect.Once.On(mockModule).GetProperty("Inputs").Will(Return.Value(moduleInputSlotList));
                Expect.Once.On(mockModule).GetProperty("Outputs").Will(Return.Value(moduleOutputSlotList));
            }

            List<ValidationError> errors = testModuleGraphProcessor.Validate(testModuleGraph);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(typeof(UnlinkedInputSlotValidationError), errors[0].GetType());
            Assert.AreSame(moduleInputSlot, ((UnlinkedInputSlotValidationError)errors[0]).InputSlot);
        }

        /// <summary>
        /// Tests that an UnlinkedOutputSlotValidationError is returned if the Validate() method is called on a module with an unlinked output slot.
        /// </summary>
        [Test]
        public void Validate_UnlinkedOutputSlot()
        {
            IModule mockModule = mockery.NewMock<IModule>();
            ModuleGraph testModuleGraph = new ModuleGraph();
            testModuleGraph.AddModule(mockModule);
            OutputSlot moduleOutputSlot = new OutputSlot("ModuleOutput", "ModuleOutput description", typeof(Int32), mockModule);
            List<InputSlot> moduleInputSlotList = new List<InputSlot>();
            List<OutputSlot> moduleOutputSlotList = new List<OutputSlot>();
            moduleOutputSlotList.Add(moduleOutputSlot);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockModule).GetProperty("Inputs").Will(Return.Value(moduleInputSlotList));
                Expect.Once.On(mockModule).GetProperty("Outputs").Will(Return.Value(moduleOutputSlotList));
            }

            List<ValidationError> errors = testModuleGraphProcessor.Validate(testModuleGraph);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(typeof(UnlinkedOutputSlotValidationError), errors[0].GetType());
            Assert.AreSame(moduleOutputSlot, ((UnlinkedOutputSlotValidationError)errors[0]).OutputSlot);
        }
    }
}
