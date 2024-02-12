Problem: We need to put inside the instancesDictionary this method's delcaration, but we can't create a Method since that responsability is exclusive to the METHODBUILDER
We don't need callsite, but we NEED the Method class since the other MethodInstances need the complete Method
THEN, we actually don't need this method, because the goal of handling the declaration of a method is to make the MethodInstances receive the complete Method in order to:
1. Get the return type and rename the aliases with the return type AND
2. Put the Method inside the Callsite which was incomplete
With that in mind, we then need just a way to SEND to those MethodInstaces the Method when it's created, and the easiest way to do that is to have a list of all the MethodInstances with their unknown Method, and everytime a Method is created, make them receive the Method and CHECK if their parts of a method is the same as the Method received, and all is done
PS: Since the major operation we need to do is to COMPARE a MethodInstance with a Method, we can make a List of MethodInstances that request to be compared, and then a Dictionary of Methods, where the scenario is...




//===========================  
//===========================  「 Feature 」     ※ Mediator between AntlrVisitor and instancesManager
        Responsibilities(+|-):
+Decoupled way to handle the info and then convert the info to instanceManager info
-Drawbacks

//===========================  MAKE TEST
//===========================  MAKE TEST
//===========================  MAKE TEST
//===========================  MAKE TEST
「 Scenario 」     ※ Receive info to create Instances
Actors:
  -Instance:
    The instances are generated from properties, methodCalls(which will be registered inside the instancesDictionary and need special id), parameters and local variables(which WON'T be registered into the instancesDictionary and DON'T need special id)
    The instance will be equal to another instance in 2 cases:
    1. If the property 'inheritanceList' has one element, then they should have the same name
    2. If true, we ignore namespaces and check if they have at least one value in their inheritanceList that matches another value in the other instance
Given:
        -Analysis of AntlrVisitor is ongoing
        -Mediator is also doing other things
        -Properties and MethodCalls(including each component of this methodCallInstance, class name and properties) get their inheritanceList filled if the current class analyzed inherits from other classes or interfaces
        -Properties, Parameters and Local variables are stored inside the mediator to link them with other instances assigned by them
When:
        -Mediator has received the current namespace, className alongside the inheritance classes and methodName the AntlrVisitor is analyzing
        -Mediator receives info for instances which could be a local variable, property, parameter or methodCall
Then:
        -Make the identifier for the received instance
        -Put the instance with the identifier into the instancesManager through the corresponding method depending of the type of instance to be sent
        -New Instances that are parts of a methodCall or assignments of local variables by properties, params or local variables MUST be linked to them and share their type property
          -If we can't connect this new instance with another instance, we must make data to recognize at a later time the instance that should have been defined AND store it into the instancesDictionary
        -IF the class has inheritance
         ALL methodCalss AND properties must have contents in their inheritanceList containing the parents of the current class analyzed AND ALSO the grandparents
         The components of the methodCallInstance MUST share their inheritanceList with the MethodInstance they are diretly linked to


「 Scenario 」     ※ Instance is equal to another instance
Given:
        -The instances have inheritance
        -Instances are:
        1. namespace
When:
        -
Then:
        -




//===========================  
//===========================  「 Feature 」     ※ InstancesManager
        Responsibilities(+|-):
+Decoupled way to handle the info and then convert the info to instanceManager info
-Drawbacks


「 Scenario 」     ※ Assign Method to almost complete MethodInstances
Given:
        -The analysis of the files has been completed
        -ALL methodBuilders built all the Methods
        -The Methods built registered themselves in a Dictionary<string, Method> where the key is the method identification for the instancesManager(classOwner, methodName, params) and the value is the Method itself
        -The instancesManager cleaned the instancesDictionary from aliases at least once

When:
        -There are MethodInstances that are clear from aliases and just need the actual Method
        -These MethodInstances register themselves to the List which contains MethodInstances that only need the actual Method
Then:
        -The instancesManager goes through all the list of almost complete MethodInstances and makes them compare their method identifiers with the Dictionary<string, Method>
        -If there is a match, use the Method to make the callsite and set return type in the instancesDictionary, and remove itself from the List of almost complete MethodInstances
        -If not do nothing 



//===========================  
//===========================  「 Feature 」     ※ Instances
        Responsibilities(+|-):
+Represent objects used in the analyzed code like variables and methodCalls
+Lets us identify the objects a Class or Method manage
+Lets us recognize who is the instance that assigned another instance
++For MethodInstances; since Methods are created AFTER the analysis of the code is finished, there is no need for Method declaration to register into the instancesDictionary*
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


「 Scenario 」     ※ Get the last assigner of an instance
This aims to check when an instance has been assigned a value by another instance, like "team = manager.CreateTeam()", and "manager = CreateManager()"
Given:
        -AntlrVisitor finished analysing the code
        -instancesDictionary has been filled
        -
When:
        -We need to clean aliases
Then:
        -



namespace.(class1,class2,class3).property : int
myNum : namespace2.(class0, class1).property