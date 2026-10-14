# AgentAvenUnity — Project Atlas

> Living project map: what exists, where it lives, what it does, and why.

**Unity:** 6.3 LTS · Universal 2D (URP)  
**Milestone:** M1 — Application Shell & Main Menu  
**Current Phase:** D.2 — Complete Navigation Verification  
**Editors:** Visual Studio (C#) · VS Code (Documentation)

## Quick Navigation

[Scenes](#scenes) · [Code Map](#code-map) · [Main Menu](#main-menu) · [Architecture](#architecture) · [Diagnostics](#diagnostics) · [Roadmap](#roadmap) · [Git](#git-and-checkpoints)

## Scenes

| Scene | Responsibility | Status |
|---|---|---|
| [Boot_00](Scenes/Boot_00.unity) | App initialization | Verified |
| [MainMenu_01](Scenes/MainMenu_01.unity) | UI and navigation | Functional; verification pending |
| [Game_02](Scenes/Game_02.unity) | Shared gameplay scene | Created |

Startup: BootLoader → AppRoot → SceneFlowService → MainMenu_01.

## Code Map

Paths relative to `Assets/_AgentAvenUnity/`.

| Component | Location | Responsibility |
|---|---|---|
| AppRoot | [AppRoot.cs](Scripts/Application/Bootstrap/AppRoot.cs) | Singleton / Composition Root |
| BootLoader | [BootLoader.cs](Scripts/Application/Bootstrap/BootLoader.cs) | Startup transition |
| SceneFlowService | [SceneFlowService.cs](Scripts/Application/SceneFlow/SceneFlowService.cs) | Scene loading |
| DevMode | [DevMode.cs](Scripts/Application/Diagnostics/DevMode.cs) | Diagnostic configuration |
| DevModeHotkeys | [DevModeHotkeys.cs](Scripts/Application/Diagnostics/DevModeHotkeys.cs) | Developer shortcuts |
| IGameLogger | [IGameLogger.cs](Scripts/Application/Diagnostics/IGameLogger.cs) | Logging contract |
| GameLog | [GameLog.cs](Scripts/Application/Diagnostics/GameLog.cs) | Logging implementation |
| MenuPage | [MenuPage.cs](Scripts/UI/MainMenu/MenuPage.cs) | Page identifiers |
| MenuNavigator | [MenuNavigator.cs](Scripts/UI/MainMenu/MenuNavigator.cs) | Dictionary / Stack / Visibility |
| MainMenuController | [MainMenuController.cs](Scripts/UI/MainMenu/MainMenuController.cs) | UI commands |
| ProjectFolderColors | [ProjectFolderColors.cs](Editor/ProjectFolderColors.cs) | Editor folder colors |

## Main Menu

**Technology:** uGUI + TextMeshPro.

### Scene Hierarchy

```text
MainMenu_01
├── Main Camera
├── MainMenuRoot
│   ├── MenuNavigator
│   └── MainMenuController
├── MainMenuCanvas
│   ├── Menu_BG_IMG
│   ├── Panel_Main
│   │   ├── Title_TMP
│   │   └── ButtonsContainer
│   │       ├── BTN_SinglePlayer
│   │       ├── BTN_MultiPlayer
│   │       ├── BTN_HowToPlay
│   │       ├── BTN_Settings
│   │       ├── BTN_AboutDev
│   │       └── BTN_Exit
│   ├── Panel_SinglePlayer
│   │   ├── SPT_TMP
│   │   └── BTN_Back
│   ├── Panel_Multiplayer
│   │   ├── MPT_TMP
│   │   └── BTN_Back
│   ├── Panel_HowToPlay
│   │   ├── HTPT_TMP
│   │   └── BTN_Back
│   ├── Panel_Settings
│   │   ├── ST_TMP
│   │   └── BTN_Back
│   ├── Panel_AboutDev
│   │   ├── ADT_TMP
│   │   └── BTN_Back
│   └── OverlayLayer
│       ├── Overlay_TMP
│       └── BTN_Back (temporary)
└── EventSystem
```

### Navigation System

**Design:** Controller + enum + serialized List + Dictionary + Stack.

- MenuPage: Named page identifiers.
- MenuNavigator: Owns panel visibility and navigation history.
- MainMenuController: Receives UI button actions.
- BTN_Back: Reusable prefab; scene instances bind button callbacks.

**Verified logs:**

```text
[MenuNav] Initialized 6 menu pages.
[MainMenu] Controller initialized.
[MenuNav] Opened SinglePlayer. History: 1
[MenuNav] Back to Main. History: 0
[MainMenu] Exit requested.
```

Exit logging works in Unity Editor; Application.Quit does not close the Editor.

### Prefabs

| Asset | Location | Status |
|---|---|---|
| BTN_Back | [Prefabs/UI/Buttons/BTN_Back.prefab](Prefabs/UI/Buttons/BTN_Back.prefab) | Created; five subpanel instances wired |
| BTN_Menu | Prefabs/UI/Buttons/ | Proposed; not created |

**Prefab strategy:** Reuse appearance and UI components. Scene-specific OnClick references belong on instances.

### Shared Background

Menu_BG_IMG is a direct child of MainMenuCanvas, outside Panel_Main.

It remains visible when pages change.

### Overlay Layer

OverlayLayer is reserved for future popups, dialogs and quick settings.

An additional BTN_Back currently exists under OverlayLayer and is wired to GoHome(). This is temporary and should be disabled until overlay behavior is implemented.

Future overlay close actions will not necessarily modify the navigation Stack.

### Current UI Notes

- ButtonsContainer: 700 × 700.
- Vertical Layout Group controls layout.
- BTN_Back currently: 160 × 30.
- BTN_Back uses top-left anchors/pivot.
- BTN_Back highlight color: Yellow.
- Decorative menu sprites are being tested.
- Sprite sizing / 9-slice setup in progress.
- Panel layouts and responsive behavior remain unfinished.
- Final animation is deferred.

## Architecture

Detailed diagrams: [Architecture Overview](Docs/ARCHITECTURE_OVERVIEW.md).

| Approach | Application |
|---|---|
| Singleton | AppRoot |
| Composition Root | AppRoot |
| Constructor DI | SceneFlowService |
| Method DI | MenuNavigator |
| Dependency Inversion | IGameLogger |
| Separation of Concerns | Controller / Navigator |
| Serialized List + Dictionary | Inspector config + runtime lookup |
| Stack | Back navigation |
| Prefab reuse | Shared Back button |
| Repository (planned) | Settings persistence |
| Adapter (planned) | Networking |
| Strategy (planned) | AI behavior |

### Key Design Decisions

- One persistent AppRoot.
- One shared MainMenu scene containing multiple Panels.
- One shared Game_02 scene for single/multiplayer.
- Navigation belongs to MenuNavigator.
- UI buttons invoke MainMenuController.
- Prefabs hold reusable UI structure.
- Scene references are configured on instances.
- Overlay logic is separate from page history.
- Shared services use explicit dependencies.

## Diagnostics

**Logger:** IGameLogger → GameLog → Unity Console.

**Levels:** Error / Warning / Info / Verbose.

**DevMode:** Ctrl + Shift + F8; shortcut retest remains pending.

**Known warning:** UDP 56618 for VS/Unity messaging. Not currently blocking gameplay or scene loading.

## Roadmap

### Phase D.2 — Navigation Verification
- [x] Create five Back prefab instances
- [x] Wire five Back buttons (reported)
- [x] Verify SinglePlayer → Back
- [x] Verify Exit log
- [ ] Test Multiplayer → Back
- [ ] Test HowToPlay → Back
- [ ] Test Settings → Back
- [ ] Test AboutDev → Back
- [ ] Verify only one page visible
- [ ] Save and confirm MainMenu_01

### Phase E — GitHub and Checkpoints
- [ ] Create repository with Unity .gitignore
- [ ] Ensure .meta files are tracked
- [ ] Configure Git and a feature branch
- [ ] Implement Checkpoint.ps1
- [ ] Commit first verified menu foundation

### Phase F — UI Foundation / Visual Polish
- [ ] Validate Canvas Scaler and anchoring
- [ ] Correct sprite slicing and sizing
- [ ] Create shared Menu Button Prefab
- [ ] Finish visual hierarchy and panel layouts
- [ ] Test aspect ratios and screen resolutions

### Phase G — Application Services
- [ ] SettingsService
- [ ] PlayerPrefsSettingsRepository
- [ ] AudioService and AudioMixer
- [ ] Master / Music / SFX / Mute
- [ ] Language infrastructure (EN / HE)
- [ ] Quick Settings popup

### Phase H — Functional Menu Content
- [ ] Single Player options and MatchConfig
- [ ] Multiplayer Lobby mock
- [ ] How To Play shell
- [ ] About / Dev information
- [ ] Settings persistence tests

### Phase I — Boot and Integration
- [ ] Startup intro animation
- [ ] Loading screen / transitions
- [ ] Enter Game_02 using MatchConfig
- [ ] Final menu integration tests


### Git — Foundation Checkpoint

- Repository: DorManDel/AgentAvenUnity
- Initial commit: chore: initialize Unity gitignore
- main: Published and verified
- .gitignore: Working; Unity cache folders excluded
- Development branch: feature/menu-foundation (next)
- Unity project upload: Pending
- Checkpoint automation: Pending


Checkpoint script:
1. Check working directory and branch.
2. Reject main/master and detached HEAD.
3. Inspect changes.
4. Commit approved project files.
5. Push to the current feature branch.
6. Report success or failure.

No automatic commit on every Unity save.

## Documentation

- [Project Atlas](PROJECT_ATLAS.md) — Navigation and progress
- [Architecture Overview](Docs/ARCHITECTURE_OVERVIEW.md) — Components and diagrams
- [Unity Reference](Docs/UNITY_UI_REFERENCE.md) — Unity UI and architecture concepts
- [C# Reference](Docs/CSHARP_REFERENCE.md) — Language conventions and examples

**Code comments:** WHAT / HOW / WHY / DESIGN.

**Formatting:** UTF-8, LF, four-space C# indentation.

**Workflow:** Plan → Implement → Understand → Test → Document → Checkpoint.

---

**Next action:** Finish Phase D.2 verification, then set up GitHub and checkpoint automation.s