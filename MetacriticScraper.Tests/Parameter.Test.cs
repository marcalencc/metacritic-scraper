using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class ParameterTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Test_Valid_Parameter_Value()
        {
            string parameter = "param1=value1&param2=value2&param3=value3";
            ParameterData parameterData = new ParameterData(parameter);
            string value2 = parameterData.GetParameterValue("param2");
            string value3 = parameterData.GetParameterValue("param3");

            Assert.AreEqual(value2, "value2");
            Assert.AreEqual(value3, "value3");
        }

        [Test]
        public void Test_Invalid_Parameter_Value()
        {
            string parameter = "param1=value1&param2=value2&param3=value3";
            ParameterData parameterData = new ParameterData(parameter);
            string value4 = parameterData.GetParameterValue("param4");

            Assert.AreEqual(value4, "");
        }
    }
}
