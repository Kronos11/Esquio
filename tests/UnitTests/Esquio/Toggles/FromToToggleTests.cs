﻿using Esquio;
using Esquio.Abstractions;
using Esquio.Toggles;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using UnitTests.Builders;
using Xunit;
namespace UnitTests.Esquio.Toggles
{
    public class fromto_toggle_should
    {
        private const string From = nameof(From);
        private const string To = nameof(To);


        [Fact]
        public async Task be_not_active_if_now_is_not_between_configured_dates()
        {
            var toggle = Build
                .Toggle<FromToToggle>()
                .AddParameter(From, DateTime.UtcNow.AddMonths(-1).ToString(FromToToggle.DEFAULT_FORMAT_DATE))
                .AddParameter(To, DateTime.UtcNow.AddMonths(-1).AddDays(10).ToString(FromToToggle.DEFAULT_FORMAT_DATE))
                .Build();

            var feature = Build
                .Feature(Constants.FeatureName)
                .AddOne(toggle)
                .Build();

            var active = await new FromToToggle().IsActiveAsync(
                ToggleExecutionContext.FromToggle(
                    feature.Name,
                    EsquioConstants.DEFAULT_PRODUCT_NAME,
                    EsquioConstants.DEFAULT_DEPLOYMENT_NAME,
                    toggle));

            active.Should().BeFalse();
        }

        [Fact]
        public async Task be_active_if_now_is_between_configured_dates()
        {
            var toggle = Build
                .Toggle<FromToToggle>()
                .AddParameter(From, DateTime.UtcNow.AddMonths(-1).ToString(FromToToggle.DEFAULT_FORMAT_DATE))
                .AddParameter(To, DateTime.UtcNow.AddMonths(1).ToString(FromToToggle.DEFAULT_FORMAT_DATE))
                .Build();

            var feature = Build
                .Feature(Constants.FeatureName)
                .AddOne(toggle)
                .Build();

            var active = await new FromToToggle().IsActiveAsync(
                ToggleExecutionContext.FromToggle(
                    feature.Name,
                    EsquioConstants.DEFAULT_PRODUCT_NAME,
                    EsquioConstants.DEFAULT_DEPLOYMENT_NAME,
                    toggle));

            active.Should().BeTrue();
        }

        [Theory()]
        [InlineData("2001-01-01 0:00:00", "3001-01-01 00:00:00")]
        [InlineData("2001-01-01 1:00:00", "3001-01-01 1:00:00")]
        [InlineData("2001-1-1 00:00:00", "3001-1-1 00:00:00")]
        [InlineData("2001-01-1 00:00:00", "3001-01-1 00:00:00")]
        [InlineData("2001-1-1 00:00:00", "3001-1-01 00:00:00")]
        [InlineData("2001-1-1 1:00:00", "3001-1-01 00:00:00")]
        public async Task be_active_if_now_is_between_dates_with_alternates_configuration(string fromDate, string toDate)
        {
            var toggle = Build
                .Toggle<FromToToggle>()
                .AddParameter(From, fromDate)
                .AddParameter(To, toDate)
                .Build();

            var feature = Build
                .Feature(Constants.FeatureName)
                .AddOne(toggle)
                .Build();

            var active = await new FromToToggle().IsActiveAsync(
                 ToggleExecutionContext.FromToggle(
                     feature.Name,
                     EsquioConstants.DEFAULT_PRODUCT_NAME,
                     EsquioConstants.DEFAULT_DEPLOYMENT_NAME,
                     toggle));

            active.Should().BeTrue();
        }
        [Theory()]
        [InlineData("2001-01-01 0:00:00")]
        [InlineData("2001-01-01 1:00:00")]
        [InlineData("2001-1-1 00:00:00")]
        [InlineData("2001-01-1 00:00:00")]
        [InlineData("2011-01-1 00:00:00")]
        [InlineData("2001-1-1 1:00:00")]
        public async Task be_active_if_now_is_greater_than_from_date(string fromDate)
        {
            var toggle = Build
                .Toggle<FromToToggle>()
                .AddParameter(From, fromDate)
                .Build();

            var feature = Build
                .Feature(Constants.FeatureName)
                .AddOne(toggle)
                .Build();

            var active = await new FromToToggle().IsActiveAsync(
                 ToggleExecutionContext.FromToggle(
                     feature.Name,
                     EsquioConstants.DEFAULT_PRODUCT_NAME,
                     EsquioConstants.DEFAULT_DEPLOYMENT_NAME,
                     toggle));

            active.Should().BeTrue();
        }
    }
}
