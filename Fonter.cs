/**
 * Author: Timo Wiren
 * Date: 2014-11-22
 **/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using OpenTK;

public class Fonter
{
    private class Character
    {
        public int x, y;
        public int width, height;
        public int xOffset, yOffset;
        public int xAdvance;
    }

    private Vector2 spacing;
    private Vector4 padding;
    private int lineHeight;
    private int top2basePixels;
    private int textureWidth = 256;
    private int textureHeight = 256;
    private Dictionary< int, Character > characters = new Dictionary< int, Character >();

    public Vector4[] CreateGeometry( string text, float x, float y, float scale )
    {
        Vector4[] geom = new Vector4[ text.Length * 6 ];

        float accumX = 0;

        for (int i = 0; i < text.Length; ++i)
        {
            Character ch = characters[ (int)text[ i ] ];

            float offx = x + ch.xOffset * scale + accumX * scale;
            float offy = y + ch.yOffset * scale;

            accumX += ch.xAdvance;

            float u0 = ch.x / (float)textureWidth;
            float u1 = (ch.x + ch.width) / (float)textureWidth;

            float v0 = (ch.y + ch.height) / (float)textureHeight;
            float v1 = (ch.y) / (float)textureHeight;

            // Upper triangle.
            geom[ i * 6 + 0 ] = new Vector4( offx, offy, u0, v1 );
            geom[ i * 6 + 1 ] = new Vector4( offx + ch.width * scale, offy, u1, v1 );
            geom[ i * 6 + 2 ] = new Vector4( offx, offy + ch.height * scale, u0, v0 );
            // Lower triangle.
            geom[ i * 6 + 3 ] = new Vector4( offx + ch.width * scale, offy, u1, v1 );
            geom[ i * 6 + 4 ] = new Vector4( offx + ch.width * scale, offy + ch.height * scale, u1, v0 );
            geom[ i * 6 + 5 ] = new Vector4( offx, offy + ch.height * scale, u0, v0 );
        }

        return geom;
    }

    public void LoadBMFontMetaText( string path )
    {
        string[] metaFileLines;

        try
        {
            metaFileLines = File.ReadAllLines( path );
        }
        catch (FileNotFoundException fex)
        {
            Console.WriteLine( "File not found: " + path + ", exception: " + fex.ToString() );
            return;
        }

        ParseInfoLine( metaFileLines[ 0 ] );
        ParseCommonLine( metaFileLines[ 1 ] );
        ParseChars( metaFileLines );
    }

    private void ParseChars( string[] lines )
    {
        char[] delimiters = { ' ', '.', ':', '\t', '=' };
        string[] tokens = lines[3].Split( delimiters );
        int charCount = int.Parse( tokens[ 2 ] );

        for (int i = 4; i < charCount + 4; ++i)
        {
            Character character = new Character();

            string[] charTokens = lines[ i ].Split( delimiters );
            int id = int.Parse( charTokens[ 2 ] );

            for (int t = 0; t < charTokens.Length; ++t)
            {
                if (charTokens[t] == "x")
                {
                    character.x = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }
                else if (charTokens[t] == "y")
                {
                    character.y = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }
                else if (charTokens[t] == "width")
                {
                    character.width = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }
                else if (charTokens[t] == "height")
                {
                    character.height = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }
                else if (charTokens[t] == "xoffset")
                {
                    character.xOffset = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }
                else if (charTokens[t] == "yoffset")
                {
                    character.yOffset = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }
                else if (charTokens[t] == "xadvance")
                {
                    character.xAdvance = int.Parse( charTokens[ t + 1 ] );
                    ++t;            
                }

            }

            characters[ id ] = character;
        }
    }

    private void ParseCommonLine( string commonLine )
    {
        char[] delimiters = { ' ', '.', ':', '\t' };
        string[] tokens = commonLine.Split( delimiters );

        foreach (string token in tokens)
        {
            if (token.Contains("lineHeight"))
            {
                char[] paddingDelimiters = { '=', ',' };
                string[] paddingTokens = token.Split( paddingDelimiters );

                lineHeight = int.Parse( paddingTokens[1] );
            }
            else if (token.Contains("base"))
            {
                char[] paddingDelimiters = { '=', ',' };
                string[] paddingTokens = token.Split( paddingDelimiters );

                top2basePixels = int.Parse( paddingTokens[1] );
            }
        }
    }

    private void ParseInfoLine( string infoLine )
    {
        char[] delimiters = { ' ', '.', ':', '\t' };
        string[] tokens = infoLine.Split( delimiters );

        foreach (string token in tokens)
        {
            if (token.Contains( "padding" ))
            {
                char[] paddingDelimiters = { '=', ',' };
                string[] paddingTokens = token.Split( paddingDelimiters  );

                padding.X = float.Parse( paddingTokens[1] );
                padding.Y = float.Parse( paddingTokens[2] );
                padding.Z = float.Parse( paddingTokens[3] );
                padding.W = float.Parse( paddingTokens[4] );
            }
            else if (token.Contains( "spacing" ))
            {
                char[] spacingDelimiters = { '=', ',' };
                string[] spacingTokens = token.Split( spacingDelimiters  );

                spacing.X = float.Parse( spacingTokens[1] );
                spacing.Y = float.Parse( spacingTokens[2] );
            }
        }
    }
}

