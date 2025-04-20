using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniRater.Interfaces;
using MiniRater.Models;
using MiniRater.Services;
using Moq;

namespace Coterie.UnitTests
{
    [TestFixture]
    public class MiniRaterServiceTests
    {
        private IRateCalculatorService _rateCalculatorService;
        private RateConfig _config;

        [SetUp]
        public void Setup()
        {

           
            var config = new RateConfig
            {
                BasePremium = 1000,
                HazardFactor = 4,
                BusinessFactors = new Dictionary<string, double>
                {
                    { "PLUMBER", 0.5 },
                    { "ARCHITECT", 0.7 },
                    { "PROGRAMMER", 0.4 }
                },
                StateFactors = new Dictionary<string, double>
                {
                    { "TX", 0.943 },
                    { "FL", 1.2 },
                    { "OH", 1.0 }
                },
                Business = new List<string> { "PLUMBER", "ARCHITECT", "PROGRAMMER" },
                States = new List<string> { "TX", "FL", "OH" },
                StateAbbr = new Dictionary<string, string>
                {
                    { "TEXAS", "TX" },
                    { "FLORIDA", "FL" },
                    { "OHIO", "OH" }
                }
            };

            var mockOptions = new Mock<IOptions<RateConfig>>();
            mockOptions.Setup(o => o.Value).Returns(config);

            var mockLogger = new Mock<ILogger<RateCalculatorService>>();

            _rateCalculatorService = new RateCalculatorService(mockOptions.Object, mockLogger.Object);

        }

        [Test]
        public async Task CalculatePremium_WithValidInput_ReturnsExpectedPremiums()
        {
            // Arrange
            var request = new RateCalculatorRequestModel
            {
                Business = "Plumber",
                Revenue = 6000000,
                States = new List<string> { "FL", "OH", "TX" }
            };

            // Act
            var result = await _rateCalculatorService.CalculatePremiumAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.That(result.Premiums.Count, Is.EqualTo(3));
            Assert.IsTrue(result.Premiums.All(p => p.Premium > 0));
        }

        [Test]
        public async Task CalculatePremium_WithVaryStates_ReturnsExpectedPremiums()
        {
            // Arrange
            var request = new RateCalculatorRequestModel
            {
                Business = "Architect",
                Revenue = 600000,
                States = new List<string> { "Florida", "Ohio", "TX" }
            };

            // Act
            var result = await _rateCalculatorService.CalculatePremiumAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.That(result.Premiums.Count, Is.EqualTo(3));
            Assert.IsTrue(result.Premiums.All(p => p.Premium > 0));
        }

        [Test]
        public async Task CalculatePremium_WithMixStates_ReturnsExpectedPremiums()
        {
            // Arrange
            var request = new RateCalculatorRequestModel
            {
                Business = "Architect",
                Revenue = 600000,
                States = new List<string> { "New York","Florida", "Ohio", "TX", "India" }
            };

            // Act
            var result = await _rateCalculatorService.CalculatePremiumAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccessful);
            Assert.That(result.Premiums.Count, Is.EqualTo(3));
            Assert.IsTrue(result.Premiums.All(p => p.Premium > 0));
        }

        [Test]
        public async Task CalculatePremiumAsync_WithInvalidBusiness_ReturnsUnsuccessful()
        {
            // Arrange
            var request = new RateCalculatorRequestModel
            {
                Business = "President",
                Revenue = 500000,
                States = new List<string> { "TX" }
            };

            // Act
            var result = await _rateCalculatorService.CalculatePremiumAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(0, result.Premiums.Count);
        }

        [Test]
        public async Task CalculatePremiumAsync_WithInvalidState_ReturnsUnsuccessful()
        {
            // Arrange
            var request = new RateCalculatorRequestModel
            {
                Business = "PLUMBER",
                Revenue = 500000,
                States = new List<string> { "Nevada" }
            };

            // Act
            var result = await _rateCalculatorService.CalculatePremiumAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(0, result.Premiums.Count);
        }

        [Test]
        public async Task CalculatePremiumAsync_WithInvalidRequest_ReturnsUnsuccessful()
        {
            // Arrange
            var request = new RateCalculatorRequestModel();
            

            // Act
            var result = await _rateCalculatorService.CalculatePremiumAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
           
        }



    }
}

