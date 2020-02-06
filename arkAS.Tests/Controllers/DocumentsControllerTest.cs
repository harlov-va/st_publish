using System;
using arkAS.Areas.harlov;
using arkAS.Areas.harlov.BLL;
using arkAS.Areas.harlov.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Moq;
using arkAS.BLL;
using System.Collections.Generic;
using System.Web;
using Microsoft.CSharp;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace arkAS.Tests.Controllers
{
    [TestClass]
    public class DocumentsControllerTest : Controller
    {
        #region System
        private DocumentsController controller;
        private ViewResult result;
        private List<h_docTypes> docTypesList;
        private Manager _mng;
        private string msg;
        [TestInitialize]
        public void SetupContext()
        {
            msg = "";
            _mng = new Manager(new Repository(new LocalSqlServer()));
            docTypesList = new List<h_docTypes>();
            docTypesList.Add(new h_docTypes { id = 2, name = "Дополнительное соглашение", code = "Supplementary agreement" });
            docTypesList.Add(new h_docTypes { id = 3, name = "Договор", code = "Contract" });
            docTypesList.Add(new h_docTypes { id = 1, name = "Акт", code = "Act" });
            controller = new DocumentsController(_mng);
            var jsonRequest = "{\"page\":1,\"pageSize\":5,\"filter\":{\"text\":\"\"},\"sort\":\",\",\"direction\":\",\",\"mode\":{\"type\":\"\",\"visibleCols\":[]}}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonRequest.ToCharArray());
            var stream = new MemoryStream(bytes);

            var mock = new Mock<IManager>();
            //mock.Setup(a => a.GetUser()).Returns(new aspnet_Users() { UserName = "roshAdmin@email.com" });
            //mock.Setup(a => a.Documents.GetDocTypes(It.IsAny<aspnet_Users>(), out msg)).Returns(docTypesList);
            //controller = new DocumentsController(mock.Object);
            //fakeHttpContext.Setup(t => t.User.Identity.Name).Returns("roshAdmin@email.com");
            var fakeHttpContext = new Mock<HttpContextBase>();
            fakeHttpContext.Setup(m => m.Request.InputStream).Returns(stream);
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);
            controller.ControllerContext = controllerContext.Object;
            System.Web.HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost", ""),
                new HttpResponse(new StringWriter()));

            System.Web.HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity("roshAdmin@email.com"),
                new string[0]
                );
        }
        #endregion
        #region docTypes
        [TestMethod]
        public void DocTypesList_getItems_test()
        {
            var msg2 = "";
            var total = 3;
            var jsonResponse = JsonConvert.SerializeObject(new
            {
                items = docTypesList.Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    code = x.code
                }),
                msg = msg2,
                total = total
            });
            //Act
            var result = controller.DocTypesList_getItems() as JsonResult;
            //Assert
            //Content(jsonResponse, "application/json")
            Assert.AreEqual(jsonResponse, result.Data);
        }

        [TestMethod]
        public void DocTypesList_test()
        {
            result = controller.DocTypesList() as ViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void DocTypesList_remove_test()
        {
            //var total = _mng.Mails.GetMails(out msg, _mng.GetUser()).Count();
            var jsonResponse = Json(new
            {
                result = true
            });
            //Act
            var result = controller.DocTypesList_remove(3) as JsonResult;

            //Assert

            Assert.AreEqual(jsonResponse.Data.ToString(), result.Data.ToString());
        }
        #endregion
    }
}
