using System;
using OpenTK.Graphics.OpenGL;

public class Renderer
{
    public Renderer()
    {
    }

    public void ClearScreen()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void ErrorCheck()
    {
        ErrorCode errorCode;

        while ((errorCode = GL.GetError()) != ErrorCode.NoError)
        {
            Console.WriteLine(errorCode.ToString());
        }        
    }

    public void SetViewport( int x, int y, int width, int height )
    {
        GL.Viewport( 0, 0, width, height );
    }
}

