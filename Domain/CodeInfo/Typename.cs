using System.Reflection.Metadata;

namespace Domain.CodeInfo
{
    /// <summary>
    /// Represents a typename that a class or method can have
    /// Like having "public class SNode<T, R>", would mean that the class
    /// "SNode" will have 2 Typenames, the T and R
    /// </summary>
    public class Typename
    {
        public string name{ get; set; }
        public Typename(string name)
        {
            this.name = name;
        }
        /// <summary>
        /// Creates a List of type Typename from the typename parameters that came from a class or method
        /// </summary>
        /// <param name="rawTypenameString">The typename parameters to be converted into classes like
        /// "myClass<T, R>", which results into a List of Typenames containing "T, R"</param>
        /// <returns></returns>
        public static List<Typename> GetTypenameList(string rawTypenameString)
        {
            int typenameParamteresStartIndex = rawTypenameString.IndexOf('<');
            if (typenameParamteresStartIndex < 0)
            {
                throw new ArgumentException("The provided string did not contain typename parameters");
            }
            rawTypenameString = rawTypenameString.Substring(typenameParamteresStartIndex, rawTypenameString.Length - typenameParamteresStartIndex);
            rawTypenameString = rawTypenameString.Replace(" ", "").Replace("<", "").Replace(">", "");
            var typenameArray = rawTypenameString.Split(",");
            var typenameList = new List<Typename>();
            foreach (var typename in typenameArray)
            {
                typenameList.Add(new Typename(typename));
            }
            return typenameList;
        }
    }
}
