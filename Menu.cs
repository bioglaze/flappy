using System;

public class Menu
{
    public enum ButtonType { NewGame, HighScores, Quit, About, Max }

    public Menu()
    {
        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i].type = (ButtonType)i;
            buttons[i].rectangle = new Rectangle( 100, 100 + i * 70, 200, 50 );
        }
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

        private int x, y;
        private int width, height;
    }

    private Button[] buttons = new Button[ (int)ButtonType.Max ];
}

