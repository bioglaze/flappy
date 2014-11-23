/**
 * Flappy Clone
 * 
 * Author: Timo Wiren
 * Date: 2014-11-23
 **/
using System;

public class Menu
{
    public enum ButtonType { NewGame, HighScores, Quit, About, Max }

    public Menu( Renderer aRenderer )
    {
        renderer = aRenderer;

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i] = new Button();
            buttons[i].type = (ButtonType)i;
            buttons[i].rectangle = new Renderer.Rectangle( 600, 200 + i * 120, 350, 100 );
            buttons[i].label = buttonLabels[i];
        }
    }

    public void Draw()
    {
        foreach (var button in buttons)
        {
            float marginX = 10;
            renderer.DrawRectangle( button.rectangle );
            renderer.DrawText(button.label, button.rectangle.x + marginX, button.rectangle.y, 2);
        }
    }

    public bool IsCursorOverButton( int cursorX, int cursorY, ButtonType buttonType )
    {
        foreach (var button in buttons)
        {
            bool xOk = cursorX >= button.rectangle.x && cursorX < button.rectangle.x + button.rectangle.width;
            bool yOk = cursorY >= button.rectangle.y && cursorY < button.rectangle.y + button.rectangle.height;
            bool typeOk = buttonType == button.type;

            if (xOk && yOk && typeOk)
            {
                return true;
            }
        }

        return false;
    }

    private class Button
    {
        public ButtonType type;
        public Renderer.Rectangle rectangle;
        public string label;
    }
        
    private Button[] buttons = new Button[ (int)ButtonType.Max ];
    private string[] buttonLabels = { "New Game", "High Scores", "About", "Quit" };
    private readonly Renderer renderer;
}

