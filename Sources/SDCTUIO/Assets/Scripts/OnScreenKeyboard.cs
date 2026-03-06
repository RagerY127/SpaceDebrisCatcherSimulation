using System.Diagnostics;

class OnScreenKeyboard
{
    public static void ShowTouchKeyboard()
    {
        /**
            TODO: Documenter le fait qu'il faut cette clé dans le registre pour que le clavier s'affiche
            mdr
            [HKEY_CURRENT_USER\Software\Microsoft\TabletTip\1.7]
            ""EnableDesktopModeAutoInvoke""=dword:00000001
        */
        ExternalCall(@"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe", null, false);
    }

    public static void KillTouchKeyboard()
    {
        try
        {
            foreach (var p in Process.GetProcessesByName("TextInputHost"))
            {
                p.Kill();
            }
        }
        catch
        {
            // do nothing, we just want to kill the keyboard whatever happens
        }
    }

    private static Process ExternalCall(string filename, string arguments, bool hideWindow)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        var psi = new ProcessStartInfo();
        psi.FileName = filename;
        psi.Arguments = arguments;

        // si c'est seulement, pas d'affichage de la console
        if (hideWindow)
        {
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
        }

        var process = new Process();
        process.StartInfo = psi;
        process.Start();
        return process;
#else
        return null;
#endif
    }
}