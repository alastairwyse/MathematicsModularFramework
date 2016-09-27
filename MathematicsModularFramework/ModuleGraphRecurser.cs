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
using ApplicationLogging;

namespace MathematicsModularFramework
{
    /// <summary>
    /// Recurses a module graph, allowing the functionality at various points during recursion to be defined by delegates.
    /// </summary>
    /// <remarks>This class was created to reduce code duplication in methods which recurse a module graph.  E.g. prior to the creation of this class, the Process(), Copy(), and Validate() methods in the ModuleGraphProcessor class all implemented separate, but similar recursion logic.  This class allowed the recursion logic to be consolidated.</remarks>
    public class ModuleGraphRecurser
    {
        /// <summary>Holds a collection of modules which have been visited as part of the recursion process.</summary>
        private HashSet<IModule> visitedModules;
        /// <summary>Holds a collection of modules which have been recursed (i.e. where parents of the module have also been recursed) as part of the recursion process.</summary>
        private HashSet<IModule> recursedModules;
        /// <summary>The code to execute if a circular reference is encountered in the graph.</summary>
        private Action<IModule> circularReferenceAction;
        /// <summary>The code to execute if an unlinked input slot is encountered in the graph.</summary>
        private Action<InputSlot> unlinkedInputSlotAction;
        /// <summary>The code to execute if an input slot is encountered which does not have data assigned to it.</summary>
        private Action<InputSlot> dataUnassignedInputSlotAction;
        /// <summary>The code to execute to process each module in the graph that is recursed.</summary>
        private Action<IModule> recursionAction;
        /// <summary>The code to execute for each output slot in a module after processing.</summary>
        private Action<OutputSlot> outputSlotAction;
        /// <summary>The code to execute after processing the module.</summary>
        private Action<IModule> postRecursionAction;
        /// <summary>The logger to write log events to.</summary>
        private IApplicationLogger logger;
        /// <summary>Utility class used to write log events.</summary>
        private LoggingUtilities loggingUtilities;

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraphRecurser class.
        /// </summary>
        /// <param name="circularReferenceAction">The code to execute if a circular reference is encountered in the graph.</param>
        /// <param name="unlinkedInputSlotAction">The code to execute if an unlinked input slot is encountered in the graph.</param>
        /// <param name="dataUnassignedInputSlotAction">The code to execute if an input slot is encountered which does not have data assigned to it.</param>
        /// <param name="recursionAction">The code to execute to process each module in the graph that is recursed.</param>
        /// <param name="outputSlotAction">The code to execute for each output slot in a module after processing.</param>
        /// <param name="postRecursionAction">The code to execute after processing the module.</param>
        public ModuleGraphRecurser(Action<IModule> circularReferenceAction, Action<InputSlot> unlinkedInputSlotAction, Action<InputSlot> dataUnassignedInputSlotAction, Action<IModule> recursionAction, Action<OutputSlot> outputSlotAction, Action<IModule> postRecursionAction)
        {
            visitedModules = new HashSet<IModule>();
            recursedModules = new HashSet<IModule>();
            this.circularReferenceAction = circularReferenceAction;
            this.unlinkedInputSlotAction = unlinkedInputSlotAction;
            this.dataUnassignedInputSlotAction = dataUnassignedInputSlotAction;
            this.recursionAction = recursionAction;
            this.outputSlotAction = outputSlotAction;
            this.postRecursionAction = postRecursionAction;
            logger = new NullApplicationLogger();
            loggingUtilities = new LoggingUtilities(logger);

        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraphRecurser class.
        /// </summary>
        /// <param name="circularReferenceAction">The code to execute if a circular reference is encountered in the graph.</param>
        /// <param name="unlinkedInputSlotAction">The code to execute if an unlinked input slot is encountered in the graph.</param>
        /// <param name="dataUnassignedInputSlotAction">The code to execute if an input slot is encountered which does not have data assigned to it.</param>
        /// <param name="recursionAction">The code to execute to process each module in the graph that is recursed.</param>
        /// <param name="outputSlotAction">The code to execute for each output slot in a module after processing.</param>
        /// <param name="postRecursionAction">The code to execute after processing the module.</param>
        /// <param name="logger">The logger to write log events to.</param>
        public ModuleGraphRecurser(Action<IModule> circularReferenceAction, Action<InputSlot> unlinkedInputSlotAction, Action<InputSlot> dataUnassignedInputSlotAction, Action<IModule> recursionAction, Action<OutputSlot> outputSlotAction, Action<IModule> postRecursionAction, IApplicationLogger logger)
            : this(circularReferenceAction, unlinkedInputSlotAction, dataUnassignedInputSlotAction, recursionAction, outputSlotAction, postRecursionAction)
        {
            this.logger = logger;
            loggingUtilities = new LoggingUtilities(logger);
        }

        /// <summary>
        /// Recurses the specified module graph.
        /// </summary>
        /// <param name="graph">The module graph to recurse.</param>
        /// <param name="removeModuleReferencesAfterRecursing">Whether to remove references to a module from the classes internal members after recursing the module.  This is necessary for some types of recursion operation, e.g. ModuleGraphProcessor.Process(), which removes modules from the graph during the recursion process to allow garbage collection.  If references to the modules are held in internal members for the durtion of processing, garbage collection cannot occur.</param>
        public void Recurse(ModuleGraph graph, Boolean removeModuleReferencesAfterRecursing)
        {
            visitedModules.Clear();
            recursedModules.Clear();

            // Add all end points modules in the graph to a queue
            //   Adding modules to a queue rather than simply enumerating the 'EndPoints' property is required for methods like ModuleGraphProcessor.Process() which remove modules from the graph as they recurse.  This would not be permitted if the end points were being enumerated when the removal was attempted.
            Queue<IModule> moduleQueue = new Queue<IModule>();
            foreach (IModule currentModule in graph.EndPoints)
            {
                moduleQueue.Enqueue(currentModule);
            }

            // Recurse each of the endpoint modules
            while (moduleQueue.Count > 0)
            {
                IModule currentModule = moduleQueue.Dequeue();
                RecurseModule(currentModule, graph, removeModuleReferencesAfterRecursing);
            }

            visitedModules.Clear();
            recursedModules.Clear();
        }

        /// <summary>
        /// Recurses an individual module in a module graph.
        /// </summary>
        /// <param name="module">The module to recurse.</param>
        /// <param name="graph">The graph that the specified module is a member of.</param>
        /// <param name="removeModuleReferencesAfterRecursing">Whether to remove references to a module from the classes internal members after recursing the module.  This is necessary for some type of recursion operation, e.g. ModuleGraphProcessor.Process(), which removes modules from the graph during the recursion process to allow garbage collection.  If references to the modules are held in internal members for the durtion of processing, garbage collection cannot occur.</param>
        private void RecurseModule(IModule module, ModuleGraph graph, Boolean removeModuleReferencesAfterRecursing)
        {
            if (recursedModules.Contains(module) == false)
            {
                if (visitedModules.Contains(module) == true)
                {
                    // A circular reference has been identified, so invoke the relevant action
                    circularReferenceAction.Invoke(module);
                }
                else
                {
                    visitedModules.Add(module);
                    // Iterate each of the module's input slots in order to recurse its parent modules
                    foreach (InputSlot currentInputSlot in module.Inputs)
                    {
                        if (graph.OutputSlotIsLinkedToInputSlot(currentInputSlot) == true)
                        {
                            OutputSlot linkedOutputSlot = graph.GetOutputSlotLinkedToInputSlot(currentInputSlot);
                            loggingUtilities.Log(this, LogLevel.Debug, "Recursing from module '" + module.GetType().FullName + "' to module '" + linkedOutputSlot.Module.GetType().FullName + "'.");
                            RecurseModule(linkedOutputSlot.Module, graph, removeModuleReferencesAfterRecursing);
                        }
                        else
                        {
                            unlinkedInputSlotAction.Invoke(currentInputSlot);
                        }
                        if (currentInputSlot.DataAssigned == false)
                        {
                            dataUnassignedInputSlotAction.Invoke(currentInputSlot);
                        }
                    }
                }

                // Invoke the main action of the recursion (e.g. processing, etc...)
                recursionAction.Invoke(module);
                recursedModules.Add(module);

                foreach (OutputSlot currentOutputSlot in module.Outputs)
                {
                    outputSlotAction.Invoke(currentOutputSlot);
                }

                postRecursionAction.Invoke(module);

                if (removeModuleReferencesAfterRecursing == true)
                {
                    visitedModules.Remove(module);
                    recursedModules.Remove(module);
                }
            }
        }
    }
}
