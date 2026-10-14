# AgentAvenUnity — Unity Reference

> Unity UI, C# architecture concepts and practical examples from our game.

## Navigation

[RectTransform](#1-recttransform) · [Canvas](#2-canvas-and-layout) · [Serialization](#3-serialization) · [Collections](#4-collections) · [Dependency Injection](#5-constructors-and-dependency-injection) · [Events](#6-menu-events-and-back-navigation) · [Debugging](#7-debugging-and-code-style)

## 1. RectTransform

Unity's UI transform, used for sizing, anchoring and positioning interface elements.

| Property | Meaning | Example |
|---|---|---|
| Anchor | Reference point or region in the parent | Middle Center |
| Pivot | Reference point inside the element | (0.5, 0.5) |
| Anchored Position | Pivot offset from anchor reference | (0, -65) |
| Width / Height | Dimensions for fixed anchors | 700 × 700 |
| Stretch Offsets | Margins from stretched anchors | 0 |
| Local Scale | Scale multiplier | (1, 1, 1) |

**Anchor:** Where the element attaches to its parent.

**Pivot:** The internal reference point used for positioning, rotation and scaling.

**Anchored Position:** The offset from the position determined by the anchors to the element's pivot.

Example: ButtonsContainer is anchored to the center of Panel_Main, with an Anchored Position of (0, -65).

**Rule:** Use anchors for container placement and Layout Groups to position children.

## 2. Canvas and Layout

### Canvas Render Modes

| Mode | Purpose |
|---|---|
| Screen Space - Overlay | Render UI directly over the screen |
| Screen Space - Camera | Render UI using a camera |
| World Space | Place UI in the game world |

**Current choice:** Screen Space - Overlay.

### Canvas Scaler

Controls how the UI adapts to different screen resolutions.

Our starting design reference is 1920 × 1080 using Scale With Screen Size.

### Layout Components

| Component | Responsibility |
|---|---|
| Vertical Layout Group | Stack children vertically |
| Horizontal Layout Group | Arrange children horizontally |
| Grid Layout Group | Arrange elements in a grid |
| Layout Element | Specify preferred dimensions |
| Content Size Fitter | Adjust size to content |

**Current ButtonsContainer:** 700 × 700, Scale 1, Spacing 12, button Preferred Height 64.

### TextMeshPro

- Font Asset: Typeface.
- Font Size: Preferred size.
- Auto Size: Resizes text within limits.
- Alignment: Text placement.
- Wrapping: Multiline behavior.
- Overflow: Handling excess text.

Auto Size is useful for variable text lengths but does not replace responsive layout design.

## 3. Serialization

### What is Serialization?

Serialization represents supported object data in a form that can be saved and later restored.

Unity uses serialization to preserve Inspector data in Scenes, Prefabs and Assets.

### SerializeField

```csharp
[SerializeField]
private GameObject settingsPanel;
```

Makes a supported private field available to Unity serialization and the Inspector.

**Why?** Other C# classes do not need direct public access to the field.

### Serializable

```csharp
[System.Serializable]
public class MenuPanelEntry
{
    public MenuPage Page;
    public GameObject Panel;
}
```

Allows supported fields in a custom class to be serialized by Unity.

Our actual implementation keeps these fields private and exposes read-only properties.

### Serialized List vs Runtime Dictionary

**Inspector:**

```csharp
[SerializeField]
private List<MenuPanelEntry> menuPanels = new();
```

**Runtime:**

```csharp
private readonly Dictionary<MenuPage, GameObject> _panels = new();
```

Unity's standard Inspector serialization supports the List of serializable entries, but not an ordinary Dictionary.

We configure the List in the Inspector, then construct the Dictionary in code.

## 4. Collections

### Enum

A named set of constants.

```csharp
public enum MenuPage
{
    Main,
    SinglePlayer,
    Multiplayer,
    HowToPlay,
    Settings,
    AboutDev
}
```

**Why?** Avoid fragile string-based page identifiers.

### List&lt;T&gt;

Ordered, growable collection.

```csharp
List<string> names = new();
names.Add("Main");
```

### Dictionary&lt;TKey, TValue&gt;

Maps unique keys to values.

```csharp
Dictionary<MenuPage, GameObject> panels = new();

panels.Add(MenuPage.Settings, settingsPanel);
```

**Use case:** Finding the GameObject for a menu page.

### Stack&lt;T&gt;

Last In, First Out (LIFO).

```csharp
Stack<MenuPage> history = new();

history.Push(MenuPage.Main);
MenuPage previous = history.Pop();
```

- Push: Add to top.
- Pop: Remove and return top.
- Peek: Read top without removing.
- Clear: Empty the Stack.

**Use case:** Back-navigation history.

## 5. Constructors and Dependency Injection

### What is a Constructor?

A special method called when an instance is created.

It initializes an object and may accept parameters.

**Example — parameterless:**

```csharp
public SceneFlowService()
{
}
```

Called with:

```csharp
SceneFlowService service = new SceneFlowService();
```

**Example — with a parameter:**

```csharp
public Counter(int startingValue)
{
    _count = startingValue;
}
```

This constructor receives configuration data. That is not necessarily Dependency Injection.

### What is Dependency Injection?

Dependency Injection means a component receives an object or service it needs from an external creator instead of constructing that dependency internally.

It is a way of implementing Inversion of Control and often supports the Dependency Inversion Principle.

### Without Dependency Injection

```csharp
public sealed class SceneFlowService
{
    private readonly GameLog _logger = new GameLog();
}
```

SceneFlowService decides which logger implementation to create.

**Problem:** Switching to a FakeGameLogger for tests requires changing SceneFlowService.

### Constructor Dependency Injection

```csharp
public sealed class SceneFlowService
{
    private readonly IGameLogger _logger;

    public SceneFlowService(IGameLogger logger)
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

The caller provides the dependency:

```csharp
IGameLogger logger = new GameLog();

SceneFlowService service = new SceneFlowService(logger);
```

**Why?** SceneFlowService depends on IGameLogger rather than one concrete implementation.

### Visual Comparison

```text
WITHOUT DI

SceneFlowService
      |
      +-- creates GameLog itself


WITH CONSTRUCTOR DI

AppRoot
   |
   +-- creates GameLog
   |
   +-- creates SceneFlowService
                  |
                  +-- receives IGameLogger
```

**Important distinction:** A constructor is a language mechanism. Dependency Injection is an architectural technique. Constructor Injection is one way to perform DI. They are not competing concepts.

### Method Dependency Injection

Some Unity components already exist because Unity manages their lifecycle.

For example, MonoBehaviours are typically created and attached by Unity rather than constructed directly using new.

A dependency can be supplied after creation:

```csharp
private IGameLogger _logger;

public void Initialize(IGameLogger logger)
{
    _logger = logger;
}
```

**Use case:** MenuNavigator receiving an IGameLogger from its scene wiring component.

**Caution:** The component must not use that dependency before Initialize succeeds.

### Constructor vs Method Injection

| Feature | Constructor Injection | Method Injection |
|---|---|---|
| When | During construction | After construction |
| Ideal for | Plain C# services | Unity-managed components |
| Required dependency | Can be enforced by Constructor | Must validate initialization |
| Example | SceneFlowService | MenuNavigator (planned) |

### Does Dependency Injection mean Singleton?

No.

**Singleton:** Controls the existence or global access of an instance.

**Dependency Injection:** Controls how a dependency is supplied to another object.

A Singleton may provide dependencies from a Composition Root, but DI does not require Singleton.

### Composition Root

The place where the application creates and connects shared services.

Our example:

```csharp
Logger = new GameLog();
SceneFlow = new SceneFlowService(Logger);
```

AppRoot owns creation. SceneFlowService receives a dependency.

### readonly and Dependencies

```csharp
private readonly IGameLogger _logger;
```

The field cannot be reassigned after the constructor completes.

It does not mean:
- The dependency is a Singleton.
- The dependency cannot change internal state.
- The dependency cannot be garbage-collected after nothing references it.

**Rule:** A dependency should remain valid for as long as its consumer needs it.

## 6. Menu Events and Back Navigation

### Button.onClick

A UnityEvent invoked when a Button is clicked.

We connect it in the Inspector.

**Example:**

```text
BTN_Settings
   → MainMenuController.OpenSettings()
   → MenuNavigator.Open(MenuPage.Settings)
   → Dictionary lookup
   → Show Panel_Settings
```

### Back Button

```text
BTN_Back
   → MainMenuController.Back()
   → MenuNavigator.Back()
   → Stack.Pop()
   → ShowPage(previous)
```

**Why not give the button access to Stack?**

The button only represents a user action. The Navigator owns the navigation state.

This keeps the UI independent of the history implementation.

### MonoBehaviour Lifecycle

| Callback | Role |
|---|---|
| Awake | Early component initialization |
| OnEnable | Component becomes enabled and active |
| Start | Initialization before the first frame update |
| Update | Per-frame behavior |
| OnDisable | Deactivation handling |

**Important:** Avoid depending on arbitrary Awake execution order between separate MonoBehaviours.

For external dependencies, use a controlled initialization sequence.

## 7. Debugging and Code Style

### Diagnostics

| Level | Purpose |
|---|---|
| Error | Failure requiring attention |
| Warning | Unexpected or potentially problematic state |
| Info | Important application events |
| Verbose | Detailed execution trace |

DevMode shortcut: Ctrl + Shift + F8.

### LF vs CRLF

- LF = `\n`
- CRLF = `\r\n`
- Project convention = LF

A consistent line-ending standard reduces unnecessary Git changes and editor warnings.

### .editorconfig

Located in the Unity project root.

Defines:
- UTF-8 encoding.
- LF line endings.
- Final newline.
- Consistent indentation.
- Removal of trailing whitespace.

### Visual Studio Formatting

**Ctrl + K, Ctrl + D** — Format Document.

Hold Ctrl, press K, release K, then press D while Ctrl remains held.

### Common UI Problems

| Problem | Check |
|---|---|
| Button oversized | Scale, Force Expand, Layout Element |
| Child cannot be moved | Parent Layout Group |
| Text too small | TMP Font Size and Auto Size |
| Panel does not open | Inspector references, initialization, OnClick |
| Two pages visible | Panel activation logic |
| Back not working | Stack history and event binding |
| Scene fails to load | Build Profile and exact scene name |

---

## Reference Rules

- Explain WHAT, HOW and WHY.
- Document patterns only where they genuinely apply.
- Mark planned behavior separately from tested implementations.
- Use examples from AgentAvenUnity whenever possible.

[Project Atlas](../PROJECT_ATLAS.md) · [Architecture Overview](ARCHITECTURE_OVERVIEW.md)

## Prefabs — Reusable UI Components

### What is a Prefab?

A reusable Unity asset containing a GameObject hierarchy and its configured components.

**Example:** `BTN_Back.prefab` stores a Button, Image, TMP label and RectTransform.

**Why?** We can reuse the same visual component across multiple screens.

### Prefab Asset vs Prefab Instance

| Concept | Meaning | Example |
|---|---|---|
| Prefab Asset | Original reusable resource | `BTN_Back.prefab` in Project |
| Prefab Instance | Linked copy in a scene | `BTN_Back` inside Panel_Settings |
| Instance Override | Instance-specific property | Different label or event |
| Apply | Transfer supported overrides back to the asset | Change shared appearance |
| Revert | Restore a property to the prefab value | Undo local style override |
| Unpack | Break the prefab connection | Use only when intentionally needed |

### How Changes Propagate

If the shared Prefab Asset changes its Button colors, linked instances normally inherit the change, unless that property is locally overridden.

### UnityEvent and Scene References

A prefab asset should not rely on references to objects belonging to one specific scene.

Our BTN_Back prefab owns visuals, while each scene instance gets its own Button.onClick connection to MainMenuController.Back().

### Raycast Target

Controls whether a UI Graphic is considered by the UI raycasting system.

**Example:** Decorative background Images usually do not need Raycast Target enabled.

**Why?** Prevent decorative UI from blocking interactions with other UI elements.

### Page vs Overlay

**Page:** A navigated panel registered with MenuNavigator.

**Overlay:** A temporary UI element displayed above the current page.

**Example:** Settings is a Page; a small mute popup can later be an Overlay.

**Pattern note:** Prefab reuse is a Unity asset workflow, not itself a GoF Design Pattern.