using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

class Program
{
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    [STAThread]
    static void Main()
    {
        AllocConsole();

        Console.WriteLine("Enter your screen resolution (e.g., 1920x1080):");
        string resolutionInput = Console.ReadLine();
        var resolutionParts = resolutionInput.Split('x');

        if (resolutionParts.Length != 2 || !int.TryParse(resolutionParts[0], out int screenWidth) || !int.TryParse(resolutionParts[1], out int screenHeight))
        {
            Console.WriteLine("Invalid resolution. Exiting...");
            return;
        }

        Console.WriteLine("Enter the key bind you want to use (e.g., Q, A, 1, F1, ESC, SPACE):");
        string keyInput = Console.ReadLine()?.ToUpper();
        uint vk;

        if (!TryGetVirtualKey(keyInput, out vk))
        {
            Console.WriteLine("Invalid key input. Exiting...");
            return;
        }

        HotKeyManager hotKeyManager = new HotKeyManager(vk, screenWidth, screenHeight);
        hotKeyManager.Start();
    }

    private static bool TryGetVirtualKey(string keyInput, out uint vk)
    {
        switch (keyInput)
        {
            case "A": vk = 0x41; break;
            case "B": vk = 0x42; break;
            case "C": vk = 0x43; break;
            case "D": vk = 0x44; break;
            case "E": vk = 0x45; break;
            case "F": vk = 0x46; break;
            case "G": vk = 0x47; break;
            case "H": vk = 0x48; break;
            case "I": vk = 0x49; break;
            case "J": vk = 0x4A; break;
            case "K": vk = 0x4B; break;
            case "L": vk = 0x4C; break;
            case "M": vk = 0x4D; break;
            case "N": vk = 0x4E; break;
            case "O": vk = 0x4F; break;
            case "P": vk = 0x50; break;
            case "Q": vk = 0x51; break;
            case "R": vk = 0x52; break;
            case "S": vk = 0x53; break;
            case "T": vk = 0x54; break;
            case "U": vk = 0x55; break;
            case "V": vk = 0x56; break;
            case "W": vk = 0x57; break;
            case "X": vk = 0x58; break;
            case "Y": vk = 0x59; break;
            case "Z": vk = 0x5A; break;
            case "0": vk = 0x30; break;
            case "1": vk = 0x31; break;
            case "2": vk = 0x32; break;
            case "3": vk = 0x33; break;
            case "4": vk = 0x34; break;
            case "5": vk = 0x35; break;
            case "6": vk = 0x36; break;
            case "7": vk = 0x37; break;
            case "8": vk = 0x38; break;
            case "9": vk = 0x39; break;
            case "F1": vk = 0x70; break;
            case "F2": vk = 0x71; break;
            case "F3": vk = 0x72; break;
            case "F4": vk = 0x73; break;
            case "F5": vk = 0x74; break;
            case "F6": vk = 0x75; break;
            case "F7": vk = 0x76; break;
            case "F8": vk = 0x77; break;
            case "F9": vk = 0x78; break;
            case "F10": vk = 0x79; break;
            case "F11": vk = 0x7A; break;
            case "F12": vk = 0x7B; break;
            case "ESC": vk = 0x1B; break;
            case "SPACE": vk = 0x20; break;
            case "TAB": vk = 0x09; break;
            case "ENTER": vk = 0x0D; break;
            case "BACKSPACE": vk = 0x08; break;
            case "LEFT": vk = 0x25; break;
            case "UP": vk = 0x26; break;
            case "RIGHT": vk = 0x27; break;
            case "DOWN": vk = 0x28; break;
            default:
                vk = 0;
                return false;
        }
        return true;
    }
}

public class Clicker
{
    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    private (int X, int Y)[] klikniêcia = new (int, int)[12]
    {
        (816, 406), (816, 557), (816, 406),
        (816, 438), (852, 559), (816, 438),
        (816, 483), (888, 560), (816, 488),
        (816, 518), (919, 561), (816, 518),
    };

    private int screenWidth;
    private int screenHeight;

    public Clicker(int screenWidth, int screenHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
    }

    public void PerformClicks(int powtorzenia)
    {
        foreach (var (x, y) in klikniêcia)
        {
            int adjustedX = (int)(x * (screenWidth / 1920.0));
            int adjustedY = (int)(y * (screenHeight / 1080.0));

            for (int i = 0; i < powtorzenia; i++)
            {
                Cursor.Position = new System.Drawing.Point(adjustedX, adjustedY);
                Thread.Sleep(1);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                Thread.Sleep(1);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                Console.WriteLine($"Clicked {i + 1} at: ({adjustedX}, {adjustedY})");
            }
        }
    }
}

public class HotKeyManager
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int HOTKEY_ID = 1;
    private const uint MOD_NONE = 0x0000;

    private Clicker clicker;
    private uint hotkey;

    public HotKeyManager(uint hotkey, int screenWidth, int screenHeight)
    {
        this.hotkey = hotkey;
        clicker = new Clicker(screenWidth, screenHeight);
    }

    public void Start()
    {
        using (var form = new Form())
        {
            if (!RegisterHotKey(form.Handle, HOTKEY_ID, MOD_NONE, hotkey))
            {
                Console.WriteLine("Can't register the hotkey. Contact the dev.");
                return;
            }

            Console.WriteLine("Press the designated key to start the script.");
            Application.AddMessageFilter(new HotKeyMessageFilter(clicker));
            Application.Run();

            UnregisterHotKey(form.Handle, HOTKEY_ID);
        }
    }

    public class HotKeyMessageFilter : IMessageFilter
    {
        private Clicker clicker;

        public HotKeyMessageFilter(Clicker clicker)
        {
            this.clicker = clicker;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                if (m.WParam.ToInt32() == HOTKEY_ID)
                {
                    Console.WriteLine("Hotkey pressed, starting clicks...");
                    clicker.PerformClicks(1);
                }
            }
            return false;
        }
    }
}
