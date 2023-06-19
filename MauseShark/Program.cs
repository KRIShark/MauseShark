using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Import the mouse_event function from user32.dll
    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    // Constants for the mouse event flags
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

    // Import the GetTickCount function from kernel32.dll
    [DllImport("kernel32.dll")]
    private static extern uint GetTickCount();

    // Import the GetLastInputInfo function from user32.dll
    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    static void Main()
    {
        Console.WriteLine("Mouse Mover - Move the mouse one pixel every 10 seconds if inactive.");
        Console.WriteLine("Press any key to exit.");

        // Start a new thread to monitor user activity
        Thread activityThread = new Thread(CheckUserActivity);
        activityThread.Start();

        // Wait for a key press to exit the program
        Console.ReadKey();

        // Stop the activity thread and exit
        activityThread.Abort();
    }

    static void CheckUserActivity()
    {
        while (true)
        {
            // Check if the mouse is being moved
            if (GetIdleTime() > 10000) // 10 seconds of inactivity
            {
                // Move the mouse one pixel
                MoveMouse(1, 1);
            }

            // Wait for 1 second before checking again
            Thread.Sleep(1000);
        }
    }

    static void MoveMouse(int deltaX, int deltaY)
    {
        // Get the current mouse position
        POINT currentPosition;
        GetCursorPos(out currentPosition);

        // Calculate the new mouse position
        int newX = currentPosition.X + deltaX;
        int newY = currentPosition.Y + deltaY;

        // Move the mouse to the new position
        SetCursorPos(newX, newY);
    }

    // Structure for the LASTINPUTINFO function
    struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    // Get the idle time in milliseconds
    static uint GetIdleTime()
    {
        uint lastInputTicks = 0;
        uint idleTicks = 0;

        LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
        lastInputInfo.dwTime = 0;

        if (GetLastInputInfo(ref lastInputInfo))
        {
            lastInputTicks = lastInputInfo.dwTime;
            idleTicks = GetTickCount() - lastInputTicks;
        }

        return idleTicks;
    }

    // Import the SetCursorPos function from user32.dll
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    // Import the GetCursorPos function from user32.dll
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    // Structure for the POINT function
    struct POINT
    {
        public int X;
        public int Y;
    }
}
