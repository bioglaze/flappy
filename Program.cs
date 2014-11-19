/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-19
 **/
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

class Program : GameWindow
{
    public static Vector2 screenSize = new Vector2( 700, 650 );
    public static float ScaleFactor { get; private set; }
    private Menu menu;
    //private static string workingDirectory;
    //private bool hasAudioDevice = false;

    public Program() : 
      base( 1024, 768, GraphicsMode.Default, "Game", 0, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible )
    {
        WindowBorder = WindowBorder.Fixed;
        VSync = VSyncMode.On;
        this.MouseUp += MouseEventHandler;
        this.KeyUp += KeyUpHandler;
        InitAudio();
        InitMenu();
    }

    private void InitMenu()
    {
        menu = new Menu();
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


    private void MouseEventHandler( object sender, MouseButtonEventArgs buttonEvent )
    {
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
        GL.Viewport( 0, 0, Width, Height );
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
        //GL.ClearColor(new Color4(0.2f, 0.2f, 0.2f, 1.0f));
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        CheckForGLErrors();
        SwapBuffers();
    }

    private void LoadAssets()
    {
        //Matrix4 orthoMatrix = Matrix4.CreateOrthographic(Width, Height, 0, 1);
    }

    private void CheckForGLErrors()
    {
        ErrorCode errorCode;

        while ((errorCode = GL.GetError()) != ErrorCode.NoError)
        {
            Console.WriteLine(errorCode.ToString());
        }
    }

    private static Program InitGame()
    {
        var game = new Program();
        string version = GL.GetString(StringName.Version);
        Console.WriteLine("OpenGL context: " + version);

        ScaleFactor = game.Width / (float)screenSize.X; // derive current DPI scale factor

        // Adjusts scaling for Retina display.
        if (ScaleFactor > 1.999f)
        {
            game.Dispose();
            screenSize /= 2.0f;
            game = new Program();
        }

        game.CursorVisible = true;
        return game;
    }

    [STAThread]
    static void Main()
    {
        //workingDirectory = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

        var game = InitGame();

        game.Run(60.0);
    }
}
