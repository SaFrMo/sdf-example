// https://www.shadertoy.com/view/llt3R4

const int MAX_MARCHING_STEPS = 255;
const float MIN_DIST = 0.0;
const float MAX_DIST = 100.0;
const float EPSILON = 0.0001;

/** SDF for sphere */
float sphereSDF(vec3 samplePoint, vec3 origin, float radius) {
    return distance(samplePoint, origin) - radius;
}
float sphereSDF(vec3 samplePoint) {
    return length(samplePoint) - 1.0;
}

/** scene SDF */
float sceneSDF(vec3 samplePoint) {
    // unit sphere at origin
    return sphereSDF(samplePoint);
}

/**
 * Return the shortest distance from the eyepoint to the scene surface along
 * the marching direction. If no part of the surface is found between start and end,
 * return end.
 */
float shortestDistanceToSurface(vec3 origin, vec3 direction) {
    // clipping planes
    float near = MIN_DIST;
    float far = MAX_DIST;

    float currentDepth = near;
    for(int i = 0; i < MAX_MARCHING_STEPS; i++) {
        float dist = sceneSDF(origin + currentDepth * direction);
        if(dist < EPSILON) {
            return currentDepth;
        }
        currentDepth += dist;
        if(currentDepth >= far) {
            return far;
        }
    }
    return far;
}

/**
 * Return the normalized direction to march in from the eye point for a single pixel.
*/
vec3 rayDirection(float fov, vec2 size, vec2 fragCoord) {
    vec2 xy = fragCoord - size * 0.5;
    float z = size.y / tan(radians(fov) * 0.5);
    return normalize(vec3(xy, -z));
}

void main() {
    vec3 dir = rayDirection(45., iResolution.xy, gl_FragCoord.xy);
    vec3 camera = vec3(0., 0., 5.);
    float dist = shortestDistanceToSurface(camera, dir);

    if(dist > MAX_DIST - EPSILON) {
        // didn't hit anything, black background
        gl_FragColor = vec4(0., 0., 0., 1.);
        return;
    }

    gl_FragColor = vec4(1., 0., 0., 1.);
}