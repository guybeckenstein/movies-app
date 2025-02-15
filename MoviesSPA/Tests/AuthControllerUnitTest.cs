using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DTOs;
using Services.Services;
using Services.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Tests
{
    [TestClass]
    class AuthControllerUnitTest
    {
        private readonly IAuthService _authService;
        private readonly UserDto mockUser;
        private readonly string token;

        [TestInitialize]
        public void Setup()
        {
            // TODO
        }

        public AuthControllerUnitTest(IAuthService authService)
        {
            _authService = authService;

            mockUser = new UserDto
            {
                Username = "654321",
                Password = "123456",
            };

            token = "MySuperSecureAndRandomKeyThatLooksJustAwesomeAndNeddsToBeVeryVeryLong!!!111oneeleven";
        }

        [TestMethod]
        public void RegisterServiceTest()
        {
            var resUser = _authService.Register(mockUser);

            Assert.AreEqual(mockUser.Username, resUser.Username);
        }

        [TestMethod]
        public void LoginServiceTest()
        {
            var jwtToken = _authService.Login(mockUser, token);
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

            Console.WriteLine();
        }
    }
}
