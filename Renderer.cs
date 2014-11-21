using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

public class Renderer
{
    public struct Rectangle
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

    public Renderer()
    {
    }

    public void Init()
    {
        unlitColor.Load( "Assets/unlitcolor.vert", "Assets/unlitcolor.frag" );
        CreateQuadBuffer();
    }

    public void ClearScreen()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void DrawRectangle( Rectangle Rectangle )
    {

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

    private void CreateQuadBuffer()
    {
        Vector4[] vertices = new Vector4[ 6 ];

        vertices[0] = new Vector4( 0, 0, 0, 0 );
        vertices[1] = new Vector4( 1, 0, 0, 0 );
        vertices[2] = new Vector4( 1, 1, 0, 0 );

        vertices[3] = new Vector4( 0, 0, 0, 0 );
        vertices[4] = new Vector4( 0, 1, 0, 0 );
        vertices[5] = new Vector4( 1, 1, 0, 0 );

        GL.GenBuffers(1, out quadVBO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
        GL.BufferData< Vector4 >(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vector4.SizeInBytes), vertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        ErrorCheck();
    }

    private Shader unlitColor = new Shader();
    private uint quadVBO;
}

