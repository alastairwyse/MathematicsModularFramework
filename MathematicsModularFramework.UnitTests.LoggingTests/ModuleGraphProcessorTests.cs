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
using NMock2.Matchers;
using ApplicationLogging;
using MathematicsModularFramework;
using MathematicsModularFramework.UnitTests.TestModules;

namespace MathematicsModularFramework.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class MathematicsModularFramework.ModuleGraphProcessor.
    /// </summary>
    public class ModuleGraphProcessorTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private ModuleGraphProcessor testModuleGraphProcessor;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testModuleGraphProcessor = new ModuleGraphProcessor(mockApplicationLogger);
        }

        /// <summary>
        /// Tests all logging functionality for the Process() method.
        /// </summary>
        [Test]
        public void Process()
        {
            Module1_2 module1 = new Module1_2();
            Module1_0NoProcess module2 = new Module1_0NoProcess();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Information, "Starting module graph processing.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(new TypeMatcher(typeof(ModuleGraphRecurser)), LogLevel.Debug, "Recursing from module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' to module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2'.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Warning, "Input slot 'Input1' on module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2' does not have any data assigned to it.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Information, "Processing module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2'.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Removing slot link between output slot 'Output1' on module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2' and input slot 'Input1' on module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' from the module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Warning, "Output slot 'Output2' on module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2' is not referenced by a slot link.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Removing module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2' from the module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Information, "Processing module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess'.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Removing module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' from the module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Information, "Module graph processing completed.");
            }

            testModuleGraphProcessor.Process(graph, true);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests all logging functionality for the Copy() method.
        /// </summary>
        [Test]
        public void Copy()
        {
            Module0_2 module1 = new Module0_2();
            Module1_0NoProcess module2 = new Module1_0NoProcess();
            Module1_0NoProcess module3 = new Module1_0NoProcess();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Created a copy of module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' in destination module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Created a copy of module 'MathematicsModularFramework.UnitTests.TestModules.Module0_2' in destination module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Created a slot link between output slot 'Output1' on module 'MathematicsModularFramework.UnitTests.TestModules.Module0_2' and input slot 'Input1' on module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' in destination module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Created a copy of module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' in destination module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Module 'MathematicsModularFramework.UnitTests.TestModules.Module0_2' has already been copied to the destination module graph.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testModuleGraphProcessor, LogLevel.Debug, "Created a slot link between output slot 'Output2' on module 'MathematicsModularFramework.UnitTests.TestModules.Module0_2' and input slot 'Input1' on module 'MathematicsModularFramework.UnitTests.LoggingTests.ModuleGraphProcessorTests+Module1_0NoProcess' in destination module graph.");
            }

            testModuleGraphProcessor.Copy(graph);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        #region Test Classes

        /// <summary>
        /// Overrides the base class with a version which does nothing when Process() is called.  This is prevent exceptions in the Process() test above, as no data is passed from its parent module in that test.
        /// </summary>
        private class Module1_0NoProcess : Module1_0
        {
            protected override void ImplementProcess()
            {
            }
        }
        
        #endregion
    }
}
