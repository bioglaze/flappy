﻿/**
 * Author: Timo Wiren
 * Date: 2014-11-22
 **/
using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

public class Shader
{
    public enum Uniform
    {
        ProjectionMatrix = 0,
        ScaleAndTranslation = 1,
        TintColor = 2,
        count
    }

    public Shader()
    {
        uniforms = new int[ (int)Uniform.count ];
    }

    public void Load( string aVertexFile, string aFragmentFile )
    {
        StreamReader vsReader = new StreamReader (aVertexFile);
        string vsSource = vsReader.ReadToEnd ();
        vsReader.Close ();

        int vShader = GL.CreateShader (ShaderType.VertexShader);
        GL.ShaderSource (vShader, vsSource);
        GL.CompileShader (vShader);

        int compiled;
        GL.GetShader (vShader, ShaderParameter.CompileStatus, out compiled);

        if (compiled == 0)
        {
            Console.WriteLine( "Could not compile vertex shader!" );
            Console.WriteLine( GL.GetShaderInfoLog( vShader ) );
        }

        StreamReader fsReader = new StreamReader( aFragmentFile );
        string fsSource = fsReader.ReadToEnd();
        fsReader.Close();

        int fShader = GL.CreateShader( ShaderType.FragmentShader );
        GL.ShaderSource( fShader, fsSource );
        GL.CompileShader( fShader );

        GL.GetShader( fShader, ShaderParameter.CompileStatus, out compiled );

        if (compiled == 0)
        {
            Console.WriteLine( "Could not compile fragment shader!" );
            Console.WriteLine( GL.GetShaderInfoLog( fShader ) );
        }

        program = GL.CreateProgram();

        GL.AttachShader( program, vShader );
        GL.AttachShader( program, fShader );

        GL.LinkProgram( program );

        string programInfoLog;
        GL.GetProgramInfoLog( program, out programInfoLog );
        Console.WriteLine( programInfoLog );

        uniforms[ (int)Uniform.ProjectionMatrix ] = GL.GetUniformLocation( program, "uProjectionMatrix" );
        uniforms[ (int)Uniform.ScaleAndTranslation ] = GL.GetUniformLocation( program, "uScaleAndTranslation" );
        uniforms[ (int)Uniform.TintColor ] = GL.GetUniformLocation( program, "tintColor" );
    }

    public void SetMatrix( ref Matrix4 aMatrix, Uniform aName )
    {
        GL.UniformMatrix4( uniforms[ (int)aName ], false, ref aMatrix );
    }

    public void SetFloat( float f, Uniform aName )
    {
        GL.Uniform1( uniforms[ (int)aName ], f );
    }

    public void SetVector( Vector2 v, Uniform aName )
    {
        GL.Uniform2( uniforms[ (int)aName ], v );
    }

    public void SetVector( Vector4 v, Uniform aName )
    {
        GL.Uniform4( uniforms[ (int)aName ], v );
    }

    public void Use()
    {
        GL.UseProgram( program );
    }

    public int Program()
    {
        return program;
    }

    private int program;
    private int[] uniforms;
}

