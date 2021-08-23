void main() {
    vec2 st = gl_FragCoord.xy / iResolution.xy;
    vec3 color = vec3(st.x, st.y, abs(sin(iTime)));
    gl_FragColor = vec4(color, 1.0);
}