using System;
using System.Collections.Generic;

using NUnit.Framework;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.data;

namespace npantarhei.runtime.tests
{
	[TestFixture()]
	public class test_NotifyingSingleQueue
	{
		[Test()]
		public void Enqueue()
		{
			var messages = new PriorityQueue<string>();
			var sut = new NotifyingSingleQueue<string>(messages);
			
			sut.Enqueue(0, "x");
			
			Assert.AreSame("x", messages.Dequeue());
			Assert.AreEqual(0, messages.Count);
		}
		
		[Test]
		public void TryDequeue()
		{
			var messages = new PriorityQueue<string>();
			var sut = new NotifyingSingleQueue<string>(messages);
			
			messages.Enqueue(0, "x");
			
			string result = null;
			Assert.IsTrue(sut.TryDequeue(out result));
			Assert.AreSame("x", result);
			Assert.AreEqual(0, messages.Count);	
		}
		
		[Test]
		public void Wait_successful_after_enqueue()
		{
			var messages = new PriorityQueue<string>();
			var sut = new NotifyingSingleQueue<string>(messages);
			
			sut.Enqueue(0, "x");
			
			Assert.IsTrue(sut.Wait(500));
		}
		
		[Test]
		public void Wait_fails_after_dequeue()
		{
			var messages = new PriorityQueue<string>();
			var sut = new NotifyingSingleQueue<string>(messages);
			
			sut.Enqueue(0, "x");
			string msg;
			sut.TryDequeue(out msg);
			
			Assert.IsFalse(sut.Wait(500));
		}
	}
}

