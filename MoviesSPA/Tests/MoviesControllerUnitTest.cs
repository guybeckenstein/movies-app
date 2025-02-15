using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MoviesControllerUnitTest
    {
        [TestMethod]
        public async Task ExternalApi()
        {
            // Testing the external URL get request, validating it's service is returning status code 200
            var response = await "https://gist.githubusercontent.com/saniyusuf/406b843afdfb9c6a86e25753fe2761f4/raw/523c324c7fcc36efab8224f9ebb7556c09b69a14/Film.JSON"
                .GetAsync();

            Assert.AreEqual(response.StatusCode, 200);
        }
    }
}
