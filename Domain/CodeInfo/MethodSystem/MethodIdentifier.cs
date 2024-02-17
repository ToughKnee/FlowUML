using System;

namespace Domain.CodeInfo.MethodSystem
{
    public class MethodIdentifier
    {
        public string methodBelongingNamespace;
        public List<string> methodInstanceCandidateNamespaces;
        public string methodOwnerClass;
        public string methodName;
        public List<string> methodParameters;
        public string methodReturnType;

        public MethodIdentifier()
        {
        }

        /// <summary>
        /// This override is aimed to just contemplate the other parts of the 
        /// method EXCEPT for the namespaces, which if there are 2 methods that 
        /// are the same according to this, then we use the Equal method to dissambiguate
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + methodOwnerClass.GetHashCode()
                    * 23 + methodName.GetHashCode();
                foreach (var parameter in methodParameters)
                {
                    hash += parameter.GetHashCode() * 5;
                }
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            MethodIdentifier other = (MethodIdentifier)obj;
            if (methodOwnerClass != other.methodOwnerClass)
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

            // This part is where the namespaces are checked, if the Method(the item stored in the Dictionary) is inside the MethodInstance namespaceCandidates(the object that makes a query to the Dict, "Dict[MethInst]")
            if (other.methodInstanceCandidateNamespaces.Count <= 0)
                return false;

            bool sameMethod = false;
            foreach (string usedNamespace in other.methodInstanceCandidateNamespaces)
            {
                if (methodBelongingNamespace == usedNamespace)
                {
                    sameMethod = true;
                    break;
                }
            }

            return sameMethod;
        }

    }
}
