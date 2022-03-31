using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using Autofac.Extras.Moq;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Discord;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.UnitTests
{
    public class WorkflowBuilderTests
    {
        [Test]
        public void ShouldSetDefaultMiddlewares()
        {
            //Arrange
            var workflowMock = new Mock<IWorkflow>();
            workflowMock.Setup(x => x.AddMiddleware<IMiddleware>()).Returns(workflowMock.Object);
            var context = new Mock<IComponentContext>();
            var workflowBuilder = WorkflowBuilder.Create(string.Empty, workflowMock.Object, context.Object, useDiscordNetClient: false);

            //Act
            workflowBuilder.SetDefaultMiddlewares();

            //Assert
            workflowMock.Verify(x => x.AddMiddleware<IMiddleware>(), Times.AtLeast(4));
        }

        [Test]
        public void ShouldAddCustomMiddleware()
        {
            //Arrange
            var workflowMock = new Mock<IWorkflow>();
            var context = new Mock<IComponentContext>();
            var workflowBuilder = WorkflowBuilder.Create(string.Empty, workflowMock.Object, context.Object, useDiscordNetClient: false);

            //Act
            workflowBuilder.AddCustomMiddleware<FakeMiddleware>();

            //Assert
            workflowMock.Verify(x => x.AddMiddleware<IMiddleware>(), Times.Once);
        }

        [Test]
        public void ShouldAddOnReadyHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnReadyHandlers(
                builder => builder
                .AddHandler(() => Task.CompletedTask)
                .AddFromIoC<object>(o => () => Task.CompletedTask)),
            assert: workflow => workflow.OnReady.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnUserJoinedHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnUserJoinedHandlers(
                builder => builder
                .AddHandler((a) => Task.CompletedTask)
                .AddFromIoC<object>((o) => (a) => Task.CompletedTask)),
            assert: workflow => workflow.OnUserJoined.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnMessageReceivedHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnMessageReceivedHandlers(
                builder => builder
                .AddHandler((a) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a) => Task.CompletedTask)),
            assert: workflow => workflow.OnMessageReceived.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnDiscordServerAddedBotHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnDiscordServerAddedBotHandlers(
                builder => builder
                .AddHandler((a) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a) => Task.CompletedTask)),
            assert: workflow => workflow.OnDiscordServerAddedBot.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnChannelCreatedHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnChannelCreatedHandlers(
                builder => builder
                .AddHandler((a, b) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a, b) => Task.CompletedTask)),
            assert: workflow => workflow.OnChannelCreated.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnChannelRemovedHandler() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnChannelRemovedHandler(
                builder => builder
                .AddHandler((a, b) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a, b) => Task.CompletedTask)),
            assert: workflow => workflow.OnChannelRemoved.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnRoleUpdatedHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnRoleUpdatedHandlers(
                builder => builder
                .AddHandler((a, b) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a, b) => Task.CompletedTask)),
            assert: workflow => workflow.OnRoleUpdated.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnRoleCreatedHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnRoleCreatedHandlers(
                builder => builder
                .AddHandler((a) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a) => Task.CompletedTask)),
            assert: workflow => workflow.OnRoleCreated.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnRoleRemovedHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnRoleRemovedHandlers(
                builder => builder
                .AddHandler((a) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a) => Task.CompletedTask)),
            assert: workflow => workflow.OnRoleRemoved.Should().HaveCount(2));

        [Test]
        public void ShouldAddOnWorkflowExceptionHandlers() => this.WorkflowHandlerTest(
            act: workflowBuilder => workflowBuilder.AddOnWorkflowExceptionHandlers(
                builder => builder
                .AddHandler((a, b, c) => Task.CompletedTask)
                .AddFromIoC<object>(o => (a, b, c) => Task.CompletedTask)),
            assert: workflow => workflow.OnWorkflowException.Should().HaveCount(2));

        private void WorkflowHandlerTest(Action<WorkflowBuilder> act, Action<IWorkflow> assert)
        {
            //Arrange
            var workflowMock = new Mock<IWorkflow>();
            workflowMock = this.SetupDefaultProperties(workflowMock);
            var workflow = workflowMock.Object;
            using var loose = AutoMock.GetLoose();
            var workflowBuilder = WorkflowBuilder.Create(string.Empty, workflow, loose.Container, useDiscordNetClient: false);

            //Act
            act.Invoke(workflowBuilder);

            //Assert
            assert.Invoke(workflow);
        }

        private Mock<IWorkflow> SetupDefaultProperties(Mock<IWorkflow> workflowMock)
            => workflowMock
            .SetupProperty(x => x.OnReady, new())
            .SetupProperty(x => x.OnUserJoined, new())
            .SetupProperty(x => x.OnDiscordServerAddedBot, new())
            .SetupProperty(x => x.OnChannelCreated, new())
            .SetupProperty(x => x.OnChannelRemoved, new())
            .SetupProperty(x => x.OnRoleUpdated, new())
            .SetupProperty(x => x.OnRoleCreated, new())
            .SetupProperty(x => x.OnRoleRemoved, new())
            .SetupProperty(x => x.OnMessageReceived, new())
            .SetupProperty(x => x.OnWorkflowException, new());

        private class FakeMiddleware : IMiddleware
        {
            public IDiscordContext Process(IMessage data) => throw new NotImplementedException();
        }
    }
}
