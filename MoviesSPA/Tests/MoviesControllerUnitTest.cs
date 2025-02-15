using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DTOs;
using Models.Models;
using Services.Services.Interfaces;

namespace Tests
{
    [TestClass]
    public class MoviesControllerUnitTest
    {
        private IExternalApiService _externalApiService;
        private string _moviesUrl;

        [TestInitialize]
        public void Setup()
        {
            // TODO
        }

        public MoviesControllerUnitTest(IExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
            _moviesUrl = "https://gist.githubusercontent.com/saniyusuf/406b843afdfb9c6a86e25753fe2761f4/raw/523c324c7fcc36efab8224f9ebb7556c09b69a14/Film.JSON";
        }

        [TestMethod]
        public async Task ExternalUrlTest()
        {
            // Testing the external URL get request, validating it's service is returning status code 200
            var response = await $"{_moviesUrl}"
                .GetAsync();

            Assert.AreEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public async Task GetMoviesServiceTest()
        {
            var res = await _externalApiService.GetMovies(_moviesUrl);

            Assert.AreEqual(res.GetType(), typeof(List<MovieResponse>));
            Assert.AreNotEqual(res.GetType(), typeof(List<MovieRequest>));
            Assert.IsTrue(res.Count > 0);
        }
    }
}
