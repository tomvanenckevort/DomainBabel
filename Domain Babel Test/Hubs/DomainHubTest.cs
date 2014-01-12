using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainBabel.Test.Mocks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleInjector;

namespace DomainBabel.Test
{
    [TestClass]
    public class DomainHubTest
    {
        private DomainHub hub;

        [TestInitialize]
        public void Init()
        {
            this.Init(false);
        }

        private void Init(bool throwException)
        {
            var container = new Container();

            container.Register<ICloudTable, MockCloudTable>();
            container.Register<ITableStorage, AzureTableStorage>();
            container.RegisterSingle<ITranslationContainer>(new MockTranslationContainer(throwException: throwException));
            container.Register<ITranslation, MicrosoftTranslation>();

            container.Verify();

            DependencyResolver.SetResolver(container);

            const string connectionId = "1234";
            const string hubName = "DomainHub";
            var mockConnection = new Mock<IConnection>();
            var mockUser = new Mock<IPrincipal>();
            var mockHubPipelineInvoker = new Mock<IHubPipelineInvoker>();

            var mockRequest = new Mock<IRequest>();
            mockRequest.Setup(r => r.User).Returns(mockUser.Object);

            StateChangeTracker tracker = new StateChangeTracker();

            this.hub = new DomainHub()
            {
                Clients = new HubConnectionContext(mockHubPipelineInvoker.Object, mockConnection.Object, hubName, connectionId, tracker),
                Context = new HubCallerContext(mockRequest.Object, connectionId)
            };

            this.hub.OnConnected();
        }

        [TestMethod]
        public void Disconnect()
        {
            this.hub.OnDisconnected();
        }

        [TestMethod]
        public void Reconnect()
        {
            this.hub.OnReconnected();
        }

        [TestMethod]
        public async Task Search()
        {
            string text = "test.com";

            await this.hub.Search(text);
        }

        [TestMethod]
        public async Task SearchWithoutDomain()
        {
            string text = "test";

            await this.hub.Search(text);
        }

        [TestMethod]
        public async Task SearchTooShort()
        {
            string text = "t.com";

            await this.hub.Search(text);
        }

        [TestMethod]
        public async Task SearchNull()
        {
            string text = null;

            await this.hub.Search(text);
        }

        [TestMethod]
        public async Task SearchDisconnected()
        {
            string text = "test.com";

            await this.hub.OnDisconnected();
            await this.hub.Search(text);
        }

        [TestMethod]
        public async Task SearchEmptyLanguages()
        {
            this.Init(true);

            string text = "test.com";

            await this.hub.Search(text);
        }
    }
}
