using System;
using System.Collections.Immutable;

namespace Domain.CodeInfo.MethodSystem
{
    public class MethodIdentifier
    {
        public string methodBelongingNamespace;
        public List<string> methodInstanceCandidateNamespaces;
        public List<string> ownerClassNameAndInheritedClasses;
        public string methodName;
        public List<string> methodParameters;
        public string methodReturnType;

        public MethodIdentifier()
        {
        }

        /// <summary>
        /// This override is aimed to just contemplate the other parts of the 
        /// method EXCEPT for the namespaces and owner class name, which if there are 2 methods that 
        /// are the same according to this, then we use the Equal method to dissambiguate
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + methodName.GetHashCode();
                // TODO: When the maxTries reaches a big number, start just comparing methods with only their NAME, NUMBER of parameters AND matching usings statement
                foreach (var parameter in methodParameters)
                {
                    hash += parameter.GetHashCode() * 5;
                }
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            // TODO: When the maxTries reaches a big number, start just comparing methods with only their NAME, NUMBER of parameters AND matching usings statement
            if (obj == null || GetType() != obj.GetType())
                return false;

            // Getting the real Method and the MethodInstance identifiers
            MethodIdentifier other = (MethodIdentifier)obj;

            // Comparing if the owner class name between the Method and MethodInstance match through of polymorphism
            HashSet<string> actualMethodClasses = new HashSet<string>(this.ownerClassNameAndInheritedClasses);
            HashSet<string> methodInstanceClasses = new HashSet<string>(other.ownerClassNameAndInheritedClasses);
            IEnumerable<string> commonElements = actualMethodClasses.Intersect(methodInstanceClasses);
            // If there was no polymorphsim(no classes in the inheritance that matched), then return false
            if (commonElements.Count() == 0)
                return false;

            if (methodName != other.methodName)
                return false;

            if (methodParameters.Count != other.methodParameters.Count)
                return false;

            for (int i = 0; i < methodParameters.Count; i++)
            {
                if (methodParameters[i] != other.methodParameters[i])
                    return false;
            }

            // This part is where the namespaces are checked
            // We dirst check we can compare the namespaces
            if (other.methodInstanceCandidateNamespaces.Count <= 0)
                return false;

            bool sameMethod = false;

            
            // If the Method(the item stored in the Dictionary/the "this") is inside the MethodInstance namespaceCandidates(the object that makes a query to the Dict, "Dict[MethInst]"/ the "other"), then we found a match
            foreach (string usedNamespace in other.methodInstanceCandidateNamespaces)
            {
                if (this.methodBelongingNamespace == usedNamespace)
                {
                    sameMethod = true;
                    break;
                }
            }

            return sameMethod;
        }

    }
}
