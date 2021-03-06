using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeCamp.Core.Data.Entities;
using CodeCamp.Core.Messaging.Messages;
using CodeCamp.Core.ViewModels;
using NUnit.Framework;

namespace CodeCamp.Core.Tests.ViewModelTests
{
    public class SponsorViewModelTests : ViewModelTestsBase
    {
        [Test]
        public async void Init_DataLoadsSuccessfully_LoadsSponsor()
        {
            var sponsor = new Sponsor { Id = 42 };
            var data = new CampData { Sponsors = new List<Sponsor> { sponsor } };
            DataClient.GetDataBody = () => Task.FromResult(data);
            var viewModel = new SponsorViewModel(Messenger, CodeCampService, WebBrowserTask);

            Assert.True(viewModel.IsLoading);

            await viewModel.Init(new SponsorViewModel.NavigationParameters(sponsor.Id));

            Assert.False(viewModel.IsLoading);
            Assert.AreEqual(sponsor, viewModel.Sponsor);
        }

        [Test]
        public async void Init_ExceptionThrown_ReportsError()
        {
            DataClient.GetDataBody = delegate { throw new Exception(); };
            string errorMessage = null;
            Messenger.Subscribe<ErrorMessage>(msg => errorMessage = msg.Message);

            var viewModel = new SponsorViewModel(Messenger, CodeCampService, WebBrowserTask);

            await viewModel.Init(new SponsorViewModel.NavigationParameters(42));

            Assert.NotNull(errorMessage);
            Assert.False(viewModel.IsLoading);
            Assert.Null(viewModel.Sponsor);
        }

        [Test]
        public async void ViewWebsiteCommand_Executed_GoesToSponsorWebsite()
        {
            var sponsor = new Sponsor { Id = 42, Website = "http://codecampnyc.org" };
            var data = new CampData { Sponsors = new List<Sponsor> { sponsor } };
            DataClient.GetDataBody = () => Task.FromResult(data);
            var viewModel = new SponsorViewModel(Messenger, CodeCampService, WebBrowserTask);
            await viewModel.Init(new SponsorViewModel.NavigationParameters(sponsor.Id));

            viewModel.ViewWebsiteCommand.Execute(null);

            Assert.AreEqual(1, WebBrowserTask.UrlRequests.Count);
            Assert.AreEqual(sponsor.Website, WebBrowserTask.UrlRequests.First());
        }
    }
}