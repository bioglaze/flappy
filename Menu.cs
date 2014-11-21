using System;

public class Menu
{
    public enum ButtonType { NewGame, HighScores, Quit, About, Max }

    public bool IsActive { get; set; }

    public Menu( Renderer aRenderer )
    {
        renderer = aRenderer;
        IsActive = true;

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i] = new Button();
            buttons[i].type = (ButtonType)i;
            buttons[i].rectangle = new Renderer.Rectangle( 100, 100 + i * 70, 200, 50 );
        }
    }

    public void Draw()
    {
        foreach (var button in buttons)
        {
            renderer.DrawRectangle( button.rectangle );
        }
    }

    public bool IsCursorOverButton( int cursorX, int cursorY, ButtonType buttonType )
    {
        foreach (var button in buttons)
        {
            bool xOk = cursorX >= button.rectangle.x && cursorX < button.rectangle.width;
            bool yOk = cursorY >= button.rectangle.y && cursorY < button.rectangle.height;
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
    }
        
    private Button[] buttons = new Button[ (int)ButtonType.Max ];
    private readonly Renderer renderer;
}

