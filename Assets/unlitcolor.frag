#version 330

uniform sampler2D texture;
uniform float uOpacity;

in vec2 vUV;

out vec4 fragColor;

void main()
{
    vec4 tex = texture( texture, vUV );
    fragColor = vec4( tex.rgb, uOpacity );
}
