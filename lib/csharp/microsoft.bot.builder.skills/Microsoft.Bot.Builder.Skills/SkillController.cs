﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Skills.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Bot.Builder.Skills
{
    /// <summary>
    /// This is the default Controller that contains APIs for handling
    /// calls from a channel and
    /// calls from a parent bot (to a skill bot)
    /// </summary>
    [ApiController]
    [Authorize]
    public abstract class SkillController : ControllerBase
    {
        private readonly IBot _bot;
        private readonly IBotFrameworkHttpAdapter _botFrameworkHttpAdapter;
        private readonly SkillAdapter _skillAdapter;
        private readonly ISkillAuthProvider _skillAuthProvider;

        public SkillController(IServiceProvider serviceProvider)
        {
            _botFrameworkHttpAdapter = serviceProvider.GetService<IBotFrameworkHttpAdapter>() ?? throw new ArgumentNullException(nameof(IBotFrameworkHttpAdapter));
            _skillAdapter = serviceProvider.GetService<SkillAdapter>() ?? throw new ArgumentNullException(nameof(SkillAdapter));
            _bot = serviceProvider.GetService<IBot>() ?? throw new ArgumentNullException(nameof(IBot));
            _skillAuthProvider = serviceProvider.GetService<ISkillAuthProvider>();
        }

        /// <summary>
        /// This API is the endpoint for when a bot receives a message from a channel or a parent bot
        /// </summary>
        /// <returns></returns>
        [Route("api/messages")]
        [HttpPost]
        [AllowAnonymous]
        public async Task BotMessage()
        {
            await _botFrameworkHttpAdapter.ProcessAsync(Request, Response, _bot, default(CancellationToken));
        }

        /// <summary>
        /// This API is the endpoint the bot exposes as skill
        /// </summary>
        /// <returns></returns>
        [Route("api/skill/messages")]
        [HttpPost]
        public async Task SkillMessage()
        {
            if (_skillAuthProvider != null && !_skillAuthProvider.Authenticate(HttpContext))
            {
                Response.StatusCode = 401;
                return;
            }

            await _skillAdapter.ProcessAsync(Request, Response, _bot, default(CancellationToken));
        }
    }
}