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
        // TODO: Make a MethodInstanceBuilder, and send to the builder the data ALREADY segmented for each component of any methodinstace, this is making properties in here exclusively for "chainedProperties", "bracketIndexer", caller class propertyChain, bwing part of another propertyChain(this would remove the list of MethodCallData sent to the mediator), and others that are relevant components of the MethodInstance
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
