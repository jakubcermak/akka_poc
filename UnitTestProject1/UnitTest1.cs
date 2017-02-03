using System;
using System.Threading;
using Akka.Actor;
using Akka.TestKit.VsTest;
using ConsoleApplication1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject1
{
    class TestActor : ReceiveActor
    {
        public int i = 0;

        public TestActor()
        {
            Receive<int>(x =>
            {
                Sender.Tell(x + 1);
                i++;
            });
        }
    }


    [TestClass]
    public class UnitTest1 : TestKit
    {

        const string Config = @"        my-mailbox {
            mailbox-type: ""ConsoleApplication1.FoxyMailbox, ConsoleApplication1""
        }";

        public UnitTest1() : base(Config)
        {
            
        }

        [TestMethod]
        public void TestMethod1()
        {
            var actor = ActorOfAsTestActorRef<TestActor>();
            //var actor = Sys.ActorOf<TestActor>();
            actor.Tell(5);
            var result = ExpectMsg(6);
            Assert.AreEqual(1, actor.UnderlyingActor.i);
        }

        [TestMethod]
        public void TestStatusActor_InProgress()
        {
            var actor = ActorOfAsTestActorRef<StatusActor>();
            var statusMessage = new StatusMessage() {JobId = Guid.NewGuid(), Started = new DateTime(2000,1,1)};
            actor.Tell(statusMessage);
            ExpectNoMsg(1000);
            Assert.IsFalse(actor.UnderlyingActor.statuses[statusMessage.JobId].Completed);
        }
        [TestMethod]
        public void TestStatusActor_Completed()
        {
            var actor = ActorOfAsTestActorRef<StatusActor>();
            var statusMessage = new StatusMessage() {JobId = Guid.NewGuid(), Started = new DateTime(2000,1,1)};
            var statusMessage2 = new StatusMessage() {JobId = statusMessage.JobId, Completed = true};
            actor.Tell(statusMessage);
            ExpectNoMsg(1000);
            actor.Tell(statusMessage2);
            ExpectNoMsg(1000);
            StatusMessage result = actor.UnderlyingActor.statuses[statusMessage.JobId];
            Assert.IsTrue(result.Completed);
            Assert.AreEqual(statusMessage.Started, result.Started);
        }

        //[TestMethod]
        //public void CoordinatorTest()
        //{
        //    var id = Guid.NewGuid();
        //    var creditInfoReportMessage = new CreditInfoReportMessage() {JobId = id};
        //    var actpr = ActorOfAsTestActorRef<CoordinatorActor>();

        //    var childActor = new Mock<IActorRef>();
        //    actpr.UnderlyingActor.creditInfoReportActor = childActor.Object;
        //    actpr.Tell(creditInfoReportMessage);
            
        //   childActor.Verify(x=>x.Tell(creditInfoReportMessage, It.IsAny<IActorRef>()), Times.Once);

        //}

        [TestMethod]
        public void CoordinatorTest()
        {
            var id = Guid.NewGuid();
            var creditInfoReportMessage = new CreditInfoReportMessage() {JobId = id};
            var actpr = ActorOfAsTestActorRef<CoordinatorActor>();

            var childActor = CreateTestProbe(); ;
            actpr.UnderlyingActor.creditInfoReportActor = childActor;
            actpr.Tell(creditInfoReportMessage);

            childActor.ExpectMsg(creditInfoReportMessage);

        }
    }
}
