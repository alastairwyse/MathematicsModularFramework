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
    /// A graph structure defining the relationships and computational flow between a set of modules.
    /// </summary>
    public class ModuleGraph
    {
        /// <summary>A collection of all the modules in the graph</summary>
        private HashSet<IModule> modules;
        /// <summary>A dictionary containing all of the links between module's output slots and input slots contained in the graph (indexed by the module that output slot in the link belongs to)</summary>
        private Dictionary<IModule, List<SlotLink>> outputSlotLinks;
        /// <summary>A dictionary containing all of the links between module's output slots and input slots contained in the graph (indexed by the module that the input slot in the link belongs to)</summary>
        private Dictionary<IModule, List<SlotLink>> inputSlotLinks;
        /// <summary>A collection of all the modules which are end points in the graph.</summary>
        //   When parsing the graph these end points are used as the starting point for recursing the graph.
        private HashSet<IModule> endPointModules;

        /// <summary>
        /// An enumerable collection of all the end point modules in the graph.
        /// </summary>
        public IEnumerable<IModule> EndPoints
        {
            get
            {
                return endPointModules;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraph class.
        /// </summary>
        public ModuleGraph()
        {
            modules = new HashSet<IModule>();
            outputSlotLinks = new Dictionary<IModule, List<SlotLink>>();
            inputSlotLinks = new Dictionary<IModule, List<SlotLink>>();
            endPointModules = new HashSet<IModule>();
        }

        /// <summary>
        /// Adds a module to the graph.
        /// </summary>
        /// <param name="module">The module to add.</param>
        /// <exception cref="System.ArgumentException">If the specified module already exists in the graph.</exception>
        public void AddModule(IModule module)
        {
            // Check whether the module already exists in the graph
            if (modules.Contains(module) == true)
            {
                throw new ArgumentException("The specified module '" + module.GetType().FullName + "' already exists in the graph.", "module");
            }

            modules.Add(module);
            // When first added, a module will not have any links to or from its slots, and hence will be an end point
            endPointModules.Add(module);
        }

        /// <summary>
        /// Removes a module from the graph
        /// </summary>
        /// <param name="module">The module to remove.</param>
        /// <exception cref="System.ArgumentException">If the specified module doesn't exist in the graph.</exception>
        /// <exception cref="System.ArgumentException">If the specified module is referenced by slot links in the graph.</exception>
        public void RemoveModule(IModule module)
        {
            // Check whether the module exists in the graph
            if (modules.Contains(module) == false)
            {
                throw new ArgumentException("The specified module '" + module.GetType().FullName + "' does not exist in the graph.", "module");
            }
            // Check that no slot links reference the module
            if(inputSlotLinks.ContainsKey(module) == true)
            {
                throw new ArgumentException("The graph contains slot links referencing the input slot(s) of module '" + module.GetType().FullName + "'.", "module");
            }
            if (outputSlotLinks.ContainsKey(module) == true)
            {
                throw new ArgumentException("The graph contains slot links referencing the output slot(s) of module '" + module.GetType().FullName + "'.", "module");
            }

            endPointModules.Remove(module);
            modules.Remove(module);
        }

        /// <summary>
        /// Creates a slot link or between the specified output slot and input slot.  A slot link represents a path in the module graph between an output slot of one module and an input slot of another module.  After a module is processed the resulting data in the module's output slots is passed to any input slots referenced by slot links.
        /// </summary>
        /// <param name="outputSlot">The output slot in the link.</param>
        /// <param name="inputSlot">The input slot in the link.</param>
        public void CreateSlotLink(OutputSlot outputSlot, InputSlot inputSlot)
        {
            CheckSlotsParentModulesExistInGraph(outputSlot, inputSlot);
            if(outputSlot.Module == inputSlot.Module)
            {
                throw new ArgumentException("Attempt to create a slot link between an output on module '" + outputSlot.Module.GetType().FullName + "' and an input on the same module would create a circular reference.", "inputSlot");
            }
            // Check that the input slot is not already referenced by a slot link
            if (GetSlotLinkFor(inputSlot) != null)
            {
                throw new ArgumentException("Input slot '" + inputSlot.Name + "' on module '" + inputSlot.Module.GetType().FullName + "' is already referenced by a slot link.", "inputSlot");
            }

            // Add the slot link to the graph
            if (outputSlotLinks.ContainsKey(outputSlot.Module) == false)
            {
                outputSlotLinks.Add(outputSlot.Module, new List<SlotLink>());
                // As the module that the output slot belongs to was not in the 'outputSlotLinks' Dictionary, it means it was also previously an end point, however now it must be removed from the set of end points
                endPointModules.Remove(outputSlot.Module);
            }
            // Note below constructor for SlotLink checks that the data types of the output slot and input slot are compatible
            SlotLink newSlotLink = new SlotLink(outputSlot, inputSlot);
            outputSlotLinks[outputSlot.Module].Add(newSlotLink);

            if (inputSlotLinks.ContainsKey(inputSlot.Module) == false)
            {
                inputSlotLinks.Add(inputSlot.Module, new List<SlotLink>());
            }
            inputSlotLinks[inputSlot.Module].Add(newSlotLink);
        }

        /// <summary>
        /// Removes the link between the specified output slot and input slot.
        /// </summary>
        /// <param name="outputSlot">The output slot in the link.</param>
        /// <param name="inputSlot">The input slot in the link.</param>
        public void RemoveSlotLink(OutputSlot outputSlot, InputSlot inputSlot)
        {
            CheckSlotsParentModulesExistInGraph(outputSlot, inputSlot);
            // Check that a link exists between the specified slots
            //   n.b. Might have been nicer to implement the second condition via the Contains<SlotLink> extension method, but would have had to instantiate a SlotLink to do that, and that risks tripping another exception if the slots have different types, and then returning a confusing exception to the client.
            if (outputSlotLinks.ContainsKey(outputSlot.Module) == false || ListContainsSlotLinkBetween(outputSlotLinks[outputSlot.Module], outputSlot, inputSlot) == false)
            {
                throw new ArgumentException("A slot link does not exist between output slot '" + outputSlot.Name + "' on module '" + outputSlot.Module.GetType().FullName + "',  and input slot '" + inputSlot.Name + "' on module '" + inputSlot.Module.GetType().FullName + "'.'", "outputSlot");
            }

            // Remove the link from the graph
            //   TODO: Find a better way to do this than private method.  Was hoping there would be a Remove<> extension method to allow passing in an IEqualityComparer implementation, but does not seem to exist.
            SlotLink slotLinkToRemove = GetSlotLinkBetween(outputSlotLinks[outputSlot.Module], outputSlot, inputSlot);
            outputSlotLinks[outputSlot.Module].Remove(slotLinkToRemove);
            inputSlotLinks[inputSlot.Module].Remove(slotLinkToRemove);


            // If the module that the output slot belongs to has no other slot links, then remove it from the 'outputSlotLinks' Dictionary, and add it back to the set of end points
            if (outputSlotLinks[outputSlot.Module].Count == 0)
            {
                outputSlotLinks.Remove(outputSlot.Module);
                endPointModules.Add(outputSlot.Module);
            }

            // If the module that the input slot belongs to has no other slot links, then remove it from the 'inputSlotLinks' Dictionary
            if (inputSlotLinks[inputSlot.Module].Count == 0)
            {
                inputSlotLinks.Remove(inputSlot.Module);
            }
        }

        /// <summary>
        /// Returns the output slot that is linked to the specified input slot.
        /// </summary>
        /// <param name="inputSlot">The input slot to retrieve the linked output slot for.</param>
        /// <returns>The output slot.</returns>
        /// <exception cref="System.ArgumentException">If the specified input slot has not been linked to an output slot.</exception>
        public OutputSlot GetOutputSlotLinkedToInputSlot(InputSlot inputSlot)
        {
            SlotLink slotLink = GetSlotLinkFor(inputSlot);
            if (slotLink == null)
            {
                throw new ArgumentException("Input slot '" + inputSlot.Name + "' on module '" + inputSlot.Module.GetType().FullName + "' is not referenced by a slot link.", "inputSlot");
            }
            return slotLink.OutputSlot;
        }

        /// <summary>
        /// Checks whether an output slot has been linked to the specified input slot.
        /// </summary>
        /// <param name="inputSlot">The input slot to check.</param>
        /// <returns>Whether an output slot has been linked to the specified input slot.</returns>
        public Boolean OutputSlotIsLinkedToInputSlot(InputSlot inputSlot)
        {
            if (GetSlotLinkFor(inputSlot) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// An enumerable collection of the input slots that are linked to the specified output slot.
        /// </summary>
        /// <param name="outputSlot">The output slot to retrieve the linked input slots for.</param>
        /// <returns>The input slots.</returns>
        public IEnumerable<InputSlot> GetInputSlotsLinkedToOutputSlot(OutputSlot outputSlot)
        {
            List<InputSlot> returnList = new List<InputSlot>();

            if (outputSlotLinks.ContainsKey(outputSlot.Module) == true)
            {
                foreach (SlotLink currentSlotLink in outputSlotLinks[outputSlot.Module])
                {
                    if (currentSlotLink.OutputSlot == outputSlot)
                    {
                        returnList.Add(currentSlotLink.InputSlot);
                    }
                }
            }

            return returnList;
        }

        #region Private Methods

        /// <summary>
        /// Used by methods CreateSlotLink() and RemoveSlotLink() to confirm that the modules that the specified slots belong to exist in the graph.
        /// </summary>
        /// <param name="outputSlot">The output slot.</param>
        /// <param name="inputSlot">The input slot</param>
        private void CheckSlotsParentModulesExistInGraph(OutputSlot outputSlot, InputSlot inputSlot)
        {
            // Check that the module that the output slot belongs to exists in the graph
            if (modules.Contains(outputSlot.Module) == false)
            {
                throw new ArgumentException("Output slot '" + outputSlot.Name + "'s parent module '" + outputSlot.Module.GetType().FullName + "' does not exist in the graph.", "outputSlot");
            }
            // Check that the module that the input slot belongs to exists in the graph
            if (modules.Contains(inputSlot.Module) == false)
            {
                throw new ArgumentException("Input slot '" + inputSlot.Name + "'s parent module '" + inputSlot.Module.GetType().FullName + "' does not exist in the graph.", "inputSlot");
            }
        }

        /// <summary>
        /// Checks whether the specified list of slot links contains a link between the specified output and input slots.
        /// </summary>
        /// <param name="slotLinkList">The list of slot links to check.</param>
        /// <param name="outputSlot">The output slot to check for in the link.</param>
        /// <param name="inputSlot">The input slot to check for in the link.</param>
        /// <returns>Whether the pair of slots exist as a link in the list.</returns>
        private Boolean ListContainsSlotLinkBetween(List<SlotLink> slotLinkList, OutputSlot outputSlot, InputSlot inputSlot)
        {
            Boolean result = false;
            foreach(SlotLink currentSlotLink in slotLinkList)
            {
                if(currentSlotLink.OutputSlot == outputSlot && currentSlotLink.InputSlot == inputSlot)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the slot link which references to the specified input slot.
        /// </summary>
        /// <param name="inputSlot">The input slot to get the slot link for.</param>
        /// <returns>The slot link which references to the input slot, or null if no slot link has been created for the input slot.</returns>
        private SlotLink GetSlotLinkFor(InputSlot inputSlot)
        {
            SlotLink returnSlotLink = null;

            if (inputSlotLinks.ContainsKey(inputSlot.Module) == false)
            {
                return returnSlotLink;
            }

            foreach (SlotLink currentSlotLink in inputSlotLinks[inputSlot.Module])
            {
                if (currentSlotLink.InputSlot == inputSlot)
                {
                    returnSlotLink = currentSlotLink;
                }
            }
            return returnSlotLink;
        }

        /// <summary>
        /// Returns a slot link between the specified output slot and input slot from the specified list.
        /// </summary>
        /// <param name="slotLinkList"></param>
        /// <param name="outputSlot"></param>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">If a slot link between the specified output slot and input slot does not exist in the list.</exception>
        private SlotLink GetSlotLinkBetween(List<SlotLink> slotLinkList, OutputSlot outputSlot, InputSlot inputSlot)
        {
            SlotLink returnSlotLink = null;
            foreach (SlotLink currentSlotLink in slotLinkList)
            {
                if (currentSlotLink.OutputSlot == outputSlot && currentSlotLink.InputSlot == inputSlot)
                {
                    returnSlotLink = currentSlotLink;
                }
            }

            if(returnSlotLink == null)
            {
                throw new ArgumentException("The specified list does not contain a slot link referencing output slot '" + outputSlot.Name + "' and input slot '" + inputSlot.Name + "'.", "slotLinkList");
            }
            else
            {
                return returnSlotLink;
            }
        }

        #endregion
    }
}
