using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Metaseed.MetaStudioTest.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
namespace Metaseed.MetaStudioTest
{
    [TestClass]
    public class ValidatableModelTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = new UserInput() { UserName = string.Empty, Email = "44555", RepeatEmail = "sda" };
            await test.ValidateAsync();

            {
                var errors = test.GetErrors("UserName");
                Assert.IsNotNull(errors);
                var e = errors.Cast<object>().ToList()[0];
                Debug.WriteLine(e);
                Assert.IsNotNull(e);
            }

            {
                var errors = test.GetErrors("Email");
                Assert.IsNotNull(errors);
                var e = errors.Cast<object>().ToList()[0];
                Debug.WriteLine(e);
                Assert.IsNotNull(e);
            }

            {
                var errors = test.GetErrors("RepeatEmail");
                Assert.IsNotNull(errors);
                var e = errors.Cast<object>().ToList()[0];
                Debug.WriteLine(e);
                Assert.IsNotNull(e);
            }

        }
    }
}
