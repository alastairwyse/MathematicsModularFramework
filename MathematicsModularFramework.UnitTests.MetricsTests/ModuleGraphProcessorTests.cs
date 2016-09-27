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
using ApplicationLogging;
using ApplicationMetrics;
using MathematicsModularFramework;
using MathematicsModularFramework.Metrics;
using MathematicsModularFramework.UnitTests.TestModules;

namespace MathematicsModularFramework.UnitTests.MetricsTests
{
    /// <summary>
    /// Tests for the metrics logging functionality in class MathematicsModularFramework.ModuleGraphProcessor.
    /// </summary>
    public class ModuleGraphProcessorTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private ModuleGraphProcessor testModuleGraphProcessor;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            testModuleGraphProcessor = new ModuleGraphProcessor(mockMetricLogger);
        }

        /// <summary>
        /// Tests all metric logging functionality for the Process() method.
        /// </summary>
        [Test]
        public void Process()
        {
            Module0_0 module1 = new Module0_0();
            Module0_0 module2 = new Module0_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleGraphProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("End").With(IsMetric.Equal(new ModuleProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new ModuleProcessed()));
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("End").With(IsMetric.Equal(new ModuleProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new ModuleProcessed()));
                Expect.Once.On(mockMetricLogger).Method("End").With(IsMetric.Equal(new ModuleGraphProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new ModuleGraphProcessed()));
            }

            testModuleGraphProcessor.Process(graph, true);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that cancel begin of a 'ModuleProcessingTime' metric is logged if an exception is thrown during the Process() method.
        /// </summary>
        [Test]
        public void Process_CancelBeginMetricLoggedWhenExceptionOccurs()
        {
            ExceptionModule module1 = new ExceptionModule();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleGraphProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("CancelBegin").With(IsMetric.Equal(new ModuleProcessingTime()));
                Expect.Once.On(mockMetricLogger).Method("CancelBegin").With(IsMetric.Equal(new ModuleGraphProcessingTime()));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testModuleGraphProcessor.Process(graph, true);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Processing Exception"));

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests all metric logging functionality for the Copy() method.
        /// </summary>
        [Test]
        public void Copy()
        {
            Module0_0 module1 = new Module0_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleGraphCopyingTime()));
                Expect.Once.On(mockMetricLogger).Method("End").With(IsMetric.Equal(new ModuleGraphCopyingTime()));
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new ModuleGraphCopied()));
            }

            testModuleGraphProcessor.Copy(graph);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that cancel begin of a 'ModuleGraphCopyingTime' metric is logged if an exception is thrown during the Copy() method.
        /// </summary>
        //[Test]
        public void Copy_CancelBeginMetricLoggedWhenExceptionOccurs()
        {
            // TODO: Need to find a way to implement this test.  Currently can't find a way to force an exception to be thrown when the graph is copied.
            ExceptionModule module1 = new ExceptionModule();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new ModuleGraphCopyingTime()));
                Expect.Once.On(mockMetricLogger).Method("CancelBegin").With(IsMetric.Equal(new ModuleGraphCopyingTime()));
            }

            // A TargetInvocationException is thrown by the call to Activator.CreateInstance() within the Copy() method, so need to expect that type of exception
            System.Reflection.TargetInvocationException e = Assert.Throws<System.Reflection.TargetInvocationException>(delegate
            {
                testModuleGraphProcessor.Copy(graph);
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
