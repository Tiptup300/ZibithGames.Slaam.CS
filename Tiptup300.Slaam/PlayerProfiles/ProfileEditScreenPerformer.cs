using SlaamMono.Library;
using SlaamMono.Library.Graphing;
using SlaamMono.Library.Input;
using SlaamMono.Library.ResourceManagement;
using SlaamMono.Menus;
using Tiptup300.Slaam.Library.Rendering;
using ZzziveGameEngine;
using ZzziveGameEngine.StateManagement;

namespace SlaamMono.PlayerProfiles;

public class ProfileEditScreenPerformer : IPerformer<ProfileEditScreenState>, IRenderer<ProfileEditScreenState>
{

   private readonly IResolver<MainMenuRequest, IState> _menuStateResolver;
   private readonly IResources _resources;
   private readonly IRenderService _renderService;
   private readonly IInputService _inputService;

   public ProfileEditScreenPerformer(
       IResolver<MainMenuRequest, IState> menuStateResolver,
       IResources resources,
       IRenderService renderGraph,
       IInputService inputService)
   {
      _menuStateResolver = menuStateResolver;
      _resources = resources;
      _renderService = renderGraph;
      _inputService = inputService;
   }

   public void InitializeState()
   {

   }

   public IState Perform(ProfileEditScreenState state)
   {
      if (state.CurrentMenu.Value == 0)
      {
         if (state.WaitingForQwerty)
         {
            // this replaced qwerty functions
            string qwertyStringBack = "";
            if (qwertyStringBack != "")
            {
               ProfileManager.AddNewProfile(new PlayerProfile(qwertyStringBack, false));

            }
            state.WaitingForQwerty = false;

            setupMainMenu(state);
         }
         else
         {
            if (_inputService.GetPlayers()[0].PressedUp)
            {
               state.CurrentMenuChoice.Sub(1);
               state.MainMenu.SetHighlight(state.CurrentMenuChoice.Value);
            }
            else if (_inputService.GetPlayers()[0].PressedDown)
            {
               state.CurrentMenuChoice.Add(1);
               state.MainMenu.SetHighlight(state.CurrentMenuChoice.Value);
            }
            if (_inputService.GetPlayers()[0].PressedAction)
            {
               if (state.MainMenu.Items[state.CurrentMenuChoice.Value].Details[1] == "new")
               {
                  state.WaitingForQwerty = true;
               }
               else
               {
                  state.EditingProfile = int.Parse(state.MainMenu.Items[state.CurrentMenuChoice.Value].Details[1]);
                  state.CurrentMenu.Value = 1;
                  state.CurrentMenuChoice = new IntRange(0, 0, 2);
                  state.SubMenu.SetHighlight(0);
               }
            }
            if (_inputService.GetPlayers()[0].PressedAction2)
            {
               return _menuStateResolver.Resolve(new MainMenuRequest());
            }
         }
      }
      else if (state.CurrentMenu.Value == 1)
      {
         if (state.WaitingForQwerty)
         {
            string qwertyStringBack = "";
            if (qwertyStringBack != "")
            {
               ProfileManager.PlayableProfiles[state.EditingProfile].Name = qwertyStringBack;
               ProfileManager.SaveProfiles();
            }
            state.WaitingForQwerty = false;

            state.CurrentMenu.Value = 0;
            setupMainMenu(state);
         }
         else
         {
            if (_inputService.GetPlayers()[0].PressedUp)
            {
               state.CurrentMenuChoice.Sub(1);
               state.SubMenu.SetHighlight(state.CurrentMenuChoice.Value);
            }
            else if (_inputService.GetPlayers()[0].PressedDown)
            {
               state.CurrentMenuChoice.Add(1);
               state.SubMenu.SetHighlight(state.CurrentMenuChoice.Value);
            }
            if (_inputService.GetPlayers()[0].PressedAction)
            {
               if (state.SubMenu.Items[state.CurrentMenuChoice.Value].Details[1] == "del")
               {
                  ProfileManager.RemovePlayer(ProfileManager.PlayableProfiles.GetRealIndex(state.EditingProfile));
                  state.EditingProfile = -1;
                  state.CurrentMenu.Value = 0;
                  setupMainMenu(state);
               }
               else if (state.SubMenu.Items[state.CurrentMenuChoice.Value].Details[1] == "ren")
               {
                  string qwertyString = ProfileManager.PlayableProfiles[state.EditingProfile].Name;

                  state.WaitingForQwerty = true;
               }
               else if (state.SubMenu.Items[state.CurrentMenuChoice.Value].Details[1] == "clr")
               {
                  ProfileManager.PlayableProfiles[state.EditingProfile].TotalDeaths = 0;
                  ProfileManager.PlayableProfiles[state.EditingProfile].TotalGames = 0;
                  ProfileManager.PlayableProfiles[state.EditingProfile].TotalKills = 0;
                  ProfileManager.PlayableProfiles[state.EditingProfile].TotalPowerups = 0;
                  ProfileManager.PlayableProfiles[state.EditingProfile].BestGame = TimeSpan.Zero;
                  ProfileManager.SaveProfiles();
                  state.EditingProfile = -1;
                  state.CurrentMenu.Value = 0;
                  setupMainMenu(state);
               }
            }
            if (_inputService.GetPlayers()[0].PressedAction2)
            {
               state.CurrentMenu.Value = 0;
               setupMainMenu(state);
            }
         }
      }
      return state;
   }

   private void setupMainMenu(ProfileEditScreenState _state)
   {
      _state.MainMenu.Items.Columns.Clear();
      _state.MainMenu.Items.Columns.Add("PROFILES");
      _state.MainMenu.Items.Clear();
      for (int x = 1; x < ProfileManager.PlayableProfiles.Count; x++)
      {
         _state.MainMenu.Items.Add(true, new GraphItem(ProfileManager.PlayableProfiles[x].Name, x.ToString()));
      }
      _state.MainMenu.Items.Add(true, new GraphItem("Create New Profile...", "new"));
      _state.MainMenu.SetHighlight(0);
      _state.CurrentMenuChoice = new IntRange(0, 0, _state.MainMenu.Items.Count - 1);
   }

   public void Render(ProfileEditScreenState state)
   {
      _renderService.Render(batch =>
      {
         if (state.CurrentMenu.Value == 0)
         {
            state.MainMenu.Draw(batch);
         }
         else
         {
            state.SubMenu.Draw(batch);
         }
      });
   }
}
