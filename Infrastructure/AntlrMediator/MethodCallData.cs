using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.Builders;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Infrastructure.Mediators
{
    /// <summary>
    /// Data class that contains the necessary data of a Methodcall to be handled by the 
    /// Mediator when the ANTLR Visitor is performing the extraction of information
    /// </summary>
    public record MethodCallData
    {
        public string? calledClassName { get; init; }
        public string calledMethodName { get; init; }
        /// <summary>
        /// This list contains objects because it will hold string and MethodCallData types
        /// </summary>
        public List<object>? calledParameters { get; init; }
        public string? propertyChain { get; init; }
        public MethodBuilder linkedMethodBuilder { get; init; }
        public bool isConstructor { get; init; }

        public MethodCallData(string calledClassName, string calledMethodName, List<object>? calledParameters, string propertyChain, MethodBuilder linkedMethodBuilder, bool isConstructor)
        {
            this.calledClassName = calledClassName;
            this.calledMethodName = calledMethodName;
            this.calledParameters = calledParameters;
            this.propertyChain = propertyChain;
            this.linkedMethodBuilder = linkedMethodBuilder;
            this.isConstructor = isConstructor;
        }

        internal void Deconstruct(out string calledClassName, out string calledMethodName, out List<object> calledParameters, out string propertyChain, out MethodBuilder linkedMethodBuilder, out bool isConstructor)
        {
            calledClassName = this.calledClassName;
            calledMethodName = this.calledMethodName;
            calledParameters = this.calledParameters;
            propertyChain = this.propertyChain;
            linkedMethodBuilder = this.linkedMethodBuilder;
            isConstructor = this.isConstructor;
        }
    }
}   
