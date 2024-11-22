﻿using AutoFixture;
using FluentAssertions;
using Google.Apis.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;

namespace uniexetask.services.tests.Services
{
    public class ChatGroupServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChatGroupService _sut;
        public ChatGroupServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize<ChatGroup>(composer => composer
            .Without(x => x.Users)
            .Without(x => x.ChatMessages)
            .Without(x => x.Owner)
            .Without(x => x.CreatedByNavigation));
            _fixture.Customize<ChatMessage>(composer => composer
            .Without(x => x.User)
            .Without(x => x.ChatGroup));
            _fixture.Customize<User>(composer => composer
            .Without(x => x.Campus)
            .Without(x => x.ChatGroupCreatedByNavigations)
            .Without(x => x.ChatGroupOwners)
            .Without(x => x.ChatMessages)
            .Without(x => x.Mentors)
            .Without(x => x.NotificationReceivers)
            .Without(x => x.NotificationSenders)
            .Without(x => x.ProjectScores)
            .Without(x => x.RefreshTokens)
            .Without(x => x.Role)
            .Without(x => x.Students)
            .Without(x => x.ChatGroups));

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new ChatGroupService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByChatGroupId_ShouldReturnData_WhenDataFound()
        {
            // Arrange
            var chatGroupMock = _fixture.Create<ChatGroup>();
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId)).ReturnsAsync(chatGroupMock);

            // Act
            var result = await _sut.GetChatGroupByChatGroupId(chatGroupMock.ChatGroupId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ChatGroup> ();
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByChatGroupId_ShouldReturnNull_WhenDataNotFound()
        {
            // Arrange
            ChatGroup? chatGroupMock = null;
            var id = _fixture.Create<int>();
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(id)).ReturnsAsync(chatGroupMock);

            // Act
            var result = await _sut.GetChatGroupByChatGroupId(id);

            // Assert
            result.Should().BeNull();
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetByIDAsync(id), Times.Once());
        }


        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByUserId_ShouldReturnChatGroups_WhenValidUserId()
        {
            // Arrange
            var chatGroupsMock = _fixture.Create<IEnumerable<ChatGroup>>().ToList();
            var usersMock = _fixture.Create<User>();

            usersMock.ChatGroups = chatGroupsMock;

            _unitOfWorkMock.Setup(x => x.Users.GetUserWithChatGroupByUserIdAsyn(usersMock.UserId))
                           .ReturnsAsync(usersMock);

            // Act
            var result = await _sut.GetChatGroupByUserId(usersMock.UserId, 0, 5, "");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeAssignableTo<IEnumerable<ChatGroup>>();
            _unitOfWorkMock.Verify(x => x.Users.GetUserWithChatGroupByUserIdAsyn(usersMock.UserId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByUserId_ShouldReturnNull_WhenInvalidUserId()
        {
            // Arrange
            User? userMock = null;
            var id = _fixture.Create<int>();

            _unitOfWorkMock.Setup(x => x.Users.GetUserWithChatGroupByUserIdAsyn(id))
                           .ReturnsAsync(userMock);

            // Act
            var result = await _sut.GetChatGroupByUserId(id, 0, 5, "");

            // Assert
            result.Should().BeNull();
            _unitOfWorkMock.Verify(x => x.Users.GetUserWithChatGroupByUserIdAsyn(id), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupByUserId_ShouldReturFilledChatGroups_WhenKeywordProvided()
        {
            // Arrange
            var chatGroupsMock = _fixture.Create<IEnumerable<ChatGroup>>().ToList();
            var usersMock = _fixture.Create<User>();

            usersMock.ChatGroups = chatGroupsMock;

            _unitOfWorkMock.Setup(x => x.Users.GetUserWithChatGroupByUserIdAsyn(usersMock.UserId))
                           .ReturnsAsync(usersMock);

            var keyword = chatGroupsMock.First().ChatGroupName;

            // Act
            var result = await _sut.GetChatGroupByUserId(usersMock.UserId, 0, 5, keyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().BeAssignableTo<IEnumerable<ChatGroup>>();
            _unitOfWorkMock.Verify(x => x.Users.GetUserWithChatGroupByUserIdAsyn(usersMock.UserId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupWithUsersByChatGroupId_ShouldReturnData_WhenDataFound()
        {
            // Arrange
            var chatGroupMock = _fixture.Create<ChatGroup>();
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupMock.ChatGroupId)).ReturnsAsync(chatGroupMock);

            // Act
            var result = await _sut.GetChatGroupWithUsersByChatGroupId(chatGroupMock.ChatGroupId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ChatGroup>();
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupMock.ChatGroupId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetChatGroupWithUsersByChatGroupId_ShouldReturnNull_WhenDataNotFound()
        {
            // Arrange
            ChatGroup? chatGroupMock = null;
            var id = _fixture.Create<int>();
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(id)).ReturnsAsync(chatGroupMock);

            // Act
            var result = await _sut.GetChatGroupWithUsersByChatGroupId(id);

            // Assert
            result.Should().BeNull();
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(id), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetLatestMessageInChatGroup_ShouldReturnData_WhenDataFound()
        {
            // Arrange
            var chatMessageMock = _fixture.Create<ChatMessage>();
            _unitOfWorkMock.Setup(x => x.ChatMessages.GetLatestMessageInChatGroup(chatMessageMock.ChatGroupId)).ReturnsAsync(chatMessageMock);

            // Act
            var result = await _sut.GetLatestMessageInChatGroup(chatMessageMock.ChatGroupId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ChatMessage>();
            _unitOfWorkMock.Verify(x => x.ChatMessages.GetLatestMessageInChatGroup(chatMessageMock.ChatGroupId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetLatestMessageInChatGroup_ShouldReturnNull_WhenDataNotFound()
        {
            // Arrange
            ChatMessage? chatMessageMock = null;
            var id = _fixture.Create<int>();
            _unitOfWorkMock.Setup(x => x.ChatMessages.GetLatestMessageInChatGroup(id)).ReturnsAsync(chatMessageMock);

            // Act
            var result = await _sut.GetLatestMessageInChatGroup(id);

            // Assert
            result.Should().BeNull();
            _unitOfWorkMock.Verify(x => x.ChatMessages.GetLatestMessageInChatGroup(id), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMessagesInChatGroup_ShouldReturnData_WhenMessagesExist()
        {
            // Arrange
            var chatGroupMock = _fixture.Create<ChatGroup>();
             _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId))
                .ReturnsAsync(chatGroupMock);

            var chatMessagesMock = _fixture.Create<IEnumerable<ChatMessage>>();
            _unitOfWorkMock.Setup(x => x.ChatMessages.GetMessagesInChatGroup(chatGroupMock.ChatGroupId, 0, 5))
                .ReturnsAsync(chatMessagesMock);

            // Act
            var result = await _sut.GetMessagesInChatGroup(chatGroupMock.ChatGroupId, 0, 5);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            _unitOfWorkMock.Verify(x => x.ChatMessages.GetMessagesInChatGroup(chatGroupMock.ChatGroupId, 0, 5), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMessagesInChatGroup_ShouldReturnNull_WhenMessagesNotFound()
        {
            // Arrange
            var chatGroupMock = _fixture.Create<ChatGroup>();
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId))
               .ReturnsAsync(chatGroupMock);

            IEnumerable<ChatMessage>? chatMessagesMock = null;
            _unitOfWorkMock.Setup(x => x.ChatMessages.GetMessagesInChatGroup(chatGroupMock.ChatGroupId, 0, 5))
                           .ReturnsAsync(chatMessagesMock);

            // Act
            var result = await _sut.GetMessagesInChatGroup(chatGroupMock.ChatGroupId, 0, 5);

            // Assert
            result.Should().BeNull();
            _unitOfWorkMock.Verify(x => x.ChatMessages.GetMessagesInChatGroup(chatGroupMock.ChatGroupId, 0, 5), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMessagesInChatGroup_ShouldReturnException_WhenChatGroupNotFound()
        {
            // Arrange
            var chatGroupId = _fixture.Create<int>();
            ChatGroup? chatGroupMock = null;
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupId))
                           .ReturnsAsync(chatGroupMock);

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _sut.GetMessagesInChatGroup(chatGroupId, 0, 5);

            // Assert
            act.Should().NotBeNull();
            await act.Should().ThrowAsync<Exception>().WithMessage("Chat Group not found");
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetByIDAsync(chatGroupId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task SaveMessageAsync_ShouldReturnChatMessage_WhenUpdateSuccess_WithChatGroupFound()
        {
            // Arrange
            var chatGroupMock = _fixture.Create<ChatGroup>();
            var chatMessageMock = _fixture.Create<ChatMessage>();
            chatMessageMock.ChatGroupId = chatGroupMock.ChatGroupId;

            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId)).ReturnsAsync(chatGroupMock);

            _unitOfWorkMock.Setup(x => x.ChatGroups.Update(It.IsAny<ChatGroup>())).Verifiable();

            _unitOfWorkMock.Setup(x => x.ChatMessages.InsertAsync(It.IsAny<ChatMessage>())).Returns(System.Threading.Tasks.Task.CompletedTask).Verifiable();

            _unitOfWorkMock.Setup(x => x.Save()).Verifiable();
            _unitOfWorkMock.Setup(x => x.Commit()).Verifiable();

            // Act
            var result = await _sut.SaveMessageAsync(chatMessageMock.ChatGroupId, chatMessageMock.UserId, chatMessageMock.MessageContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ChatMessage>();
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId), Times.Once());
            _unitOfWorkMock.Verify(x => x.ChatGroups.Update(It.IsAny<ChatGroup>()), Times.Once());
            _unitOfWorkMock.Verify(x => x.ChatMessages.InsertAsync(It.IsAny<ChatMessage>()), Times.Once());
            _unitOfWorkMock.Verify(x => x.Save(), Times.Once());
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task SaveMessageAsync_ShouldReturnException_WhenUpdateFail_WithChatGroupFound()
        {
            // Arrange
            var chatGroupMock = _fixture.Create<ChatGroup>();
            var chatMessageMock = _fixture.Create<ChatMessage>();
            chatMessageMock.ChatGroupId = chatGroupMock.ChatGroupId;

            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId)).ReturnsAsync(chatGroupMock);

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _sut.SaveMessageAsync(chatMessageMock.ChatGroupId, chatMessageMock.UserId, chatMessageMock.MessageContent);

            // Assert
            act.Should().NotBeNull();
            await act.Should().ThrowAsync<Exception>();
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetByIDAsync(chatGroupMock.ChatGroupId), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task SaveMessageAsync_ShouldReturnException_WhenChatGroupNotFound()
        {
            // Arrange
            var chatGroupId = _fixture.Create<int>();
            var chatMessageMock = _fixture.Create<ChatMessage>();
            ChatGroup? chatGroupMock = null;
            _unitOfWorkMock.Setup(x => x.ChatGroups.GetByIDAsync(chatGroupId))
                           .ReturnsAsync(chatGroupMock);

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _sut.SaveMessageAsync(chatGroupId, chatMessageMock.UserId, chatMessageMock.MessageContent);

            // Assert
            act.Should().NotBeNull();
            await act.Should().ThrowAsync<Exception>().WithMessage("Chat Group not found");
            _unitOfWorkMock.Verify(x => x.ChatGroups.GetByIDAsync(chatGroupId), Times.Once());
        }
    }
}