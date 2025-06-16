# CRay
Tray icons library for C#.

Supports operating systems:
- Windows
- Linux

# Use

This is signature of main CRay class:

```cs
namespace CRay;

public sealed class CRayIcon : ICRayIcon {
    public bool Visible { get; set; }

    public bool Attention { get; set; }

    public string IconsPath { get; set; }

    public string IconName { get; set; }

    public string AttentionIconName { get; set; }

    public CRayIcon(string iconsPath, string iconName);

    public CRayIcon(string iconName = "icon");

    public static void Initialize();

    public void AddMenuItem(string label, Action action);

    public void Dispose();
}
```

Call `Initialize()` before creating first instance of CRayIcon. If you are using GLFW, call it before `GLFW.Init()`. Actually, this method do something only on Linux.

Set `IconsPath` to directory with icons, and set `IconName` and `AttentionIconName` as filenames without extensions.

Use `Attention` to switch between usual icon and attention icon.

`Visible` property allows you to temporarily disable icon.

Use `AddMenuItem()` for adding buttons to menu.

Don't forget to call `Dispose()` when `CRayIcon` is no more in use.

# Authors
- [FN](https://github.com/FireNameFN)
