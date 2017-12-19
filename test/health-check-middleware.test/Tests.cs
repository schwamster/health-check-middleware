using System;
using HealthCheck;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using FluentAssertions;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void NewOptions_CreatesOptions() 
        {
            var loggerFactory = new Moq.Mock<ILoggerFactory>();
            var healthCheck = new HealthCheckMiddleware(null, loggerFactory.Object, new HealthCheckOptions());
            Assert.NotNull(healthCheck);
        }


        [Fact]
        public async void InvokeTest_GivenDefaultOptions_ReturnsAliveMessage()
        {
            //Arrange

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthcheckEndpoint(new HealthCheckOptions() { });
                });

            var server = new TestServer(builder);

            //Act 
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "/healthcheck");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            //Assert
            var result = responseMessage.Content.ReadAsStringAsync().Result;
            var expected = "{\"message\":\"i am alive!\",\"version\":\"*.*.*\",\"app\":\"testhost\"}";
            result.Should().Match(expected);

        }

        [Fact]
        public async void InvokeTest_GivenOptionsThatAddVersion_ReturnsVersionAndMessage()
        {
            var expected = "{\"message\":\"i am alive!\",\"version\":\"*.*.*\",\"app\":\"SomeApp\"}";
            //Arrange

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthcheckEndpoint(new HealthCheckOptions() { App = "SomeApp"});
                });

            var server = new TestServer(builder);

            //Act 
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "/healthcheck");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            //Assert
            var result = responseMessage.Content.ReadAsStringAsync().Result;

            result.Should().Match(expected);

        }
    }
}
