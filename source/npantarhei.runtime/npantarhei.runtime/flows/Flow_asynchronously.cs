using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using npantarhei.runtime.data;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.flows
{
	internal class Flow_asynchronously
	{
		private readonly Process_message _processMessage;
		
		public Flow_asynchronously()
		{
			// Build
			var async = new Asynchronize<IMessage>();
		    var throttle = new Throttle_message_flow();
			_processMessage = new Process_message();

            Action<IMessage> enqueue = _ => async.Process(_, throttle.Process);

			// Bind
		    throttle.Continue += _processMessage.Process;
		    _process += enqueue;
		    _processMessage.Message += _ => Message(_);
		    _processMessage.Continue += enqueue;
			_processMessage.Result += _ => Result(_);
		    _processMessage.UnhandledException += _ => UnhandledException(_);

		    _execute += _processMessage.Execute;

			_start += async.Start;
			_stop += async.Stop;
		    _throttle += throttle.Delay;
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{
			_processMessage.Inject(streams, operations);
		}
		
		private readonly Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }

	    private readonly Action<Task> _execute;
        public void Execute(Task task) { _execute(task); }

	    public event Action<IMessage> Message;
		public event Action<IMessage> Result;
	    public event Action<FlowRuntimeException> UnhandledException;
		
		private readonly Action _start;
		public void Start() { _start(); }
		
		private readonly Action _stop;
		public void Stop() { _stop(); }

        private readonly Action<int> _throttle;
        public void Throttle(int delayMilliseconds) { _throttle(delayMilliseconds); }
    }
}

