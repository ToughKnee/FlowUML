namespace Domain.CodeInfo.InstanceDefinitions
{
    /// <summary>
    /// Special class for the instances that are return types from methods
    /// At first it will store the parts of a method(owner class, name and parameters) and 
    /// mark the 'methodIsUnknown' bool as true, thus 
    /// When 'methodIsUnknown' is false, then it means that the owner class of this method defined it
    /// and the MOST important part is that we can know its return type everytime this method is called,
    /// meaning that all the normal instances that are assigned by this method will be known now
    /// </summary>
    public class MethodInstance : AbstractInstance
    {

        // TODO: Make a List of strings which has the namespaces where this callsite was called, to help identify from which namespace came this method if there are multiple classes with the same name
        // TODO: Make this class subscribe to a method which receives a Method, and this will be called everytime a Method instance is built, to fill, BUT we also need to have the instancesDictionary clean this class to be able to compare the received Method first

        /// <summary>
        /// This helps in identifying the namespaces under which this method was called, if 
        /// there are 2 classes with the same name in the entire code but different namespaces
        /// </summary>
        public List<string> candidateNamespaces= new List<string>();
        /// <summary>
        /// This helps in traversing all the MehtodInstances which don't know the 
        /// return type of the method they hold
        /// </summary>
        public static List<MethodInstance> methodInstancesWithUndefinedCallsite = new List<MethodInstance>();

        public override string name => linkedCallsite.calledMethod.name;
        /// <summary>
        /// This makes this MethodInstance know which Callsite it is linked to,
        /// Because the callsite must be generated alongside the instances, and when
        /// the method is unknown, we must define it later and make the callsite created
        /// know the method when it is known, thus we don't need to 
        /// </summary>
        public Callsite? linkedCallsite { get; set; }
        /// <summary>
        /// If this is not false, then this MethodInstance has a Callsite which has the definition 
        /// of a Method that came from a ClassEntity being defined
        /// </summary>
        public bool methodIsUnknown { get; private set; } = true;
        /// <summary>
        /// Alias of the name the class is known by its alias
        /// </summary>
        public string? aliasClassName;
        /// <summary>
        /// The parameters of the method used in the call known by their aliases
        /// </summary>
        public List<string>? aliasParameters { get; private set; } = new List<string>();
        public MethodInstance(Callsite callsite)
        {
            this.linkedCallsite = callsite;
            RegisterToTheMethodInstancesList(this);
        }
        public MethodInstance(Method method)
        {
            this.linkedCallsite = new Callsite(method);
            methodIsUnknown = false;
            RegisterToTheMethodInstancesList(this);
        }
        public static void RegisterToTheMethodInstancesList(MethodInstance methodInstance)
        {
            MethodInstance.methodInstancesWithUndefinedCallsite.Add(methodInstance);
        }

        /// <summary>
        /// This method checks the instancesDIctionary looking for the implementation of the aliases found and 
        /// defined when analysing the code files, in order to be complete and add itself to the methodInstancesWithUndefinedCallsite
        /// so that it is candidate to receive the method definition through subcribing to the instancesManager and finally be complete
        /// Called after the analysis is finished and the instancesManager cleaned the instancesDictionary
        /// </summary>
        public void CheckTypesIninstancesDictionary()
        {

        }
    }
}