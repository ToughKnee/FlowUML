//===========================  
//===========================  「 Feature 」     ※ Mediator between AntlrVisitor and instancesManager
        Responsibilities(+|-):
+Decoupled way to handle the info and then convert the info to instanceManager info
+Mediator is responsible of 'caching' the raw info to be able to link Instances between them(like a MethodInstance containing another Instance)
+Can be modularized separating the Concrete Instances creation, making room for more complex logic when creating the Instances
+Is the intermediary step between **extracting raw data** and **making the links between this data**
+Makes instances get linked with other instances that relate to each other to later find their types
+Stores data into the other DictionaryManagers to then be used by other classes
-Drawbacks

「 Scenario 」     ※ Receive info to create Instances
Given:
        -Analysis of AntlrVisitor is ongoing
        -Mediator receives all the raw info from the ANTLR Visitor of the code 
        -Mediator has to handle this info and create the respective Instance classes from that(this needs to have special logic to also handle cases like inheritance)
        --The Mediator has to link with the respective Instances the Properties, Parameters and Local variables if they are related somehow
When:
        -Mediator has received the current namespace, className alongside the inherited classes and methodName the AntlrVisitor is analyzing
        -Mediator receives info for instances which could be a local variable, property, parameter or methodCall
Then:
        -New Instances that are parts of a methodCall or assignments of local variables by properties, params or local variables MUST be linked to them and share their type property
                -If we can't connect this new instance with another ALREADY existing instance, we must make data to recognize at a later time the instance that should have been defined(NOT implemented)
        -IF the class has inheritance AND the Instance to be made requires knowing the inheritance of the class to know its type(like an Instance named the "this" keyword) then ALL methodCalss AND properties must have contents in their inheritanceList containing the parents of the current class analyzed AND ALSO the grandparents
                -The components of the methodCallInstance MUST share their inheritanceList with the MethodInstance they are directly linked to
        -inheritanceDictionary is filled by the Mediator when processing the raw data

「 Scenario 」     ※ EXTENSION::: Receive info to create Instances:Parent method called
Given:
        -We are in the same context and kind of scenario as in the previous scenario
When:
        -There is a MethodInstance that is a call to a PARENT method, which was inherited
        -The calls was performed using the "this." prefix or just the name of the method
Then:
        -The MethodInstance to be created sets a boolean as "true", which represents that it needs the class that owns the method it has AND it matches one of the string in the inheritanceList
        -When we call the MethodInstance, the method that checks the types of the aliases on the instances dictioanry(CheckTypesOfAliases), has to do something before doing anything else, which is a comparison of the owner class of each method and see if any matches an element from the inheritanceList


「 Scenario 」     ※ Resolution of type of components of the MethodInstances
Given:
        -Analysis of all code files FINISHED
        -InheritanceDictionary completed
        -Method and ClassEntity Builders ready to build their classes from the analyzed code files
        -Filled the list of MethodInstances with unknown type complete and wich will be traversed in this scenario
        --The Builders(ClassEntity and Method) are complete and ready to create the classes of their correspondent type
                These builders are crucial for this scenario
When:
        -We have finished the analysis of code files and have all the Method, ClassEntity and MethodInstance Builders
        -Start building the ClassEntities(which also build the Methods they belong to)
        -Also start building the MethodInstances
        -After building the MethodInstances, **start defining their types**
                -This means that the Mediator starts using the **extracted raw data** to start **creating and linking Instances** to each other for them to later define themselves 
                -(Storing the true Instance when there are many variable aliases) When there is a case like "var myClassFromCollectionHard = otherMyth.myList[0].myProperty;", and a MethodInstance is received with "myClassFromCollectionHard.normalThing()", then the Mediator must make a Dict with <string, Instance> where "myClassFromCollectionHard" results in "otherMyth.myList[0].myProperty" and handed to that MethodInstance
        -We need the Callsites to be completed so that we can start making Diagrams
Then:
        -ClassEntityBuilders use the inheritance manager to link the ClassEntities with their inherited classes
        -Make the MethodBuilders store their built Method to the MethodDictionaryManager, which will be used for the resolution of MethodInstances
        ---(BIG STEP)Pass through all the List of MethodInstances with unknown type and call the "SolveTypesOfComponents()" method, which will solve the type of the components of this MethodInstance(owner class name, parameters and all the property chains) and then query the methodDictionary for the actual Method represented by the MethodInstance
                -If the methodInstance found a match, then set the respective data and remove itself from the list of MethodInstances with undefined callsite
        -Keep going until a max number of attempts since there may be methodCalls that can't be tracked
Result:
        -The ClassEntityManager has all the ClassEntities with the respective Methods which contain their Callsites defined to all the parts of the code thanks to the MethodInstances and we have finished the code analysis

//===========================   「 Feature 」     ※ MethodIdentifier: Recognizer between MethodInstanceSignature and MethodSignature
        Reason:
-We need to find a match between info that a MethodInstance has and info an actual Method has
-Both objects have the same info, EXCEPT for the returnType and namespaces, which the Method knows well and the MethodInstance doesn't but has a List of possible candidates

        Responsibilities(+|-):
+Special lookup when MethodInstance requests the actual Method to the InstancesManager using the `GetHash` and `Equals` methods
-Drawbacks


