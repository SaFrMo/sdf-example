// https://www.shadertoy.com/view/llt3R4
precision highp float;

const int MAX_MARCHING_STEPS = 255;
const float MIN_DIST = 0.0;
const float MAX_DIST = 100.0;
const float EPSILON = 0.0001;

/** SDF for sphere */
float sphereSDF(vec3 samplePoint, vec3 origin, float radius) {
    return distance(samplePoint, origin) - radius;
}
float sphereSDF(vec3 samplePoint) {
    return sphereSDF(samplePoint, vec3(0.), 1.);
}

/** scene SDF */
float sceneSDF(vec3 samplePoint) {
    // unit sphere at origin
    return sphereSDF(samplePoint, vec3(0.), 1.);
}

/**
 * Using the gradient of the SDF, estimate the normal on the surface at point p.
 */
vec3 estimateNormal(vec3 p) {
    float x = sceneSDF(vec3(p.x + EPSILON, p.yz)) - sceneSDF(vec3(p.x - EPSILON, p.yz));
    float y = sceneSDF(vec3(p.x, p.y + EPSILON, p.z)) - sceneSDF(vec3(p.x, p.y - EPSILON, p.z));
    float z = sceneSDF(vec3(p.xy, p.z + EPSILON)) - sceneSDF(vec3(p.xy, p.z - EPSILON));
    return normalize(vec3(x, y, z)) * 0.5 + 0.5;
}

/**
 * Return the shortest distance from the eyepoint to the scene surface along
 * the marching direction. If no part of the surface is found between start and end,
 * return end.
 */
float shortestDistanceToSurface(vec3 origin, vec3 direction, out vec3 normal, out float lightIntensity) {
    // clipping planes
    float near = MIN_DIST;
    float far = MAX_DIST;

    float currentDepth = near;
    for(int i = 0; i < MAX_MARCHING_STEPS; i++) {
        vec3 currPos = origin + currentDepth * direction;
        float dist = sceneSDF(currPos);
        if(dist <= EPSILON) {
            // calc normal
            normal = estimateNormal(currPos);
            // calc lighting
            vec3 lightPos = vec3(0., 1., 0.);
            // calc lighting
            vec3 dirToLight = normalize(currPos - lightPos);
            lightIntensity = max(0., dot(normal, dirToLight));
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
    return normalize(vec3(xy, z));
}

void main() {
    vec3 dir = rayDirection(45., iResolution.xy, gl_FragCoord.xy);
    vec3 camera = vec3(0., 0., -5.);
    vec3 normal = vec3(1.);
    float intensity = 1.;
    float dist = shortestDistanceToSurface(camera, dir, normal, intensity);

    if(dist > MAX_DIST - EPSILON) {
        // didn't hit anything, black background
        gl_FragColor = vec4(0., 0., 0., 1.);
        return;
    }

    gl_FragColor = vec4(normal * intensity, 1.);
}