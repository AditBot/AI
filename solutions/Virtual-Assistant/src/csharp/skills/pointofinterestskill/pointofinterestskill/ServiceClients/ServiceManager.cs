﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Bot.Solutions.Skills;

namespace PointOfInterestSkill.ServiceClients
{
    public class ServiceManager : IServiceManager
    {
        public IGeoSpatialService InitMapsService(SkillConfigurationBase services, string locale = "en")
        {
            services.Properties.TryGetValue("FoursquareClientId", out var clientId);
            services.Properties.TryGetValue("FoursquareClientSecret", out var clientSecret);

            var clientIdStr = (string)clientId;
            var clientSecretStr = (string)clientSecret;

            if (clientIdStr != null && clientSecretStr != null)
            {
                return new FoursquareGeoSpatialService().InitClientAsync(clientIdStr, clientSecretStr).Result;
            }
            else
            {
                var key = GetAzureMapsKey(services);

                return new AzureMapsGeoSpatialService().InitKeyAsync(key, locale).Result;
            }
        }

        /// <summary>
        /// Gets the supported GeoSpatialService for route directions.
        /// Azure Maps is the only supported provider.
        /// </summary>
        /// <param name="services">The SkillConfigurationBase services.</param>
        /// <param name="locale">The user's locale.</param>
        /// <returns>IGeoSpatialService.</returns>
        public IGeoSpatialService InitRoutingMapsService(SkillConfigurationBase services, string locale = "en")
        {
            var key = GetAzureMapsKey(services);

            return new AzureMapsGeoSpatialService().InitKeyAsync(key, locale).Result;
        }

        protected string GetAzureMapsKey(SkillConfigurationBase services)
        {
            services.Properties.TryGetValue("AzureMapsKey", out var key);

            var keyStr = (string)key;
            if (string.IsNullOrWhiteSpace(keyStr))
            {
                throw new Exception("Could not get the required Azure Maps key. Please make sure your settings are correctly configured.");
            }
            else
            {
                return keyStr;
            }
        }
    }
}