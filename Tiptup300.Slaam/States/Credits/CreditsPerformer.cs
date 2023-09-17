using Microsoft.Xna.Framework;
using SlaamMono.Library;
using SlaamMono.Library.Input;
using SlaamMono.Library.Rendering;
using SlaamMono.Library.ResourceManagement;
using SlaamMono.x_;
using Tiptup300.Slaam.Library.Rendering;
using ZzziveGameEngine;
using ZzziveGameEngine.StateManagement;

namespace SlaamMono.Menus.Credits;

public partial class CreditsPerformer : IPerformer<CreditsState>, IRenderer<CreditsState>
 {
     private const float PIXELS_PER_MINUTE = 1.5f / 60f;

     private readonly IResources _resources;
     private readonly IInputService _inputService;
     private readonly IFrameTimeService _frameTimeService;
     private readonly IResolver<MainMenuRequest, IState> _mainMenuResolver;
     private readonly IRenderService _renderService;

     public CreditsPerformer(
         IResources resources,
         IInputService inputService,
         IFrameTimeService frameTimeService,
         IResolver<MainMenuRequest, IState> mainMenuResolver,
         IRenderService renderService)
     {
         _resources = resources;
         _inputService = inputService;
         _frameTimeService = frameTimeService;
         _mainMenuResolver = mainMenuResolver;
         _renderService = renderService;
     }

     public void InitializeState()
     {
         // to remove
     }

     public IState Perform(CreditsState state)
     {
         if (_inputService.GetPlayers()[0].PressedAction)
         {
             state.Active = !state.Active;
         }

         if (state.Active)
         {
             state.TextCoords = new Vector2(state.TextCoords.X, state.TextCoords.Y - PIXELS_PER_MINUTE * _frameTimeService.GetLatestFrame().MovementFactor);
         }

         if (state.TextCoords.Y < -state.TextHeight - 50)
         {
             state.TextCoords = new Vector2(state.TextCoords.X, GameGlobals.DRAWING_GAME_HEIGHT);
         }

         if (_inputService.GetPlayers()[0].PressedAction2)
         {
             return _mainMenuResolver.Resolve(new MainMenuRequest());
         }
         return state;
     }

     public void Render(CreditsState state)
     {
         _renderService.Render(batch =>
         {

             float Offset = 0;

             for (int CurrentCredit = 0; CurrentCredit < state.CreditsListings.Count; CurrentCredit++)
             {
                 if (state.TextCoords.Y + Offset > 0 && state.TextCoords.Y + Offset + 20 < GameGlobals.DRAWING_GAME_HEIGHT + _resources.GetFont("SegoeUIx32pt").MeasureString(state.CreditsListings[CurrentCredit].Name).Y)
                 {
                     _renderService.RenderText(state.CreditsListings[CurrentCredit].Name, new Vector2(state.TextCoords.X, state.TextCoords.Y + Offset), _resources.GetFont("SegoeUIx32pt"), state.MainCreditColor, Alignment.TopLeft, false);
                 }
                 Offset += _resources.GetFont("SegoeUIx32pt").MeasureString(state.CreditsListings[CurrentCredit].Name).Y / 1.5f;
                 for (int x = 0; x < state.CreditsListings[CurrentCredit].Credits.Count; x++)
                 {
                     if (state.TextCoords.Y + Offset > 0 && state.TextCoords.Y + Offset + 10 < GameGlobals.DRAWING_GAME_HEIGHT + _resources.GetFont("SegoeUIx14pt").MeasureString(state.CreditsListings[CurrentCredit].Credits[x]).Y)
                     {
                         _renderService.RenderText(state.CreditsListings[CurrentCredit].Credits[x], new Vector2(state.TextCoords.X + 10, state.TextCoords.Y + Offset), _resources.GetFont("SegoeUIx14pt"), state.SubCreditColor, Alignment.TopLeft, false);
                     }
                     Offset += (int)_resources.GetFont("SegoeUIx14pt").MeasureString(state.CreditsListings[CurrentCredit].Credits[x]).Y;
                 }
                 Offset += 20;
             }

             state.TextHeight = Offset;
         });
     }

 }
