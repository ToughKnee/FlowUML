﻿using Domain.CodeInfo;
using Infrastructure.Builders;

namespace Infrastructure.Mediators
{
    public class AntlrMediator : IMediator
    {
        public void ReceiveClassEntityBuilder(List<AbstractBuilder<ClassEntity>> builders)
        {
            throw new NotImplementedException();
        }



        public void ReceiveMethodBuilder(List<AbstractBuilder<Method>> builders)
        {
            throw new NotImplementedException();
        }

        public void ReceiveParameters(string type, string identifier)
        {
            throw new NotImplementedException();
        }
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner)
        {
            throw new NotImplementedException();
        }
        public void ReceiveMethodAnalysisEnd()
        {
            throw new NotImplementedException();
        }
    }
}   
