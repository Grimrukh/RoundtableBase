using Keystone;
using PropertyHook;

namespace RoundtableBase.Memory;

public abstract class BaseGameHook : PHook
{
    public IntPtr MainModuleBaseAddress { get; private set; }
    protected static readonly Engine Engine = new(Architecture.X86, Mode.X64);

    public bool Focused => Hooked && User32.GetForegroundProcessID() == Process.Id;

    public long GameLoadedTimeMs { get; protected set; } = -1;  // default is unloaded

    public bool IsGameLoaded => GameLoadedTimeMs >= 0;  // i.e. not -1
    
    /// <summary>
    /// Event that fires when the game is loaded. Firing trigger is game-specific (e.g. `PlayerIns` is valid).
    /// </summary>
    public event EventHandler<PHEventArgs>? OnGameLoaded;
    
    /// <summary>
    /// Event that fires when the game is loaded. Firing trigger is game-specific (e.g. `PlayerIns` is invalid).
    /// </summary>
    public event EventHandler<PHEventArgs>? OnGameUnloaded;

    public BaseGameHook(int refreshInterval, int minLifetime, string windowTitle) :
        base(refreshInterval, minLifetime, p => p.MainWindowTitle == windowTitle)
    {
        OnHooked += OnHookedHandler;
        OnUnhooked += OnUnhookedHandler;
    }

    protected virtual void OnHookedHandler(object? sender, PHEventArgs e)
    {
        // Record main module base address.
        if (Process.MainModule != null) 
            MainModuleBaseAddress = Process.MainModule.BaseAddress;
    }

    protected virtual void OnUnhookedHandler(object? sender, PHEventArgs e)
    {
        // No base logic.
    }
    
    public override void Refresh()
    {
        base.Refresh();
        if (!Hooked)
            return;
        bool isGameLoadedNow = CheckIsGameLoaded();
        switch (IsGameLoaded)
        {
            case false when isGameLoadedNow:
                // Game has just loaded.
                OnGameLoaded?.Invoke(this, new PHEventArgs(this));
                GameLoadedTimeMs = 0;
                break;
            case true when !isGameLoadedNow:
                // Game has just unloaded. Note that this CANNOT trigger before `OnGameLoaded` triggers, as IsGameLoaded
                // will only be set to true when the hook first sees that the game is loaded.
                OnGameUnloaded?.Invoke(this, new PHEventArgs(this));
                GameLoadedTimeMs = -1;
                break;
            case true:
                // Still loaded. Increment loaded time.
                GameLoadedTimeMs += RefreshInterval;
                break;
        }
    }

    /// <summary>
    /// Must be implemented by derived classes to check if the game is loaded.
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckIsGameLoaded();

    /// <summary>
    /// Convert to array of ints for AOBScanner.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static int[] BytesToPattern(byte[] bytes)
    {
        int[] pattern = new int[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
            pattern[i] = bytes[i];
        return pattern;
    }
    
    public bool AssembleAndExecute(string asm)
    {
        //Console.WriteLine(asm);
        
        if (Process.MainModule == null)
            return false;
        
        // Assemble from base address to get array size.
        EncodedData? bytes = Engine.Assemble(asm, (ulong)Process.MainModule.BaseAddress);
        KeystoneError error = Engine.GetLastKeystoneError();
        if (error != KeystoneError.KS_ERR_OK)
            throw new Exception($"Something went wrong during assembly. Code could not be assembled. Error: {error}");

        IntPtr? startPtr = null;
        try
        {
            startPtr = AllocateClose(bytes.Buffer.Length, flProtect: Kernel32.PAGE_EXECUTE_READWRITE);
            // Reassemble with the location of `startPtr` to support relative instructions.
            bytes = Engine.Assemble(asm, (ulong)startPtr);
            error = Engine.GetLastKeystoneError();
            if (error != KeystoneError.KS_ERR_OK) // would be very unusual
            {
                Logging.Debug($"Something went wrong during assembly. Code could not be assembled. Error: {error}");
                return false;
            }
            Kernel32.WriteBytes(Handle, startPtr.Value, bytes.Buffer);
            //DebugPrintArray(bytes.Buffer);
            Execute(startPtr.Value);
        }
        catch (Exception e)
        {
            Logging.Debug($"Failed to assemble and execute code: {e.Message}");
            return false;
        }
        finally
        {
            if (startPtr != null)
                Free(startPtr.Value);
        }
        return true;
    }

    public bool AssembleAndInject(string asm, IntPtr address, int injectionSiteLength, IntPtr? basePtr = null)
    {
        if (Process.MainModule == null)
            return false;
        if (basePtr == null)
            basePtr = Process.MainModule.BaseAddress;
        
        EncodedData? bytes = Engine.Assemble(asm, (ulong)basePtr);
        KeystoneError error = Engine.GetLastKeystoneError();
        if (error != KeystoneError.KS_ERR_OK)
            throw new Exception($"Something went wrong during assembly. Code could not be assembled. Error: {error}");

        if (bytes.Buffer.Length < injectionSiteLength)
        {
            // Add padding "nop" instructions to `asm` and reassemble.
            int paddingLength = injectionSiteLength - bytes.Buffer.Length;
            for (int i = 0; i < paddingLength; i++)
                asm += "\nnop";
            bytes = Engine.Assemble(asm, (ulong)basePtr);
            error = Engine.GetLastKeystoneError();
            if (error != KeystoneError.KS_ERR_OK)
                throw new Exception($"Something went wrong during assembly. Code could not be assembled. Error: {error}");
        }
        
        Kernel32.WriteBytes(Handle, address, bytes.Buffer);
        //DebugPrintArray(bytes.Buffer);
        
        return true;
    }
    
    public IntPtr AssembleAllocateWrite(string asm, IntPtr? basePtr = null, bool executable = true)
    {
        //Console.WriteLine(asm);
        
        if (Process.MainModule == null)
            return IntPtr.Zero;
        
        // Assemble from base address to get array size.
        EncodedData? bytes = Engine.Assemble(asm, (ulong)Process.MainModule.BaseAddress);
        KeystoneError error = Engine.GetLastKeystoneError();
        if (error != KeystoneError.KS_ERR_OK)
            throw new Exception($"Something went wrong during assembly. Code could not be assembled. Error: {error}");

        IntPtr startPtr = AllocateClose(bytes.Buffer.Length, basePtr, flProtect: executable ? Kernel32.PAGE_EXECUTE_READWRITE : Kernel32.PAGE_READWRITE);

        // Reassemble with the location of `startPtr` to support relative instructions.
        bytes = Engine.Assemble(asm, (ulong)startPtr);
        error = Engine.GetLastKeystoneError();
        if (error != KeystoneError.KS_ERR_OK)  // would be very unusual
            throw new Exception($"Something went wrong during assembly. Code could not be assembled. Error: {error}");

        Kernel32.WriteBytes(Handle, startPtr, bytes.Buffer);
        //DebugPrintArray(bytes.Buffer);
        
        return startPtr;
    }
}