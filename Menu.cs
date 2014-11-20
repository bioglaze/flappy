using System;

public class Menu
{
    public enum ButtonType { NewGame, HighScores, Quit, About, Max }

    public bool IsActive { get; set; }

    public Menu()
    {
        IsActive = true;

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i] = new Button();
            buttons[i].type = (ButtonType)i;
            buttons[i].rectangle = new Rectangle( 100, 100 + i * 70, 200, 50 );
        }
    }

    public void Draw()
    {

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
        public Rectangle rectangle;
    }

    private struct Rectangle
    {
        public Rectangle( int aX, int aY, int aWidth, int aHeight )
        {
            x = aX;
            y = aY;
            width = aWidth;
            height = aHeight;
        }

        public int x, y;
        public int width, height;
    }

    private Button[] buttons = new Button[ (int)ButtonType.Max ];
}

