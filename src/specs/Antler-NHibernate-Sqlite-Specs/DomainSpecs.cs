﻿// ReSharper disable InconsistentNaming

using System.Reflection;
using FluentAssertions;
using NHibernate;
using NUnit.Framework;
using SmartElk.Antler.Core.Abstractions.Configuration;
using SmartElk.Antler.Core.Domain;
using SmartElk.Antler.Core.Domain.Configuration;
using SmartElk.Antler.NHibernate.Sqlite.Configuration;
using SmartElk.Antler.NHibernate.Sqlite.Specs.Configuration;
using SmartElk.Antler.Specs.Shared.CommonSpecs;
using SmartElk.Antler.Specs.Shared.Entities;
using SmartElk.Antler.Windsor;

namespace SmartElk.Antler.NHibernate.Sqlite.Specs
{
    public class DomainSpecs
    {                
        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_get_one_employee : TestingScenario
        {                                              
           [Test]
           public void should_return_employee()
           {
               CommonDomainSpecs.when_trying_to_get_one_employee.should_return_employee();                  
           }            
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_get_all_teams : TestingScenario
        {           
            [Test]
            public void should_return_all_teams()
            {
                CommonDomainSpecs.when_trying_to_get_all_teams.should_return_all_teams();
            }            
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_get_all_employees : TestingScenario
        {
            [Test]
            public static void should_return_all_employees()
            {
                CommonDomainSpecs.when_trying_to_get_all_employees.should_return_all_employees();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_find_employee_by_name : TestingScenario
        {
            [Test]
            public static void should_return_employee()
            {
                CommonDomainSpecs.when_trying_to_find_employee_by_name.should_return_employee();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_modify_employees_teams : TestingScenario
        {
            [Test]
            public static void should_modify_teams()
            {
                CommonDomainSpecs.when_trying_to_modify_employees_teams.should_modify_teams();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_find_team_by_country_name : TestingScenario
        {
            [Test]
            public void should_return_country()
            {
                CommonDomainSpecs.when_trying_to_find_team_by_country_name.should_return_country();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_delete_team_by_id : TestingScenario
        {
            [Test]
            public void should_delete_team()
            {
                CommonDomainSpecs.when_trying_to_delete_team_by_id.should_delete_team();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_trying_to_query_using_nhibernate_internal_session_directly : TestingScenario
        {
            [Test]
            public void should_return_result()
            {
                UnitOfWork.Do(uow =>
                    {
                        //arrange
                        var country1 = new Country {Name = "USA", Language = "English"};
                        uow.Repo<Country>().Insert(country1);

                        var country2 = new Country {Name = "Mexico", Language = "Spanish"};
                        uow.Repo<Country>().Insert(country2);

                        var team1 = new Team() {Name = "Super", BusinessGroup = "SuperBg", Country = country1};
                        uow.Repo<Team>().Insert(team1);

                        var team2 = new Team() {Name = "Awesome", BusinessGroup = "AwesomeBg", Country = country2};
                        uow.Repo<Team>().Insert(team2);

                        //act                    
                        var internalSession = uow.CurrentSession.GetInternal<ISession>();
                        var result = internalSession.QueryOver<Team>().Where(t => t.Name == "Awesome").List();

                        //assert
                        result.Count.Should().Be(1);
                        result[0].Id.Should().Be(team2.Id);
                        result[0].Name.Should().Be("Awesome");
                        result[0].BusinessGroup.Should().Be("AwesomeBg");
                        result[0].Country.Name.Should().Be("Mexico");
                    });
            }
        }


        #region Configuration
        public class TestingScenario
        {
            protected IAntlerConfigurator Configurator { get; set; }
            protected ConfigurationResult AsInMemoryStorageResult { get; set; }
            private ISession session;

            [SetUp]
            public void SetUp()
            {
                Configurator = new AntlerConfigurator();
                Configurator.UseWindsorContainer().UseStorage(NHibernatePlusSqlite.Use.AsInMemoryStorage().WithMappings(Assembly.GetExecutingAssembly()));

                session = Configurator.CreateNHibernateSession();
            }

            [TearDown]
            public void TearDown()
            {
                Configurator.ResetNHibernateSession(session);
                Configurator.Dispose();
            }
        } 
        #endregion
    }
}

// ReSharper restore InconsistentNaming