/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-23
 **/
using System;
using OpenTK;

public class Game
{
    private struct Assets
    {
        public void Load( Renderer renderer )
        {
            playerTex1 = renderer.LoadTexture( "Assets/player1.png" );
            playerTex2 = renderer.LoadTexture( "Assets/player2.png" );
            pipeTex = renderer.LoadTexture( "Assets/pipe.png" );
        }

        public Renderer.Texture playerTex1;
        public Renderer.Texture playerTex2;
        public Renderer.Texture pipeTex;
    }

    public Game( Renderer aRenderer )
    {
        renderer = aRenderer;
        assets.Load(renderer);
        activePlayerTex = assets.playerTex1;
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
        if (frameNo % 7 == 0 && !PlayerMovingDown())
        {
            activePlayerTex = activePlayerTex == assets.playerTex1 ? assets.playerTex2 : assets.playerTex1;
        }

        renderer.BindTexture(activePlayerTex);
        renderer.DrawRectangle( new Renderer.Rectangle((int)playerPosition.X, (int)playerPosition.Y, activePlayerTex.width*2, activePlayerTex.height*2) );
        renderer.BindTexture(assets.pipeTex);
        renderer.DrawRectangle( new Renderer.Rectangle((int)pipePosition.X, (int)pipePosition.Y, assets.pipeTex.width, assets.pipeTex.height) );
        renderer.DrawRectangle( new Renderer.Rectangle((int)pipePosition.X, assets.pipeTex.height/*(int)pipePosition.Y*/, assets.pipeTex.width, -assets.pipeTex.height) );
        renderer.DrawText("score: " + score, 10, 10, 2);
        ++frameNo;
    }

    public void Simulate( double deltaTimeInSeconds )
    {
        playerPosition.Y += (float)deltaTimeInSeconds * yMovement;

        if (playerPosition.Y < 0)
        {
            playerPosition.Y = 0;
        }

        if (playerPosition.Y < yPositionWhenFlapped - flapDistance || playerPosition.Y <= 0)
        {
            yMovement = fallingSpeed;
        }
    }

    public void StartNewGame()
    {
        playerPosition.X = 100;
        playerPosition.Y = renderer.Height() / 2;
        pipePosition.X = 500;
        pipePosition.Y = renderer.Height() - assets.pipeTex.height / 2;
    }

    private bool PlayerMovingDown()
    {
        return yMovement > 0;
    }

    private Vector2 playerPosition = new Vector2();
    private Vector2 pipePosition = new Vector2();
    private Assets assets = new Assets();
    private readonly Renderer renderer;
    private const float fallingSpeed = 160;
    private const float flapSpeed = -160;
    private const float flapDistance = 200;
    private float yPositionWhenFlapped;
    private float yMovement = fallingSpeed;
    private int frameNo;
    private int score;
    private Renderer.Texture activePlayerTex;
}

