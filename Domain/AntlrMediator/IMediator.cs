using Domain.CodeInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Mediators
{
    /// <summary>
    /// Interface to connect the Visitor from ANTlR to other domain classes that need info from 
    /// the code and use it
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Receives the information from the visitor visiting a method rule and deliver it to the 
        /// classes that will need it
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public void ReceiveMethodInfo(string returnType, string name, List<string> parameters);
    }
}
