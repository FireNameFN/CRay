using System;
using System.IO;
using System.Threading;
using CRay;

Console.WriteLine("Hello, World!");

CRayIconWindows iconW = null;

CRayIconWindows iconW2 = null;

if(OperatingSystem.IsWindows()) {
    //CRayIconWindows.Initialize();

    iconW = new(Path.Combine(AppContext.BaseDirectory, "icon.png"));

    iconW.AddMenuItem("Click", () => Console.WriteLine("Click"));

    iconW2 = new(Path.Combine(AppContext.BaseDirectory, "icon.png"));

    iconW2.AddMenuItem("Click2", () => Console.WriteLine("Click"));
} else if(OperatingSystem.IsLinux()) {
    CRayIconLinux.Initialize();

    CRayIconLinux icon = new();

    icon.AddMenuItem("Click", () => Console.WriteLine("Click"));
}

Console.WriteLine(2);

//CRayIconWindows.Loop();

Thread.Sleep(2000);

//iconW.Dispose();

Thread.Sleep(Timeout.Infinite);
