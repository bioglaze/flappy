/**
 * Author: Timo Wiren
 * Date: 2014-11-23
 **/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

using OpenTK;

public class Renderer
{
    public enum ButtonType { NewGame, Quit, Max }

    public class Texture
    {
        public int id;
        public int width, height;
    }

    public class Button
    {
        public bool IsCursorInside(int cursorX, int cursorY)
        {
            bool xOk = cursorX >= rectangle.x && cursorX < rectangle.x + rectangle.width;
            bool yOk = cursorY >= rectangle.y && cursorY < rectangle.y + rectangle.height;
            return xOk && yOk;
        }

        public ButtonType type;
        public Renderer.Rectangle rectangle;
        public string label;
        public Vector4 tintColor = Vector4.One;
    }

    public class Rectangle
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

    public void Init( int width, int height )
    {
        windowSize = new Vector2( width, height );

        GL.Viewport( 0, 0, width, height );
        SetClearColor(new Color4(0.8f, 0.8f, 1.0f, 1.0f));
        CreateQuadBuffer();

        Matrix4 orthoMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
        unlitColor.Load( "Assets/unlitcolor.vert", "Assets/unlitcolor.frag" );
        unlitColor.Use();
        unlitColor.SetMatrix(ref orthoMatrix, Shader.Uniform.ProjectionMatrix);

        fonter.LoadBMFontMetaText( "Assets/font.fnt" );
        fontTexture = LoadTexture( "Assets/font.png" );
        fontVBO = GL.GenBuffer();
        fontVAO = GL.GenVertexArray();
    }

    public void BindTexture( Texture texture )
    {
        GL.BindTexture(TextureTarget.Texture2D, texture.id);
    }
        
    public void ClearScreen()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    // Use negative size for mirroring sprites.
    public void DrawRectangle( Rectangle rectangle, Vector4 color)
    {
        unlitColor.SetVector( new Vector4( rectangle.width, rectangle.height, rectangle.x, rectangle.y ), Shader.Uniform.ScaleAndTranslation );
        unlitColor.SetVector( color, Shader.Uniform.TintColor );

        const int quadVertexCount = 6;
        BindVAO(quadVAO);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        GL.DrawArrays(PrimitiveType.Triangles, 0, quadVertexCount);

        GL.Disable(EnableCap.Blend);
        unlitColor.SetVector( new Vector4( 1, 1, 1, 1 ), Shader.Uniform.TintColor );
    }

    public void DrawText( string text, float x, float y, float scale )
    {
        unlitColor.SetVector( new Vector4( 1, 1, 0, 0 ), Shader.Uniform.ScaleAndTranslation );
        unlitColor.SetVector( new Vector4( 0, 0, 0, 1 ), Shader.Uniform.TintColor );

        Vector4[] fontGeom = fonter.CreateGeometry( text, x, y, scale );

        BindVAO(fontVAO);

        GL.BindBuffer( BufferTarget.ArrayBuffer, fontVBO );
        GL.BufferData< Vector4 >( BufferTarget.ArrayBuffer, new IntPtr( fontGeom.Length * Vector4.SizeInBytes ), fontGeom, BufferUsageHint.StaticDraw );

        GL.VertexAttribPointer( 0, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0 );
        GL.EnableVertexAttribArray( 0 );

        GL.BindTexture( TextureTarget.Texture2D, fontTexture.id );
        GL.ValidateProgram(unlitColor.Program());

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        GL.DrawArrays( PrimitiveType.Triangles, 0, fontGeom.Length );
        unlitColor.SetVector( new Vector4( 1, 1, 1, 1 ), Shader.Uniform.TintColor );

        GL.Disable(EnableCap.Blend);
    }

    public void ErrorCheck()
    {
        ErrorCode errorCode;

        while ((errorCode = GL.GetError()) != ErrorCode.NoError)
        {
            Console.WriteLine("GL error: " + errorCode.ToString());
        }        
    }

    public Texture LoadTexture( string fileName )
    {
        Texture outTexture = new Texture();

        if (!System.IO.File.Exists( fileName ))
        {
            Console.WriteLine( "File not found: " + fileName );
            return outTexture;
        }

        outTexture.id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, outTexture.id);

        Bitmap bmp;

        try
        {
            bmp = new Bitmap(fileName);
        }
        catch (System.IO.FileNotFoundException e)
        {
            Console.WriteLine( e.Message );
            return outTexture;
        }

        BitmapData bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        outTexture.width = bmp_data.Width;
        outTexture.height = bmp_data.Height;

        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest );
        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest );
        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge );
        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge );

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

        bmp.UnlockBits(bmp_data);

        return outTexture;
    }

    public float Width()
    {
        return windowSize.X;
    }

    public float Height()
    {
        return windowSize.Y;
    }

    public void SetClearColor(Color4 color)
    {
        GL.ClearColor(color);
    }

    private void CreateQuadBuffer()
    {
        // Vector contains: x, y, u, v.
        Vector4[] vertices = new Vector4[ 6 ];

        vertices[0] = new Vector4( 0, 0, 0, 0 );
        vertices[1] = new Vector4( 1, 0, 1, 0 );
        vertices[2] = new Vector4( 1, 1, 1, 1 );

        vertices[3] = new Vector4( 0, 0, 0, 0 );
        vertices[4] = new Vector4( 0, 1, 0, 1 );
        vertices[5] = new Vector4( 1, 1, 1, 1 );

        quadVAO = GL.GenVertexArray();
        GL.BindVertexArray( quadVAO );

        quadVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
        GL.BufferData< Vector4 >(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vector4.SizeInBytes), vertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        ErrorCheck();
    }

    private void BindVAO( int id )
    {
        if (id != currentVAO)
        {
            GL.BindVertexArray(id);
            currentVAO = id;
        }
    }

    private Shader unlitColor = new Shader();
    private int quadVBO;
    private int quadVAO;
    private Fonter fonter = new Fonter();
    private Texture fontTexture;
    private int fontVBO;
    private int fontVAO;
    private int currentVAO;
    private Vector2 windowSize = new Vector2( 640, 480 );
}

