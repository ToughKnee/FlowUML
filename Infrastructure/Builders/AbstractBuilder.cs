
namespace Infrastructure.Builders
{
    /// <summary>
    /// Interface to be used by classes that need to receive
    /// a certain class that was built by another with a 
    /// concrete builder
    /// </summary>
    public interface AbstractBuilder<T>
    {
        /// <summary>
        /// Returns the class built by the concrete builder
        /// </summary>
        /// <returns></returns>
        public T Build();
    }
}
