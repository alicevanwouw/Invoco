using Xunit;
using Moq;
using FluentAssertions;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using LacrmIntegration.Application.Services;
using LacrmIntegration.Application.Common;
using Microsoft.Extensions.Logging;

namespace LacrmIntegration.Application.Tests.Services
{
    public class CallEventServiceTests
    {
        private readonly Mock<ICallEventLogStore> _logStoreMock = new();
        private readonly Mock<ILacrmClient> _lacrmClientMock = new();
        private readonly Mock<ILogger<LacrmClient>> _loggerMock = new();

        private CallEventService CreateService() =>
            new(_logStoreMock.Object, _lacrmClientMock.Object, _loggerMock.Object);

        [Fact]
        public async Task HandleCallEventAsync_ShouldReturnConflict_WhenContactExists()
        {
            // Arrange
            var dto = new CallEventDto
            {
                CallerName = "Test",
                CallerTelephoneNumber = "01527306999",
                CallStart = DateTime.Now,
                EventName = "CallLog",
            };

            _lacrmClientMock.Setup(x => x.ContactExistsAsync(dto.CallerTelephoneNumber))
                            .ReturnsAsync(true);

            var service = CreateService();

            // Act
            var result = await service.HandleCallEventAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
            result.Message.Should().Be(CallEventConstants.DuplicateContactMessage);

            _logStoreMock.Verify(x => x.Add(It.IsAny<CallEventLogEntry>()), Times.Once);
        }

        [Fact]
        public void AddNote_ShouldReturnTrue_WhenLogExists()
        {
            // Arrange
            var logId = Guid.NewGuid();
            var log = new CallEventLogEntry
            {
                Id = logId,
                Notes = new List<string>()
            };

            _logStoreMock.Setup(x => x.GetAll()).Returns(new List<CallEventLogEntry> { log });

            var service = CreateService();

            // Act
            var result = service.AddNote(logId, "Test Note");

            // Assert
            result.Should().BeTrue();
            log.Notes.Should().Contain("Test Note");
        }

        [Fact]
        public void AddLog_ShouldCallAddMethodWithCorrectLog()
        {
            // Arrange
            var logId = Guid.NewGuid();
            var log = new CallEventLogEntry
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-30),
                Endpoint = CallEventConstants.ContactsAddEndpoint,
                StatusCode = 200,
                ResponseMessage = "Contact created successfully",
                Notes = new List<string> { "Call from Garry at 2025-04-10 8:30. Number: 0824563546" }
            };

            _logStoreMock.Setup(x => x.GetAll()).Returns(new List<CallEventLogEntry> { log });

            var service = CreateService();

            // Act
            service.Add(log);

            // Assert
            _logStoreMock.Verify(x => x.Add(It.Is<CallEventLogEntry>(entry =>
                   entry.Id == log.Id &&
                   entry.StatusCode == log.StatusCode &&
                   entry.ResponseMessage == log.ResponseMessage &&
                   entry.Notes.SequenceEqual(log.Notes)
               )), Times.Once);
        }
    }
}
