#version 330

uniform sampler2D colorMap;

in vec2 vUV;
out vec4 fragColor;

void main()
{
    //fragColor = vec4( 1.0, 0.0, 0.0, 1.0 );
    fragColor = texture( colorMap, vUV );
}