//===========================  
//===========================  「 Feature 」     ※ Statement handling
        Description:


        Responsibilities(+|-):
+Class representing ifs, while and other statements that use '{}'
++Lets measurement of Ciclomatic Complexity
-Another class to be created and linked by the Mediator


「 Scenario 」     ※ Process block statement
Given:
        -During code analysis and Instances building
When:
        -ANTLR Visitor finds a block statement(code inside curly braces like if statements)
Then:
        -

//===========================  
//===========================  「 Feature 」     ※ Remark Class' property **usage** in Methods
        Description:
Whenever there is an "advancedIdentifier" in a node in the ANTLR Visitor, then there is an instantiation of a class in the code being used, and we will register this info in the Method class to be able to, aside of marking the method calls to other methods, also be able to remark the usage of properties and **stateness** in the Method, and how much influence of the state of the properties impacts the Method

        Responsibilities(+|-):
+Measure the **"statefulness"** of this method to other components
        Measure that through checking where does this function is **WRITING TO**, which clearly means changes in state, rather than being purely functional
++APPLY GROUPING OF RELATED CLASSES and such, in a way that the groupings are made accoring to a criteria(like their share of common types of properties, or even their OBJECTIVES to accomplish, kinda like structural security)
+++Look how much data IS WRITTEN to other components to mark the **Influence** of this Method to other parts of the code(This will also enable **implicit changes**, when a method uses another method, where all the WRITES that other method has are trasnfered to the original method automatically)
++Look how much data IS READ from other components and mark the **Susceptibility** of this Method to changes from other places
+Be able to know when there are 2 methods connected by accesing data, and remarking if we are adding complexity if both methods read AND WRITE, and it would be better to just make one READ and the other WRITE
+Be able to mark special data **if they are used in if statements(or similar)**, remarking how important the data is and its influence on other parts of other methods
+Able to say **which method calls DEFINE this data**, which help know how 
-Create a new class that must manage all of this
-Make the Instance class send that information whenever its **type is defined**


「 Scenario 」     ※ ANTLR Visitor visits an "advancedIdentifier" node
Given:
        -Code analysis by the visitor is ongoing
        -We must send info of instantiated classes where the Method is using **properties**
        -We also are sending to the Mediator info of MethodCalls, which contain also **usage of properties**
When:
        -There is a visit to an "advancedIdentifier" node
Then:
        -We process the instance by converting it into an **InstanceBuilder**
        -Send the Instance to the mediator

「 Scenario 」     ※ Mediator receives a class' property
Given:
        -The previous scenario happened
        -There are methodCall converted into MethodInstances in the same Mediator(which means there are properties of instantiated classes already being **defined**)
When:
        -We avoid visiting "advancedIdentifier" nodes inside MethodCalls(to ensure we are not wasting processing in properties already identified)
        -The Mediator receives an InstanceBuilder from the Visitor 
Then:
        -Find the type of the instance received
        -Get the owner of the property















//===========================  
//===========================  「 Feature 」     ※ Instances
        Responsibilities(+|-):
+Represent objects used in the analyzed code like variables and methodCalls
+Lets us identify the objects a Class or Method manage
+Lets us recognize who is the instance that assigned another instance
++For MethodInstances; since Methods are created AFTER the analysis of the code is finished, there is no need for Method declaration to register into an instances dictionary*
-Drawbacks

*Because we needed that when a Method Instance created itself before the Method declaration

//===========================  
//===========================  「 Feature 」     ※ InstanceBuilder
        Responsibilities(+|-):
++Be responsible for the creation of Instances and setting their data accordingly, **based on the data received from the ANTLR Visitor and the data from the Mediator**
-Drawbacks

*Because we needed that when a Method Instance created itself before the Method declaration

「 Scenario 」     ※ Variable declaration
Given:
        -AntlrVisitor analysis ongoing
When:
        -The visitor finds a local variable definition statement
        -There is a type in the statement(if there is no type node in the variable definition, then exit)
Then:
        -We check into the instancesDictionary to see if the assigner or components of the methodCall have already been 
        identified(only used to link the new instance with the instance that is assigning a value)
        -If there is no instance that matched the assigner or components of the methodCall, we must add the assigne and assigner, or just the methodCall, to the instancesDictionary, in order to later identify them
        -We check if the type is present(like "MyType Instance;") and set the type if so


//===========================  
//===========================  「 Feature 」     ※ Special rule in methodCall for expression inside parentheses
        Responsibilities(+|-):
+Able to keep track of multiple special method inside parentheses
-Major overhead

「 Scenario 」     ※ Process "specialExpressionInParentheses"
Given:
        -ANTLR analysis ongoing
        -Mediator building method instances
When:
        -ANTLR Visitor found a "specialExpressionInParentheses" from ANY PART of the code(local var declaration, in method argument list, etc)
        -This expression may contain a **variable** or **method call** with an optional expressionChain
Then(if we found a method call):
        -Get the methodCall inside the parentheses and then get the methodCall outside of them ensuring they are treated just like a methodCall being the class caller of another method call would be
Then(if we found a variable):
        -Get the expression inside parentheses and mix it with the expression outside of the parenthesese connected by the '.'
Then(if we found another kind of expression):
        -Raise an error since that shouldn't be happening


