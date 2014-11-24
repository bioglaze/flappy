/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-23
 * 
 * Note: Developed on a Retina MacBook Pro, not tested on non-retina resolution.
 * 
 * Note: Xamarin Studio seems to currently have a bug that prevents me from adding Assets folder
 *       into the project, so if you build the project, copy it manually to the bin/Debug folder.
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
    public Program() : 
        base( width, height, GraphicsMode.Default, "Flappy Clone", 0, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible )
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
        int div = IsRetina() ? 1 : 1;
        int x = buttonEvent.X / div;
        int y = buttonEvent.Y / div;

        if (gameState == GameState.Menu && buttonEvent.Button == MouseButton.Left)
        {
            if (menu.IsCursorOverButton(x, y, Menu.ButtonType.NewGame))
            {
                gameState = GameState.Game;
                game.StartNewGame();
            }
            else if (menu.IsCursorOverButton(x, y, Menu.ButtonType.Quit))
            {
                Exit();
            }
        }
    }

    private void KeyUpHandler( object sender, KeyboardKeyEventArgs args )
    {
        if (args.Key == Key.Escape && gameState == GameState.Game)
        {
            gameState = GameState.Menu;
        }

        if (args.Key == Key.Space && gameState == GameState.Game)
        {
            if (game.IsPaused())
            {
                game.Play();
            }

            game.ApplyFlap();
        }

        if (args.Key == Key.Escape && gameState == GameState.Menu)
        {
            Exit();
        }
    }

    protected override void OnLoad( EventArgs e )
    {
        renderer.Init( Width, Height );
        menu = new Menu( renderer );
        game = new Game( renderer );
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
        if (gameState == GameState.Game)
        {
            game.Simulate(e.Time);
        }
    }

    protected override void OnRenderFrame( FrameEventArgs e )
    {
        renderer.ClearScreen();

        if (gameState == GameState.Menu)
        {
            menu.Draw();
        }

        if (gameState == GameState.Game)
        {
            game.Draw();
        }

        renderer.ErrorCheck();
        SwapBuffers();
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

    public static float ScaleFactor { get; private set; }
    private const int width = 512;
    private const int height = 384;
    private static Vector2 screenSize = new Vector2( width, height );
    private enum GameState { Menu, Game }
    private GameState gameState = GameState.Menu;
    private Game game;
    private Renderer renderer = new Renderer();
    private Menu menu;
    //private static string workingDirectory;
    //private bool hasAudioDevice = false;
}
