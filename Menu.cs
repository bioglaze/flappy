/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-25
 **/
using System;
using OpenTK;

public class Menu
{
    public Menu( Renderer aRenderer )
    {
        renderer = aRenderer;
        buttonTexture = renderer.LoadTexture( "Assets/button.png" );

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i] = new Renderer.Button();
            buttons[i].type = (Renderer.ButtonType)i;
            buttons[i].rectangle = new Renderer.Rectangle( 300, 200 + i * 120, 350, 100 );
            buttons[i].label = buttonLabels[i];
        }
    }

    public void Draw()
    {
        foreach (var button in buttons)
        {
            float marginX = 50;
            float marginY = 20;
            renderer.BindTexture( buttonTexture );
            renderer.DrawRectangle( button.rectangle, button.tintColor );
            renderer.DrawText(button.label, button.rectangle.x + marginX, button.rectangle.y + marginY, 2);
        }
    }

    public bool IsCursorOverButton( int cursorX, int cursorY, Renderer.ButtonType buttonType )
    {
        foreach (var button in buttons)
        {
            bool typeOk = buttonType == button.type;

            if (typeOk && button.IsCursorInside(cursorX, cursorY))
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateButtonHoverHighlights(int cursorX, int cursorY)
    {
        foreach (var button in buttons)
        {
            button.tintColor = button.IsCursorInside(cursorX, cursorY) ? new Vector4(0.8f, 0.8f, 1, 1) : Vector4.One;
        }   
    }
                
    private Renderer.Button[] buttons = new Renderer.Button[ (int)Renderer.ButtonType.Max ];
    private string[] buttonLabels = { "New Game", "Quit" };
    private readonly Renderer renderer;
    private Renderer.Texture buttonTexture;
}

