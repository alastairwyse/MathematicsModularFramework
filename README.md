MathematicsModularFramework
---
Mathematics Modular Framework (MMF) is a framework which allows simple construction and rearrangement of workflows for finance, mathematics, and machine learning applications.

It allows the packaging of a program unit that performs a mathematical function into a 'module', which has strongly defined and typed inputs and outputs. The outputs of modules can then be linked to the inputs of other modules to form a directed graph (known as a 'module graph') which defines the workflow. The hierarchy implicit in the module graph allows the workflow to be executed automatically in order of dependency when processed.  Workflows can also be serialized to and from XML documents, and hence written to and read from persistent storage (file, database, network, etc...).

MMF was created primarily for financial maths and machine learning, but works with any data types in the .NET framework (including custom classes defined in an application or framework), and hence is suitable for any application where workflows must be flexibly defined at runtime.

##### Links

For detailed information including an explanation of the classes see...<br>
[http://www.alastairwyse.net/mathematicsmodularframework/](http://www.alastairwyse.net/mathematicsmodularframework/index.html)

Sample implementation and use case examples are provided in the [SimpleML](https://github.com/alastairwyse/SimpleML/) project...<br>
[http://www.alastairwyse.net/simpleml/](http://www.alastairwyse.net/simpleml/index.html)

##### Notes
- After opening the solution in Visual Studio, the referenced NuGet packages should be restored using the 'Restore' button in the 'Manage NuGet Packages' window.