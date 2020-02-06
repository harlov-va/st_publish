using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using arkAS.BLL;
using System.Web.Mvc;
using arkAS.Areas.harlov.Controllers;
using arkAS.Areas.harlov.BLL;
using System.Linq;
using arkAS.Areas.harlov;
using System.IO;
using Moq;
using System.Web;
using System.Security.Principal;
using Newtonsoft.Json;

namespace arkAS.Tests.Controllers
{
    /// <summary>
    /// Сводное описание для MailControllerTest
    /// </summary>
    [TestClass]
    public class MailControllerTest:Controller
    {
        public MailControllerTest()
        {
            //
            // TODO: добавьте здесь логику конструктора
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты тестирования
        //
        // При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        // ClassInitialize используется для выполнения кода до запуска первого теста в классе
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // TestInitialize используется для выполнения кода перед запуском каждого теста 
        private MailController mailsController;
        private List<h_mails> mailsList;
        private Manager _mng;
        private string msg;
        [TestInitialize()]
        public void MyTestInitialize()
        {

            msg = "";
            _mng = new Manager(new Repository(new LocalSqlServer()));
            var jsonRequest = "{\"page\":1,\"pageSize\":5,\"filter\":{\"text\":\"\"},\"sort\":\",\",\"direction\":\",\",\"mode\":{\"type\":\"\",\"visibleCols\":[]}}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonRequest.ToCharArray());
            var stream = new MemoryStream(bytes);
            var mock = new Mock<IManager>();
            var fakeHttpContext = new Mock<HttpContextBase>();
            fakeHttpContext.Setup(m => m.Request.InputStream).Returns(stream);
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);
            mailsController = new MailController(_mng);
            mailsController.ControllerContext = controllerContext.Object;

            System.Web.HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost", ""),
                new HttpResponse(new StringWriter()));

            System.Web.HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity("roshAdmin@email.com"),
                new string[0]
                );
            //var userMock = new Mock<aspnet_Users>();
            //userMock.Setup(u => u.aspnet_Roles).Returns(new HashSet<aspnet_Roles>());
            
        }
        //
        // TestCleanup используется для выполнения кода после завершения каждого теста
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        #region Mails
        [TestMethod]
        public void MailsList_getItems_Test()
        {
            mailsList = _mng.Mails.GetMails(out msg, _mng.GetUser()).OrderByDescending(p => p.fromSender).Take(5).ToList();
            var total = _mng.Mails.GetMails(out msg, _mng.GetUser()).Count();
            var jsonResponse = JsonConvert.SerializeObject(new
            {
                items = mailsList.Select(x => new
                {
                    id = x.id,
                    uniqueCode = x.uniqueCode,
                    date = x.date.ToShortDateString(),
                    fromSender = x.fromSender,
                    toRecipient = x.toRecipient,
                    description = x.description,
                    trackNumber = x.trackNumber,
                    backTrackNumber = x.backTrackNumber,
                    backDateRecipient = x.backDateRecipient,
                    delSystemsName = x.h_deliverySystems.name,
                    mailStatuses = x.h_mailStatuses.name
                }),
                msg = msg,
                total = total
            });
            //Act
            var result = mailsController.MailsList_getItems() as JsonResult;

            //Assert

            Assert.AreEqual(jsonResponse, result.Data);
        }
        [TestMethod]
        public void MailsList_remove_Test()
        {
            
            var total = _mng.Mails.GetMails(out msg, _mng.GetUser()).Count();
            var jsonResponse = Json(new
            {
                result = true
            });
                //Act
                var result = mailsController.MailsList_remove(total-1) as JsonResult;

            //Assert

            Assert.AreEqual(jsonResponse.Data.ToString(), result.Data.ToString());
        }
        #endregion
        #region DeliverySystems
        [TestMethod]
        public void DeliverySystemsList_remove_Test()
        {

            var total = _mng.Mails.GetDeliverySystems(out msg, _mng.GetUser()).Count();
            var jsonResponse = Json(new
            {
                result = true
            });
            //Act
            var result = mailsController.DeliverySystemsList_remove(total - 1) as JsonResult;

            //Assert

            Assert.AreEqual(jsonResponse.Data.ToString(), result.Data.ToString());
        }
        #endregion
    }
}
