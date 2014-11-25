/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-25
 **/
using System;
using OpenTK;
using OpenTK.Graphics;

public class Game
{
    private struct Assets
    {
        public void Load( Renderer renderer )
        {
            playerTex1 = renderer.LoadTexture( "Assets/player1.png" );
            playerTex2 = renderer.LoadTexture( "Assets/player2.png" );
            pipeTex = renderer.LoadTexture( "Assets/pipe.png" );
            buttonTexture = renderer.LoadTexture( "Assets/button.png" );
        }

        public Renderer.Texture playerTex1;
        public Renderer.Texture playerTex2;
        public Renderer.Texture pipeTex;
        public Renderer.Texture buttonTexture;
    }

    public Game( Renderer aRenderer )
    {
        renderer = aRenderer;
        assets.Load(renderer);
        restartButton.label = "Restart";
        restartButton.rectangle = new Renderer.Rectangle( (int)renderer.Width() / 2 - 150, (int)renderer.Height() / 2, 350, 100);
    }

    public void ApplyFlap()
    {
        if (PlayerMovingDown())
        {
            yMovement = flapSpeed;
            yPositionWhenFlapped = playerPosition.Y;
        }
    }

    public void Draw()
    {
        renderer.SetClearColor( isDead ? new Color4( 1, 0.5f, 0.5f, 1 ) : new Color4(0.8f, 0.8f, 1, 1));

        var tex = ((frameNo % 15) < 7 && !PlayerMovingDown() && !isDead) ? assets.playerTex1 : assets.playerTex2;
        renderer.BindTexture(tex);

        renderer.DrawRectangle( new Renderer.Rectangle((int)playerPosition.X, (int)playerPosition.Y, assets.playerTex1.width*2, assets.playerTex1.height*2), Vector4.One );
        renderer.BindTexture(assets.pipeTex);
        var lowerPipe = new Renderer.Rectangle((int)upperPipePosition.X, (int)renderer.Height() - (int)assets.pipeTex.height, assets.pipeTex.width, assets.pipeTex.height);
        renderer.DrawRectangle( lowerPipe, Vector4.One );
        renderer.DrawRectangle( new Renderer.Rectangle((int)upperPipePosition.X, assets.pipeTex.height, assets.pipeTex.width, -assets.pipeTex.height), Vector4.One );
        renderer.DrawText("score: " + score, 10, 10, 2);

        if (!isPlaying)
        {
            renderer.DrawText("press space", renderer.Width() / 2 - 100, renderer.Height() / 2 + 20, 2);
        }

        if (isDead)
        {
            float marginX = 50;
            float marginY = 20;

            renderer.BindTexture( assets.buttonTexture );
            renderer.DrawRectangle( restartButton.rectangle, Vector4.One);
            renderer.DrawText(restartButton.label, restartButton.rectangle.x + marginX, restartButton.rectangle.y + marginY, 2);

        }

        ++frameNo;
    }

    public bool IsCursorOverRestartButton( int cursorX, int cursorY )
    {
        return restartButton.IsCursorInside(cursorX, cursorY);
    }

    public bool IsPaused()
    {
        return !isPlaying;
    }

    public void Play()
    {
        isPlaying = true;
    }

    public void Simulate( double deltaTimeInSeconds )
    {
        if (!isPlaying || isDead)
        {
            return;
        }

        SimulatePlayer(deltaTimeInSeconds);
        SimulatePipe(deltaTimeInSeconds);
    }

    public void StartNewGame()
    {
        playerPosition.X = 100;
        playerPosition.Y = renderer.Height() / 2;
        upperPipePosition.X = 600;
        upperPipePosition.Y = renderer.Height() - assets.pipeTex.height / 2;
        isPlaying = false;
        isDead = false;
        score = 0;
    }
        
    private bool PlayerMovingDown()
    {
        return yMovement > 0;
    }

    private void SimulatePlayer(double deltaTimeInSeconds)
    {
        playerPosition.Y += (float)deltaTimeInSeconds * yMovement;

        if (playerPosition.Y < 0)
        {
            playerPosition.Y = 0;
            isDead = true;
        }

        if (playerPosition.Y < yPositionWhenFlapped - flapDistance || playerPosition.Y <= 0)
        {
            yMovement = fallingSpeed;
        }

        bool diedFalling = playerPosition.Y + assets.playerTex1.height*2 >= renderer.Height();

        if (diedFalling)
        {
            isDead = true;
            playerPosition.Y = renderer.Height() - + assets.playerTex1.height * 2;
        }

        if (Math.Abs(playerPosition.X - upperPipePosition.X) < 1)
        {
            ++score;
        }
    }

    private void SimulatePipe(double deltaTimeInSeconds)
    {
        upperPipePosition.X -= (float)deltaTimeInSeconds * 140;

        if (upperPipePosition.X + assets.pipeTex.width * 2 <= 0)
        {
            upperPipePosition.X = renderer.Width();
        }

        bool xInPipe = playerPosition.X + assets.playerTex1.width * 2 > upperPipePosition.X &&
                       playerPosition.X < upperPipePosition.X + assets.pipeTex.width;

        bool yInUpperPipe = playerPosition.Y < assets.pipeTex.height;
        bool yInLowerPipe = playerPosition.Y + assets.playerTex1.height * 2 > renderer.Height() - assets.pipeTex.height;

        isDead = xInPipe && (yInUpperPipe || yInLowerPipe);
    }

    private Vector2 playerPosition = new Vector2();
    private Vector2 upperPipePosition = new Vector2();
    private Assets assets = new Assets();
    private readonly Renderer renderer;
    private const float fallingSpeed = 160;
    private const float flapSpeed = -160;
    private const float flapDistance = 200;
    private float yPositionWhenFlapped;
    private float yMovement = fallingSpeed;
    private int frameNo;
    private int score;
    private bool isPlaying = false;
    private bool isDead = false;
    private Renderer.Button restartButton = new Renderer.Button();
}

