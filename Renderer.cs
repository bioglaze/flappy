/**
 * Author: Timo Wiren
 * Date: 2014-11-23
 **/
using System;
using System.Drawing;
using System.Drawing.Imaging;
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

    public void Init( int width, int height )
    {
        GL.Viewport( 0, 0, width, height );

        CreateQuadBuffer();

        Matrix4 orthoMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
        unlitColor.Load( "Assets/unlitcolor.vert", "Assets/unlitcolor.frag" );
        unlitColor.Use();
        unlitColor.SetMatrix(ref orthoMatrix, Shader.Uniform.ProjectionMatrix);

        buttonTextureId = LoadTexture( "Assets/white.png" );

        fonter.LoadBMFontMetaText( "Assets/font.fnt" );
        fontTextureId = LoadTexture( "Assets/font.png" );
        fontVBO = GL.GenBuffer();
        fontVAO = GL.GenVertexArray();
    }

    public void ClearScreen()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void DrawRectangle( Rectangle rectangle )
    {
        unlitColor.SetVector( new Vector4( rectangle.width, rectangle.height, rectangle.x, rectangle.y ), Shader.Uniform.ScaleAndTranslation );
        const int quadVertexCount = 6;
        BindVAO(quadVAO);
        GL.BindTexture( TextureTarget.Texture2D, buttonTextureId );

        GL.DrawArrays(PrimitiveType.Triangles, 0, quadVertexCount);
    }

    public void DrawText( string text, float x, float y, float scale )
    {
        unlitColor.SetVector( new Vector4( 1, 1, 0, 0 ), Shader.Uniform.ScaleAndTranslation );

        Vector4[] fontGeom = fonter.CreateGeometry( text, x, y, scale );

        BindVAO(fontVAO);

        GL.BindBuffer( BufferTarget.ArrayBuffer, fontVBO );
        GL.BufferData< Vector4 >( BufferTarget.ArrayBuffer, new IntPtr( fontGeom.Length * Vector4.SizeInBytes ), fontGeom, BufferUsageHint.StaticDraw );

        GL.VertexAttribPointer( 0, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0 );
        GL.EnableVertexAttribArray( 0 );

        GL.BindTexture( TextureTarget.Texture2D, fontTextureId );
        GL.ValidateProgram(unlitColor.Program());

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        GL.DrawArrays( PrimitiveType.Triangles, 0, fontGeom.Length );

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

    public int LoadTexture( string fileName )
    {
        if (!System.IO.File.Exists( fileName ))
        {
            Console.WriteLine( "File not found: " + fileName );
            return 0;
        }

        int id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, id);

        Bitmap bmp;

        try
        {
            bmp = new Bitmap(fileName);
        }
        catch (System.IO.FileNotFoundException e)
        {
            Console.WriteLine( e.Message );
            return 0;
        }

        BitmapData bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest );
        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest );
        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge );
        GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge );

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

        bmp.UnlockBits(bmp_data);

        return id;
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
    private int buttonTextureId;
    private Fonter fonter = new Fonter();
    private int fontTextureId;
    private int fontVBO;
    private int fontVAO;
    private int currentVAO;
}

