using System;
using System.Threading;
using CRay;

Console.WriteLine("Hello, World!");

if(OperatingSystem.IsWindows()) {
    
} else if(OperatingSystem.IsLinux()) {
    CRayIconLinux.Initialize();

    CRayIconLinux icon = new();

    icon.AddMenuItem("Click", () => Console.WriteLine("Click"));
}

Console.WriteLine(2);

Thread.Sleep(Timeout.Infinite);
