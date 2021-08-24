// https://www.shadertoy.com/view/Ms2SWw
uniform sampler2D buffer0;

vec3 tex(vec2 p) {
    return texture2D(buffer0, p).rgb;
}

vec3 fxaa(vec2 p) {
    const float FXAA_SPAN_MAX = 16.0;
    const float FXAA_REDUCE_MUL = 1.0 / 16.0;
    const float FXAA_REDUCE_MIN = 0.0;//1.0 / 128.0;

    // 1st stage - Find edge
    vec3 rgbNW = tex(p + (vec2(-1., -1.) / iResolution.xy));
    vec3 rgbNE = tex(p + (vec2(1., -1.) / iResolution.xy));
    vec3 rgbSW = tex(p + (vec2(-1., 1.) / iResolution.xy));
    vec3 rgbSE = tex(p + (vec2(1., 1.) / iResolution.xy));
    vec3 rgbM = tex(p);

    const vec3 luma = vec3(1.0 / 3.0);//vec3(0.299, 0.587, 0.114);

    float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
    float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM = dot(rgbM, luma);

    vec2 dir;
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y = ((lumaNW + lumaSW) - (lumaNE + lumaSE));

    float lumaSum = lumaNW + lumaNE + lumaSW + lumaSE;
    float dirReduce = max(lumaSum * (0.25 * FXAA_REDUCE_MUL), FXAA_REDUCE_MIN);
    float rcpDirMin = 1. / (min(abs(dir.x), abs(dir.y)) + dirReduce);

    dir = min(vec2(FXAA_SPAN_MAX), max(vec2(-FXAA_SPAN_MAX), dir * rcpDirMin)) / iResolution.xy;

    // 2nd stage - Blur
    vec3 rgbA = .5 * (tex(p + dir * (1. / 3. - .5)) +
        tex(p + dir * (2. / 3. - .5)));
    vec3 rgbB = rgbA * .5 + .25 * (tex(p + dir * (0. / 3. - .5)) +
        tex(p + dir * (3. / 3. - .5)));

    float lumaB = dot(rgbB, luma);

    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

    return ((lumaB < lumaMin) || (lumaB > lumaMax)) ? rgbA : rgbB;
}

void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    gl_FragColor = vec4(fxaa(uv), texture2D(buffer0, uv).a);
}
