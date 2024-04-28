//===========================  
//===========================  「 Feature 」     ※ Mediator between AntlrVisitor and instancesManager
        Responsibilities(+|-):
+Decoupled way to handle the info and then convert the info to instanceManager info
+Mediator is responsible of 'caching' the raw info to be able to link Instances between them
+Can be modularized separating the Concrete Instances creation, making room for more complex logic when creating the Instances
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

//===========================  
//===========================   「 Feature 」     ※ Mediator
        Responsibilities(+|-):
+Is the intermediary step between **extracting raw data** and **making the links between this data**
+Makes instances get linked with other instances that relate to each other to later find their types
+Uses the other Managers that store data to be facilitated to other classes
-Drawbacks

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


//===========================   「 Feature 」     ※ MethodDictionaryInstance
        Responsibilities(+|-):
+Lets the MethodInstances access this dictionary and check with their complete signature the Method instance they needed
-Drawbacks


//===========================   「 Feature 」     ※ MethodIdentifier: Recognizer between MethodInstanceSignature and MethodSignature
        Reason:
-We need to find a match between info that a MethodInstance has and info an actual Method has
-Both objects have the same info, EXCEPT for the returnType and namespaces, which the Method knows well and the MethodInstance doesn't but has a List of possible candidates

        Responsibilities(+|-):
+Special lookup when MethodInstance requests the actual Method to the InstancesManager
-Drawbacks


「 Scenario 」     ※ Linkage with Instance at 'Deconstruct' assignment
-This refers to the case when the ANTLR Visitor found an assignment like 'var (calledClassName, calledMethodName, calledParameters, propertyChainString, linkedMethodBuilder, isConstructor) = callData;', where 'callData' is a record, and we need to assign to EACH identifier inside the parentheses the value defined in the 'Deconstruct' method of the record class
Given:
        -The ANTLR Visitor found the described local variable assignment
When:
        -Mediator is creating the Instances for all the new variables and the MethodInstance
Then:
        -The Mediator must use something that creates and links the Instances with the MethodInstance that will give the type to each Instance to be linked
        -(TODO)Make the MethodInstance have a new class that will take care of the Instances

「 Scenario 」     ※ Type resolution for 'Deconstruct' MethodInstances
Given:
        -ANTLR Visitor finished analysis
        -The MethodInstance knows when it is of type Deconstruct and 
        -The MethodInstance class is linked to each Instance
When:
        -
Then:
        -


「 Feature 」     ※ MethodInstance Lists recognition?????????????????
        Responsibilities(+|-):
+Benefits
-Drawbacks


「 Scenario 」     ※ Found new Collection Instance  
This Instance could be like "var myList = new List<int>()", or "var myDict = new Dictionary<string, int>()"
Given:
        -Antlr Visitor is performing analysis
        -Antlr Visitor finds a Collection Instance declaration
When:
        -Mediator receives from the Antlr Visitor the Collection Instance declaration
Then:
        -Mediator sets the kind of the MethodInstance and sends it to the MethodInstance which manages it

「 Feature 」     ※ Statement handling
        Description:


        Responsibilities(+|-):
+Class representing ifs, while and other statements that use '{}'
++Lets measurement of Ciclomatic Complexity
-Another class to be created and linked by the Mediator


「 Scenario 」     ※ PlayAsSinglePlayer
Given:
        -
When:
        -
Then:
        -















//===========================  
//===========================  「 Feature 」     ※ Instances
        Responsibilities(+|-):
+Represent objects used in the analyzed code like variables and methodCalls
+Lets us identify the objects a Class or Method manage
+Lets us recognize who is the instance that assigned another instance
++For MethodInstances; since Methods are created AFTER the analysis of the code is finished, there is no need for Method declaration to register into an instances dictionary*
-Drawbacks

*Because we needed that when a Method Instance created itself before the Method declaration

「 Scenario 」     ※ Create a new instance from code
PS: This requires the usage of the InstancesDictionary
Given:
        -AntlrVisitor analysis ongoing
        -There are instances in the knownInstancesDict, which may be retrieved
When:
        -We find code which creates an instance(local variable or methodCall)
Then:
        -We check into the instancesDictionary to see if the assigner or components of the methodCall have already been 
        identified(only used to link the new instance with the instance that is assigning a value)
        -If there is no instance that matched the assigner or components of the methodCall, we must add the assigne and assigner, or just the methodCall, to the instancesDictionary, in order to later identify them
