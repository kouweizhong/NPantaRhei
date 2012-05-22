﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.flows
{
    public abstract class AFlow : AOperation, IFlow
    {
        protected AFlow(string name) : base(name) {}


        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (IsFlowInputMessage(input))
            {
                var output = new Message(string.Format("{0}/{0}.{1}", input.Port.OperationName, input.Port.Name), input.Data)
                                {
                                    Causalities = input.Causalities,
                                    FlowStack = input.FlowStack
                                };

                if (input.Port.Path != "") output.FlowStack.Push(input.Port.Path);

                continueWith(output);
            }
            else
            {
                var parentFlowname = "";
                if (!input.FlowStack.IsEmpty) parentFlowname = input.FlowStack.Pop() + "/";

                var output = new Message(string.Format("{0}{1}.{2}", parentFlowname, input.Port.OperationName, input.Port.Name), input.Data)
                                {
                                    Causalities = input.Causalities,
                                    FlowStack = input.FlowStack
                                };

                continueWith(output);
            }
        }

        private static bool IsFlowInputMessage(IMessage input)
        {
            return input.Port.Path != input.Port.OperationName;
        }


        public virtual IEnumerable<IStream> Streams { get { return BuildStreams(); } }
        public virtual IEnumerable<IOperation> Operations { get { return BuildOperations(new FlowOperationContainer())
                                                                         .Concat(Nested_flow_operations()); } }

        private IEnumerable<IOperation> Nested_flow_operations()
        {
            var flownames = BuildStreams().SelectMany(stream => new[] { stream.FromPort, stream.ToPort })
                                          .Where(port => port.IsQualified)
                                          .Select(port => port.Path.Substring(1))
                                          .Distinct();
            return flownames.Select(flowname => new Flow(flowname));
        }


        protected virtual IEnumerable<IStream> BuildStreams() { return new IStream[] { }; }
        protected virtual IEnumerable<IOperation> BuildOperations(FlowOperationContainer container) { return new IOperation[] { }; }
    }
}
