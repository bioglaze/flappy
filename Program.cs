/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-20
 **/
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

class Program : GameWindow
{
    public static Vector2 screenSize = new Vector2( 700, 650 );
    public static float ScaleFactor { get; private set; }
    private Game game = new Game();
    private Renderer renderer = new Renderer();
    private Menu menu;
    //private static string workingDirectory;
    //private bool hasAudioDevice = false;

    public Program() : 
      base( 1024, 768, GraphicsMode.Default, "Game", 0, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible )
    {
        WindowBorder = WindowBorder.Fixed;
        VSync = VSyncMode.On;
        this.MouseUp += MouseUpHandler;
        this.KeyUp += KeyUpHandler;
        InitAudio();
    }

    private void InitAudio()
    {
        // OpenAL using C#/OpenTK works on my machine only on Linux and OS X, not on Windows, so don't enable audio on Windows.
        if (Environment.OSVersion.Platform.ToString() != "Unix")
        {
            return;
        }

        /*try
        {
            AudioContext ac = new AudioContext();
            ac.CheckErrors();
            hasAudioDevice = true;
        }
        catch (AudioException e)
        {
            Console.WriteLine("Unable to init audio device: " + e.ToString());
            return;
        }

        int buffer = AL.GenBuffer();
        soundMoveSrc = AL.GenSource();

        WaveData waveFile = new WaveData(workingDirectory + "/Resources/failed.wav");

        AL.BufferData(buffer, waveFile.SoundFormat, waveFile.SoundData, waveFile.SoundData.Length, waveFile.SampleRate);
        waveFile.dispose();

        AL.Source(soundMoveSrc, ALSourcei.Buffer, buffer);
        AL.Source(soundMoveSrc, ALSourceb.Looping, false);
        AL.GenSources(soundMoveSrc);

        buffer = AL.GenBuffer();

        waveFile = new WaveData(workingDirectory + "/Resources/badmove.wav");

        AL.BufferData(buffer, waveFile.SoundFormat, waveFile.SoundData, waveFile.SoundData.Length, waveFile.SampleRate);
        waveFile.dispose();*/
    }
        
    private void MouseUpHandler( object sender, MouseButtonEventArgs buttonEvent )
    {
        int div = IsRetina() ? 2 : 1;
        int x = buttonEvent.X / div;
        int y = buttonEvent.Y / div;

        Console.WriteLine("Testing button1: " + x + ", " + y);

        if (menu.IsActive)
        {
            if (buttonEvent.Button == MouseButton.Button1)
            {
                Console.WriteLine("Testing button1: " + buttonEvent.X + ", " + buttonEvent.Y);
                if (menu.IsCursorOverButton(x, y, Menu.ButtonType.NewGame))
                {
                    Console.WriteLine("new game");
                    menu.IsActive = false;
                    game.IsActive = true;
                }
            }
        }
    }

    private void KeyUpHandler( object sender, KeyboardKeyEventArgs args )
    {
        if (args.Key == Key.Escape)
        {
            Exit();
        }
    }

    protected override void OnLoad( EventArgs e )
    {
        renderer.Init();
        renderer.SetViewport(0, 0, Width, Height);
        menu = new Menu( renderer );
    }

    protected override void OnUnload( EventArgs e )
    {
    }

    protected override void OnResize( EventArgs e )
    {
        base.OnResize(e);
    }

    protected override void OnUpdateFrame( FrameEventArgs e )
    {
        //if (MatchFree.Utils.stopWatch.ElapsedMilliseconds - lastActionMs > 500)
    }

    protected override void OnRenderFrame( FrameEventArgs e )
    {
        renderer.ClearScreen();

        if (menu.IsActive)
        {
            menu.Draw();
        }

        if (game.IsActive)
        {
            game.Draw();
        }

        renderer.ErrorCheck();
        SwapBuffers();
    }

    private void LoadAssets()
    {
        //Matrix4 orthoMatrix = Matrix4.CreateOrthographic(Width, Height, 0, 1);
    }
        
    private static bool IsRetina()
    {
        return ScaleFactor > 1.999f;
    }

    private static Program InitProgram()
    {
        var program = new Program();

        ScaleFactor = program.Width / (float)screenSize.X; // derive current DPI scale factor

        // Adjusts scaling for Retina display.
        if (IsRetina())
        {
            program.Dispose();
            screenSize /= 2.0f;
            program = new Program();
        }

        program.CursorVisible = true;
        return program;
    }

    [STAThread]
    static void Main()
    {
        //workingDirectory = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        var program = InitProgram();

        program.Run(60.0);
    }
}
