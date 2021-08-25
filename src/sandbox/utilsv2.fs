precision highp float;
const float EPSILON = 0.00001;
const vec3 EPSILON_V3 = vec3(EPSILON, 0., 0.);
const float MAX_DISTANCE = 1000.;
const int MAX_STEPS = 32;
const vec4 BLACK = vec4(0., 0., 0., 1.);
const float PI = 3.14159;

// https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
/** random, given seed*/
float rand(float n) {
    return fract(sin(n) * 43758.5453123);
}

// polynomial smooth min (k = 0.1);
// https://iquilezles.org/www/articles/smin/smin.htm
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * (1.0 / 4.0);
}

// ==============
// RAYMARCH HELPERS
// ==============
/**
 * Return the normalized direction to march in from the eye point for a single pixel.
*/
vec3 rayDirection(vec2 fragCoord, float fov, vec2 size) {
    vec2 xy = fragCoord - size * 0.5;
    float z = size.y / tan(radians(fov) * 0.5);
    return normalize(vec3(xy, z));
}
/** Normalized raymarch direction using the default 45deg FOV and shader resolution */
vec3 rayDirection(vec2 fragCoord) {
    return rayDirection(fragCoord, 45., iResolution.xy);
}

// Template for raycast function
/*
vec4 castRay(vec3 origin, vec3 dir) {
    float distanceTraveled = 0.;
    for(int i = 0; i < MAX_STEPS; ++i) {
        vec3 pos = origin + dir * distanceTraveled;

        // assuming function `scene` that defines full scene
        float distanceToClosestEntity = scene(pos);

        if(distanceToClosestEntity < EPSILON) {
            // TODO: write your own color/texture results
            return vec3(1., 0., 0.);
        }

        // prep to continue march
        distanceTraveled += distanceToClosestEntity;
        if(distanceTraveled > MAX_DISTANCE) {
            return BLACK;
        }
    }

    // TODO: write your own background color
    return BLACK;
}
* /

// ==============
// MISC UTILITIES
// ==============
/** get matcap UV coordinates from camera and normal */
// https://www.clicktorelease.com/blog/creating-spherical-environment-mapping-shader/
vec2 matcapUv(vec3 camera, vec3 normal) {
    vec3 reflected = reflect(normalize(camera), normalize(normal));
    float m = 2. * sqrt(pow(reflected.x, 2.) +
        pow(reflected.y, 2.) +
        pow(reflected.z + 1., 2.));
    return reflected.xy / m + .5;
}

// for sdf primitives below
float dot2(in vec3 v) {
    return dot(v, v);
}
float ndot(in vec2 a, in vec2 b) {
    return a.x * b.x - a.y * b.y;
}

// ==============
// SDF SHAPES
// ==============
/** add a sphere with specified radius at specified point */
float sdfSphere(vec3 point, vec3 center, float radius) {
    return length(point - center) - radius;
}

/** unit sphere at origin */
float sdfSphere(vec3 point) {
    return sdfSphere(point, vec3(0.), 1.);
}

/** plane with normal N at height H */
float sdfPlane(vec3 point, vec3 n, float h) {
    // n must be normalized
    return dot(point, n) + h;
}

/** upward-facing plane at y=0 */
float sdfPlane(vec3 point) {
    return sdfPlane(point, vec3(0., 1., 0.), 0.);
}

// ==============
// SHAPING
// ==============
float opUnion(float d1, float d2) {
    return min(d1, d2);
}