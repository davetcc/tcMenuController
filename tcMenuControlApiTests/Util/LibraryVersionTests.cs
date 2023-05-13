using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tcMenuControlApi.Util;

namespace tcMenuControlApiTests.Util
{
    [TestClass]
    public class LibraryVersionTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void TestLibraryVersion()
        {
            var v1_1_0 = new LibraryVersion(1, 1, 0);
            var v1_0_1 = new LibraryVersion(1, 0, 1);
            var v2_0_0 = new LibraryVersion("2.0.0");

            // test the optional patch version in the string formatting.
            var v2_0 = new LibraryVersion("2.0");
            Assert.AreEqual(v2_0, v2_0_0);

            Assert.AreEqual("1.0.1", v1_0_1.ToString());
            Assert.AreEqual("1.1.0", v1_1_0.ToString());
            Assert.AreEqual("2.0.0", v2_0_0.ToString());

            Assert.IsTrue(v1_0_1.IsSameOrNewerThan(v1_0_1));
            Assert.IsTrue(v1_1_0.IsSameOrNewerThan(v1_0_1));
            Assert.IsTrue(v2_0_0.IsSameOrNewerThan(v1_1_0));
            Assert.IsFalse(v1_0_1.IsSameOrNewerThan(v2_0_0));
            Assert.IsFalse(v1_0_1.IsSameOrNewerThan(v1_1_0));

            Assert.IsTrue(v1_0_1.IsSameOrNewerThan(LibraryVersion.ERROR_VERSION));
            Assert.IsFalse(LibraryVersion.ERROR_VERSION.IsSameOrNewerThan(v2_0_0));
        }
    }
}
