﻿// =================================================================================================================================
// Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
// =================================================================================================================================

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidField.SolidInstruments.Command;
using RapidField.SolidInstruments.InversionOfControl;
using RapidField.SolidInstruments.Prototype.DatabaseModel;
using RapidField.SolidInstruments.Prototype.DatabaseModel.CommandHandlers;
using RapidField.SolidInstruments.Prototype.DatabaseModel.Commands;
using RapidField.SolidInstruments.TextEncoding;
using System;

namespace RapidField.SolidInstruments.DataAccess.EntityFramework.UnitTests
{
    [TestClass]
    public class EntityFrameworkCommandHandlerTests
    {
        [TestMethod]
        public void FunctionalLifeSpanTest_ShouldProduceDesiredResults()
        {
            // Arrange.
            var package = new SimulatedAutofacDependencyPackage();
            var databaseName = EnhancedReadabilityGuid.New().ToString();
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder.Build();
            var scope = (IDependencyScope)null;
            var commandMediator = (ICommandMediator)null;
            var command = new GetFibonacciNumberValuesCommand();
            var fibonacciNumberSeriesValues = new Int64[] { 0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89 };

            using (var engine = package.CreateEngine(configuration))
            {
                // Arrange.
                scope = engine.Container.CreateScope();
                commandMediator = scope.Resolve<ICommandMediator>();

                using (var context = new PrototypeInMemoryContext(configuration, databaseName).WithTestData())
                {
                    using (var repositoryFactory = new PrototypeRepositoryFactory(configuration, context))
                    {
                        using (var commandHandler = new GetFibonacciNumberValuesCommandHandler(commandMediator, repositoryFactory))
                        {
                            // Act.
                            var result = commandHandler.Process(command);

                            // Assert.
                            result.Should().BeEquivalentTo(fibonacciNumberSeriesValues);
                        }
                    }
                }
            }
        }
    }
}