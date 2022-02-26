﻿using Microsoft.Xna.Framework;
using SlaamMono.Composition.x_;
using SlaamMono.Gameplay;
using SlaamMono.Library.Input;
using SlaamMono.Library.Logging;
using SlaamMono.Library.ResourceManagement;
using SlaamMono.Library.Screens;
using SlaamMono.MatchCreation;
using SlaamMono.PlayerProfiles;
using System.Collections.Generic;
using ZzziveGameEngine;

namespace SlaamMono.Survival
{
    public class SurvivalCharacterSelectionScreen : CharacterSelectionScreen
    {
        private readonly IScreenManager _screenDirector;
        private readonly IResolver<GameScreenRequestState, SurvivalGameScreen> _survivalGameScreenRequest;

        public SurvivalCharacterSelectionScreen(
            ILogger logger,
            IScreenManager screenDirector,
            IResolver<GameScreenRequestState, SurvivalGameScreen> survivalGameScreenRequest,
            IResolver<LobbyScreenRequestState, LobbyScreen> lobbyScreenResolver)
            : base(logger, screenDirector, lobbyScreenResolver)
        {
            _screenDirector = screenDirector;
            _survivalGameScreenRequest = survivalGameScreenRequest;
        }

        public override void ResetBoxes()
        {
            _state.SelectBoxes = new CharSelectBox[1];
            _state.SelectBoxes[0] = new CharSelectBox(
                new Vector2(340, 427),
                SkinTexture,
                ExtendedPlayerIndex.One,
                Skins,
                x_Di.Get<PlayerColorResolver>(),
                x_Di.Get<IResources>());
            _state.SelectBoxes[0].Survival = true;
        }

        public override void GoBack()
        {
            base.GoBack();
        }

        public override void GoForward()
        {
            List<CharacterShell> list = new List<CharacterShell>();
            list.Add(_state.SelectBoxes[0].GetShell());
            GameScreen.Instance = _survivalGameScreenRequest.Resolve(new GameScreenRequestState(list));
            _screenDirector.ChangeTo(GameScreen.Instance);
        }
    }
}
